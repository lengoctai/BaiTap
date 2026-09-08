using System;
using System.Linq;
using System.Web.Mvc;
using BaiTap.Models;
using System.Data.Entity;

namespace BaiTap.Areas.Admin.Controllers
{
    public class ChuDeController : Controller
    {
        ThuVienSachEntities db = new ThuVienSachEntities();

        // GET: ChuDe
        public ActionResult Index()
        {
            var listChuDe = db.CHUDEs.OrderBy(x => x.TenChuDe).ToList();
            return View(listChuDe);
        }

        // GET: ChuDe/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            CHUDE chuDe = db.CHUDEs.Find(id);
            if (chuDe == null)
            {
                return HttpNotFound();
            }
            return View(chuDe);
        }

        // GET: ChuDe/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ChuDe/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ChuDeViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra trùng tên chủ đề
                if (db.CHUDEs.Any(x => x.TenChuDe == viewModel.TenChuDe))
                {
                    ModelState.AddModelError("TenChuDe", "Tên chủ đề đã tồn tại");
                    return View(viewModel);
                }

                CHUDE chuDe = new CHUDE
                {
                    TenChuDe = viewModel.TenChuDe
                    // Lưu ý: Email, DienThoai, Diachi không có trong model CHUDE
                    // Nếu cần lưu, cần thêm các trường này vào database và model
                };

                db.CHUDEs.Add(chuDe);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Thêm chủ đề thành công!";
                return RedirectToAction("Index");
            }
            return View(viewModel);
        }

        // GET: ChuDe/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            CHUDE chuDe = db.CHUDEs.Find(id);
            if (chuDe == null)
            {
                return HttpNotFound();
            }

            ChuDeViewModel viewModel = new ChuDeViewModel
            {
                MaCD = chuDe.MaCD,
                TenChuDe = chuDe.TenChuDe
            };

            return View(viewModel);
        }

        // POST: ChuDe/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ChuDeViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                CHUDE chuDe = db.CHUDEs.Find(viewModel.MaCD);
                if (chuDe == null)
                {
                    return HttpNotFound();
                }

                // Kiểm tra trùng tên chủ đề (trừ bản thân)
                if (db.CHUDEs.Any(x => x.TenChuDe == viewModel.TenChuDe && x.MaCD != viewModel.MaCD))
                {
                    ModelState.AddModelError("TenChuDe", "Tên chủ đề đã tồn tại");
                    return View(viewModel);
                }

                chuDe.TenChuDe = viewModel.TenChuDe;
                // Lưu ý: Email, DienThoai, Diachi không có trong model CHUDE
                // Nếu cần lưu, cần thêm các trường này vào database và model

                db.Entry(chuDe).State = EntityState.Modified;
                db.SaveChanges();
                TempData["SuccessMessage"] = "Cập nhật chủ đề thành công!";
                return RedirectToAction("Index");
            }
            return View(viewModel);
        }

        // GET: ChuDe/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            CHUDE chuDe = db.CHUDEs.Find(id);
            if (chuDe == null)
            {
                return HttpNotFound();
            }

            // Kiểm tra xem chủ đề có đang được sử dụng trong bảng SACH không
            if (chuDe.SACHes != null && chuDe.SACHes.Count > 0)
            {
                TempData["ErrorMessage"] = "Không thể xóa chủ đề này vì đang có sách thuộc chủ đề này!";
                return RedirectToAction("Index");
            }

            return View(chuDe);
        }

        // POST: ChuDe/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CHUDE chuDe = db.CHUDEs.Find(id);
            if (chuDe == null)
            {
                return HttpNotFound();
            }

            // Kiểm tra lại lần nữa
            if (chuDe.SACHes != null && chuDe.SACHes.Count > 0)
            {
                TempData["ErrorMessage"] = "Không thể xóa chủ đề này vì đang có sách thuộc chủ đề này!";
                return RedirectToAction("Index");
            }

            db.CHUDEs.Remove(chuDe);
            db.SaveChanges();
            TempData["SuccessMessage"] = "Xóa chủ đề thành công!";
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
