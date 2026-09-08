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

        [Required(ErrorMessage = "Email không được để trống")]
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [StringLength(100, ErrorMessage = "Email không được vượt quá 100 ký tự")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [Display(Name = "Số điện thoại")]
        [StringLength(50, ErrorMessage = "Số điện thoại không được vượt quá 50 ký tự")]
        [RegularExpression(@"^[0-9]{10,11}$", ErrorMessage = "Số điện thoại phải là số và có từ 10-11 chữ số")]
        public string DienThoai { get; set; }

        [Required(ErrorMessage = "Địa chỉ không được để trống")]
        [Display(Name = "Địa chỉ")]
        [StringLength(100, ErrorMessage = "Địa chỉ không được vượt quá 100 ký tự")]
        public string Diachi { get; set; }
    }
}

