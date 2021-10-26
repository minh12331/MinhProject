using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebApplication_EProject.Models;
using WebApplication_EProject.Models.NhanVienModel;

namespace WebApplication_EProject.Controllers
{
	public class HomeController : Controller
	{
		ModelContext db = new ModelContext();
        public ActionResult Index()
        {
			return View();
        }
        public ActionResult Error_Nothing()
		{

			return View();
		}

		public ActionResult Login()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Login(NhanVien user)
		{

			var account = db.NhanViens.SingleOrDefault(x => x.Email == user.Email);
			

			if (user.MatKhau == null || user.Email == null)
			{
				ViewBag.haha = "Missing Email or Password";
			}
			else
			{
				
				if (account == null || !BCrypt.Net.BCrypt.Verify(user.MatKhau, account.MatKhau))
				{
					ViewBag.haha = "User Login Details Failed!!";
				}
				else
				{
					int i = 0;
					i++;
					if (account.Status == 0)
					{
						ViewBag.haha = "Account currently disabled";
                        //return RedirectToAction("Login");
                    }
                    else
                    {
						TempData["Alert"] = "Login Successfully";
						FormsAuthentication.SetAuthCookie(user.Email, true);
						return RedirectToAction("Index", "NhanViens");
                    }
					
				}
				return View();
			}
			return View();
		}
		public ActionResult Logout()
		{
			FormsAuthentication.SignOut();
			return RedirectToAction("Login");
		}


		public ActionResult Forget()
		{
			if (TempData["err_wr_e"] != null)
			{
				ViewBag.err_wr_e = TempData["err_wr_e"];
			}
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Forget(string email)
		{
			bool has = db.NhanViens.Any(cus => cus.Email == email);

			var emailcheck = db.NhanViens.SingleOrDefault(x => x.Email == email);
			if (has == false)
			{
				//insert into db with random number
				TempData["err_wr_e"] = "Wrong Email";
				return RedirectToAction("Forget");
			}

			//create random number
			Random _random = new Random();
			int num = _random.Next(100000, 999999);

			//send email with token
			MailMessage Msg = new MailMessage();
			Msg.From = new MailAddress("eprojectaptech20@gmail.com", "The reseter");// replace with valid value
			Msg.Subject = "Recover Password";
			Msg.To.Add(email); //replace with correct values
			Msg.Body = "The Code for reset password is: " + Convert.ToString(num);
			Msg.IsBodyHtml = true;
			Msg.Priority = MailPriority.High;
			SmtpClient smtp = new SmtpClient();
			smtp.Host = "smtp.gmail.com";
			smtp.Port = 587;
			smtp.Credentials = new System.Net.NetworkCredential("eprojectaptech20@gmail.com", "<aptechloveproject17");// replace with valid value
			smtp.EnableSsl = true;
			smtp.Timeout = 20000;

			smtp.Send(Msg);

			// begin upload to db
			ResetPass ac = new ResetPass();
			ac.PassCodeNum = num;
			ac.ResetPass_ID = emailcheck.Employee_ID;
			db.ResetPasses.Add(ac);
			db.SaveChanges();
			// end upload to db

			TempData["check"] = email;
			return RedirectToAction("ResetByCode");
		}

		public ActionResult ResetByCode()
		{
			if (TempData["check"] != null)
			{
				ViewBag.check = TempData["check"];
			}
			if (TempData["err_wr_e"] != null)
			{
				ViewBag.err_wr_e = TempData["err_wr_e"];
			}
			return View();
		}

		[HttpPost]
		//[ValidateAntiForgeryToken]
		public ActionResult ResetByCode(String email, String code)
		{
			try
			{
				long coden = Convert.ToInt64(code);
				bool has = db.ResetPasses.Any(cus => cus.PassCodeNum == coden);
				var codecheck = db.ResetPasses.SingleOrDefault(x => x.PassCodeNum == coden);

				if (has == false)
				{
					TempData["err_wr_e"] = "Wrong Code";
					return RedirectToAction("ResetByCode");

				}
				ResetPass resetpass = db.ResetPasses.Find(codecheck.ResetPass_ID);
				db.ResetPasses.Remove(resetpass);
				db.SaveChanges();
				TempData["email_see"] = email;

				return RedirectToAction("ResetPassword");
			}
			catch
			{
				TempData["err_wr_e"] = "Wrong Code";
				return RedirectToAction("ResetByCode");
			}


		}

		public ActionResult ResetPassword()
		{
			if (TempData["email_see"] != null)
			{
				ViewBag.email_see = TempData["email_see"];
			}
			return View();
		}

		String SqlCon = ConfigurationManager.ConnectionStrings["ModelContext"].ConnectionString;
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult ResetPassword(String email, NhanVien nhanVien)
		{
			var emailcheck = db.NhanViens.SingleOrDefault(x => x.Email == email);

			string passwordHash = BCrypt.Net.BCrypt.HashPassword(nhanVien.MatKhau);

			SqlConnection con = new SqlConnection(SqlCon);
			string sqlQuery = "Update NhanViens set MatKhau = @MatKhau where Employee_ID = @Employee_ID";
			con.Open();
			SqlCommand cmd = new SqlCommand(sqlQuery, con);
			cmd.Parameters.AddWithValue("@MatKhau", passwordHash);
			cmd.Parameters.AddWithValue("@Employee_ID", emailcheck.Employee_ID);
			int check = cmd.ExecuteNonQuery();
			con.Close();
			if (check == 1)
			{
				TempData["Successful"] = "Insert Succesful";
				return RedirectToAction("ResetByCode");
			}
			return View();
		}

		//[Authorize(Roles = "Admin, Staff")]
		public ActionResult Account_Details()
		{
			
			string email = User.Identity.GetUserName();
			if (email == null)
			{
				return RedirectToAction("Error_Nothing", "Home");
			}
			int i = 0;
			i++;
			var dbEntry = db.NhanViens.FirstOrDefault(acc => acc.Email == email);

			if (dbEntry == null)
			{
				return RedirectToAction("Error_Nothing", "Home");
			}
			return View(dbEntry);
		}

		// Get:
		//[Authorize(Roles = "Admin, Staff")]
		public ActionResult Edit()
        {
            string email = User.Identity.GetUserName();
            if (email == null)
            {
                return RedirectToAction("Error_Nothing", "Home");
            }
            var dbEntry = db.NhanViens.FirstOrDefault(acc => acc.Email == email);
            if (dbEntry == null)
            {
                return HttpNotFound();
            }
			if(TempData["old_check"] != null)
            {
				ViewBag.old_check = TempData["old_check"];

			}
            return View(dbEntry);
        }

        // POST: 
        //[Authorize(Roles = "Admin, Staff")]
        [HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit([Bind(Include = "Employee_ID,Ten,Email,NgayTao,NgaySua,DOB,MatKhau,Role_ID")] NhanVien nhanVien)
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
					DOB = nhanVien.DOB,
					Email = nhanVien.Email,
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



		//public ActionResult About()
		//{
		//	ViewBag.Message = "Your application description page.";

		//	return View();
		//}

		//public ActionResult Contact()
		//{
		//	ViewBag.Message = "Your contact page.";

		//	return View();
		//}
	}
}