using Microsoft.EntityFrameworkCore;
using System;
namespace outStagram.Models
{
    public class PostContext : DbContext
    {
        public DbSet<Post> Posts { get; set; }

        public PostContext(DbContextOptions<PostContext> options) : base(options)
        {
        }
    }
}
