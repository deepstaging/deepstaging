namespace Deepstaging.Roslyn.Tests.Emit;

public class TypeBuilderTests : RoslynTestBase
{
    [Test]
    public async Task Can_emit_simple_class()
    {
        var result = TypeBuilder
            .Class("Customer")
            .InNamespace("MyApp.Domain")
            .WithAccessibility(Accessibility.Public)
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.IsValid(out var validEmit)).IsTrue();
        await Assert.That(validEmit.Code).Contains("public class Customer");
        await Assert.That(validEmit.Code).Contains("namespace MyApp.Domain");
    }

    [Test]
    public async Task Can_emit_interface()
    {
        var result = TypeBuilder
            .Interface("IRepository")
            .InNamespace("MyApp.Core")
            .WithAccessibility(Accessibility.Public)
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("public interface IRepository");
    }

    [Test]
    public async Task Can_emit_struct()
    {
        var result = TypeBuilder
            .Struct("Point")
            .InNamespace("MyApp.Types")
            .WithAccessibility(Accessibility.Public)
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("public struct Point");
    }

    [Test]
    public async Task Emits_valid_compilable_code()
    {
        var result = TypeBuilder
            .Class("Customer")
            .InNamespace("MyApp.Domain")
            .AddUsing("System")
            .Emit();

        await Assert.That(result.Success).IsTrue();

        // Try to compile the generated code
        var compilation = CompilationFor(result.Code!);
        var diagnostics = compilation.GetDiagnostics()
            .Where(d => d.Severity == DiagnosticSeverity.Error);

        await Assert.That(diagnostics).IsEmpty();
    }

    [Test]
    public async Task Can_emit_class_with_attribute()
    {
        var result = TypeBuilder
            .Class("Customer")
            .InNamespace("MyApp.Domain")
            .AddUsing("System")
            .WithAttribute("Serializable")
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("[Serializable]");
        await Assert.That(result.Code).Contains("public class Customer");
    }

    [Test]
    public async Task Can_emit_class_with_attribute_with_arguments()
    {
        var result = TypeBuilder
            .Class("Customer")
            .InNamespace("MyApp.Domain")
            .AddUsing("System")
            .WithAttribute("Obsolete", attr => attr.WithArgument("\"Use CustomerV2 instead\""))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("[Obsolete(\"Use CustomerV2 instead\")]");
    }

    [Test]
    public async Task Can_emit_class_with_multiple_attributes()
    {
        var result = TypeBuilder
            .Class("Customer")
            .InNamespace("MyApp.Domain")
            .AddUsing("System")
            .WithAttribute("Serializable")
            .WithAttribute("Obsolete", attr => attr.WithArgument("\"Deprecated\""))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("[Serializable]");
        await Assert.That(result.Code).Contains("[Obsolete(\"Deprecated\")]");
    }

    [Test]
    public async Task Can_emit_property_with_attribute()
    {
        var result = TypeBuilder
            .Class("Customer")
            .InNamespace("MyApp.Domain")
            .AddProperty("Name", "string", p => p
                .WithAutoPropertyAccessors()
                .WithAttribute("Required"))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("[Required]");
        await Assert.That(result.Code).Contains("public string Name");
    }

    [Test]
    public async Task Can_emit_method_with_attribute()
    {
        var result = TypeBuilder
            .Class("Customer")
            .InNamespace("MyApp.Domain")
            .AddMethod("GetName", m => m
                .WithReturnType("string")
                .WithAttribute("Obsolete")
                .WithExpressionBody("\"\""))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("[Obsolete]");
        await Assert.That(result.Code).Contains("public string GetName()");
    }
}
