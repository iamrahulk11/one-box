using domain.DTOs;
using domain.DTOs.Requests.Login;
using System.Data;

namespace domain.Interfaces
{
    public interface ILogin
    {
        Task<DataSet> loginByPasswordAsync(LoginRequestDto request_body);
        Task<DataTable> createUserAsync(userCreationRequestDto request_body, tokenInfoResponseDto token);
        Task<DataTable> loginChecksAsync(LoginRequestDto request_body);
        Task<DataTable> updateLoginAttempts(string ukey);
    }
}
