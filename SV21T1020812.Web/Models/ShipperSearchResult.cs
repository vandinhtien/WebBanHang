using SV21T1020742.DomainModels;

namespace SV21T1020742.Web.Models
{
	public class ShipperSearchResult : PaginationSearchResult
	{
		public List<Shipper> Data { get; set; }
	}
}
