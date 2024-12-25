using System.ComponentModel.DataAnnotations;

namespace SV21T1020742.Shop.Models
{
    public class CategoryModel
    {
        [Key]
        public int Id { get; set; }
        [Required, MinLength(4, ErrorMessage = "Yêu cầu nhập Tên Danh mục")]
        public string Name { get; set; }
        [Required, MinLength(4, ErrorMessage = "Yêu cần nhập Mô tả Danh mục")]
        public string Description { get; set; }
        [Required]
        public string Sluy { get; set; }
        public int Status { get; set; }
    }
}
