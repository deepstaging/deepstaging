// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Tests.Generators.Effects.Writers;

public class EffectsModuleWriterTests : RoslynTestBase
{
    [Test]
    public async Task ProducesCapabilityInterface()
    {
        var emit = SymbolsFor(
                """
                using System.Threading.Tasks;

                namespace TestApp;

                public interface IEmailService
                {
                    Task SendAsync(string to, string subject, string body);
                    Task<bool> ValidateAsync(string email);
                }

                [Deepstaging.Effects.EffectsModule(typeof(IEmailService))]
                public partial class EmailEffects;
                """)
            .RequireNamedType("EmailEffects")
            .QueryEffectsModules()
            .First()
            .WriteCapabilityInterface();

        await Assert.That(emit).IsSuccessful();
    }

    [Test]
    public async Task ProducesStaticEffectsModule()
    {
        var model = SymbolsFor(
                """
                using System.Threading.Tasks;

                namespace TestApp;

                /// <summary>
                ///    Service for sending emails
                /// </summary>
                public interface IEmailService
                {
                    /// <summary>
                    ///  Sends an email
                    /// </summary>
                    /// <param name="to">The recipient email address</param>
                    /// <param name="subject">The email subject</param>
                    /// <param name="body">The email body</param>
                    Task SendAsync(string to, string subject, string body);
                    Task<bool> ValidateAsync(string email);
                }

                [Deepstaging.Effects.EffectsModule(typeof(IEmailService))]
                public partial class EmailEffects;
                """)
            .RequireNamedType("EmailEffects")
            .QueryEffectsModules()
            .First();

        var effectsModule = model.WriteEffectsModule();

        await Assert.That(effectsModule).IsSuccessful();
        await Verify(effectsModule.Code);

    }

    [Test]
    public async Task ProducesDbContextEffectsModule()
    {
        var model = SymbolsFor(
                """
                using Microsoft.EntityFrameworkCore;

                namespace TestApp;

                public class User { public int Id { get; set; } }

                public class AppDbContext : DbContext
                {
                    public DbSet<User> Users { get; set; } = null!;
                }

                [Deepstaging.Effects.EffectsModule(typeof(AppDbContext), Name = "Database")]
                public partial class MyEffects;
                """)
            .RequireNamedType("MyEffects")
            .QueryEffectsModules()
            .First();

        var effectsModule = model.WriteEffectsModule();

        await Assert.That(effectsModule).IsSuccessful();
        await Verify(effectsModule.Code);
    }
}
