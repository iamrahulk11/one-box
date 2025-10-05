
using domain.DTOs;

namespace domain.Interfaces
{
    public interface ICommon
    {
        Task userTokenInfoAsync(string ukey, tokenInfoResponseDto token);
    }
}
