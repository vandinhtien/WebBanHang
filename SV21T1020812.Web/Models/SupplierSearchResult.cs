using SV21T1020742.DomainModels;

namespace SV21T1020742.Web.Models
{
    public class SupplierSearchResult : PaginationSearchResult
    {
        public List<Supplier> Data { get; set; }
    }
}
