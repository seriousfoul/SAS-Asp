using Microsoft.AspNetCore.Mvc;
using Steam_Analyze_Statistics_ASP.Models;
using X.PagedList.Extensions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Steam_Analyze_Statistics_ASP.Controllers
{
    public class MemberController : Controller
    {
        
        public IActionResult UserInfo()
        {
            if (isLogin())
            {
                var data = new DBWorker().getUserInfo(HttpContext.Session.GetString("user"));
                return View(model: data);
            }
            else
                return RedirectToAction("Login", "LoginAndRigister");
        }

        [HttpGet]
        public IActionResult EditUser()
        {
            if (isLogin())
            {
                var data = new DBWorker().getUserInfo(HttpContext.Session.GetString("user"));
                ViewBag.data = data;
                return View();
            }
            else
                return RedirectToAction("Login", "LoginAndRigister");
        }

        [HttpPost]
        public IActionResult EditUser(bool b)
        {
            
            var data = new UserInfo { 
                name = Request.Form["name"],
                email = Request.Form["email"],
                phone = Request.Form["phone"],
                sex = Request.Form["sex"],
                year = Request.Form["year"],
                month = Request.Form["month"],
                day = Request.Form["day"],
                address = Request.Form["address"]
            };
            if (ModelState.IsValid)
            {
                if (new DBWorker().EditUserInfo(data, HttpContext.Session.GetString("user")))
                {
                    TempData["message"] = "更新資料成功";
                    return RedirectToAction("User");
                }                    
                else
                {
                    TempData["message"] = "更新資料失敗";
                    return View();
                }
            }
            TempData["message"] = "表單驗證失敗";
            return View();            
        }

        [HttpGet]
        public IActionResult EditPassword()
        {
            if (isLogin())
                return View();
            else
                return RedirectToAction("Login", "LoginAndRigister");
        }

        [HttpPost]
        public IActionResult EditPassword(bool b)
        {
            string password = Request.Form["newPassword"];

            if (ModelState.IsValid)
            {
                if (new DBWorker().EditPassword(password, HttpContext.Session.GetString("user")))
                {
                    TempData["message"] = "更新密碼成功";
                    return RedirectToAction("User");
                }
                else
                {
                    TempData["message"] = "更新密碼失敗";
                    return View();
                }
            }
            TempData["message"] = "表單驗證失敗";
            return View();
        }

        public IActionResult CheckPassword(string oldPassword)
        {
            DBWorker dbWork = new DBWorker();
            if (dbWork.CheckPassword(oldPassword, HttpContext.Session.GetString("user")))
                return Json(true);
            else
                return Json($"密碼錯誤");
        }

        public IActionResult CheckEditEmail(string email)
        {
            DBWorker dbWork = new DBWorker();
            if (dbWork.CheckEditEmail(email, HttpContext.Session.GetString("user")))
                return Json(true);
            else
                return Json($"{email} 被使用了");
        }

        public IActionResult SelfTopic(string type = "topic", int page = 1)
        {
            int dataTotal = 10;
            if (isLogin())
            {                
                var data = new DBWorker().SelfTopicInfo(type, HttpContext.Session.GetString("user"));

                var result = data.ToPagedList(page, dataTotal);
                ViewData["type"] = type;
                return View(model: result);
            }
            else
                return RedirectToAction("Login", "LoginAndRigister");
        }
        public IActionResult SelfOrder(string type = "topic", int page = 1)
        {
            int dataTotal = 10;
            if (isLogin())
            {
                var data = new DBWorker().SelfTopicInfo(type, HttpContext.Session.GetString("user"));

                var result = data.ToPagedList(page, dataTotal);
                ViewData["type"] = type;
                return View(model: result);
            }
            else
                return RedirectToAction("Login", "LoginAndRigister");
        }

        public bool isLogin()
        {
            try
            {
                if (HttpContext.Session.GetString("user") == null && Request.Cookies["login"] != "")
                {
                    Response.Cookies.Delete("login");
                    ViewData["user"] = "";
                    ViewData["CartAmount"] = HttpContext.Session.GetInt32("CartAmount").ToString();
                    return false;
                }
                else if (Request.Cookies["login"] == "")
                {
                    ViewData["user"] = "";

                    if (HttpContext.Session.GetInt32("CartAmount") != null)
                        ViewData["CartAmount"] = HttpContext.Session.GetInt32("CartAmount").ToString();

                    return false;
                }
                else
                {
                    if ((Convert.ToDateTime(Request.Cookies["login"]) - DateTime.Now).TotalDays > 0)
                    {
                        Response.Cookies.Append("login", DateTime.Now.AddDays(2).ToString(), new CookieOptions
                        {
                            Expires = DateTime.Now.AddDays(2),
                            HttpOnly = true,
                            Secure = true,
                            Path = "/"
                        });
                        ViewData["user"] = HttpContext.Session.GetString("user");

                        if (HttpContext.Session.GetInt32("CartAmount") != null)
                            ViewData["CartAmount"] = HttpContext.Session.GetInt32("CartAmount").ToString();

                        return true;
                    }
                    else
                    {
                        HttpContext.Session.Clear();
                        Response.Cookies.Delete("login");
                        ViewData["user"] = "";
                        return false;
                    }
                }
            }
            catch (NullReferenceException e)
            {
                return false;
            }
        }

    }
}
