﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Web.WebView2.Core;
using Snap.Hutao.Core.DataTransfer;
using Snap.Hutao.Factory.Picker;
using Snap.Hutao.Service.Notification;
using System.Net.Http;

namespace Snap.Hutao.Web.Bridge;

internal sealed class BridgeShareContext
{
    private readonly CoreWebView2 coreWebView2;
    private readonly ITaskContext taskContext;
    private readonly HttpClient httpClient;
    private readonly IInfoBarService infoBarService;
    private readonly IClipboardProvider clipboardProvider;
    private readonly JsonSerializerOptions jsonSerializerOptions;
    private readonly IFileSystemPickerInteraction fileSystemPickerInteraction;
    private readonly BridgeShareSaveType shareSaveType;

    public BridgeShareContext(CoreWebView2 coreWebView2, ITaskContext taskContext, HttpClient httpClient, IInfoBarService infoBarService, IClipboardProvider clipboardProvider, JsonSerializerOptions jsonSerializerOptions, IFileSystemPickerInteraction fileSystemPickerInteraction, BridgeShareSaveType shareSaveType)
    {
        this.httpClient = httpClient;
        this.taskContext = taskContext;
        this.infoBarService = infoBarService;
        this.clipboardProvider = clipboardProvider;
        this.coreWebView2 = coreWebView2;
        this.jsonSerializerOptions = jsonSerializerOptions;
        this.fileSystemPickerInteraction = fileSystemPickerInteraction;
        this.shareSaveType = shareSaveType;
    }

    public CoreWebView2 CoreWebView2 { get => coreWebView2; }

    public ITaskContext TaskContext { get => taskContext; }

    public HttpClient HttpClient { get => httpClient; }

    public IInfoBarService InfoBarService { get => infoBarService; }

    public IClipboardProvider ClipboardProvider { get => clipboardProvider; }

    public JsonSerializerOptions JsonSerializerOptions { get => jsonSerializerOptions; }

    public IFileSystemPickerInteraction FileSystemPickerInteraction { get => fileSystemPickerInteraction; }

    public BridgeShareSaveType ShareSaveType { get => shareSaveType; }
}
