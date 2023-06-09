﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AuthApi.Models.Domain
{
    public class DatabaseContext :IdentityDbContext<ApplicationUser>
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {

        }
        public DbSet<TokenInfo> TokenInfo { get; set; }
        public DbSet<AllUsers> AllUsers { get; set; }
        public DbSet<UserImages> UserImages { get; set; }
    }
}
