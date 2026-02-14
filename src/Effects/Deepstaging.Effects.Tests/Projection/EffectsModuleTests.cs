// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Effects.Tests.Projection;

public class EffectsModuleTests : RoslynTestBase
{
    [Test]
    public async Task Name_UsesCustomName_WhenSpecified()
    {
        var model = SymbolsFor(
                """
                using Deepstaging;
                using System.Threading.Tasks;

                namespace TestApp;

                public interface IEmailService
                {
                     Task SendAsync(string to, string body);
                     Task<bool> ValidateAsync(string email);
                }

                [EffectsModule(typeof(IEmailService), Name = "Emails")]
                public partial class EmailEffects;
                """)
            .RequireNamedType("EmailEffects")
            .QueryEffectsModules()
            .First();

        await Assert.That(model.Name).IsEqualTo("Emails");
        await Assert.That(model.EffectsContainerName).IsEqualTo("EmailEffects");
        await Assert.That(model.Namespace).IsEqualTo("TestApp");
        await Assert.That(model.Accessibility).IsEqualTo("public");
        await Assert.That(model.TargetTypeName).IsEqualTo("IEmailService");
        await Assert.That(model.Instrumented).IsTrue();
        await Assert.That(model.IsDbContext).IsFalse();
        await Assert.That(model.DbSets).IsEmpty();
        await Assert.That(model.Methods.Length).IsEqualTo(2);
        await Assert.That(model.Capability.DependencyType.PropertyName).IsEqualTo("EmailService");
        await Assert.That(model.Capability.Interface).IsEqualTo("IHasEmailService");

        var sendMethod = model.Methods.First(m => m.EffectName == "SendAsync");
        await Assert.That(sendMethod.SourceMethodName).IsEqualTo("SendAsync");
        await Assert.That(sendMethod.EffResultType).IsEqualTo("Unit");
        await Assert.That(sendMethod.LiftingStrategy).IsEqualTo(EffectLiftingStrategy.AsyncVoid);
        await Assert.That(sendMethod.Parameters.Length).IsEqualTo(2);
        await Assert.That(sendMethod.Parameters[0].Name).IsEqualTo("to");
        await Assert.That(sendMethod.Parameters[0].Type).IsEqualTo("string");
        await Assert.That(sendMethod.Parameters[1].Name).IsEqualTo("body");
        await Assert.That(sendMethod.Parameters[1].Type).IsEqualTo("string");

        var validateMethod = model.Methods.First(m => m.EffectName == "ValidateAsync");
        await Assert.That(validateMethod.SourceMethodName).IsEqualTo("ValidateAsync");
        await Assert.That(validateMethod.EffResultType).IsEqualTo("bool");
        await Assert.That(validateMethod.LiftingStrategy).IsEqualTo(EffectLiftingStrategy.AsyncValue);
        await Assert.That(validateMethod.Parameters.Length).IsEqualTo(1);
        await Assert.That(validateMethod.Parameters[0].Name).IsEqualTo("email");
        await Assert.That(validateMethod.Parameters[0].Type).IsEqualTo("string");
    }
}
