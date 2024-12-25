using SV21T1020742.DomainModels;

namespace SV21T1020742.Web.Models
{
    public class EmployeeSearchResult : PaginationSearchResult
    {
        public List<Employee> Data { get; set; }
    }
}
