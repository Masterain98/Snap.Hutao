﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Messaging;
using Microsoft.EntityFrameworkCore;

namespace Snap.Hutao.Core.Database;

/// <summary>
/// 数据库当前项
/// 简化对数据库中选中项的管理
/// </summary>
/// <typeparam name="TEntity">实体的类型</typeparam>
/// <typeparam name="TMessage">消息的类型</typeparam>
internal class DbCurrent<TEntity, TMessage>
    where TEntity : class, ISelectable
    where TMessage : Message.ValueChangedMessage<TEntity>
{
    private readonly DbContext dbContext;
    private readonly DbSet<TEntity> dbSet;
    private readonly IMessenger messenger;

    private TEntity? current;

    /// <summary>
    /// 构造一个新的数据库当前项
    /// </summary>
    /// <param name="dbContext">数据库上下文</param>
    /// <param name="dbSet">数据集</param>
    /// <param name="messenger">消息器</param>
    public DbCurrent(DbContext dbContext, DbSet<TEntity> dbSet, IMessenger messenger)
    {
        this.dbContext = dbContext;
        this.dbSet = dbSet;
        this.messenger = messenger;
    }

    /// <summary>
    /// 当前选中的项
    /// </summary>
    public TEntity? Current
    {
        get => current;
        set
        {
            // prevent useless sets
            if (current == value)
            {
                return;
            }

            // only update when not processing a deletion
            if (value != null)
            {
                if (current != null)
                {
                    current.IsSelected = false;
                    dbSet.Update(current);
                    dbContext.SaveChanges();
                }
            }

            TMessage message = (TMessage)Activator.CreateInstance(typeof(TMessage), current, value)!;
            current = value;

            if (current != null)
            {
                current.IsSelected = true;
                dbSet.Update(current);
                dbContext.SaveChanges();
            }

            messenger.Send(message);
        }
    }
}

/// <summary>
/// 数据库当前项
/// 简化对数据库中选中项的管理
/// </summary>
/// <typeparam name="TObservable">绑定类型</typeparam>
/// <typeparam name="TEntity">实体的类型</typeparam>
/// <typeparam name="TMessage">消息的类型</typeparam>
[SuppressMessage("", "SA1402")]
internal class DbCurrent<TObservable, TEntity, TMessage>
    where TObservable : class
    where TEntity : class, ISelectable
    where TMessage : Message.ValueChangedMessage<TObservable>
{
    private readonly DbContext dbContext;
    private readonly DbSet<TEntity> dbSet;
    private readonly IMessenger messenger;
    private readonly Func<TObservable, TEntity> selector;

    private TObservable? current;

    /// <summary>
    /// 构造一个新的数据库当前项
    /// </summary>
    /// <param name="dbContext">数据库上下文</param>
    /// <param name="dbSet">数据集</param>
    /// <param name="selector">选择器</param>
    /// <param name="messenger">消息器</param>
    public DbCurrent(DbContext dbContext, DbSet<TEntity> dbSet, Func<TObservable, TEntity> selector, IMessenger messenger)
    {
        this.dbContext = dbContext;
        this.dbSet = dbSet;
        this.selector = selector;
        this.messenger = messenger;
    }

    /// <summary>
    /// 当前选中的项
    /// </summary>
    public TObservable? Current
    {
        get => current;
        set
        {
            // prevent useless sets
            if (current == value)
            {
                return;
            }

            // only update when not processing a deletion
            if (value != null)
            {
                if (current != null)
                {
                    TEntity entity = selector(current);
                    entity.IsSelected = false;
                    dbSet.Update(entity);
                    dbContext.SaveChanges();
                }
            }

            TMessage message = (TMessage)Activator.CreateInstance(typeof(TMessage), current, value)!;
            current = value;

            if (current != null)
            {
                TEntity entity = selector(current);
                entity.IsSelected = true;
                dbSet.Update(entity);
                dbContext.SaveChanges();
            }

            messenger.Send(message);
        }
    }
}