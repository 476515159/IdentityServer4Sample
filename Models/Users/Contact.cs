using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using MongoDB.Bson.Serialization.Attributes;

namespace Models.Users
{
    [BsonIgnoreExtraElements] //解决以外字段_id:Element '_id' does not match any field or property of class
    public class Contact
    {
        //public ObjectId _id { get; set; } //或者加个这个
        public int UserId { get; set; }

        public string Name { get; set; }

        public string Company { get; set; }

        /// <summary>
        /// 工作职位
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        public string Avatar { get; set; }


        /// <summary>
        /// 用户标签
        /// </summary>
        public List<string> Tags { get; set; }

    }
}
