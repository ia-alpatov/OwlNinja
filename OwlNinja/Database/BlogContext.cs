using Microsoft.EntityFrameworkCore;
using OwlNinja.Database.Models;

namespace OwlNinja.Database
{
    public class BlogContext : DbContext
    {
        public BlogContext(DbContextOptions opt) : base(opt) { }

        public DbSet<Post> Posts { get; set; }

        public DbSet<Comment> Comments { get; set; }

        public DbSet<PostTag> PostTags { get; set; }

        public DbSet<Admin> Admins { get; set; }

        public DbSet<Settings> Settings { get; set; }

        
    }
}
