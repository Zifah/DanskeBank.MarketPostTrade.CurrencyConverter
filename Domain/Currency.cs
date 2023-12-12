using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public record Currency
    {
        public string ISOCode { get; set; }
        public string Name { get; set; }
    }
}
