using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class CapConsulConfig
    {
        public string UseConsul { get; set; }
        public string CurrentNodeHostName { get; set; }
        public int CurrentNodePort { get; set; }
        public string DiscoveryServerHostName { get; set; }
        public int DiscoveryServerPort { get; set; }
        public int NodeId { get; set; }
        public string NodeName { get; set; }
        public string MatchPath { get; set; }
    }
}
