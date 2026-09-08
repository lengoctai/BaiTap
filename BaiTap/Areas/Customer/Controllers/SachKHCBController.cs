using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using BaiTap.Models;
using PagedList;
using PagedList.Mvc;

namespace BaiTap.Controllers
{

    public class SachKHCBController : Controller
    {
        ThuVienSachEntities db = new ThuVienSachEntities();
        // GET: SachKHCB
        public PartialViewResult Index()
        {
            var listSachkhcb = db.SACHes.Where(x => x.IdNhom == 3).OrderBy(x => x.Masach).Take(10).ToList();
            return PartialView(listSachkhcb);
        }
        public ActionResult ChiTietSachKHCB (int idSach)
        {
            SACH Id = db.SACHes.SingleOrDefault(n => n.Masach == idSach);
            if(Id == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            string temp = "";
            if(Id.IdNhom > 0)
            {
                NhomSach n = db.NhomSaches.Find(Id.IdNhom);
                if( n != null)
                {
                    temp = n.TenNhom;
                }
            }
            ViewBag.Tennhomsach = temp;
            return View(Id);

        }
        public ActionResult NhomSachKHCB(int idnhom, int? Page)
        {
            int pageSize = 9;
            int pageNumber = (Page ?? 1);

            string temp = "";
            if (idnhom > 0)
            {
                NhomSach n = db.NhomSaches.Find(idnhom);
                if (n != null)
                {
                    temp = n.TenNhom;
                }

            }
            ViewBag.tennhomsach = temp;
            ViewBag.IDNhomSach = idnhom;
            NhomSach id = db.NhomSaches.SingleOrDefault(n => n.IdNhom == idnhom);
            if (id == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            //truy xuat nhom tin
            var listSach = db.SACHes.Where(x => x.IdNhom == idnhom).OrderByDescending(x => x.Masach).ToPagedList(pageNumber, pageSize);
            if (listSach.Count == 0)
            {
                ViewBag.newst = "Thông tin đang cập nhật...";

            }
            return View(listSach);

        }

    }
}