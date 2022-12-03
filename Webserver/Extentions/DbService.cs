using Microsoft.EntityFrameworkCore;
using Webserver.Model;

namespace Webserver.Extentions;

internal class DbService : DbContext
{
	public DbService(DbContextOptions options) : base(options)
	{

	}

	internal DbSet<Product> Products { get; set; }

	internal static DbService? _currentContext = null;
	internal static DbService GetContext()
	{
		if (_currentContext is not null)
			return _currentContext;

		var builder = new DbContextOptionsBuilder().UseInMemoryDatabase("test");
		return new DbService(builder.Options);
	}
}