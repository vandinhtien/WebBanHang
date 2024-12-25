using SV21T1020742.DomainModels;

namespace SV21T1020742.Web.Models
{
	public class CategorySearchResult : PaginationSearchResult
	{
		public List<Category> Data { get; set; }
	}
}
