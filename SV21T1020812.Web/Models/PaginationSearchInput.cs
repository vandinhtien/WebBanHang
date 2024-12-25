namespace SV21T1020742.Web.Models
{
    public class PaginationSearchInput
    {
        // Trang cần hiển thị
        public int Page { get; set; } = 1;

        //Số dòng hiển thị trên mỗi trang
        public int PageSize { get; set; }

        // Chuỗi giá trị cần tìm kiếm
        public string SearchValue { get; set; } = "";

    }
}
