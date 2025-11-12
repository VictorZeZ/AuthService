using AuthService.Tests.TestUtils;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace AuthService.Tests.Integration
{
    /// <summary>
    /// Minimal integration test to validate the /register endpoint.
    /// </summary>
    public class UserRegistrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public UserRegistrationTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact(DisplayName = "Register_Should_Return_Created_When_Data_Is_Valid")]
        public async Task Register_Should_Return_Created_When_Data_Is_Valid()
        {
            var payload = new
            {
                username = $"integration_{Guid.NewGuid():N}".Substring(0, 20),
                email = $"integration_{Guid.NewGuid():N}@example.com",
                password = "MyPass123!",
                deviceInfo = "IntegrationTest"
            };

            var response = await _client.PostAsJsonAsync("/api/user/register", payload);
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var responseBody = await response.Content.ReadAsStringAsync();
            responseBody.Should().Contain("token");
        }
    }
}
