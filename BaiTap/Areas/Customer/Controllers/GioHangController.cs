using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BaiTap.Models;
using Newtonsoft.Json;

namespace BaiTap.Controllers
{
    public class GioHangController : Controller
    {
        ThuVienSachEntities db = new ThuVienSachEntities();

        // ─────────────────────────────────────────────────────────────
        // Helpers
        // ─────────────────────────────────────────────────────────────

        /// <summary>Lấy giỏ hàng từ Session (hoặc tạo mới nếu chưa có).</summary>
        private List<CartItem> LayGioHang()
        {
            var json = Session["GioHang"] as string;
            if (string.IsNullOrEmpty(json))
                return new List<CartItem>();
            return JsonConvert.DeserializeObject<List<CartItem>>(json) ?? new List<CartItem>();
        }

        /// <summary>Lưu giỏ hàng vào Session.</summary>
        private void LuuGioHang(List<CartItem> items)
        {
            Session["GioHang"] = JsonConvert.SerializeObject(items);
            // Cập nhật tổng số lượng để hiển thị trên header
            Session["SoLuongGioHang"] = items.Sum(x => x.SoLuong);
        }

        // ─────────────────────────────────────────────────────────────
        // XemGioHang – GET /GioHang/Index
        // ─────────────────────────────────────────────────────────────
        public ActionResult Index()
        {
            var items = LayGioHang();
            var vm = new CartViewModel { Items = items };
            return View(vm);
        }

        // ─────────────────────────────────────────────────────────────
        // ThemVaoGio – POST /GioHang/ThemVaoGio
        // ─────────────────────────────────────────────────────────────
        [HttpPost]
        public ActionResult ThemVaoGio(int maSach, int soLuong = 1)
        {
            SACH sach = db.SACHes.Find(maSach);
            if (sach == null)
                return Json(new { success = false, message = "Sách không tồn tại." });

            if (sach.Soluongton == null || sach.Soluongton <= 0)
                return Json(new { success = false, message = "Sách hiện đã hết hàng." });

            if (soLuong <= 0) soLuong = 1;

            var items = LayGioHang();
            var existing = items.FirstOrDefault(x => x.MaSach == maSach);

            if (existing != null)
            {
                int tongSauKhiThem = existing.SoLuong + soLuong;
                if (tongSauKhiThem > sach.Soluongton.Value)
                {
                    return Json(new
                    {
                        success = false,
                        message = $"Chỉ còn {sach.Soluongton} quyển trong kho."
                    });
                }
                existing.SoLuong = tongSauKhiThem;
            }
            else
            {
                if (soLuong > sach.Soluongton.Value)
                    soLuong = sach.Soluongton.Value;

                items.Add(new CartItem
                {
                    MaSach = sach.Masach,
                    TenSach = sach.Tensach,
                    AnhBia = sach.Anhbia,
                    DonGia = sach.Giaban ?? 0,
                    SoLuong = soLuong
                });
            }

            LuuGioHang(items);

            return Json(new
            {
                success = true,
                message = "Đã thêm sách vào giỏ hàng!",
                soLuongGioHang = items.Sum(x => x.SoLuong)
            });
        }

        // ─────────────────────────────────────────────────────────────
        // CapNhatSoLuong – POST /GioHang/CapNhatSoLuong
        // ─────────────────────────────────────────────────────────────
        [HttpPost]
        public ActionResult CapNhatSoLuong(int maSach, int soLuong)
        {
            var items = LayGioHang();
            var item = items.FirstOrDefault(x => x.MaSach == maSach);

            if (item == null)
                return Json(new { success = false, message = "Sản phẩm không có trong giỏ." });

            if (soLuong <= 0)
            {
                items.Remove(item);
            }
            else
            {
                // Kiểm tra tồn kho
                SACH sach = db.SACHes.Find(maSach);
                if (sach != null && soLuong > (sach.Soluongton ?? 0))
                    soLuong = sach.Soluongton ?? 1;

                item.SoLuong = soLuong;
            }

            LuuGioHang(items);

            decimal tongTien = items.Sum(x => x.ThanhTien);
            decimal thanhTienItem = item != null ? item.ThanhTien : 0;

            return Json(new
            {
                success = true,
                thanhTienItem = thanhTienItem.ToString("N0"),
                tongTien = tongTien.ToString("N0"),
                soLuongGioHang = items.Sum(x => x.SoLuong)
            });
        }

        // ─────────────────────────────────────────────────────────────
        // XoaKhoiGio – POST /GioHang/XoaKhoiGio
        // ─────────────────────────────────────────────────────────────
        [HttpPost]
        public ActionResult XoaKhoiGio(int maSach)
        {
            var items = LayGioHang();
            var item = items.FirstOrDefault(x => x.MaSach == maSach);

            if (item != null)
                items.Remove(item);

            LuuGioHang(items);

            return Json(new
            {
                success = true,
                soLuongGioHang = items.Sum(x => x.SoLuong),
                tongTien = items.Sum(x => x.ThanhTien).ToString("N0")
            });
        }

