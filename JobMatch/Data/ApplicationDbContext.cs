using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using JobMatch.Models;

namespace JobMatch.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<JobMatch.Models.Job> Job { get; set; }
        public DbSet<JobMatch.Models.Company> Company { get; set; }
        public DbSet<JobMatch.Models.UserViewModel> UserViewModel { get; set; }
    }
}
