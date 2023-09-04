using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logger
{
    public class CashBookLoggerSettings
    {
         
        public string WriteTo { get; set; }
        public string RollingType { get; set; }
        public string LogLevel { get; set; }
    }
}

