using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Borsa.WebUI.Identity
{
	public class ApplicationIdentityDbContext : IdentityDbContext<ApplicationUser>
	{
		public ApplicationIdentityDbContext() : base(new DbContextOptionsBuilder<ApplicationIdentityDbContext>().UseSqlServer("server =.\\SQLEXPRESS; Database=BorsaDb;integrated security=true; TrustServerCertificate=True; ").Options)
		{

		}
     }

	//public ApplicationIdentityDbContext(DbContextOptions<ApplicationIdentityDbContext> options) : base(options)
	//{

	//}
}