        // ─────────────────────────────────────────────────────────────
        // ThanhToan GET – hiển thị form xác nhận
        // ─────────────────────────────────────────────────────────────
        public ActionResult ThanhToan()
        {
            // Bắt buộc đăng nhập
            if (Session["MaKH"] == null)
            {
                TempData["WarningMessage"] = "Vui lòng đăng nhập để tiếp tục thanh toán.";
                return RedirectToAction("DangNhap", "Home",
                    new { returnUrl = Url.Action("ThanhToan", "GioHang") });
            }

            var items = LayGioHang();
            if (items == null || items.Count == 0)
            {
                TempData["WarningMessage"] = "Giỏ hàng của bạn đang trống.";
                return RedirectToAction("Index");
            }

            int maKH = (int)Session["MaKH"];
            KHACHHANG kh = db.KHACHHANGs.Find(maKH);

            var vm = new CheckoutViewModel
            {
                Items = items,
                HoTenNguoiNhan = kh?.HoTen,
                DiaChiGiaoHang = kh?.DiachiKH,
                SoDienThoai = kh?.DienthoaiKH
            };

            return View(vm);
        }

        // ─────────────────────────────────────────────────────────────
        // ThanhToan POST – xử lý đặt hàng
        // ─────────────────────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ThanhToan(CheckoutViewModel model)
        {
            // Bắt buộc đăng nhập
            if (Session["MaKH"] == null)
            {
                TempData["WarningMessage"] = "Vui lòng đăng nhập để tiếp tục thanh toán.";
                return RedirectToAction("DangNhap", "Home");
            }

            // Lấy lại giỏ hàng (không để người dùng giả mạo danh sách từ form)
            var items = LayGioHang();
            model.Items = items;

            if (items == null || items.Count == 0)
            {
                ModelState.AddModelError("", "Giỏ hàng của bạn đang trống.");
                return View(model);
            }

            if (!ModelState.IsValid)
                return View(model);

            int maKH = (int)Session["MaKH"];

            // ── Kiểm tra tồn kho trước khi đặt ──────────────────────
            foreach (var item in items)
            {
                SACH sach = db.SACHes.Find(item.MaSach);
                if (sach == null)
                {
                    ModelState.AddModelError("",
                        $"Sách \"{item.TenSach}\" không còn tồn tại trong hệ thống.");
                    return View(model);
                }
                if ((sach.Soluongton ?? 0) < item.SoLuong)
                {
                    ModelState.AddModelError("",
                        $"Sách \"{item.TenSach}\" chỉ còn {sach.Soluongton} quyển trong kho.");
                    return View(model);
                }
            }

            // ── Tạo đơn hàng ─────────────────────────────────────────
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    bool daThanhToan = model.PhuongThucThanhToan != "CashOnDelivery";

                    var donHang = new DONDATHANG
                    {
                        MaKH = maKH,
                        Ngaydat = DateTime.Now,
                        Dathanhtoan = daThanhToan,
                        Tinhtranggiaohang = false
                    };

                    db.DONDATHANGs.Add(donHang);
                    db.SaveChanges(); // sinh MaDonHang

                    // ── Tạo chi tiết đơn hàng & giảm tồn kho ───────
                    foreach (var item in items)
                    {
                        db.CHITIETDONTHANGs.Add(new CHITIETDONTHANG
                        {
                            MaDonHang = donHang.MaDonHang,
                            Masach = item.MaSach,
                            Soluong = item.SoLuong,
                            Dongia = item.DonGia
                        });

                        // Giảm tồn kho
                        SACH sach = db.SACHes.Find(item.MaSach);
                        sach.Soluongton -= item.SoLuong;
                    }

                    db.SaveChanges();
                    transaction.Commit();

                    // ── Xóa giỏ hàng ────────────────────────────────
                    Session["GioHang"] = null;
                    Session["SoLuongGioHang"] = 0;

                    // ── Lưu thông tin để hiển thị trang thành công ──
                    var thanhCongVm = new DatHangThanhCongViewModel
                    {
                        MaDonHang = donHang.MaDonHang,
                        HoTenNguoiNhan = model.HoTenNguoiNhan,
                        DiaChiGiaoHang = model.DiaChiGiaoHang,
                        SoDienThoai = model.SoDienThoai,
                        PhuongThucThanhToan = model.PhuongThucThanhToan,
                        NgayDat = donHang.Ngaydat ?? DateTime.Now,
                        TongTien = items.Sum(x => x.ThanhTien),
                        Items = items
                    };

                    TempData["DatHangThanhCong"] = JsonConvert.SerializeObject(thanhCongVm);
                    return RedirectToAction("DatHangThanhCong");
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    ModelState.AddModelError("",
                        "Đã xảy ra lỗi khi xử lý đơn hàng. Vui lòng thử lại.");
                    return View(model);
                }
            }
        }

        // ─────────────────────────────────────────────────────────────
        // DatHangThanhCong – trang xác nhận sau khi đặt hàng
        // ─────────────────────────────────────────────────────────────
        public ActionResult DatHangThanhCong()
        {
            var json = TempData["DatHangThanhCong"] as string;
            if (string.IsNullOrEmpty(json))
                return RedirectToAction("Index", "Home");

            var vm = JsonConvert.DeserializeObject<DatHangThanhCongViewModel>(json);
            return View(vm);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
