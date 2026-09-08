using System.ComponentModel.DataAnnotations;

namespace BaiTap.Models
{
    public class QuenMatKhauViewModel
    {
        [Required(ErrorMessage = "Email không được để trống")]
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [StringLength(100, ErrorMessage = "Email không được vượt quá 100 ký tự")]
        public string Email { get; set; }
    }
}

