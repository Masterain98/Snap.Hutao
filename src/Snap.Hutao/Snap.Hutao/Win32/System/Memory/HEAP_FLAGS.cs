﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Win32.System.Memory;

[Flags]
internal enum HEAP_FLAGS : uint
{
    HEAP_NONE = 0U,
    HEAP_NO_SERIALIZE = 1U,
    HEAP_GROWABLE = 2U,
    HEAP_GENERATE_EXCEPTIONS = 4U,
    HEAP_ZERO_MEMORY = 8U,
    HEAP_REALLOC_IN_PLACE_ONLY = 0x10U,
    HEAP_TAIL_CHECKING_ENABLED = 0x20U,
    HEAP_FREE_CHECKING_ENABLED = 0x40U,
    HEAP_DISABLE_COALESCE_ON_FREE = 0x80U,
    HEAP_CREATE_ALIGN_16 = 0x10000U,
    HEAP_CREATE_ENABLE_TRACING = 0x20000U,
    HEAP_CREATE_ENABLE_EXECUTE = 0x40000U,
    HEAP_MAXIMUM_TAG = 0xFFFU,
    HEAP_PSEUDO_TAG_FLAG = 0x8000U,
    HEAP_TAG_SHIFT = 0x12U,
    HEAP_CREATE_SEGMENT_HEAP = 0x100U,
    HEAP_CREATE_HARDENED = 0x200U,
}