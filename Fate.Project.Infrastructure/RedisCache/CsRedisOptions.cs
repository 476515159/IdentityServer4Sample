using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fate.Project.Infrastructure.RedisCache
{
    public class CsRedisOptions:IOptions<CsRedisOptions>
    {
        public List<string> RedisConnectionStrings { get; set; }

        public string RedisKeyPrefix { get; set; }

        public CsRedisOptions Value => this;
    }
}
