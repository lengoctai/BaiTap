using System.ComponentModel.DataAnnotations;

namespace BaiTap.Models
{
    public class ChuDeViewModel
    {
        public int MaCD { get; set; }

        [Required(ErrorMessage = "Tên chủ đề không được để trống")]
        [Display(Name = "Tên chủ đề")]
        [StringLength(50, ErrorMessage = "Tên chủ đề không được vượt quá 50 ký tự")]
        public string TenChuDe { get; set; }
    }
}
