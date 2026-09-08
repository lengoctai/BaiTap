using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BaiTap.Models;
namespace BaiTap.Controllers
{
    public class SachThieuNhiController : Controller
    {
        ThuVienSachEntities db = new ThuVienSachEntities();
        // GET: SachThieuNhi
        public PartialViewResult IndexThieuNhi()
        {
            var listSachkhcb = db.SACHes.Where(x => x.IdNhom == 3).OrderBy(x => x.Masach).Take(8).ToList();
            return PartialView(listSachkhcb);
        }
    }
}