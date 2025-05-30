using Microsoft.EntityFrameworkCore;
using LojaDoManoel.API.Models;

namespace LojaDoManoel.API.Data
{
    public class ApiDbContext : DbContext
    {
        public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options)
        {
        }

        public DbSet<PackingJobLog> PackingJobLogs { get; set; }
    }
}