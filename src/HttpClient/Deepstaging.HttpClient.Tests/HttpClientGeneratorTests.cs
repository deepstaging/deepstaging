// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.HttpClient.Generators;

namespace Deepstaging.HttpClient.Tests;

public class HttpClientGeneratorTests : RoslynTestBase
{
    [Test]
    public async Task GeneratesSimpleGetClient()
    {
        const string source = """
                              using Deepstaging.HttpClient;

                              namespace TestApp;

                              public record MyConfig(string ApiKey);

                              [HttpClient<MyConfig>()]
                              public partial class UsersClient
                              {
                                  [Get("/users/{id}")]
                                  private partial User GetUser(int id);
                              }

                              public record User(int Id, string Name);
                              """;

        await GenerateWith<HttpClientGenerator>(source)
            .ShouldGenerate()
            .WithFileContaining("public partial class UsersClient")
            .WithFileContaining("public partial interface IUsersClient")
            .CompilesSuccessfully()
            .VerifySnapshot();
    }
}
