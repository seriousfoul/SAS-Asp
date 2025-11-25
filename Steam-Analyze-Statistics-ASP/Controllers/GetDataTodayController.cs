using Microsoft.AspNetCore.Mvc;

using Steam_Analyze_Statistics_ASP.Models;
using System.Collections.Generic;
using X.PagedList.Extensions;

namespace Steam_Analyze_Statistics_ASP.Controllers
{
    public class GetDataTodayController : Controller
    {
        int dataTotal = 10;

        [HttpGet]
        public IActionResult GetDataToday(string tableName, int page = 1)
        {
            // cookie and session
            isLogin();

            var result = new DBWorker().GetDataToday(tableName).ToPagedList(page, dataTotal);
            ViewData["tableName"] = tableName;
            return View(result);
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
