using AuthApi.Models.Domain;

namespace AuthApi.Repositories.Abstract
{
    public interface IAdminService
    {
        Task<List<AllUsers>?> GetAllUsers();
        Task<AllUsers?> GetSingleUser(int id);
    }
}
