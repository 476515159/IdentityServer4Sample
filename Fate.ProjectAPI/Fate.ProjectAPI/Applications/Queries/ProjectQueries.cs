using Microsoft.Extensions.Options;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using MySql.Data.MySqlClient;
using Fate.Project.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using System.Data;

namespace Fate.ProjectAPI.Applications.Queries
{
    public class ProjectQueries : IProjectQueries
    {
        private readonly ProjectContext _projectContext;
        public ProjectQueries(ProjectContext projectContext)
        {
            _projectContext = projectContext;
        }

        public async Task<List<dynamic>> Get_ProjectByUserId(int UserId)
        {
            using (var conn = _projectContext.Database.GetDbConnection())
            {
                conn.Open();
                string sql = @"select * from Project where UserId=@UserId";
                //var a = conn.CreateCommand();
                //a.CommandText = sql;
                
                //DbDataAdapter dbDataAdapter = DbProviderFactories.GetFactory(conn).CreateDataAdapter();
                //dbDataAdapter.SelectCommand=a;
                //DataSet ds = new DataSet();
                //dbDataAdapter.Fill(ds);

                
                return (await conn.QueryAsync<dynamic>(sql, new { UserId })).ToList();
            }

        }

        public async Task<dynamic> Get_ProjectDetail(int ProjectId)
        {
            using (var conn = _projectContext.Database.GetDbConnection())
            {
                conn.Open();
                string sql = @"select 
a.Company,
a.City,
a.AreaName,
a.Province,
a.FinStage,
a.FinMoney,
a.Valuation,
a.FinPercentage,
a.Introduction,
a.UserId,
a.Income,
a.Revenue,
a.Avatar,
a.BrokerageOptions,
b.Tags,
b.Visible
FROM project a INNER JOIN
prjectvisiblerule b on a.Id=b.ProjectId
where a.Id=@ProjectId
";
                return await conn.QueryFirstOrDefaultAsync<dynamic>(sql, new { ProjectId });
            }
        }
    }
}
