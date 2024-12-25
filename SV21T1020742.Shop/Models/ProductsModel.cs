using System.ComponentModel.DataAnnotations;

namespace SV21T1020742.Shop.Models
{
    public class ProductsModel
    {
        [Key]
        public int Id { get; set; }
		[Required, MinLength(4, ErrorMessage = "Yêu cầu nhập Tên Sẩn phẩm")]
		public string Name { get; set; }
        public string Slug { get; set; }
		[Required, MinLength(4, ErrorMessage = "Yêu cầu nhập Tên Danh mục")]
		public string Description { get; set; }
		[Required, MinLength(4, ErrorMessage = "Yêu cầu nhập Giá Sản phẩm")]
		public decimal Price { get; set; }
        public int BrandId { get; set; }
        public int CategoryId { get; set; }
        public CategoryModel Category { get; set; }
        public BrandModel Brand { get; set; }
    }
}
