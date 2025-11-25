using Microsoft.AspNetCore.Mvc;
using Steam_Analyze_Statistics_ASP.Models;
using X.PagedList.Extensions;

namespace Steam_Analyze_Statistics_ASP.Controllers
{
    public class ForumController : Controller
    {

        public IActionResult Forum(int page = 1)
        {
            int dataTotal = 10;

            // cookie and session
            isLogin();

            var topicData = new DBWorker().GetTopicTitle();
            var result = topicData.ToPagedList(page, dataTotal);

            return View(result);
        }

        [HttpGet]
        public IActionResult Topic(int id, int page = 1)
        {
            new DBWorker().addTopicView(id);

            int dataTotal = 20;

            isLogin();

            var topicInfo = new DBWorker().GetTopicInfo(id);
            ViewBag.topicInfo = topicInfo;

            var topicReplyInfo = new DBWorker().GetTopicReplyInfo(id);
            var result = topicReplyInfo.ToPagedList(page, dataTotal);
            ViewBag.id = id;


            return View(result);
        }

        [HttpGet]
        public IActionResult NewTopic()
        {
            // cookie and session
            if (isLogin())
                return View();
            else
                return RedirectToAction("Login", "LoginAndRigister");
        }
        [HttpPost]
        public IActionResult NewTopic(NewTopicForm form)
        {
            bool b = new DBWorker().addTopic(form, HttpContext.Session.GetString("user"));

            // cookie and session
            if (b)
                return RedirectToAction("Forum", "Forum");                
            else
            {
                TempData["Message"] = "發文未成功";
                return View();
            }
                
        }

        [HttpPost]

        public JsonResult TopicReply([FromBody] TopicReplyForm trData)
        {

            if (trData.replyContent == null)
                return Json(false);

            bool b = new DBWorker().addTopicReply(trData, HttpContext.Session.GetString("user"));
            return Json(b);
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
