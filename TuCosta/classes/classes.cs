using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TuCosta.classes
{
    public class resultReason
    {
        public string reason { get; set; }
        public DateTime date_entry { get; set; }
    }

    public class places
    {
        public int idplace { get; set; }
        public int idtype { get; set; }
        public string state { get; set; }
        public string type { get; set; }
        public string place { get; set; }
        public string description { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string schedule { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public string image { get; set; }
    }

    public class result
    {
        public int created { get; set; }
    }

    public class types
    {
        public int idtype { get; set; }
        public string name { get; set; }
        public string image { get; set; }
    }

    public class UserInfo
    {
        string uname;
        string pwd;
        public string Username
        {
            get { return uname; }
            set { uname = value; }
        }
        public string Password
        {
            get { return pwd; }
            set { pwd = value; }
        }
    }

}
