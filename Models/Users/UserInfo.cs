using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Models.Users
{
    public class UserInfo
    {
        public int ID { get; set; }

        public string UserName { get; set; }

        public string UserPassword { get; set; }

        public string Name { get; set; }

        public string Company { get; set; }

        public string Title { get; set; }

        public string Avatar { get; set; }

        public string Phone { get; set; }

        public DateTime? LastLoginTime { get; set; } = DateTime.Now;

        //[NotMapped]
        //public List<string> Tags { get; set; }

        //public ContactBook Contactbook { get; set; }


    }
}
