using Microsoft.EntityFrameworkCore;
using OwlNinja.Database.Models;

namespace OwlNinja.Database
{
    public class BlogContext : DbContext
    {
       
        public DbSet<Post> Posts { get; set; }

        public DbSet<Comment> Comments { get; set; }

        public DbSet<PostTag> PostTags { get; set; }

        public DbSet<Admin> Admins { get; set; }

        public DbSet<Settings> Settings { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
           => optionsBuilder
               .UseMySql(@"Server=localhost;database=owlninja;uid=owlninja;pwd=U!D0s!S4!R0a!vwXKWKhyoWwOaNFw4@H;");
    }
}
