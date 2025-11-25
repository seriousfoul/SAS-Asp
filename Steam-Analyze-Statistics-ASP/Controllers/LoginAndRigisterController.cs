using Azure;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Steam_Analyze_Statistics_ASP.Models;

namespace Steam_Analyze_Statistics_ASP.Controllers
{
    public class LoginAndRigisterController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            if (Request.Cookies["login"] != "" && HttpContext.Session.GetString("user") != null)
            {
                return RedirectToAction("index", "Home");
            }
            ViewData["CartAmount"] = HttpContext.Session.GetInt32("CartAmount").ToString() ?? "";
            return View();
        }

        [HttpPost]
        public IActionResult Login(bool b)
        {           
            try
            {
                DBWorker dbWork = new DBWorker();
            
                if (dbWork.getAccount(Request.Form["account"], Request.Form["password"]))
                {
                    
                    var sessionInfo = DateTime.Now;
                    HttpContext.Session.SetString("user", Request.Form["account"].ToString());

                    Response.Cookies.Append("login", DateTime.Now.AddDays(2).ToString(), new CookieOptions
                    {
                        Expires = DateTime.Now.AddDays(2),
                        HttpOnly = true,
                        Secure = true,
                        Path = "/"
                    });

                    if (dbWork.CheckAdmin(Request.Form["account"].ToString()))
                        HttpContext.Session.SetString("level", "admin");
                    else
                        HttpContext.Session.SetString("level", "user");

                    TempData["message"] = "";

                    if (HttpContext.Session.GetInt32("CartAmount") != null)
                    {
                        var cart = HttpContext.Session.GetObject<List<ProductInfo>>("Cart");

                        foreach (var data in cart)
                        {
                            new DBWorker().AddCart(data, HttpContext.Session.GetString("user").ToString());
                        }
                        HttpContext.Session.Remove("Cart");
                    }
                    
                    int count = new DBWorker().GetCartCount(HttpContext.Session.GetString("user").ToString());
                    HttpContext.Session.SetInt32("CartAmount", count);


                    return RedirectToAction("index", "Home");
                }
                else
                {
                    TempData["Message"] = "帳號密碼錯誤，登入失敗";
                    return RedirectToAction("Rigister");
                }
            }
            catch
            {
                return View();
            }
        }

        [HttpGet]
        public IActionResult Rigister()
        {
            if (Request.Cookies["login"] != "" && HttpContext.Session.GetString("user") != null)
            {
                return RedirectToAction("index", "Home");
            }
            ViewData["CartAmount"] = HttpContext.Session.GetInt32("CartAmount").ToString() ?? "";
            return View();
        }

        [HttpPost]
        public IActionResult Rigister(User_Account record)
        {
            if (ModelState.IsValid)
            {
                DBWorker dbWork = new DBWorker();
                dbWork.setAccount(record);
                TempData["Message"] = "註冊成功";
                return RedirectToAction("Login");
            }
            TempData["Message"] = "註冊失敗";
            return View();
        }

        public IActionResult CheckAccount(string account)
        {
            DBWorker dbWork = new DBWorker();
            if (dbWork.CheckAccount(account))
               return Json(true);
            else
                return Json($"{account} is already in use." );
        }

        public IActionResult EditEmail(string email)
        {
            DBWorker dbWork = new DBWorker();
            if (dbWork.CheckEditEmail(email, HttpContext.Session.GetString("user")))
                return Json(true);
            else
                return Json($"{email} 被使用了");
        }

    }
}
