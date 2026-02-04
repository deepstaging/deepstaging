// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
using Microsoft.EntityFrameworkCore;

namespace Deepstaging.ToDo.Services.Data;

/// <summary>
///     Database context for TODO items
/// </summary>
public class TodoDbContext(DbContextOptions<TodoDbContext> options) : DbContext(options)
{
    public DbSet<TodoItem> TodoItems => Set<TodoItem>();
}