using LoginLogout.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace LoginLogout.Controllers
{
    public class UserController : Controller
    {
        contactsdatabaseEntities db = new contactsdatabaseEntities();

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult LogIn()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LogIn(LoginLogout.Models.User user)
        {
            if (ModelState.IsValid)
            {
                if (Isvalid(user.Email, user.Password))
                {
                    FormsAuthentication.SetAuthCookie(user.Email, false);
                    return RedirectToAction("Index", "User");
                }
                else
                {
                    ModelState.AddModelError("", "Inloggningsinformationen är fel.");
                }
            }
            return View(user);
        }

        [HttpGet]
        public ActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Registration(LoginLogout.Models.User user)
        {
            if (ModelState.IsValid)
            {
                using (db)
                {                   
                    //Skapar en instans av SimpleCrypto NuGet Package.
                    var crypto = new SimpleCrypto.PBKDF2();
                    //Krypterar lösenordet.
                    var encrypPass = crypto.Compute(user.Password);
                    //Skapar en användare.
                    var sysUser = db.Users.Create();

                    sysUser.Email = user.Email;
                    sysUser.Password = encrypPass;
                    sysUser.PasswordSalt = crypto.Salt;
                    sysUser.UserID = Guid.NewGuid();

                 
                    db.Users.Add(sysUser);
                    db.SaveChanges();

                    return RedirectToAction("Index", "User");          
                }
            }
            else
            {
                ModelState.AddModelError("", "Inloggningsinformationen är fel.");
            }
            return View();
        }

        public ActionResult RemoteValidation(User pd)
        {
            return View(pd);
        }

        public JsonResult doesEmailExist(string Email)
        {
            var data = db.Users.Where(p => p.Email.Equals(Email, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();

            if (data != null)
            {
                return Json("Denna email är upptagen", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }
     
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "User");
        }

        private bool Isvalid(string email, string password)
        {
            var crypto = new SimpleCrypto.PBKDF2();

            bool isVaild = false;

            using (db)
            {
                var user = db.Users.FirstOrDefault(u => u.Email == email);

                if (user != null)
                {
                    if (user.Password == crypto.Compute(password, user.PasswordSalt))
                    {
                        isVaild = true;
                    }
                }
            }

            return isVaild;
        }
    }
}
