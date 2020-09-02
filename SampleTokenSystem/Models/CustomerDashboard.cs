using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TokenManagementSystem.Models
{
    public class CustomerDashboard
    {
        public int TokenNumber { get; set; }
        public int Counter { get; set; }
        public int EstimatedWaitingTime { get; set; } 
    }
}
