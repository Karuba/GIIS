using Microsoft.EntityFrameworkCore;

namespace GIIS_lab2.Repository
{
    public class RepositoryContext : DbContext
    {
        public DbSet<Contact> Contacts { get; set; } = null!;

        public RepositoryContext(DbContextOptions<RepositoryContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
        protected override void OnModelCreating(ModelBuilder model)
        {
            model.Entity<Contact>().HasData(
                     new Contact() { Id = Guid.NewGuid(), Name = "Alex", Address = "Kolotushkino" },
                     new Contact() { Id = Guid.NewGuid(), Name = "Dranik", Address = "Bombass" }
                );
        }

    }
}
