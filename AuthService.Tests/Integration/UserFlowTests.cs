using FluentAssertions;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using AuthService.Tests.Integration.Fixtures;

namespace AuthService.Tests.Integration
{
    /// <summary>
    /// Integration tests verifying user registration, login, profile retrieval, update, and deletion.
    /// Uses UserFlowFixture for shared user/token setup.
    /// </summary>
    [Collection("UserFlow")]
    public class UserFlowTests
    {
        private readonly HttpClient _client;
        private readonly UserFlowFixture _fixture;

        public UserFlowTests(UserFlowFixture fixture)
        {
            _fixture = fixture;
            _client = fixture.Client;
        }

        private async Task<(string email, string token)> RegisterAndLoginNewUserAsync()
        {
            var email = $"integration_{Guid.NewGuid():N}@example.com";

            var registerPayload = new
            {
                username = $"testuser_{Guid.NewGuid():N}".Substring(0, 20),
                email,
                password = "MyPass123!",
                deviceInfo = "IntegrationTest"
            };

            var regResp = await _client.PostAsJsonAsync("/api/user/register", registerPayload);
            regResp.StatusCode.Should().Be(HttpStatusCode.Created, await regResp.Content.ReadAsStringAsync());

            var loginPayload = new
            {
                usernameOrEmail = email,
                password = "MyPass123!",
                deviceInfo = "IntegrationTest"
            };

            var loginResp = await _client.PostAsJsonAsync("/api/user/login", loginPayload);
            loginResp.StatusCode.Should().Be(HttpStatusCode.OK, await loginResp.Content.ReadAsStringAsync());

            var loginBody = await loginResp.Content.ReadFromJsonAsync<Dictionary<string, object>>();
            var token = loginBody!["token"]!.ToString()!;
            return (email, token);
        }

        [Fact(DisplayName = "1️⃣ Register_Should_Return_Created_When_Data_Is_Valid")]
        public async Task Register_Should_Return_Created_When_Data_Is_Valid()
        {
            var email = $"register_{Guid.NewGuid():N}@example.com";
            var payload = new
            {
                username = $"testuser_{Guid.NewGuid():N}".Substring(0, 20),
                email,
                password = "MyPass123!",
                deviceInfo = "IntegrationTest"
            };

            var response = await _client.PostAsJsonAsync("/api/user/register", payload);
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var json = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();
            json.Should().ContainKey("token");
        }

        [Fact(DisplayName = "2️⃣ Login_Should_Return_Token_When_Credentials_Are_Valid")]
        public async Task Login_Should_Return_Token_When_Credentials_Are_Valid()
        {
            var payload = new
            {
                usernameOrEmail = _fixture.Email,
                password = "MyPass123!",
                deviceInfo = "IntegrationTest"
            };

            var response = await _client.PostAsJsonAsync("/api/user/login", payload);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var json = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();
            json.Should().ContainKey("token");
        }

        [Fact(DisplayName = "3️⃣ GetProfile_Should_Return_Profile_When_Authorized")]
        public async Task GetProfile_Should_Return_Profile_When_Authorized()
        {
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _fixture.Token);

            var response = await _client.GetAsync("/api/user/profile");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var json = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();
            json.Should().ContainKey("user");
        }

        [Fact(DisplayName = "4️⃣ UpdateProfile_Should_Return_OK_When_Successful")]
        public async Task UpdateProfile_Should_Return_OK_When_Successful()
        {
            var (email, token) = await RegisterAndLoginNewUserAsync();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var updatePayload = new
            {
                username = $"updated_{Guid.NewGuid():N}".Substring(0, 20),
                email = $"updated_{email}",
                password = "NewPass123!"
            };

            var response = await _client.PutAsJsonAsync("/api/user/update", updatePayload);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var loginPayload = new
            {
                usernameOrEmail = $"updated_{email}",
                password = "NewPass123!",
                deviceInfo = "IntegrationTest"
            };
            var loginResp = await _client.PostAsJsonAsync("/api/user/login", loginPayload);
            loginResp.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact(DisplayName = "5️⃣ DeleteAccount_Should_Return_OK_When_Successful")]
        public async Task DeleteAccount_Should_Return_OK_When_Successful()
        {
            var (email, token) = await RegisterAndLoginNewUserAsync();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.DeleteAsync("/api/user/delete");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var loginPayload = new
            {
                usernameOrEmail = email,
                password = "MyPass123!",
                deviceInfo = "IntegrationTest"
            };
            var loginResp = await _client.PostAsJsonAsync("/api/user/login", loginPayload);
            loginResp.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}
