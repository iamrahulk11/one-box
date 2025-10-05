using System.Data;

namespace domain.Interfaces
{
    public interface IUser
    {
        Task<DataTable> fetchUserDetailsAsync(string ukey);
    }
}
