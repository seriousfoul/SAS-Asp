
using Microsoft.AspNetCore.Mvc;
using Steam_Analyze_Statistics_ASP.Models;
using System.Diagnostics;
using System.Text;
using static Steam_Analyze_Statistics_ASP.Models.DbData;

namespace Steam_Analyze_Statistics_ASP.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;     

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }


        public IActionResult Index()
        {
            // cookie and session
            isLogin();        

            DBWorker dw = new DBWorker();
            ViewBag.Top5SellGame = dw.ViewTopGame("Top5SellGame");
            ViewBag.Top5PeopleGame = dw.ViewTopGame("Top5PeopleGame");
            ViewBag.Top10SellGameType = dw.ViewType("Top10SellGameType");
            ViewBag.Top10PeopleGameType = dw.ViewType("Top10PeopleGameType");
            ViewBag.TopSellPricePie = dw.ViewType("TopSellPricePie");
            ViewBag.MostPlayPricePie = dw.ViewType("MostPlayPricePie");

            return View();
        }
        public IActionResult Analyze_Data()
        {
            isLogin();

            DateTime date = DateTime.Now;
            ViewData["month"] = date.Year + "-" + date.Month;
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            Response.Cookies.Delete("login");
            ViewData["user"] = "";

            return RedirectToAction("index", "Home");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
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
