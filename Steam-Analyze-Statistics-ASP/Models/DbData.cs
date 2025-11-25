using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Steam_Analyze_Statistics_ASP.Models
{
    public class DbData
    {
        public struct TopSellers
        {
            public string name { get; init; }
            public string image { get; init; }
            public int price { get; init; }
            public int sale { get; init; }
            public int rank { get; init; }
            public string link { get; init; }
            public string type { get; init; }
            public DateTime date { get; init; }
        }

        public struct MostPlayed
        {
            public string name { get; init; }
            public string image { get; init; }
            public int price { get; init; }
            public int sale { get; init; }
            public int rank { get; init; }
            public string link { get; init; }
            public string type { get; init; }
            public DateTime date { get; init; }
        }

        public struct Top5SellGame
        {
            public string name { get; init; }
            public int rank { get; init; }
            public DateTime month { get; init; }
            public String image { get; init; }
            public String link { get; init; }
        }

        public struct Top5PeopleGame
        {
            public string name { get; init; }
            public int rank { get; init; }
            public DateTime month { get; init; }
            public String image { get; init; }
            public String link { get; init; }
        }

        public struct Top10PeopleGameType
        {
            public string type { get; init; }
            public int count { get; init; }
            public int rank { get; init; }
            public DateTime month { get; init; }
        }

        public struct Top10SellGameType
        {
            public string type { get; init; }
            public int count { get; init; }
            public int rank { get; init; }
            public DateTime month { get; init; }
        }

        public struct TopSellPricePie
        {
            public string type { get; init; }
            public string percent { get; init; }
            public DateTime month { get; init; }
        }

        public struct MostPlayPricePie
        {
            public string type { get; init; }
            public string percent { get; init; }
            public DateTime month { get; init; }
        }

    }
    public class NewTopicForm
    {
        [Required(ErrorMessage = "標題必須填")]
        [StringLength(50, MinimumLength = 4, ErrorMessage = "標題長度必需介於4到50個字")]
        public string title { get; set; }

        [Required(ErrorMessage = "內容必須填")]
        [StringLength(5000, MinimumLength = 10, ErrorMessage = "內容最少10個字")]
        public string content { get; set; }
    }
    public class GetData
    {
        public string name { get; set; }
        public string image { get; set; }
        public int price { get; set; }
        public int sale { get; set; }
        public int rank { get; set; }
        public string link { get; set; }
    }

    public class User_Account
    {
        [Remote(action: "CheckAccount", controller: "LoginAndRigister")]
        [Required(ErrorMessage = "帳號必須填")]
        [StringLength(20, MinimumLength = 4, ErrorMessage = "姓名長度必需介於4到20個字")]
        public string account { get; set; }

        [Required(ErrorMessage = "密碼必須填")]
        [StringLength(50, MinimumLength = 4, ErrorMessage = "密碼長度必需介於4到50個字")]
        public string password { get; set; }

        [Required(ErrorMessage = "二次確認必須填")]
        [Compare("password", ErrorMessage = "必須與密碼相同")]
        public string confirmPassword { get; set; }

        [Remote(action: "CheckEmail", controller: "LoginAndRigister")]
        [Required(ErrorMessage = "信箱必須填")]
        [RegularExpression(@"\w+@\w+\.com{1}\.?\w*", ErrorMessage = "信箱格式錯誤")]
        [StringLength(50, ErrorMessage = "信箱長度最多50個字")]
        public string email { get; set; }
    }

    public class TopicInfo
    {
        public int id { get; set; }
        public string title { get; set; }
        public string user { get; set; }
        public int views { get; set; }
        public string date { get; set; }
        public string lastUpdate { get; set; }
        public string content { get; set; }
    }


    public class TopicReplyInfo
    {
        public string user { get; set; }
        public string date { get; set; }
        public string content { get; set; }
        public int rank { get; set; }
    }

    public class TopicReplyForm
    {
        [Required(ErrorMessage = "內容必須填")]
        [StringLength(5000, MinimumLength = 10, ErrorMessage = "內容最少10個字")]
        public string replyContent { get; set; }
        public int topicId { get; set; }
    }

    public class UserInfo
    {
        [StringLength(20, ErrorMessage = "名字長度最多20個字")]
        public string name { get; set; }

        [Remote(action: "CheckEditEmail", controller: "Member")]
        [Required(ErrorMessage = "信箱必須填")]
        [RegularExpression(@"\w+@\w+\.com{1}\.?\w*", ErrorMessage = "信箱格式錯誤")]
        [StringLength(50, ErrorMessage = "信箱長度最多50個字")]
        public string email { get; set; }

        [RegularExpression(@"^0{1}\d{9}$", ErrorMessage = "電話格式錯誤")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "電話長度10個字")]
        public string phone { get; set; }

        public string sex { get; set; }
        public string year { get; set; }
        public string month { get; set; }
        public string day { get; set; }

        [StringLength(100, ErrorMessage = "信箱長度最多100個字")]
        public string address { get; set; }
    }

    public class EditPasswordInfo
    {
        [Remote(action: "CheckPassword", controller: "Member")]
        [Required(ErrorMessage = "舊密碼必須填")]        
        public string oldPassword { get; set; }

        [Required(ErrorMessage = "新密碼密碼必須填")]
        [StringLength(50, MinimumLength = 4, ErrorMessage = "密碼長度必需介於4到50個字")]
        public string newPassword { get; set; }

        [Required(ErrorMessage = "二次確認必須填")]
        [Compare("newPassword", ErrorMessage = "必須與密碼相同")]
        public string confirmPassword { get; set; }
    }

    public class SelfTopicInfo
    {
        public int topicId { get; set; }
        public int topicUserId { get; set; }
        public string title { get; set; }
        public string topicContent { get; set; }
        public int view { get; set; }
        public string topicDate { get; set; }
        public string topicLastUpdate { get; set; }
        public int topicReplyId { get; set; }
        public int topicReplyIdUserId { get; set; }
        public string topicReplyContent { get; set; }
        public string topicReplyLastUpdate { get; set; }
        public int userId { get; set; }
        public string account { get; set; }
    }

    public class ProductInfo
    {
        public int productId { get; set; }
        public string name { get; set; }
        public int price { get; set; }
        public int remain { set; get; }
        public string image { get; set; }
        public int cartId { get; set; }
        public int amount { get; set; }
        public int subTotal { set; get; }
        public int userId {  set; get; }
    }

    

}
