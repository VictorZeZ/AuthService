using AuthService.Tests.TestUtils;
using System.Net.Http.Json;

namespace AuthService.Tests.Integration.Fixtures
{
    /// <summary>
    /// Shared setup for integration tests that need a pre-registered user and valid token.
    /// </summary>
    public class UserFlowFixture : IAsyncLifetime
    {
        public HttpClient Client { get; private set; } = null!;
        public string Email { get; private set; } = null!;
        public string Token { get; private set; } = null!;

        private static bool _initialized = false;
        private static readonly object _lock = new();

        private CustomWebApplicationFactory _factory = null!;

        public async Task InitializeAsync()
        {
            lock (_lock)
            {
                if (_initialized) return;
                _initialized = true;
            }

            // Initialize custom factory
            _factory = new CustomWebApplicationFactory();
            _factory.ClientOptions.BaseAddress = new Uri("http://localhost");
            _factory.Server.PreserveExecutionContext = true;

            Client = _factory.CreateClient();

            // Create a unique user for fixture setup
            Email = $"fixture_{Guid.NewGuid():N}@example.com";

            var registerPayload = new
            {
                username = "fixture_user",
                email = Email,
                password = "MyPass123!",
                deviceInfo = "IntegrationTest"
            };

            var regResp = await Client.PostAsJsonAsync("/api/user/register", registerPayload);

            if (!regResp.IsSuccessStatusCode)
            {
                var body = await regResp.Content.ReadAsStringAsync();
                throw new InvalidOperationException($"Fixture registration failed: {regResp.StatusCode} => {body}");
            }

            var json = await regResp.Content.ReadFromJsonAsync<Dictionary<string, object>>();
            Token = json!["token"].ToString()!;
        }

        public Task DisposeAsync()
        {
            _factory.Dispose();
            Client.Dispose();
            return Task.CompletedTask;
        }
    }
}
