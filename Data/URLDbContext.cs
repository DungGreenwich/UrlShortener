using Microsoft.EntityFrameworkCore;
using URLShortener.Models;
using URLShortener.Constants;

namespace URLShortener.Data
{
	public class URLDbContext : DbContext
	{
		public URLDbContext(DbContextOptions<URLDbContext> options)
		   : base(options)
		{
		}

		// Do some validation for each URL object created
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			// Example configuration
			modelBuilder.Entity<URL>()
				.Property(t => t.OriginalUrl)
				.IsRequired()
				.HasMaxLength(MaxLengths.OriginalURL);
		}

		public DbSet<URL> URLs { get; set; }
	}
}
