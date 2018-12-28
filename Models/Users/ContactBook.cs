using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Users
{
    [BsonIgnoreExtraElements]
    public class ContactBook
    {
        public ContactBook()
        {
            Contacts = new List<Contact>();
        }
        public int UserId { get; set; }

        /// <summary>
        /// 联系人列表
        /// </summary>
        public List<Contact> Contacts { get; set; }
    }
}
