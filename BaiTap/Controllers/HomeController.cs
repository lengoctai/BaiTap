using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BaiTap.Models;
using BaiTap.Helpers;
using PagedList.Mvc;
using PagedList;
using CaptchaMvc.HtmlHelpers;


namespace BaiTap.Controllers
{
    public class HomeController : Controller
    {
        ThuVienSachEntities db = new ThuVienSachEntities();
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }
        public PartialViewResult Header_Area()
        {
            return PartialView();
        }
        public PartialViewResult Slider_With_Banner()
        {
            return PartialView();

        }
        public PartialViewResult customer_Support()
        {
            return PartialView();
        }
        public PartialViewResult Product_Three()
        {
            var listSachNp = db.SACHes.Where(x => x.Noibat == true).Take(10).ToList();
            return PartialView(listSachNp);

        }
        public PartialViewResult Banner_Wrap()
        {
            return PartialView();
        }

        // public PartialViewResult Banner_With_List_Product()
        //{
        //  return PartialView();
        //}
       
        public PartialViewResult Banner_With_List_Product1()
        {
            return PartialView();
        }
        public PartialViewResult Banner_Wrap1()
        {
            return PartialView();
        }
        public PartialViewResult Product_Three1()
        {
            return PartialView();
        }
        public PartialViewResult Footer()
        {
            return PartialView();
        }
        public PartialViewResult Quick_View()
        {
            return PartialView();
        }
        // GET: Home/DangKy
        public ActionResult DangKy()
        {
            return View();
        }

        // POST: Home/DangKy
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DangKy(DangKyViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra trùng tài khoản
                if (db.KHACHHANGs.Any(x => x.Taikhoan == viewModel.Taikhoan))
                {
                    ModelState.AddModelError("Taikhoan", "Tài khoản đã tồn tại. Vui lòng chọn tài khoản khác.");
                    return View(viewModel);
                }

                // Kiểm tra trùng email
                if (db.KHACHHANGs.Any(x => x.Email == viewModel.Email))
                {
                    ModelState.AddModelError("Email", "Email đã được sử dụng. Vui lòng sử dụng email khác.");
                    return View(viewModel);
                }

                // Kiểm tra trùng số điện thoại
                if (db.KHACHHANGs.Any(x => x.DienthoaiKH == viewModel.DienthoaiKH))
                {
                    ModelState.AddModelError("DienthoaiKH", "Số điện thoại đã được sử dụng.");
                    return View(viewModel);
                }
                if (!this.IsCaptchaValid("Captcha is not valid"))
                {
                    ModelState.AddModelError("Captcha", "Sai mã Captcha");

                    return View(viewModel);
                }

                // Tạo khách hàng mới
                KHACHHANG khachHang = new KHACHHANG
                {
                    HoTen = viewModel.HoTen,
                    Email = viewModel.Email,
                    DienthoaiKH = viewModel.DienthoaiKH,
                    DiachiKH = viewModel.DiachiKH,
                    Taikhoan = viewModel.Taikhoan,
                    Matkhau = PasswordHelper.HashPassword(viewModel.Matkhau) // Hash mật khẩu trước khi lưu
                };

                db.KHACHHANGs.Add(khachHang);
                db.SaveChanges();

                TempData["SuccessMessage"] = "Đăng ký thành công! Bạn có thể đăng nhập ngay bây giờ.";
                return RedirectToAction("DangNhap");
            }

            return View(viewModel);

        }

        // GET: Home/DangNhap
        public ActionResult DangNhap()
        {
            return View();
        }

        // POST: Home/DangNhap
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DangNhap(DangNhapViewModel viewModel, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                // Tìm khách hàng theo tài khoản
                var khachHang = db.KHACHHANGs.FirstOrDefault(x => x.Taikhoan == viewModel.Taikhoan);

                // Verify mật khẩu đã hash
                if (khachHang != null && PasswordHelper.VerifyPassword(viewModel.Matkhau, khachHang.Matkhau))
                {
                    // Lưu thông tin đăng nhập vào Session
                    Session["MaKH"] = khachHang.MaKH;
                    Session["HoTen"] = khachHang.HoTen;
                    Session["TaiKhoan"] = khachHang.Taikhoan;

                    // Nếu có RememberMe, có thể lưu vào Cookie (tùy chọn)
                    if (viewModel.RememberMe)
                    {
                        HttpCookie cookie = new HttpCookie("RememberMe");
                        cookie["TaiKhoan"] = khachHang.Taikhoan;
                        cookie.Expires = DateTime.Now.AddDays(30);
                        Response.Cookies.Add(cookie);
                    }

                    TempData["SuccessMessage"] = "Đăng nhập thành công! Chào mừng " + khachHang.HoTen;

                    // Chuyển hướng đến trang trước đó hoặc trang chủ
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Tài khoản hoặc mật khẩu không đúng!");
                }
            }

            return View(viewModel);
        }

        // GET: Home/DangXuat
        public ActionResult DangXuat()
        {
            Session.Clear();
            Session.Abandon();

            // Xóa cookie RememberMe nếu có
            if (Request.Cookies["RememberMe"] != null)
            {
                HttpCookie cookie = new HttpCookie("RememberMe");
                cookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(cookie);
            }

            TempData["SuccessMessage"] = "Đăng xuất thành công!";
            return RedirectToAction("Index", "Home");
        }

        // GET: Home/QuenMatKhau
        public ActionResult QuenMatKhau()
        {
            return View();
        }

        // POST: Home/QuenMatKhau
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult QuenMatKhau(QuenMatKhauViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                // Tìm khách hàng theo email
                var khachHang = db.KHACHHANGs.FirstOrDefault(x => x.Email == viewModel.Email);

                if (khachHang != null)
                {
                    // Trong thực tế, bạn nên gửi email chứa link reset password
                    // Không thể hiển thị mật khẩu vì đã được hash
                    // Trong production, nên gửi email với token reset password
                    
                    TempData["InfoMessage"] = "Vui lòng kiểm tra email để đặt lại mật khẩu. Email đã được gửi đến: " + viewModel.Email;
                    // Lưu ý: Mật khẩu đã được hash nên không thể hiển thị trực tiếp
                    // Bạn cần implement chức năng reset password với token
                    
                    return RedirectToAction("DangNhap");
                }
                else
                {
                    ModelState.AddModelError("Email", "Email này chưa được đăng ký trong hệ thống!");
                }
            }

            return View(viewModel);
        }
       

    }
}