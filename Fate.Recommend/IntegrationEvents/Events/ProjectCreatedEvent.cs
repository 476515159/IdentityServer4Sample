using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fate.Recommend.IntegrationEvents.Events
{
    public class ProjectCreatedEvent
    {

        public int Id { get; set; }
        /// <summary>
        /// 用户id
        /// </summary>
        public int? UserId { get; set; }

        /// <summary>
        /// 项目Logo
        /// </summary>
        public string Avatar { get; set; }
        /// <summary>
        /// 公司名称
        /// </summary>
        public string Company { get; set; }

        /// <summary>
        /// 原BP文件地址
        /// </summary>
        public string OriginBPFile { get; set; }
        /// <summary>
        /// 转换后的文件地址
        /// </summary>
        public string FormatBPFile { get; set; }

        /// <summary>
        /// 是否显示敏感信息
        /// </summary>
        public bool? ShowSecurityInfo { get; set; }

        /// <summary>
        /// 公司所在省id
        /// </summary>
        public int? ProvinceId { get; set; }

        /// <summary>
        /// 公司所在省名称
        /// </summary>
        public string Province { get; set; }

        /// <summary>
        /// 公司所在城市Id
        /// </summary>
        public int? CityId { get; set; }

        /// <summary>
        /// 公司所在城市名称
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// 区域Id
        /// </summary>
        public int? AreaId { get; set; }

        /// <summary>
        /// 区域名称
        /// </summary>
        public string AreaName { get; set; }

        /// <summary>
        /// 公司成立时间
        /// </summary>
        public DateTime? RegisterTime { get; set; }

        /// <summary>
        /// 项目基本信息
        /// </summary>
        public string Introduction { get; set; }

        /// <summary>
        /// 出让股份比例
        /// </summary>
        public string FinPercentage { get; set; }

        /// <summary>
        /// 融资阶段
        /// </summary>
        public string FinStage { get; set; }

        /// <summary>
        /// 融资金额 万
        /// </summary>
        public int? FinMoney { get; set; }

        /// <summary>
        /// 收入 万
        /// </summary>
        public int? Income { get; set; }

        /// <summary>
        /// 利润 万
        /// </summary>
        public int? Revenue { get; set; }

        /// <summary>
        /// 估值 万
        /// </summary>
        public int? Valuation { get; set; }

        /// <summary>
        /// 佣金分配方式
        /// </summary>
        public int? BrokerageOptions { get; set; }

        /// <summary>
        /// 是否委托给FateFox
        /// </summary>
        public bool? OnPlatform { get; set; }
    }
}
