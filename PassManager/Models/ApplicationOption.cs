using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassManager.Models
{
    public class ApplicationOption
    {
        public ApplicationOption Instance { get; } = new ApplicationOption();
        private ApplicationOption() { }

        public string DefaultPassPath { get; set; }
        public string DefaultKeyPath { get; set; }
    }
}
