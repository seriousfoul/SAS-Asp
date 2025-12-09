using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Steam_Analyze_Statistics_ASP.Models;
using X.PagedList.Extensions;
using FluentEcpay;

namespace Steam_Analyze_Statistics_ASP.Controllers
{
    public static class SessionExensions
    {
        public static void SetObject<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T GetObject<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }
    }

    public class ProductsController : Controller
    {
        int dataTotal = 15;
        
        public IActionResult Products(int page = 1)
        {
            isLogin();

            var list = new DBWorker().GetProductsInfo();
            var result = list.ToPagedList(page, dataTotal);
            return View(result);
        }

        [HttpGet]
        public IActionResult ProductDetail(int id)
        {
            isLogin();

            var list = new DBWorker().GetProductsInfo(id);
            ProductInfo data = list[0];
            return View(model:data);
        }

        [HttpPost]
        public IActionResult ProductDetail(bool b)
        {
            
            ProductInfo item = new ProductInfo()
            {
                productId = Int32.Parse(Request.Form["id"].ToString()),
                name = Request.Form["name"].ToString(),
                price = Int32.Parse(Request.Form["price"].ToString()),
                amount = Int32.Parse(Request.Form["amount"].ToString()),
                subTotal = Int32.Parse(Request.Form["price"].ToString()) * Int32.Parse(Request.Form["amount"].ToString())
            };

            if(item.productId == null)
            {
                TempData["message"] = "表單資料傳送失敗";
                return View();
            }

            if (!isLogin())
            {
                var cart = HttpContext.Session.GetObject<List<ProductInfo>>("Cart");
                int amount = new DBWorker().GetProductsInfo(Int32.Parse(Request.Form["id"].ToString()))[0].amount;
                bool isDifferent = true;

                if (cart == null)
                    cart = new List<ProductInfo>();
                else
                {
                    foreach (var data in cart)
                    {
                        if (data.productId == item.productId)
                        {
                            if (amount < data.amount + item.amount)
                            {
                                data.amount = amount;
                                TempData["message"] += "由於商品數量超過庫存數量，所以商品調整為庫存數量\n";
                            }
                            else
                                data.amount += item.amount;

                            HttpContext.Session.SetObject<List<ProductInfo>>("Cart", cart);
                            isDifferent = false;
                            break;
                        }
                    }
                }

                if (isDifferent)
                    cart.Add(item);

                HttpContext.Session.SetObject("Cart", cart);
                HttpContext.Session.SetInt32("CartAmount", cart.Count);

            }
            else
            {
                if(!new DBWorker().AddCart(item, HttpContext.Session.GetString("user").ToString()))
                {
                    TempData["message"] += "購物車資料新增失敗";
                    return View();
                }
                else
                {
                    if (HttpContext.Session.GetInt32("CartAmount") != null)
                        HttpContext.Session.SetInt32("CartAmount", HttpContext.Session.GetInt32("CartAmount").Value + 1);
                    else
                        HttpContext.Session.SetInt32("CartAmount", 1);
                }
            }
            

            TempData["message"] += "購物車資料新增成功";
            return RedirectToAction("Products");
        }

        public IActionResult ShoppingCart()
        {
            if(!isLogin())
                return RedirectToAction("Login", "LoginAndRigister");

            List<ProductInfo> data = new DBWorker().GetCart(HttpContext.Session.GetString("user"));

            int total = 0;
            foreach (var record in data)
            {
                total += record.subTotal;
            }

            ViewData["total"] = total.ToString();
            return View(model:data);
        }

        public IActionResult DeleteCart(int id)
        {
            if (new DBWorker().DeleteCart(id))
                TempData["message"] = "購物車刪除成功";
            else
                TempData["message"] = "購物車刪除失敗";

            HttpContext.Session.SetInt32("CartAmount", new DBWorker().GetCartCount(HttpContext.Session.GetString("user")));
            return RedirectToAction("ShoppingCart");
        }

        public IActionResult Settlement()
        {
            if(new DBWorker().Settlement(Int32.Parse(Request.Form["total"]), HttpContext.Session.GetString("user")))
            {
                TempData["message"] = "購物車結算成功";
                HttpContext.Session.Remove("CartAmount");
                return RedirectToAction("Products");
            }
            else
            {
                TempData["message"] = "購物車結算失敗";
                return RedirectToAction("ShoppingCart");
            }
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
