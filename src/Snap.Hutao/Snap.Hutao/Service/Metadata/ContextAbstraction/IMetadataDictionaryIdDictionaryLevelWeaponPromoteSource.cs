﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Metadata;
using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Service.Metadata.ContextAbstraction;

internal interface IMetadataDictionaryIdDictionaryLevelWeaponPromoteSource
{
    Dictionary<PromoteId, Dictionary<PromoteLevel, Promote>> IdDictionaryWeaponLevelPromoteMap { get; set; }
}