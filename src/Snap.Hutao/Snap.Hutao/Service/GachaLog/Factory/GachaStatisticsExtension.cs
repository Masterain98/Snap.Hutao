﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Metadata.Abstraction;
using Snap.Hutao.ViewModel.GachaLog;
using System.Security.Cryptography;
using System.Text;
using Windows.UI;

namespace Snap.Hutao.Service.GachaLog.Factory;

/// <summary>
/// 统计拓展
/// </summary>
internal static class GachaStatisticsExtension
{
    /// <summary>
    /// 求平均值
    /// </summary>
    /// <param name="span">跨度</param>
    /// <returns>平均值</returns>
    public static byte Average(this in Span<byte> span)
    {
        int sum = 0;
        int count = 0;
        foreach (ref readonly byte b in span)
        {
            sum += b;
            count++;
        }

        if (count == 0)
        {
            return 0;
        }

        return unchecked((byte)(sum / count));
    }

    /// <summary>
    /// 完成添加
    /// </summary>
    /// <param name="summaryItems">简述物品列表</param>
    /// <param name="guaranteeOrangeThreshold">五星保底阈值</param>
    public static void CompleteAdding(this List<SummaryItem> summaryItems, int guaranteeOrangeThreshold)
    {
        // we can't trust first item's prev state.
        bool isPreviousUp = true;

        // mark the IsGuarantee
        foreach (SummaryItem item in summaryItems)
        {
            if (item.IsUp && (!isPreviousUp))
            {
                item.IsGuarantee = true;
            }

            isPreviousUp = item.IsUp;
            item.Color = GetColorByName(item.Name);
            item.GuaranteeOrangeThreshold = guaranteeOrangeThreshold;
        }

        // reverse items
        summaryItems.Reverse();
    }

    /// <summary>
    /// 将计数器转换为统计物品列表
    /// </summary>
    /// <typeparam name="TItem">物品类型</typeparam>
    /// <param name="dict">计数器</param>
    /// <returns>统计物品列表</returns>
    public static List<StatisticsItem> ToStatisticsList<TItem>(this Dictionary<TItem, int> dict)
        where TItem : IStatisticsItemSource
    {
        return dict
            .Select(kvp => kvp.Key.ToStatisticsItem(kvp.Value))
            .OrderByDescending(item => item.Count)
            .ToList();
    }

    [SuppressMessage("", "IDE0057")]
    private static Color GetColorByName(string name)
    {
        Span<byte> codes = MD5.HashData(Encoding.UTF8.GetBytes(name));
        return Color.FromArgb(255, codes.Slice(0, 5).Average(), codes.Slice(5, 5).Average(), codes.Slice(10, 5).Average());
    }
}
