using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;

namespace Deepstaging.Tests;

internal static class TestInitializer
{
    [ModuleInitializer]
    public static void Initialize()
    {
        // Add references to Deepstaging.Effects so test compilations can resolve attributes
        // Add EF Core references for DbContext tests
        ReferenceConfiguration.AddReferencesFromTypes(
            typeof(RuntimeAttribute),
            typeof(DbContext)
        );
    }
}
