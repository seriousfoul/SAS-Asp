using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using static Steam_Analyze_Statistics_ASP.Models.DbData;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace Steam_Analyze_Statistics_ASP.Models
{
    public class DBWorker
    {

        private readonly string connStr = $"Server=sas.mysql.database.azure.com; port=3306; UserID = seriousfoul;Password=Hunter777+;Database=steam;SslMode=Required;";

        // 首頁統計數據
        public object ViewType(string tableName)
        {
            List<Top10SellGameType> top10SellGameTypeData = new List<Top10SellGameType>();
            List<Top10PeopleGameType> top10PeopleGameTypeData = new List<Top10PeopleGameType>();
            List<TopSellPricePie> topSellPricePieData = new List<TopSellPricePie>();
            List<MostPlayPricePie> mostPlayPricePieData = new List<MostPlayPricePie>();

            DateTime date = DateTime.Now;
            string sql = $"select * from {tableName} where month >= \"{date.Year}-{date.Month}-01\"";

            MySqlConnection mydb = new MySqlConnection(connStr);
            MySqlCommand mySqlCommand = new MySqlCommand(sql);
            mySqlCommand.Connection = mydb;
            mydb.Open();

            MySqlDataReader reader = mySqlCommand.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    if (tableName == "Top10SellGameType")
                    {
                        top10SellGameTypeData.Add(new Top10SellGameType()
                        {
                            type = reader.GetString(1),
                            count = reader.GetInt32(2),
                            rank = reader.GetInt32(3),
                            month = reader.GetDateTime(4)
                        });
                    }

                    else if (tableName == "Top10PeopleGameType")
                    {
                        top10PeopleGameTypeData.Add(new Top10PeopleGameType()
                        {
                            type = reader.GetString(1),
                            count = reader.GetInt32(2),
                            rank = reader.GetInt32(3),
                            month = reader.GetDateTime(4)
                        });
                    }

                    else if (tableName == "TopSellPricePie")
                    {
                        topSellPricePieData.Add(new TopSellPricePie()
                        {
                            type = reader.GetString(1),
                            percent = reader.GetString(2),
                            month = reader.GetDateTime(3)
                        });
                    }

                    else
                    {
                        mostPlayPricePieData.Add(new MostPlayPricePie()
                        {
                            type = reader.GetString(1),
                            percent = reader.GetString(2),
                            month = reader.GetDateTime(3)
                        });
                    }
                }
            }
            mydb.Close();

            if (tableName == "Top10SellGameType")
                return top10SellGameTypeData;
            else if (tableName == "Top10PeopleGameType")
                return top10PeopleGameTypeData;
            else if (tableName == "TopSellPricePie")
                return topSellPricePieData;
            else
                return mostPlayPricePieData;
        }
        // 首頁統計數據
        public object ViewTopGame(string tableName)
        {
            List<Top5SellGame> top5SellGameData = new List<Top5SellGame>();
            List<Top5PeopleGame> top5PeopleGameData = new List<Top5PeopleGame>();

            DateTime date = DateTime.Now;

            string sql = "";
            if (tableName == "Top5SellGame")
                sql = $"SELECT g.*, s.image, s.link FROM {tableName} as g LEFT JOIN(select name, min(image) as image, link from topsellers GROUP BY name,link) as s on g.name = s.name  where month = \"{date.Year}-{date.Month}-01\" and link like \"%global%\"";
            else
                sql = $"SELECT g.*, s.image, s.link FROM {tableName} as g LEFT JOIN(select name, min(image) as image, link from mostplayed GROUP BY name,link) as s on g.name = s.name  where month = \"{date.Year}-{date.Month}-01\" ";
            MySqlConnection mydb = new MySqlConnection(connStr);
            MySqlCommand sqlCommand = new MySqlCommand(sql);
            sqlCommand.Connection = mydb;
            mydb.Open();

            MySqlDataReader reader = sqlCommand.ExecuteReader();
            
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    if (tableName == "Top5SellGame")
                    {
                        top5SellGameData.Add(new Top5SellGame()
                        {
                            name = reader.GetString(1),
                            rank = reader.GetInt32(2),
                            month = reader.GetDateTime(3),
                            image = reader.GetString(4),
                            link = reader.GetString(5)
                        });
                    }

                    else
                    {
                        top5PeopleGameData.Add(new Top5PeopleGame()
                        {
                            name = reader.GetString(1),
                            rank = reader.GetInt32(2),
                            month = reader.GetDateTime(3),
                            image = reader.GetString(4),
                            link = reader.GetString(5)
                        });
                    }
                }
            }
            mydb.Close();

            if (tableName == "Top5SellGame")
                return top5SellGameData;
            else
                return top5PeopleGameData;
        }

        // 今日遊戲排名
        public List<GetData> GetDataToday(string tableName)
        {
            List<GetData> list = new List<GetData>();
            DateTime date = DateTime.Now;
            string str = $"select name,image,price,sale,`rank`,link from {tableName} where date = \"{date.ToString("yyyy-MM-dd")}\"";

            MySqlConnection mydb = new MySqlConnection(connStr);
            MySqlCommand sqlCommand = new MySqlCommand(str);
            sqlCommand.Connection = mydb;
            mydb.Open();

            MySqlDataReader sdr = sqlCommand.ExecuteReader();
            if (sdr.HasRows)
            {
                while (sdr.Read())
                {
                    list.Add(
                        new GetData()
                        {
                            name = sdr.GetString(0),
                            image = sdr.GetString(1),
                            price = sdr.GetInt32(2),
                            sale = sdr.GetInt32(3),
                            rank = sdr.GetInt32(4),
                            link = sdr.GetString(5)
                        }
                    );
                }
            }
            mydb.Close();

            return list;
        }

        // API
        public string ApiData(string gt, string tableName)
        {
            string sql = "";
            if (gt == "game")
                sql = $"select name, `rank`, month from {tableName}  Order by month desc";
            else if (gt == "type")
                sql = $"select type, count, `rank`, month from {tableName}  Order by month desc";
            else
                sql = $"select type, percent, month from {tableName}  Order by month desc";
            

            MySqlConnection mydb = new MySqlConnection(connStr);
            MySqlCommand sqlCommand = new MySqlCommand(sql);
            sqlCommand.Connection = mydb;
            mydb.Open();

            MySqlDataReader sdr = sqlCommand.ExecuteReader();

            string str = "[";
            string month = "";
            int count = 0, maxCount = 0;
            if (sdr.HasRows)
            {
                while (sdr.Read())
                {
                    if (gt == "game")
                    {
                        maxCount = 5;
                        if (month == "")
                        {
                            month = sdr.GetDateTime(2).ToString("yyyy-MM");
                            str += "{";
                            str += $"\"Month\":\"{month}\",";
                            str += "\"RankInfo\": [";
                            count = 0;
                        }
                        else if (month != sdr.GetDateTime(2).ToString("yyyy-MM"))
                        {
                            month = sdr.GetDateTime(2).ToString("yyyy-MM");
                            str += ",{";
                            str += $"\"Month\":\"{month}\",";
                            str += "\"RankInfo\": [";
                            count = 0;
                        }
                        else
                        {
                            str += ",";
                        }
                        str += "{";
                        str += $"\"name\": \"{sdr.GetString(0)}\",";
                        str += $"\"rank\": {sdr.GetInt32(1)}";
                        str += "}";
                        count++;
                        if (count == maxCount)
                            str += "]}";

                    }
                    else if (gt == "type")
                    {
                        maxCount = 10;
                        if (month == "")
                        {
                            month = sdr.GetDateTime(3).ToString("yyyy-MM");
                            str += "{";
                            str += $"\"Month\":\"{month}\",";
                            str += "\"RankInfo\": [";
                            count = 0;
                        }
                        else if (month != sdr.GetDateTime(3).ToString("yyyy-MM"))
                        {
                            month = sdr.GetDateTime(3).ToString("yyyy-MM");
                            str += ",{";
                            str += $"\"Month\":\"{month}\",";
                            str += "\"RankInfo\": [";
                            count = 0;
                        }
                        else
                        {
                            str += ",";
                        }
                        str += "{";
                        str += $"\"type\": \"{sdr.GetString(0)}\",";
                        str += $"\"count\": {sdr.GetInt32(1)},";
                        str += $"\"rank\": {sdr.GetInt32(2)}";
                        str += "}";
                        count++;
                        if (count == maxCount)
                            str += "]}";

                    }

                    else
                    {
                        maxCount = 3;
                        if (month == "")
                        {
                            month = sdr.GetDateTime(2).ToString("yyyy-MM");
                            str += "{";
                            str += $"\"Month\":\"{month}\",";
                            str += "\"RankInfo\": [";
                            count = 0;
                        }
                        else if (month != sdr.GetDateTime(2).ToString("yyyy-MM"))
                        {
                            month = sdr.GetDateTime(2).ToString("yyyy-MM");
                            str += ",{";
                            str += $"\"Month\":\"{month}\",";
                            str += "\"RankInfo\": [";
                            count = 0;
                        }
                        else
                        {
                            str += ",";
                        }
                        str += "{";
                        str += $"\"type\": \"{sdr.GetString(0)}\",";
                        str += $"\"percent\": \"{sdr.GetString(1)}\"";
                        str += "}";
                        count++;
                        if (count == maxCount)
                            str += "]}";

                    }

                }

                mydb.Close();
            }
            str += "]";

            return str;
        }

        // 檢查帳號
        public bool CheckAccount(string account)
        {
            using (var sqlconnection = new MySqlConnection(connStr))
            {
                var sqlcommand = sqlconnection.CreateCommand();
                sqlcommand.CommandText = $"select account from user where account = '{account}'";
                sqlconnection.Open();

                var reader = sqlcommand.ExecuteReader();

                if (reader.HasRows)
                {
                    sqlconnection.Close();
                    return false;
                }
                else
                {
                    sqlconnection.Close();
                    return true;
                }

            }

        }

        // 檢查信箱
        public bool CheckEmail(string email)
        {
            using (var sqlconnection = new MySqlConnection(connStr))
            {
                var sqlcommand = sqlconnection.CreateCommand();
                sqlcommand.CommandText = $"select email from user where email = '{email}'";
                sqlconnection.Open();

                var reader = sqlcommand.ExecuteReader();

                if (reader.HasRows)
                {
                    sqlconnection.Close();
                    return false;
                }
                else
                {
                    sqlconnection.Close();
                    return true;
                }

            }
        }

        // 檢查密碼
        public bool CheckPassword(string password, string account)
        {
            using (var sqlconnection = new MySqlConnection(connStr))
            {
                var sqlcommand = sqlconnection.CreateCommand();
                sqlcommand.CommandText = $"select * from user where password = @password and account = @account";
                sqlcommand.Parameters.AddWithValue("@password", password);
                sqlcommand.Parameters.AddWithValue("@account", account);
                sqlconnection.Open();
                
                var reader = sqlcommand.ExecuteReader();

                if (reader.HasRows)
                {
                    sqlconnection.Close();
                    return true;
                }
                else
                {
                    sqlconnection.Close();
                    return false;
                }

            }
        }

        // 檢查使用者權限
        public bool CheckAdmin(string account)
        {
            try
            {
                string sql = "select admin from user where account = @account";

                MySqlConnection mydb = new MySqlConnection(connStr);
                MySqlCommand MySqlCommand = new MySqlCommand(sql);
                MySqlCommand.Parameters.AddWithValue("@account", account);
                MySqlCommand.Connection = mydb;
                mydb.Open();

                MySqlDataReader reader = MySqlCommand.ExecuteReader();
                reader.Read();
                bool admin = reader.GetBoolean(0);
                mydb.Close();                
                return admin;
            }
            catch
            {
                return false;
            }
        }

        // 檢查帳號密碼
        public bool getAccount(string account, string password)
        {
            // 建立sql 連線
            using (var sqlconnection = new MySqlConnection(connStr))
            {
                // 建立sql 命令
                var sqlcommand = sqlconnection.CreateCommand();
                sqlcommand.CommandText = $"select password from user where account = @account";
                sqlcommand.Parameters.AddWithValue("@account", account);
                sqlconnection.Open(); //執行sql連線

                MySqlDataReader reader = sqlcommand.ExecuteReader();  // 執行 sql語法
                if (reader.HasRows)
                {
                    reader.Read();
                    string pw = reader.GetString(0);
                    return password == pw;
                }

                else
                    return false;
            }
        }

        // 新增帳號
        public void setAccount(User_Account record)
        {
            // sql建立連線
            using (var sqlconnection = new MySqlConnection(connStr))
            {

                // 建立sql命令
                var sqlcommand = sqlconnection.CreateCommand();

                // SQL新增指令
                //sqlcommand.CommandText = @"select * from user";
                sqlcommand.CommandText = @"INSERT INTO user(account, password, email) VALUES(@account, @password, @email)";

                sqlcommand.Parameters.AddWithValue("@account", record.account);
                sqlcommand.Parameters.AddWithValue("@password", record.password);
                sqlcommand.Parameters.AddWithValue("@email", record.email);

                sqlconnection.Open();
                sqlcommand.ExecuteNonQuery();
                // sqlconnection.Close();
            }
        }

        // 取得討論區標題
        public List<TopicInfo> GetTopicTitle()
        {
            string sql = "select t.topicId, t.title, u.account, t.views, t.date , max(r.lastUpdate) as ranklastUpdate from `Topic` t inner join `user` u on t.userId = u.id inner join TopicRank r on t.topicId = r.TopicId group by t.topicId, t.title, u.account, t.views, t.date order by ranklastUpdate desc";

            MySqlConnection mydb = new MySqlConnection(connStr);
            MySqlCommand sqlCommand = new MySqlCommand(sql);
            sqlCommand.Connection = mydb;

            mydb.Open();

            MySqlDataReader data = sqlCommand.ExecuteReader();

            List<TopicInfo> tempList = new List<TopicInfo>();

            if (data.HasRows)
            {
                while (data.Read())
                {
                    tempList.Add(new TopicInfo()
                    {
                        id = data.GetInt32(0),
                        title = data.GetString(1),
                        user = data.GetString(2),
                        views = data.GetInt32(3),
                        date = data.GetString(4),
                        lastUpdate = data.GetString(5)
                    });
                }
            }
            mydb.Close();
            return tempList;
        }

        // 取得討論區內容
        public TopicInfo GetTopicInfo(int id)
        {
            string sql = "select t.title, u.account, t.date, t.content from `Topic` t inner join `user` u on t.userId = u.id  where t.topicId = @id";

            MySqlConnection mydb = new MySqlConnection(connStr);
            MySqlCommand MySqlCommand = new MySqlCommand(sql);
            MySqlCommand.Connection = mydb;

            MySqlCommand.Parameters.AddWithValue("@id", id);

            mydb.Open();

            MySqlDataReader record = MySqlCommand.ExecuteReader();

            TopicInfo data = new TopicInfo();

            if (record.HasRows)
            {
                record.Read();

                data = new TopicInfo()
                {
                    title = record.GetString(0),
                    user = record.GetString(1),
                    date = record.GetString(2),
                    content = record.GetString(3)
                };
            }
            mydb.Close();
            return data;
        }

        // 取得討論區回覆內容
        public List<TopicReplyInfo> GetTopicReplyInfo(int id)
        {

            //public string user { get; set; }
            //public string date { get; set; }
            //public string content { get; set; }
            //public int rank { get; set; }

            string sql = "select u.account, s.lastUpdate, s.content, r.Rank from topicRank r inner join topicReply s on r.TopicReplyId = s.topicReplyId inner join `user` u on s.userId = u.id  where r.TopicId = @id order by r.Rank";

            MySqlConnection mydb = new MySqlConnection(connStr);
            MySqlCommand MySqlCommand = new MySqlCommand(sql);
            MySqlCommand.Connection = mydb;

            MySqlCommand.Parameters.AddWithValue("@id", id);

            mydb.Open();

            MySqlDataReader data = MySqlCommand.ExecuteReader();

            List<TopicReplyInfo> tempList = new List<TopicReplyInfo>();

            if (data.HasRows)
            {
                while (data.Read())
                {
                    tempList.Add(new TopicReplyInfo
                    {
                        user = data.GetString(0),
                        date = data.GetDateTime(1).ToString("yyyy-MM-dd HH:mm:ss"),
                        content = data.GetString(2),
                        rank = data.GetInt32(3)
                    });
                }
            }
            mydb.Close();
            return tempList;
        }

        // 新增討論區發文
        public bool addTopic(NewTopicForm data, string account)
        {
            try
            {
                string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                string sql = $"select id from user where account = '{account}'";
                MySqlConnection mydb = new MySqlConnection(connStr);
                MySqlCommand MySqlCommand = new MySqlCommand(sql);
                MySqlCommand.Connection = mydb;

                mydb.Open();
                MySqlDataReader reader = MySqlCommand.ExecuteReader();


                reader.Read();
                int id = reader.GetInt32(0);
                mydb.Close();

                sql = "insert into Topic (userId, title, content, date, lastUpdate) values (@userId, @title, @content, @date, @lastUpdate)";

                mydb = new MySqlConnection(connStr);
                MySqlCommand = new MySqlCommand(sql);
                MySqlCommand.Parameters.AddWithValue("@userId", id);
                MySqlCommand.Parameters.AddWithValue("@title", data.title);
                MySqlCommand.Parameters.AddWithValue("@content", data.content);
                MySqlCommand.Parameters.AddWithValue("@date", time);
                MySqlCommand.Parameters.AddWithValue("@lastUpdate", time);
                MySqlCommand.Connection = mydb;

                mydb.Open();
                MySqlCommand.ExecuteNonQuery();
                mydb.Close();

                sql = $"insert into TopicRank (TopicId, lastUpdate) values ( (select id from Topic where userId = {id} and date = '{time}'),'{time}')";

                mydb = new MySqlConnection(connStr);
                MySqlCommand = new MySqlCommand(sql);
                MySqlCommand.Connection = mydb;

                mydb.Open();
                MySqlCommand.ExecuteNonQuery();
                mydb.Close();


                sql = $"select count(*) from Topic where userId = {id} and date = '{time}'";

                mydb = new MySqlConnection(connStr);
                MySqlCommand = new MySqlCommand(sql);
                MySqlCommand.Connection = mydb;
                mydb.Open();
                reader = MySqlCommand.ExecuteReader();

                reader.Read();
                int count = reader.GetInt32(0);
                mydb.Close();
                return count > 0;
            }
            catch
            {
                return false;
            }
        }

        // 新增討論區回覆
        public bool addTopicReply(TopicReplyForm data, string account)
        {
            try
            {
                string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                string sql = $"select id from user where account = '{account}'";
                MySqlConnection mydb = new MySqlConnection(connStr);
                MySqlCommand MySqlCommand = new MySqlCommand(sql);
                MySqlCommand.Connection = mydb;

                mydb.Open();
                MySqlDataReader reader = MySqlCommand.ExecuteReader();


                reader.Read();
                int userId = reader.GetInt32(0);
                mydb.Close();

                sql = "insert into TopicReply (userId, content, lastUpdate, topicId) values (@userId, @content, @lastUpdate, @topicId)";

                MySqlConnection mydb2 = new MySqlConnection(connStr);
                MySqlCommand = new MySqlCommand(sql);
                MySqlCommand.Parameters.AddWithValue("@userId", userId);
                MySqlCommand.Parameters.AddWithValue("@content", data.replyContent);
                MySqlCommand.Parameters.AddWithValue("@lastUpdate", time);
                MySqlCommand.Parameters.AddWithValue("@topicId", data.topicId);
                MySqlCommand.Connection = mydb2;

                mydb2.Open();
                MySqlCommand.ExecuteNonQuery();
                mydb2.Close();

                sql = $"select id from TopicReply where userId = {userId} and topicId = {data.topicId} and lastUpdate = '{time}' ";
                MySqlConnection mydb3 = new MySqlConnection(connStr);
                MySqlCommand = new MySqlCommand(sql);
                MySqlCommand.Connection = mydb3;

                mydb3.Open();
                reader = MySqlCommand.ExecuteReader();
                reader.Read();
                int topicReplyId = reader.GetInt32(0);
                mydb3.Close();

                if (topicReplyId <= 0)
                    return false;

                sql = $"insert into TopicRank (topicId, topicReplyId, rank, lastUpdate) values (@topicId, @topicReplyId, (select max(`rank`)+1 from TopicRank where topicId = @topicId), @lastUpdate)";

                MySqlConnection mydb4 = new MySqlConnection(connStr);
                MySqlCommand = new MySqlCommand(sql);
                MySqlCommand.Parameters.AddWithValue("@topicId", data.topicId);
                MySqlCommand.Parameters.AddWithValue("@topicReplyId", topicReplyId);
                MySqlCommand.Parameters.AddWithValue("@lastUpdate", time);
                MySqlCommand.Connection = mydb4;

                mydb4.Open();
                MySqlCommand.ExecuteNonQuery();
                mydb4.Close();

                return true;
            }
            catch
            {
                return false;
            }


        }

        // 新增討論區瀏覽人數
        public void addTopicView(int id)
        {
            string sql = $"update Topic set views = views + 1 where Topicid = {id}";

            MySqlConnection mydb = new MySqlConnection(connStr);
            MySqlCommand sqlCommand = new MySqlCommand(sql);
            sqlCommand.Connection = mydb;

            mydb.Open();
            sqlCommand.ExecuteNonQuery();
            mydb.Close();
        }

        // 取得使用者資料
        public UserInfo getUserInfo(string account)
        {
            string sql = "select name,email,phone,birthday,address,account from user where account = @account";

            MySqlConnection mydb = new MySqlConnection(connStr);
            MySqlCommand MySqlCommand = new MySqlCommand(sql);
            MySqlCommand.Parameters.AddWithValue("@account", account);
            MySqlCommand.Connection = mydb;

            mydb.Open();
            MySqlDataReader record = MySqlCommand.ExecuteReader();
            record.Read();

            string temp = record.IsDBNull(3) ? "1950-01-01" : record.GetString(3);

            var data = new UserInfo()
            {
                name = record.IsDBNull(0) ? "" : record.GetString(0),
                email = record.IsDBNull(1) ? "" : record.GetString(1),
                phone = record.IsDBNull(2) ? "" : record.GetString(2),
                address = record.IsDBNull(4) ? "" : record.GetString(4),
                sex = "1",
                year = temp.Split("-")[0],
                month = temp.Split("-")[1],
                day = temp.Split("-")[2]
            };

            mydb.Close();
            return data;
        }

        // 檢查修改信箱
        public bool CheckEditEmail(string email, string account)
        {
            using (var sqlconnection = new MySqlConnection(connStr))
            {
                try
                {
                    var sqlcommand = sqlconnection.CreateCommand();
                    sqlcommand.CommandText = $"select email from user where email == @email and account != @account";
                    sqlcommand.Parameters.AddWithValue("@email",email);
                    sqlcommand.Parameters.AddWithValue("@account", account);
                    sqlconnection.Open();

                    var reader = sqlcommand.ExecuteReader();

                    if (reader.HasRows)
                    {
                        sqlconnection.Close();
                        return false;
                    }
                    else
                    {
                        sqlconnection.Close();
                        return true;
                    }
                }
                catch
                {
                    return false;
                }

            }
        }

        // 修改帳號資料
        public bool EditUserInfo(UserInfo data, string account)
        {
            try
            {
                string sql = "update user set name = @name, email = @email, phone = @phone, sex = @sex, birthday = @birthday, address = @address where account = @account";

                MySqlConnection mydb = new MySqlConnection(connStr);
                MySqlCommand MySqlCommand = new MySqlCommand(sql);
                MySqlCommand.Parameters.AddWithValue("@name", data.name);
                MySqlCommand.Parameters.AddWithValue("@email", data.email);
                MySqlCommand.Parameters.AddWithValue("@phone", data.phone);
                MySqlCommand.Parameters.AddWithValue("@sex", data.sex);
                MySqlCommand.Parameters.AddWithValue("@birthday", $"{data.year}-{data.month}-{data.day}");
                MySqlCommand.Parameters.AddWithValue("@address", data.address);
                MySqlCommand.Parameters.AddWithValue("@account", account);
                MySqlCommand.Connection = mydb;

                mydb.Open();
                int result = MySqlCommand.ExecuteNonQuery();
                mydb.Close();
                return result > 0;
            }
            catch
            {
                return false;
            }
        }

        // 修改密碼
        public bool EditPassword(string password, string account)
        {
            try
            {
                string sql = "update user set password = @password where account = @account";

                MySqlConnection mydb = new MySqlConnection(connStr);
                MySqlCommand MySqlCommand = new MySqlCommand(sql);
                MySqlCommand.Parameters.AddWithValue("@password", password);
                MySqlCommand.Parameters.AddWithValue("@account", account);
                MySqlCommand.Connection = mydb;

                mydb.Open();
                int result = MySqlCommand.ExecuteNonQuery();
                mydb.Close();
                return result > 0;
            }
            catch
            {
                return false;
            }
        }

        // 取得自己的討論區資料
        public List<SelfTopicInfo> SelfTopicInfo(string type, string account)
        {
            try
            {
                string sql = "";
                if (type == "topic")
                    sql = "select * from Topic inner join user on Topic.userid = user.id  where user.account = @account order by lastUpdate desc;";
                else
                    sql = "select * from TopicReply left join Topic on Topic.topicId = TopicReply.topicId left join user on Topic.userid = user.id order by lastUpdate desc;";

                MySqlConnection mydb = new MySqlConnection(connStr);
                MySqlCommand MySqlCommand = new MySqlCommand(sql);
                MySqlCommand.Parameters.AddWithValue("@account", account);
                MySqlCommand.Connection = mydb;
                mydb.Open();

                List<SelfTopicInfo> list = new List<SelfTopicInfo>();
                MySqlDataReader reader = MySqlCommand.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        if (type == "topic")
                        {
                            list.Add(new SelfTopicInfo()
                            {
                                topicId = reader.GetInt32(0),
                                title = reader.GetString(2),
                                topicContent = reader.GetString(3),
                                view = reader.GetInt32(4),
                                topicDate = reader.GetString(5),
                                topicLastUpdate = reader.GetString(6)
                            });
                        }
                        else
                        {
                            list.Add(new SelfTopicInfo()
                            {
                                topicReplyId = reader.GetInt32(0),
                                topicReplyContent = reader.GetString(2),
                                topicReplyLastUpdate = reader.GetString(3),
                                topicId = reader.GetInt32(4),
                                title = reader.GetString(7),
                                account = reader.GetString(13)
                            });
                        }
                        
                    }
                }
                
                mydb.Close();
                return list;
            }
            catch
            {
                return new List<SelfTopicInfo>();
            }
        }

        // 取得商品資訊
        public List<ProductInfo> GetProductsInfo(int id = 0)
        {
            MySqlConnection mydb = null;
            try
            {
                string sql;
                if (id == 0)
                   sql = "select * from products";
                else
                    sql = "select * from products where productId = @id";
                

                mydb = new MySqlConnection(connStr);
                MySqlCommand MySqlCommand = new MySqlCommand(sql);
                MySqlCommand.Connection = mydb;
                if (id != 0)
                    MySqlCommand.Parameters.AddWithValue("@id", id);
                mydb.Open();

                MySqlDataReader record = MySqlCommand.ExecuteReader();
                List<ProductInfo> list = new List<ProductInfo>();

                if (record.HasRows)
                {
                    while (record.Read())
                    {
                        if (record.GetInt32(5) == 1)
                        {
                            list.Add(new ProductInfo()
                            {
                                productId = record.GetInt32(0),
                                name = record.GetString(1),
                                price = record.GetInt32(2),
                                remain = record.GetInt32(3),
                                image = record.GetString(4)
                            });
                        }
                    }
                }
                
                return list;
            }
            catch
            {
                return new List<ProductInfo>();
            }
            finally
            {
                mydb?.Close();
            }
            
        }

        public bool AddCart(ProductInfo data, string account)
        {
            MySqlConnection mydb = null;
            
            try
            {
                string sql = "select id from user where account = @account";

                mydb = new MySqlConnection(connStr);
                MySqlCommand MySqlCommand = new MySqlCommand(sql);
                MySqlCommand.Connection = mydb;
                MySqlCommand.Parameters.AddWithValue("@account", account);
                mydb.Open();

                MySqlDataReader record = MySqlCommand.ExecuteReader();

                record.Read();
                int userId = record.GetInt32(0);
                mydb.Close();

                sql = "select count(*) from cart where productId = @productId and userId = @userId";

                mydb = new MySqlConnection(connStr);
                MySqlCommand = new MySqlCommand(sql);
                MySqlCommand.Connection = mydb;
                MySqlCommand.Parameters.AddWithValue("@productId", data.productId);
                MySqlCommand.Parameters.AddWithValue("@userId", userId);
                mydb.Open();

                record = MySqlCommand.ExecuteReader();

                record.Read();
                int count = record.GetInt32(0);
                mydb.Close();

                if(count == 0)
                {
                    sql = "insert into cart (userId, productId, amount, subTotal) values (@userId, @productId, @amount, @subTotal)";
                    mydb = new MySqlConnection(connStr);
                    MySqlCommand = new MySqlCommand(sql);
                    MySqlCommand.Connection = mydb;
                    MySqlCommand.Parameters.AddWithValue("@userId", userId);
                    MySqlCommand.Parameters.AddWithValue("@productId", data.productId);
                    MySqlCommand.Parameters.AddWithValue("@amount", data.amount);
                    MySqlCommand.Parameters.AddWithValue("@subTotal", data.subTotal);

                    mydb.Open();
                    MySqlCommand.ExecuteNonQuery();

                }
                else
                {
                    sql = @"
                            update cart set amount = CASE
                                            WHEN amount + @amount <= (select amount from products where productId = @productId) THEN amount + @amount 
                                            ELSE (select amount from products where productId = @productId)
                                            END,
                                            subTotal = CASE
                                            WHEN amount + @amount <= (select amount from products where productId = @productId) THEN subTotal + @subTotal
                                            ELSE (select amount from products where productId = @productId) * (select price from products where productId = @productId)
                                            END
                            where userId = @userId and productId = @productId";

                    mydb = new MySqlConnection(connStr);
                    MySqlCommand = new MySqlCommand(sql);
                    MySqlCommand.Connection = mydb;
                    MySqlCommand.Parameters.AddWithValue("@userId", userId);
                    MySqlCommand.Parameters.AddWithValue("@productId", data.productId);
                    MySqlCommand.Parameters.AddWithValue("@amount", data.amount);
                    MySqlCommand.Parameters.AddWithValue("@subTotal", data.subTotal);

                    mydb.Open();
                    MySqlCommand.ExecuteNonQuery();
                }
            }
            catch
            {
                return false;
            }
            finally
            {
                mydb?.Close();
            }
            return true;
        }

        public List<ProductInfo> GetCart(string account)
        {            
            MySqlConnection mydb = null;

            try
            {
                string sql = "select p.productId, p.name, p.price, p.amount as remain, c.cartId, c.amount, c.subTotal from cart c inner join products p on c.productId = p.productId where userid = (select id from user where account = @account)";

                mydb = new MySqlConnection(connStr);
                MySqlCommand MySqlCommand = new MySqlCommand(sql);
                MySqlCommand.Connection = mydb;
                MySqlCommand.Parameters.AddWithValue("@account", account);
                mydb.Open();

                MySqlDataReader record = MySqlCommand.ExecuteReader();
                List<ProductInfo> list = new List<ProductInfo>();

                
                if (record.HasRows)
                {
                    while (record.Read())
                    {
                        list.Add(new ProductInfo
                        {
                            productId = record.GetInt32(0),
                            name = record.GetString(1),
                            price = record.GetInt32(2),
                            remain = record.GetInt32(3),
                            cartId = record.GetInt32(4),
                            amount = record.GetInt32(5),
                            subTotal = record.GetInt32(6)
                        });
                        
                    }
                }
                else
                {

                    mydb.Close();
                    return new List<ProductInfo>();
                }

                mydb.Close();
                return list;
            }
            catch
            {
                return new List<ProductInfo>();
            }
            finally
            {
                mydb?.Close();
            }

            
        }

        public int GetCartCount(string account)
        {
            MySqlConnection mydb = null;

            try
            {
                string sql = "select count(*) from cart where userid = (select id from user where account = @account)";

                mydb = new MySqlConnection(connStr);
                MySqlCommand MySqlCommand = new MySqlCommand(sql);
                MySqlCommand.Connection = mydb;
                MySqlCommand.Parameters.AddWithValue("@account", account);
                mydb.Open();

                MySqlDataReader record = MySqlCommand.ExecuteReader();
                List<ProductInfo> list = new List<ProductInfo>();

                record.Read();
                int count = record.GetInt32(0);                   
                
                return count;
            }
            catch
            {
                return 0;
            }
            finally
            {
                mydb?.Close();
            }
        }

        public bool DeleteCart(int id)
        {
            MySqlConnection mydb = null;

            try
            {
                string sql = "delete from cart where cartId = @id";

                mydb = new MySqlConnection(connStr);
                MySqlCommand MySqlCommand = new MySqlCommand(sql);
                MySqlCommand.Connection = mydb;
                MySqlCommand.Parameters.AddWithValue("@id", id);
                mydb.Open();
                int result = MySqlCommand.ExecuteNonQuery();                

                return result > 0;
            }
            catch
            {                
                return false;
            }
            finally
            {
                mydb?.Close();
            }

        }

        public bool Settlement(int total, string account)
        {
            MySqlConnection mydb = null;
            MySqlTransaction tran = null;

            string date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            try
            {
                string sql = "select * from cart where userId = (select id from user where account = @account)";

                mydb = new MySqlConnection(connStr);
                MySqlCommand sqlCommand = new MySqlCommand(sql);
                sqlCommand.Connection = mydb;
                sqlCommand.Parameters.AddWithValue("@account", account);
                mydb.Open();
                MySqlDataReader data = sqlCommand.ExecuteReader();
                List<ProductInfo> list = new List<ProductInfo>();
               
                while (data.Read())
                {
                    list.Add(new ProductInfo()
                    {
                        cartId = data.GetInt32(0),
                        userId = data.GetInt32(1),
                        productId = data.GetInt32(2),
                        amount = data.GetInt32(3),
                        subTotal = data.GetInt32(4)
                    });
                }
                mydb.Close();

                sql = @"insert into `order` (userId, total, date) values ((select id from user where account = @account), @total, @date)";

                mydb = new MySqlConnection(connStr);
                sqlCommand = new MySqlCommand(sql);
                sqlCommand.Connection = mydb;
                sqlCommand.Parameters.AddWithValue("@account", account);
                sqlCommand.Parameters.AddWithValue("@total", total);
                sqlCommand.Parameters.AddWithValue("@date", date);
                mydb.Open();
                sqlCommand.ExecuteNonQuery();
                mydb.Close();


                sql = @"select orderId from `order` where userId = (select id from user where account = @account) and date = @date";

                mydb = new MySqlConnection(connStr);
                sqlCommand = new MySqlCommand(sql);
                sqlCommand.Connection = mydb;
                sqlCommand.Parameters.AddWithValue("@account", account);
                sqlCommand.Parameters.AddWithValue("@date", date);
                mydb.Open();
                data = sqlCommand.ExecuteReader();
                data.Read();
                int orderId = data.GetInt32(0);
                mydb.Close();

                foreach (ProductInfo item in list)
                {
                    mydb = new MySqlConnection(connStr);
                    mydb.Open();
                    tran = mydb.BeginTransaction();

                    sql = "update products set amount = amount - @amount where productId = @productId";

                    sqlCommand = mydb.CreateCommand();
                    sqlCommand.Transaction = tran;
                    sqlCommand.CommandText = sql;
                    sqlCommand.Parameters.AddWithValue("@productId", item.productId);
                    sqlCommand.Parameters.AddWithValue("@amount", item.amount);
                    sqlCommand.ExecuteNonQuery();
                    tran.Commit();
                    mydb.Close();

                    mydb = new MySqlConnection(connStr);
                    mydb.Open();
                    tran = mydb.BeginTransaction();

                    sql = "insert into orderDetail (userId, productId, amount, subTotal, orderId) values(@userId, @productId, @amount, @subTotal, @orderId)";

                    sqlCommand = mydb.CreateCommand();
                    sqlCommand.Transaction = tran;
                    sqlCommand.CommandText = sql;
                    sqlCommand.Parameters.AddWithValue("@productId", item.productId);
                    sqlCommand.Parameters.AddWithValue("@amount", item.amount);
                    sqlCommand.Parameters.AddWithValue("@userId", item.userId);
                    sqlCommand.Parameters.AddWithValue("@subTotal", item.subTotal);
                    sqlCommand.Parameters.AddWithValue("@orderId", orderId);
                    sqlCommand.ExecuteNonQuery();
                    tran.Commit();
                    mydb.Close();

                    mydb = new MySqlConnection(connStr);
                    mydb.Open();
                    tran = mydb.BeginTransaction();

                    sql = "delete from cart where cartId = @cartID";

                    sqlCommand = mydb.CreateCommand();
                    sqlCommand.Transaction = tran;
                    sqlCommand.CommandText = sql;
                    sqlCommand.Parameters.AddWithValue("@cartID", item.cartId);
                    sqlCommand.ExecuteNonQuery();
                    tran.Commit();
                    mydb.Close();
                }

                return true;

            }
            catch
            {
                tran?.Rollback();
                return false;
            }
            finally
            {
                tran?.Dispose();
                mydb?.Close();
                mydb?.Dispose();
            }
        }
    }
}
