using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fate.Recommend.Model
{
    public class RecommendProject
    {
        public int Id { get; set; }
        public int? ProjectId { get; set; }
        /// <summary>
        /// 用户id
        /// </summary>
        public int? UserId { get; set; }

        public string UserName { get; set; }

        /// <summary>
        /// 项目来源用户
        /// </summary>
        public int? FromUserId { get; set; }

        public string FromUserName { get; set; }

        /// <summary>
        /// 项目Logo
        /// </summary>
        public string Avatar { get; set; }
        /// <summary>
        /// 公司名称
        /// </summary>
        public string Company { get; set; }
    }
}
