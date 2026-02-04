// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Deepstaging.ToDo.Services.Data;

/// <summary>
///     Represents a TODO item in the system
/// </summary>
[Index(nameof(IsCompleted))]
[Index(nameof(CreatedAt))]
public class TodoItem
{
    [Key]
    public Guid Id { get; init; }

    [Required]
    [MaxLength(200)]
    public required string Title { get; init; }

    [MaxLength(2000)]
    public string? Description { get; init; }

    public bool IsCompleted { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? CompletedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public int Priority { get; init; }

    [MaxLength(500)]
    public string? Tags { get; init; }
}