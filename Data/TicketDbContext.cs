using System;
using System.Threading.Tasks;
using AuthenticationSample.Data.Entites;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationSample.Data
{
    public class TicketDbContext : DbContext, ITicketStore
    {
        public DbSet<Ticket> Tickets { get; set; }
        public TicketDbContext(DbContextOptions<TicketDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        public async Task RemoveAsync(string key)
        {
            var ticket = await Tickets.FirstOrDefaultAsync(x => x.Key == key);
            if (ticket != null)
            {
                Tickets.Remove(ticket);
                await SaveChangesAsync();
            }
        }

        public async Task RenewAsync(string key, AuthenticationTicket ticket)
        {
            var value = TicketSerializer.Default.Serialize(ticket);
            await Tickets.AddAsync(new Ticket
            {
                Key = key,
                Value = value
            });
            await SaveChangesAsync();
        }

        public async Task<AuthenticationTicket> RetrieveAsync(string key)
        {
            var ticket = await Tickets.FirstOrDefaultAsync(x => x.Key == key);
            if (ticket != null)
            {
                return TicketSerializer.Default.Deserialize(ticket.Value);
            }
            return null;
        }

        public async Task<string> StoreAsync(AuthenticationTicket ticket)
        {
            var key = Guid.NewGuid().ToString().Replace("-", "");
            await RenewAsync(key, ticket);
            return key;
        }
    }
}