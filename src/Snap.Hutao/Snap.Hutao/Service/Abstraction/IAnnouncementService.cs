﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hoyolab;
using Snap.Hutao.Web.Hoyolab.Hk4e.Common.Announcement;

namespace Snap.Hutao.Service.Abstraction;

/// <summary>
/// 公告服务
/// </summary>
[HighQuality]
internal interface IAnnouncementService
{
    /// <summary>
    /// 异步获取游戏公告与活动,通常会进行缓存
    /// </summary>
    /// <param name="languageCode">语言代码</param>
    /// <param name="region">服务器</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>公告包装器</returns>
    ValueTask<AnnouncementWrapper> GetAnnouncementWrapperAsync(string languageCode, Region region, CancellationToken cancellationToken = default);
}