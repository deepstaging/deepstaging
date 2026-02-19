// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Generators.Writers.Effects;

using System.Text;

/// <summary>
/// Emits a documentation-only partial class for effects module containers,
/// providing a comprehensive summary across all modules in the container.
/// </summary>
public static class EffectsDocumentationWriter
{
    extension(ImmutableArray<EffectsModuleModel> modules)
    {
        /// <summary>
        /// Writes documentation-only partial classes for each unique effects container,
        /// grouped by namespace and container name.
        /// </summary>
        public ImmutableArray<(string HintName, OptionalEmit Emit)> WriteEffectsDocumentation()
        {
            var results = ImmutableArray.CreateBuilder<(string, OptionalEmit)>();

            var groups = modules
                .GroupBy(m => (m.Namespace, m.EffectsContainerName));

            foreach (var group in groups)
            {
                var (ns, containerName) = group.Key;
                var groupModules = group.ToImmutableArray();

                var emit = WriteContainerDocumentation(ns, containerName, groupModules);
                var hint = new HintName(ns).Filename("Effects", $"{containerName}.Docs");
                results.Add((hint, emit));
            }

            return results.ToImmutable();
        }
    }

    private static OptionalEmit WriteContainerDocumentation(
        string ns,
        string containerName,
        ImmutableArray<EffectsModuleModel> modules)
    {
        var summary = BuildSummary(containerName, modules);
        var remarks = BuildRemarks(modules);

        var doc = XmlDocumentationBuilder.Create()
            .WithSummary(summary);

        if (remarks is not null)
            doc = doc.WithRemarks(remarks);

        foreach (var module in modules)
            doc = doc.AddSeeAlso($"global::{module.Namespace}.{module.Capability.Interface}");

        return TypeBuilder
            .Parse($"public static partial class {containerName}")
            .InNamespace(ns)
            .WithXmlDoc(_ => doc)
            .Emit();
    }

    private static string BuildSummary(string containerName, ImmutableArray<EffectsModuleModel> modules)
    {
        if (modules.Length == 1)
        {
            var m = modules[0];
            return $"Effects module wrapping <see cref=\"global::{m.Namespace}.{m.TargetTypeName}\"/> as lifted <c>Eff</c> computations.";
        }

        var sb = new StringBuilder();
        sb.AppendLine($"Effects modules for the <c>{containerName}</c> aggregate.");
        sb.AppendLine("<list type=\"bullet\">");

        foreach (var m in modules)
        {
            var desc = m.IsDbContext ? "database context" : "service";
            sb.AppendLine($"<item><term><see cref=\"{m.Name}\"/> - </term><description>Wraps <see cref=\"global::{m.Namespace}.{m.TargetTypeName}\"/> ({desc})</description></item>");
        }

        sb.Append("</list>");
        return sb.ToString();
    }

    private static string? BuildRemarks(ImmutableArray<EffectsModuleModel> modules)
    {
        var instrumented = modules.Where(m => m.Instrumented).ToArray();
        var dbModules = modules.Where(m => m.IsDbContext).ToArray();

        if (instrumented.Length == 0 && dbModules.Length == 0)
            return null;

        var sb = new StringBuilder();

        if (instrumented.Length > 0)
            sb.AppendLine("All effect methods include OpenTelemetry instrumentation via dedicated activity sources.");

        if (dbModules.Length > 0)
            sb.Append($"Database modules provide typed query builders for each <c>DbSet</c> entity set.");

        return sb.ToString().TrimEnd();
    }
}
