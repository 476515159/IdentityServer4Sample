using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Users
{
    public class UserTags
    {
        public int UserId { get; set; }

        public string Tag { get; set; }

        public DateTime? CreateTime { get; set; }
    }
}
