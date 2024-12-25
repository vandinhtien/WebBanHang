using SV21T1020742.DomainModels;

namespace SV21T1020742.Web.Models
{
    public class CustomerSearchResult : PaginationSearchResult
    {
        public List<Customer> Data { get; set; }
    }
}
