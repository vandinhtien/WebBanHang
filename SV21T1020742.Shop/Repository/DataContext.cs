using Microsoft.EntityFrameworkCore;
using SV21T1020742.Shop.Models;

namespace SV21T1020742.Shop.Repository
{
	public class DataContext : DbContext
	{
		public DataContext(DbContextOptions<DbContext> options) : base(options) 
		{ 
			
		}
		public DbSet<BrandModel> Brands { get; set; }
		public DbSet<ProductsModel> Products { get; set; }
		public DbSet<CategoryModel> Categories { get; set; }
	}
}
