using Microsoft.EntityFrameworkCore;
using PhotoOrganizerLib.Models;

namespace PhotoOrganizerLib.Data
{
    public class PhotoContext : DbContext
    {
        public PhotoContext(DbContextOptions<PhotoContext> options) : base(options) { }

        public DbSet<Photo> Photos { get; set; } = null!;
    }
}