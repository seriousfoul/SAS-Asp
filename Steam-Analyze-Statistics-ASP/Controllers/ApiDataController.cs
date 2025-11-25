using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Steam_Analyze_Statistics_ASP.Models;
using System.Text;

namespace Steam_Analyze_Statistics_ASP.Controllers
{
    public class ApiDataController : Controller
    {

        [HttpGet]
        public IActionResult Api()
        {
            // cookie and session
            isLogin();

            return View();
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
    [ApiController]
    [Route("apiData/api")]
    public class ApiGetData : ControllerBase
    {
        [HttpGet("{gt}/{sm}")]
        public IActionResult Api(string gt, string sm)
        {
            Console.WriteLine($"gt={gt}"); // , sm={sm}

            string tableName = "";
            if (gt == "game")
            {
                if (sm == "topSeller")
                    tableName = "top5sellgame";
                else if (sm == "mostPlayer")
                    tableName = "top5peoplegame";
                else
                    return BadRequest("未找到相關資料");
            }
            else if (gt == "type")
            {
                if (sm == "topSeller")
                    tableName = "top10sellgametype";
                else if (sm == "mostPlayer")
                    tableName = "top10peoplegametype";
                else
                    return BadRequest("未找到相關資料");
            }
            else if (gt == "price")
            {
                if (sm == "topSeller")
                    tableName = "topsellpricepie";
                else if (sm == "mostPlayer")
                    tableName = "mostplaypricepie";
                else
                    return BadRequest("未找到相關資料");
            }
            else
                return BadRequest("意外錯誤");


            var jsonStr = new DBWorker().ApiData(gt, tableName);
            var jsonArray = JArray.Parse(jsonStr);

            if (jsonStr != null)
            {
                var result = new
                {
                    status = "Success",
                    product = jsonArray,

                };
                var json = JsonConvert.SerializeObject(result, new JsonSerializerSettings
                {
                    StringEscapeHandling = StringEscapeHandling.Default // 保留中文
                });

                return Content(json, "application/json", Encoding.UTF8);
            }
            else
            {
                return BadRequest(new { Percentage_of_Top_Seller_Price_Type = new { status = "Failure", message = "Nothing Here" } });
            }
        }
    }
}

