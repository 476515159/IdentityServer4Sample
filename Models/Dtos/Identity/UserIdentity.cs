using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Dtos.Identity
{
    public class UserIdentity
    {
        public int UserId { get; set; }

        public string Name { get; set; }

        public string Avatar { get; set; }

        public string Company { get; set; }
        
        public string Title { get; set; }
    }
}
