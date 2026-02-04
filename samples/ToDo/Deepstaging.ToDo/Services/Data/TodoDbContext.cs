using Microsoft.EntityFrameworkCore;

namespace Todo.Data;

/// <summary>
///     Database context for TODO items
/// </summary>
public class TodoDbContext(DbContextOptions<TodoDbContext> options) : DbContext(options)
{
    public DbSet<TodoItem> TodoItems => Set<TodoItem>();
}