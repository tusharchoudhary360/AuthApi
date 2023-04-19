using AuthApi.Controllers;
using AuthApi.Models.Domain;
using AuthApi.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;

namespace AuthApi.Repositories.Domain
{
    public class AdminService : IAdminService
    {
        private readonly DatabaseContext _context;
        public AdminService(DatabaseContext context)
        {
            _context = context;
        }
        //get all user details 
        public async Task<List<AllUsers>?> GetAllUsers()
        {
            var user = await _context.AllUsers.ToListAsync();
            return user;
        }

        // get single user datails 
        public async Task<AllUsers?> GetSingleUser(int id)
        {
            var user = await _context.AllUsers.FindAsync(id);
            if (user == null)
            {
                return null;
            }
            return user;
        }
    }
}
