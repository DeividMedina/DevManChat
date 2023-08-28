using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Shared.Chat;
using System.Reflection.Emit;

namespace Web.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<ChatRoom> ChatRooms { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ChatMessage>()
                .HasKey(m => m.Id);

            builder.Entity<ChatRoom>()
                .HasKey(m => m.Id);
           
            builder.Entity<ChatMessage>()
              .HasOne(m => m.User)
              .WithMany()
              .HasForeignKey(m => m.UserId);

            builder.Entity<ChatMessage>()
              .HasOne(m => m.ChatRoom)
              .WithMany(r => r.ChatMessages)
              .HasForeignKey(m => m.ChatRoomId)
              .OnDelete(DeleteBehavior.Cascade);
        }
    }
}