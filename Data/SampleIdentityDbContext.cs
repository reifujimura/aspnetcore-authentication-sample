using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationSample.Data
{
    public class SampleIdentityDbContext : IdentityDbContext
    {
        public SampleIdentityDbContext(DbContextOptions<SampleIdentityDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
    }
}