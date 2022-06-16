﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.Web.Hoyolab.DynamicSecret;
using Snap.Hutao.Web.Response;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace Snap.Hutao.Web.Hoyolab.Bbs.User;

/// <summary>
/// 用户信息客户端
/// </summary>
[Injection(InjectAs.Transient)]
internal class UserClient
{
    private readonly IUserService userService;
    private readonly HttpClient httpClient;
    private readonly JsonSerializerOptions jsonSerializerOptions;

    /// <summary>
    /// 构造一个新的用户信息客户端
    /// </summary>
    /// <param name="userService">用户服务</param>
    /// <param name="httpClient">http客户端</param>
    /// <param name="jsonSerializerOptions">Json序列化选项</param>
    public UserClient(IUserService userService,HttpClient httpClient, JsonSerializerOptions jsonSerializerOptions)
    {
        this.userService = userService;
        this.httpClient = httpClient;
        this.jsonSerializerOptions = jsonSerializerOptions;
    }

    /// <summary>
    /// 获取当前用户详细信息
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>详细信息</returns>
    public async Task<UserInfo?> GetUserFullInfoAsync(CancellationToken token = default)
    {
        Response<UserFullInfoWrapper>? resp = await httpClient
            .UsingDynamicSecret()
            .SetUser(userService.CurrentUser)
            .GetFromJsonAsync<Response<UserFullInfoWrapper>>(ApiEndpoints.UserFullInfo, jsonSerializerOptions, token)
            .ConfigureAwait(false);

        return resp?.Data?.UserInfo;
    }

    /// <summary>
    /// 获取当前用户详细信息
    /// </summary>
    /// <param name="uid">米游社Uid</param>
    /// <param name="token">取消令牌</param>
    /// <returns>详细信息</returns>
    public async Task<UserInfo?> GetUserFullInfoAsync(string uid, CancellationToken token = default)
    {
        Response<UserFullInfoWrapper>? resp = await httpClient
            .UsingDynamicSecret()
            .SetUser(userService.CurrentUser)
            .GetFromJsonAsync<Response<UserFullInfoWrapper>>(string.Format(ApiEndpoints.UserFullInfoQuery, uid), jsonSerializerOptions, token)
            .ConfigureAwait(false);

        return resp?.Data?.UserInfo;
    }
}