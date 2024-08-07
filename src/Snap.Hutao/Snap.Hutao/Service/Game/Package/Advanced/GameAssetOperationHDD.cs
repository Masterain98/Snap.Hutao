﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Win32.SafeHandles;
using Snap.Hutao.Core.IO;
using Snap.Hutao.Core.IO.Compression.Zstandard;
using Snap.Hutao.Web.Hoyolab.Takumi.Downloader.Proto;
using System.Buffers;
using System.IO;

namespace Snap.Hutao.Service.Game.Package.Advanced;

[ConstructorGenerated(CallBaseConstructor = true)]
[Injection(InjectAs.Transient)]
[SuppressMessage("", "CA2000")]
internal sealed partial class GameAssetOperationHDD : GameAssetOperation
{
    public override async ValueTask InstallAssetsAsync(GamePackageServiceContext context, SophonDecodedBuild remoteBuild, CancellationToken token = default)
    {
        foreach (SophonDecodedManifest manifest in remoteBuild.Manifests)
        {
            IEnumerable<SophonAssetOperation> assets = manifest.ManifestProto.Assets.Select(asset => SophonAssetOperation.AddOrRepair(manifest.UrlPrefix, asset));
            foreach (SophonAssetOperation asset in assets)
            {
                await EnsureAssetAsync(context, asset, token).ConfigureAwait(false);
            }
        }
    }

    public override async ValueTask UpdateDiffAssetsAsync(GamePackageServiceContext context, List<SophonAssetOperation> diffAssets, CancellationToken token = default)
    {
        foreach (SophonAssetOperation asset in diffAssets)
        {
            ValueTask task = asset.Kind switch
            {
                SophonAssetOperationKind.AddOrRepair or SophonAssetOperationKind.Modify => EnsureAssetAsync(context, asset, token),
                SophonAssetOperationKind.Delete => DeleteAssetsAsync(context, diffAssets.Select(a => a.OldAsset), token),
                _ => ValueTask.CompletedTask,
            };

            await task.ConfigureAwait(false);
        }
    }

    public override async ValueTask PredownloadDiffAssetsAsync(GamePackageServiceContext context, List<SophonAssetOperation> diffAssets, CancellationToken token = default)
    {
        foreach (SophonAssetOperation asset in diffAssets)
        {
            IEnumerable<SophonChunk> chunks = asset.Kind switch
            {
                SophonAssetOperationKind.AddOrRepair => asset.NewAsset.AssetChunks.Select(c => new SophonChunk(asset.UrlPrefix, c)),
                SophonAssetOperationKind.Modify => asset.DiffChunks,
                _ => [],
            };

            await DownloadChunksAsync(context, chunks, token).ConfigureAwait(false);
        }
    }

    protected override async ValueTask VerifyManifestsAsync(GamePackageServiceContext context, SophonDecodedBuild build, Action<SophonAssetOperation> conflictHandler, CancellationToken token = default)
    {
        foreach (SophonDecodedManifest manifest in build.Manifests)
        {
            await VerifyManifestAsync(context, manifest, conflictHandler, token).ConfigureAwait(false);
        }
    }

    protected override async ValueTask VerifyManifestAsync(GamePackageServiceContext context, SophonDecodedManifest manifest, Action<SophonAssetOperation> conflictHandler, CancellationToken token = default)
    {
        foreach (AssetProperty asset in manifest.ManifestProto.Assets)
        {
            await VerifyAssetAsync(context, new(manifest.UrlPrefix, asset), conflictHandler, token).ConfigureAwait(false);
        }
    }

    protected override async ValueTask RepairAssetsAsync(GamePackageServiceContext context, GamePackageIntegrityInfo info, CancellationToken token = default)
    {
        foreach (SophonAssetOperation asset in info.ConflictedAssets)
        {
            await EnsureAssetAsync(context, asset, token).ConfigureAwait(false);
        }
    }

    protected override async ValueTask DownloadChunksAsync(GamePackageServiceContext context, IEnumerable<SophonChunk> sophonChunks, CancellationToken token = default)
    {
        foreach (SophonChunk chunk in sophonChunks)
        {
            await DownloadChunkAsync(context, chunk, token).ConfigureAwait(false);
        }
    }

    protected override async ValueTask MergeNewAssetAsync(GamePackageServiceContext context, AssetProperty assetProperty, CancellationToken token = default)
    {
        string path = Path.Combine(context.Operation.GameFileSystem.GameDirectory, assetProperty.AssetName);
        string? directory = Path.GetDirectoryName(path);
        ArgumentNullException.ThrowIfNull(directory);
        Directory.CreateDirectory(directory);

        using (SafeFileHandle fileHandle = File.OpenHandle(path, FileMode.Create, FileAccess.Write, FileShare.None, preallocationSize: 32 * 1024))
        {
            using (IMemoryOwner<byte> memoryOwner = MemoryPool<byte>.Shared.Rent(81920))
            {
                Memory<byte> buffer = memoryOwner.Memory;

                foreach (AssetChunk chunk in assetProperty.AssetChunks)
                {
                    string chunkPath = Path.Combine(context.Operation.ProxiedChunksDirectory, chunk.ChunkName);
                    if (!File.Exists(chunkPath))
                    {
                        continue;
                    }

                    TaskCompletionSource tcs = new();
                    while (!ProcessingChunks.TryAdd(chunk.ChunkName, tcs.Task))
                    {
                        if (ProcessingChunks.TryGetValue(chunk.ChunkName, out Task? task))
                        {
                            await task.ConfigureAwait(false);
                            token.ThrowIfCancellationRequested();
                        }
                    }

                    try
                    {
                        using (FileStream chunkFile = File.OpenRead(chunkPath))
                        {
                            using (ZstandardDecompressionStream decompressor = new(chunkFile))
                            {
                                long offset = chunk.ChunkOnFileOffset;
                                do
                                {
                                    int bytesRead = await decompressor.ReadAsync(buffer, token).ConfigureAwait(false);
                                    if (bytesRead <= 0)
                                    {
                                        break;
                                    }

                                    await RandomAccess.WriteAsync(fileHandle, buffer[..bytesRead], offset, token).ConfigureAwait(false);
                                    context.Progress.Report(new GamePackageOperationReport.Install(bytesRead, 0));
                                    offset += bytesRead;
                                }
                                while (true);
                            }
                        }
                    }
                    finally
                    {
                        tcs.TrySetResult();
                        ProcessingChunks.TryRemove(chunk.ChunkName, out _);
                        if (!DuplicatingChunkNames.Contains(chunk.ChunkName))
                        {
                            FileOperation.Delete(chunkPath);
                        }
                    }

                    context.Progress.Report(new GamePackageOperationReport.Install(0, 1));
                }
            }
        }
    }
}