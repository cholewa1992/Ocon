using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ocon.Entity;

namespace Entities
{
    public class Person : AbstractEntity
    {
        public bool Present { get; set; }
        public double Distance { get; set; }
    }
}
