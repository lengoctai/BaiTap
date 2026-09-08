using System.ComponentModel.DataAnnotations;

namespace BaiTap.Models
{
    public class DangNhapViewModel
    {
        [Required(ErrorMessage = "Tài khoản không được để trống")]
        [Display(Name = "Tài khoản")]
        public string Taikhoan { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [Display(Name = "Mật khẩu")]
        [DataType(DataType.Password)]
        public string Matkhau { get; set; }

        [Display(Name = "Ghi nhớ đăng nhập")]
        public bool RememberMe { get; set; }
    }
}

