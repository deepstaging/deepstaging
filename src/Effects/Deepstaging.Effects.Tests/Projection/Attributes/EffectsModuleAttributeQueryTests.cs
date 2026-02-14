// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Effects.Projection.Attributes;
using Deepstaging.Roslyn;

namespace Deepstaging.Effects.Tests.Projection.Attributes;

public class EffectsModuleAttributeQueryTests : RoslynTestBase
{
    [Test]
    public async Task Name_DefaultsToTargetTypeName_WhenNotSpecified()
    {
        var query = SymbolsFor(
                """
                using Deepstaging;
                using System.Threading.Tasks;

                namespace TestApp;

                public interface IEmailService;

                [EffectsModule(typeof(IEmailService))]
                public partial class EmailEffects;
                """)
            .RequireNamedType("EmailEffects")
            .GetAttribute<EffectsModuleAttribute>()
            .Map(attr => attr.AsQuery<EffectsModuleAttributeQuery>())
            .OrThrow("Expected EffectsModuleAttribute");

        await Assert.That(query.Name).IsEqualTo("EmailService");
    }

    [Test]
    public async Task Name_UsesCustomName_WhenSpecified()
    {
        var query = SymbolsFor(
                """
                using Deepstaging;
                using System.Threading.Tasks;

                namespace TestApp;

                public interface IEmailService;

                [EffectsModule(typeof(IEmailService), Name = "Emails")]
                public partial class EmailEffects;
                """)
            .RequireNamedType("EmailEffects")
            .GetAttribute<EffectsModuleAttribute>()
            .Map(attr => attr.AsQuery<EffectsModuleAttributeQuery>())
            .OrThrow("Expected EffectsModuleAttribute");

        await Assert.That(query.Name).IsEqualTo("Emails");
    }

    [Test]
    public async Task TargetType_Throws_When_TypeNotFound()
    {
        var query = SymbolsFor(
                """
                using Deepstaging;
                using System.Threading.Tasks;

                namespace TestApp;

                [EffectsModule(typeof(IEmailService))]
                public partial class EmailEffects;
                """)
            .RequireNamedType("EmailEffects")
            .GetAttribute<EffectsModuleAttribute>()
            .Map(attr => attr.AsQuery<EffectsModuleAttributeQuery>())
            .OrThrow("Expected EffectsModuleAttribute");

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
        {
            var _ = query.TargetType;
            return Task.CompletedTask;
        });
    }
}
