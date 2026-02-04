using Microsoft.EntityFrameworkCore;

namespace Deepstaging.ToDo.Services.Data;

/// <summary>
///     Database context for TODO items
/// </summary>
public class TodoDbContext(DbContextOptions<TodoDbContext> options) : DbContext(options)
{
    public DbSet<TodoItem> TodoItems => Set<TodoItem>();
}