using System;
namespace Fate.Project.Domain.AggregatesModel
{
    public class ProjectContributor:Entity
    {
        public ProjectContributor()
        {
            CreatedTime = DateTime.Now;
        }
        public int? ProjectId { get; set; }

        public int? UserId { get; set; }

        public string UserName { get; set; }

        public string Avatar { get; set; }

        public DateTime? CreatedTime { get; set; }

        /// <summary>
        /// 关闭者
        /// </summary>
        public bool? IsCloser { get; set; }

        /// <summary>
        /// 贡献者类型 1:财务顾问  2:投资机构
        /// </summary>
        public int? ContributorType { get; set; }
    }
}