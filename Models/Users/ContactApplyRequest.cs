using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Users
{
    [BsonIgnoreExtraElements]
    public class ContactApplyRequest
    {
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
        /// 申请人ID
        /// </summary>
        public int ApplierId { get; set; }

        /// <summary>
        /// 是否通过 0:未通过  1:待处理 2:已通过
        /// </summary>
        public int? Approvaled { get; set; }

        /// <summary>
        /// 处理时间
        /// </summary>
        public DateTime? HandledTime { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateTime { get; set; }
    }
}
