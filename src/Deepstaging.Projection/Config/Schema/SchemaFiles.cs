// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Projection.Config.Schema;

using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Provides access to schema files discovered from <c>AdditionalFiles</c>.
/// Used by the analyzer to check whether schema files exist and whether they are stale.
/// </summary>
public sealed class SchemaFiles
{
    private readonly Dictionary<string, string?> _hashes;

    private SchemaFiles(Dictionary<string, string?> hashes) => _hashes = hashes;

    /// <summary>
    /// Creates a <see cref="SchemaFiles"/> instance from additional texts provided by the compiler.
    /// Discovers files matching <c>*.schema.json</c> and extracts their embedded hashes.
    /// </summary>
    public static SchemaFiles From(ImmutableArray<AdditionalText> additionalTexts)
    {
        var hashes = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);

        foreach (var text in additionalTexts)
        {
            if (!text.Path.EndsWith(".schema.json", StringComparison.OrdinalIgnoreCase))
                continue;

            var fileName = Path.GetFileName(text.Path);
            var content = text.GetText()?.ToString();
            var hash = content is not null ? SchemaHash.Extract(content) : null;

            hashes[fileName] = hash;
        }

        return new SchemaFiles(hashes);
    }

    /// <summary>
    /// Whether a schema file with the given name exists.
    /// </summary>
    public bool HasFile(string fileName) => _hashes.ContainsKey(fileName);

    /// <summary>
    /// Gets the embedded hash from a schema file, or <c>null</c> if the file
    /// doesn't exist or has no embedded hash.
    /// </summary>
    public string? GetHash(string fileName) =>
        _hashes.TryGetValue(fileName, out var hash) ? hash : null;

    /// <summary>
    /// Whether any schema files were found.
    /// </summary>
    public bool IsEmpty => _hashes.Count == 0;
}
