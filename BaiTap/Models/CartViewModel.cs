using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace BaiTap.Models
{
    // Đại diện cho một mặt hàng trong giỏ
    public class CartItem
    {
        public int MaSach { get; set; }
        public string TenSach { get; set; }
        public string AnhBia { get; set; }
        public decimal DonGia { get; set; }
        public int SoLuong { get; set; }
        public decimal ThanhTien => DonGia * SoLuong;
    }

    // ViewModel cho giỏ hàng
    public class CartViewModel
    {
        public List<CartItem> Items { get; set; } = new List<CartItem>();
        public decimal TongTien => Items.Sum(x => x.ThanhTien);
        public int TongSoLuong => Items.Sum(x => x.SoLuong);
    }

    // ViewModel cho trang thanh toán
    public class CheckoutViewModel
    {
        // Danh sách sản phẩm
        public List<CartItem> Items { get; set; } = new List<CartItem>();
        public decimal TongTien => Items != null ? Items.Sum(x => x.ThanhTien) : 0;

        // Thông tin người nhận
        [Required(ErrorMessage = "Vui lòng nhập họ tên người nhận")]
        [StringLength(100, ErrorMessage = "Họ tên không được vượt quá 100 ký tự")]
        [Display(Name = "Họ tên người nhận")]
        public string HoTenNguoiNhan { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ giao hàng")]
        [StringLength(300, ErrorMessage = "Địa chỉ không được vượt quá 300 ký tự")]
        [Display(Name = "Địa chỉ giao hàng")]
        public string DiaChiGiaoHang { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [RegularExpression(@"^(0[3|5|7|8|9])+([0-9]{8})$",
            ErrorMessage = "Số điện thoại không hợp lệ (VD: 0901234567)")]
        [Display(Name = "Số điện thoại")]
        public string SoDienThoai { get; set; }

        [Display(Name = "Ghi chú")]
        [StringLength(500, ErrorMessage = "Ghi chú không được vượt quá 500 ký tự")]
        public string GhiChu { get; set; }

        // Phương thức thanh toán
        [Required(ErrorMessage = "Vui lòng chọn phương thức thanh toán")]
        [Display(Name = "Phương thức thanh toán")]
        public string PhuongThucThanhToan { get; set; }
    }

    // ViewModel cho trang xác nhận đặt hàng thành công
    public class DatHangThanhCongViewModel
    {
        public int MaDonHang { get; set; }
        public string HoTenNguoiNhan { get; set; }
        public string DiaChiGiaoHang { get; set; }
        public string SoDienThoai { get; set; }
        public string PhuongThucThanhToan { get; set; }
        public DateTime NgayDat { get; set; }
        public decimal TongTien { get; set; }
        public List<CartItem> Items { get; set; } = new List<CartItem>();
    }
}
