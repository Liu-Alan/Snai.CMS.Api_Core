﻿using Microsoft.EntityFrameworkCore;
using Snai.CMS.Api_Core.Entities.CMS;

namespace Snai.CMS.Api_Core.DataAccess
{
    public class CMSContext: DbContext
    {
        public CMSContext(DbContextOptions<CMSContext> options) : base(options) { }

        public DbSet<Admin> Admins { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Token> Tokens { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<RoleModule> RoleModules { get; set; }
    }
}
