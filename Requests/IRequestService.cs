using System.Threading;
using System.Threading.Tasks;
using gc_bot.Requests.Models;

namespace gc_bot.Requests
{
    /// <summary>
    /// Abstraction for network requests used by the application.
    /// Implement this to perform real HTTP calls.
    /// </summary>
    public interface IRequestService
    {
        /// <summary>
        /// Sends a login request and returns the login response.
        /// </summary>
        Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves user info using an online token.
        /// </summary>
        Task<UserInfoResponse> GetUserInfoAsync(string onlineToken, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves game info using an online token.
        /// </summary>
        /// <param name="onlineToken"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<GameInfoResponse> GetGameInfoAsync(string onlineToken, CancellationToken cancellationToken = default);

        Task<Object> GetStartGameAsync(string onlineToken, CancellationToken cancellationToken = default);
    }
}
