using System;
using System.Linq;
using System.Web.Mvc;
using BaiTap.Models;
using System.Data.Entity;

namespace BaiTap.Controllers
{
    public class NhaXuatBanController : Controller
    {
        ThuVienSachEntities db = new ThuVienSachEntities();

        // GET: NhaXuatBan
        public ActionResult Index()
        {
            var listNhaXuatBan = db.NHAXUATBANs.OrderBy(x => x.TenNXB).ToList();
            return View(listNhaXuatBan);
        }

        // GET: NhaXuatBan/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            NHAXUATBAN nhaXuatBan = db.NHAXUATBANs.Find(id);
            if (nhaXuatBan == null)
            {
                return HttpNotFound();
            }
            return View(nhaXuatBan);
        }

        // GET: NhaXuatBan/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: NhaXuatBan/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(NhaXuatBanViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra trùng tên nhà xuất bản
                if (db.NHAXUATBANs.Any(x => x.TenNXB == viewModel.TenNXB))
                {
                    ModelState.AddModelError("TenNXB", "Tên nhà xuất bản đã tồn tại");
                    return View(viewModel);
                }

                // Kiểm tra trùng số điện thoại
                if (db.NHAXUATBANs.Any(x => x.DienThoai == viewModel.DienThoai))
                {
                    ModelState.AddModelError("DienThoai", "Số điện thoại đã được sử dụng");
                    return View(viewModel);
                }

                NHAXUATBAN nhaXuatBan = new NHAXUATBAN
                {
                    TenNXB = viewModel.TenNXB,
                    DienThoai = viewModel.DienThoai,
                    Diachi = viewModel.Diachi
                };

                db.NHAXUATBANs.Add(nhaXuatBan);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Thêm nhà xuất bản thành công!";
                return RedirectToAction("Index");
            }
            return View(viewModel);
        }

        // GET: NhaXuatBan/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            NHAXUATBAN nhaXuatBan = db.NHAXUATBANs.Find(id);
            if (nhaXuatBan == null)
            {
                return HttpNotFound();
            }

            NhaXuatBanViewModel viewModel = new NhaXuatBanViewModel
            {
                MaNXB = nhaXuatBan.MaNXB,
                TenNXB = nhaXuatBan.TenNXB,
                DienThoai = nhaXuatBan.DienThoai,
                Diachi = nhaXuatBan.Diachi,
                Email = "" // Email sẽ được lưu riêng nếu có trong database
            };

            return View(viewModel);
        }

        // POST: NhaXuatBan/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(NhaXuatBanViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                NHAXUATBAN nhaXuatBan = db.NHAXUATBANs.Find(viewModel.MaNXB);
                if (nhaXuatBan == null)
                {
                    return HttpNotFound();
                }

                // Kiểm tra trùng tên nhà xuất bản (trừ bản thân)
                if (db.NHAXUATBANs.Any(x => x.TenNXB == viewModel.TenNXB && x.MaNXB != viewModel.MaNXB))
                {
                    ModelState.AddModelError("TenNXB", "Tên nhà xuất bản đã tồn tại");
                    return View(viewModel);
                }

                // Kiểm tra trùng số điện thoại (trừ bản thân)
                if (db.NHAXUATBANs.Any(x => x.DienThoai == viewModel.DienThoai && x.MaNXB != viewModel.MaNXB))
                {
                    ModelState.AddModelError("DienThoai", "Số điện thoại đã được sử dụng");
                    return View(viewModel);
                }

                nhaXuatBan.TenNXB = viewModel.TenNXB;
                nhaXuatBan.DienThoai = viewModel.DienThoai;
                nhaXuatBan.Diachi = viewModel.Diachi;

                db.Entry(nhaXuatBan).State = EntityState.Modified;
                db.SaveChanges();
                TempData["SuccessMessage"] = "Cập nhật nhà xuất bản thành công!";
                return RedirectToAction("Index");
            }
            return View(viewModel);
        }

        // GET: NhaXuatBan/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            NHAXUATBAN nhaXuatBan = db.NHAXUATBANs.Find(id);
            if (nhaXuatBan == null)
            {
                return HttpNotFound();
            }

            // Kiểm tra xem nhà xuất bản có đang được sử dụng trong bảng SACH không
            if (nhaXuatBan.SACHes != null && nhaXuatBan.SACHes.Count > 0)
            {
                TempData["ErrorMessage"] = "Không thể xóa nhà xuất bản này vì đang có sách thuộc nhà xuất bản này!";
                return RedirectToAction("Index");
            }

            return View(nhaXuatBan);
        }

        // POST: NhaXuatBan/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            NHAXUATBAN nhaXuatBan = db.NHAXUATBANs.Find(id);
            if (nhaXuatBan == null)
            {
                return HttpNotFound();
            }

            // Kiểm tra lại lần nữa
            if (nhaXuatBan.SACHes != null && nhaXuatBan.SACHes.Count > 0)
            {
                TempData["ErrorMessage"] = "Không thể xóa nhà xuất bản này vì đang có sách thuộc nhà xuất bản này!";
                return RedirectToAction("Index");
            }

            db.NHAXUATBANs.Remove(nhaXuatBan);
            db.SaveChanges();
            TempData["SuccessMessage"] = "Xóa nhà xuất bản thành công!";
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

