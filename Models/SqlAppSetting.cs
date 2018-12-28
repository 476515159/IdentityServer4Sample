using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class SqlAppSetting
    {
        public string SqlConnection { get; set; }

        public string MySqlConnection { get; set; }

        public int? DbType { get; set; }
    }
}
