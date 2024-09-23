using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SignalR.Models;

namespace chatLab.Context
{
    public class ChatAppDbContext : IdentityDbContext
    {
        public ChatAppDbContext(DbContextOptions<ChatAppDbContext> options) : base(options) { }
        public virtual DbSet<Message> ChatMessages { get; set; }
    }
}