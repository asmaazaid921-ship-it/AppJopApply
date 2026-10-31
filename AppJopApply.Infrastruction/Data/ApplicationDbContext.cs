using AppJopApply.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AppJopApply.Infrastruction.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Job> Jobs { get; set; }
        public DbSet<Candidate> Candidates { get; set; }
        public DbSet<JobCandidateApplication> JobCandidateApplications { get; set; }
    }
}
