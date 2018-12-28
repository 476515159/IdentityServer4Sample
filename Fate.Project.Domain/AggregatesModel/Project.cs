using System;
using System.Collections.Generic;
using System.Linq;
using Fate.Project.Domain.Events;

namespace Fate.Project.Domain.AggregatesModel
{
    public class Project:Entity,IAggregateRoot
    {

        public Project()
        {
            Viewers = new List<ProjectViewer>();
            Contributors = new List<ProjectContributor>();
            AddDomainEvent(new ProjectCreatedDomainEvent { Project=this});
        }
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

        /// <summary>
        /// 可见范围设置
        /// </summary>
        public PrjectVisibleRule VisibleRule { get; set; }

        /// <summary>
        /// 根引用项目Id
        /// </summary>
        public int? SourceId { get; set; }

        /// <summary>
        /// 上级项目引用Id
        /// </summary>
        public int? ReferenceId { get; set; }

        /// <summary>
        /// 项目标签
        /// </summary>
        public string Tags { get; set; }

        /// <summary>
        /// 项目属性:行业领域、融资币种
        /// </summary>
        public List<ProjectProperty> Properties { get; set; }

        /// <summary>
        /// 贡献者列表
        /// </summary>
        public List<ProjectContributor> Contributors { get; set; }

        /// <summary>
        /// 查看者
        /// </summary>
        public List<ProjectViewer> Viewers { get; set; }

        public DateTime? CreatedTime { get; set; }


        public DateTime? UpdateTime { get; set; }

        /// <summary>
        /// 克隆项目
        /// </summary>
        /// <param name="souce"></param>
        /// <returns></returns>
        private Project CloneProject(Project souce=null)
        {
            if (souce == null)
                souce = this;

            var newProject = new Project
            {
                AreaId = souce.AreaId,
                AreaName = souce.AreaName,
                Avatar = souce.Avatar,
                BrokerageOptions = souce.BrokerageOptions,
                City = souce.City,
                CityId=souce.CityId,
                Company=souce.Company,
                CreatedTime=DateTime.Now,
                FinMoney=souce.FinMoney,
                Contributors=new List<ProjectContributor>(),
                FinPercentage=souce.FinPercentage,
                FinStage=souce.FinStage,
                FormatBPFile=souce.FormatBPFile,
                Income=souce.Income,
                Introduction=souce.Introduction,
                OnPlatform=souce.OnPlatform,
                OriginBPFile=souce.OriginBPFile,
                Properties=souce.Properties,
                Province=souce.Province,
                ProvinceId=souce.ProvinceId,
                ReferenceId=souce.ReferenceId,
                RegisterTime=souce.RegisterTime,
                Revenue=souce.Revenue,
                ShowSecurityInfo=souce.ShowSecurityInfo,
                SourceId=souce.SourceId,
                Tags=souce.Tags,
                UpdateTime=souce.UpdateTime,
                Valuation=souce.Valuation,
                Viewers=new List<ProjectViewer>(),
                VisibleRule=souce.VisibleRule==null? null:new PrjectVisibleRule
                {
                    Visible=souce.VisibleRule.Visible,
                    Tags=souce.VisibleRule.Tags
                }
                
            };

            return newProject;
        }

        /// <summary>
        /// 参考者得到项目拷贝
        /// </summary>
        /// <param name="ContributorId"></param>
        /// <param name="souce"></param>
        /// <returns></returns>
        public Project ContributorFork(int ContributorId, Project souce = null)
        {
            if (souce == null)
                souce = this;

            var newProject = CloneProject(souce);
            newProject.UserId = ContributorId;
            newProject.UpdateTime = DateTime.Now;
            newProject.SourceId = souce.SourceId == 0 ? souce.Id : souce.SourceId;
            newProject.ReferenceId = souce.ReferenceId == 0 ? souce.Id : souce.ReferenceId;
            return newProject;
        }

        public void Add_ProjectViewer(int UserId, string UserName, string Avatar)
        {
            ProjectViewer viewer = new ProjectViewer
            {
                UserId = UserId,
                UserName = UserName,
                Avatar = Avatar,
                CreatedTime = DateTime.Now
            };

            if (!Viewers.Any(c => c.UserId == UserId))
            {
                Viewers.Add(viewer);
                AddDomainEvent(new ProjectViewerDomainEvent { ProjectViewer=viewer,Avatar=this.Avatar,Name=this.Avatar});
            }
                
        }


        public void Add_ProjectContributor(ProjectContributor model)
        {
            if (!Contributors.Any(c => c.UserId == model.UserId))
            {
                Contributors.Add(model);
                AddDomainEvent(new ProjectJoinedDomainEvent { ProjectContributor = model,Avatar=this.Avatar,Name=this.Avatar });
            }
                
        }
    }
}
