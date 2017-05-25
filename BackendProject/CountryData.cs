using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendProject
{
    class CountryData
    {
        public string CountryCode { get; set; }

        public Dictionary<int, int> budgets { get; }

        public CountryData(string code)
        {
            CountryCode = code;
            budgets = new Dictionary<int, int>();
        }

        public int getSum()
        {
            return budgets.Values.Sum();
        }
    }
}
