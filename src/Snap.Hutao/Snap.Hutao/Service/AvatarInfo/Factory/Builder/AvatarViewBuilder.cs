﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.ViewModel.AvatarProperty;

namespace Snap.Hutao.Service.AvatarInfo.Factory.Builder;

internal sealed class AvatarViewBuilder : IAvatarViewBuilder
{
    public AvatarView View { get; } = new();

    public float Score { get => View.Score; set => View.Score = value; }
}