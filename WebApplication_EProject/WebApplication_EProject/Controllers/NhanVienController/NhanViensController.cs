using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication_EProject.Models;
using WebApplication_EProject.Models.NhanVienModel;

namespace WebApplication_EProject.Controllers.NhanVienController
{
    //[Authorize(Roles = "Manager, Staff")]
    public class NhanViensController : Controller
    {
        private ModelContext db = new ModelContext();

        // GET: NhanViens
        public ActionResult Index(String Status_Check)
        {

            if (TempData["Alert"] != null)
            {
        
                ViewBag.Alert = TempData["Alert"];
            }
            return View(db.NhanViens.ToList());
        }

        // GET: NhanViens/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NhanVien nhanVien = db.NhanViens.Find(id);
            if (nhanVien == null)
            {
                return HttpNotFound();
            }
            return View(nhanVien);
        }

        //CREATE------------------------------------------------------------------------------------------------
        // GET: NhanViens/Create
        //[Authorize(Roles = "Manager")]
        public ActionResult Create()
        {
            if (TempData["err_long"] != null)
            {
                ViewBag.err_l = TempData["err_long"];

            }
            if (TempData["err_email"] != null)
            {
                ViewBag.err_email = TempData["err_email"];

            }
            if (TempData["err_SDT"] != null)
            {
                ViewBag.err_SDT = TempData["err_SDT"];

            }
            ViewBag.Role_ID = new SelectList(db.Roles, "Role_ID", "RoleName");
            return View();
        }
        // POST: NhanViens/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Manager")]
        //[Authorize(Roles = "Admin")]
        public ActionResult Create([Bind(Include = "Employee_ID,Status,Ten,Email,SDT,NgayTao,NgaySua,DOB,MatKhau,Role_ID")] NhanVien nhanVien)
        {
            if (ModelState.IsValid)
            {
                var email_dup_check = from edc in db.NhanViens select edc.Email;
                var phone_dup_check = from edc in db.NhanViens select edc.SDT;
                if (email_dup_check.Contains(nhanVien.Email))
                {
                    TempData["err_email"] = "Email already taken";
                    return RedirectToAction("Create");
                }

                if (phone_dup_check.Contains(nhanVien.SDT))
                {
                    TempData["err_SDT"] = "Phone number already taken";
                    return RedirectToAction("Create");
                }

                if (nhanVien.MatKhau.Length > 32)
                {
                    TempData["err_email"] = "Character string limited to 32 characters";
                    return RedirectToAction("Create");
                }

                string passwordHash = BCrypt.Net.BCrypt.HashPassword(nhanVien.MatKhau);
                
                nhanVien.MatKhau = passwordHash;
                nhanVien.NgayTao = DateTime.Now;
                nhanVien.NgaySua = DateTime.Now;
                db.NhanViens.Add(nhanVien);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(nhanVien);
        }

        //EDIT------------------------------------------------------------------------------------------------
        // GET: NhanViens/Edit/5
        //[Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Error_Nothing", "Home");
            }
            NhanVien nhanVien = db.NhanViens.Find(id);
            if (nhanVien == null)
            {
                return RedirectToAction("Error_Nothing", "Home");
            }
            if (TempData["err"] != null)
            {
                ViewBag.err = TempData["err"];

            }
            if (TempData["err_long"] != null)
            {
                ViewBag.err_long = TempData["err_long"];

            }
            ViewBag.Role_ID = new SelectList(db.Roles, "Role_ID", "RoleName", nhanVien.Role_ID);
            return View(nhanVien);
        }

        // POST: NhanViens/Edit/5
        String SqlCon = ConfigurationManager.ConnectionStrings["ModelContext"].ConnectionString;
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Admin")]
        public ActionResult Edit([Bind(Include = "Employee_ID,Status,Ten,Email,SDT,NgayTao,NgaySua,DOB,MatKhau,Role_ID")] NhanVien nhanVien)
        {
            if (ModelState.IsValid)
            {
                //var nhanvien_check = db.NhanViens.Find(nhanVien.Employee_ID);
                var nhanvien_check = db.NhanViens.FirstOrDefault(x => x.Employee_ID == nhanVien.Employee_ID);


                string passwordHash = BCrypt.Net.BCrypt.HashPassword(nhanVien.MatKhau);

                //begin gán cho thuộc tính của đối tượng
                nhanVien.NgaySua = DateTime.Now;
                nhanVien.NgayTao = nhanvien_check.NgayTao;
                //begin gán cho thuộc tính của đối tượng

                var nhanvien_exist = new NhanVien
                {
                    Employee_ID = nhanVien.Employee_ID,
                    Status = nhanVien.Status,
                    DOB = nhanVien.DOB,
                    Email = nhanVien.Email,
                    SDT = nhanVien.SDT,
                    Ten = nhanVien.Ten,
                    MatKhau = passwordHash,
                    NgayTao = nhanVien.NgayTao,
                    NgaySua = nhanVien.NgaySua,
                    Role_ID = nhanVien.Role_ID
                };

                db.Entry(nhanvien_check).CurrentValues.SetValues(nhanvien_exist);
                //db.Entry(nhanvien_exist).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");

            }
            ViewBag.Role_ID = new SelectList(db.Roles, "Role_ID", "RoleName", nhanVien.Role_ID);
            return View(nhanVien);
        }

        //DELETE------------------------------------------------------------------------------------------------
        // GET: NhanViens/Delete/5
        //[Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Error_Nothing", "Home");
            }
            NhanVien nhanVien = db.NhanViens.Find(id);
            if (nhanVien == null)
            {
                return RedirectToAction("Error_Nothing", "Home");
            }
            return View(nhanVien);
        }

        // POST: NhanViens/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            NhanVien nhanVien = db.NhanViens.Find(id);
            db.NhanViens.Remove(nhanVien);
            db.SaveChanges();
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

