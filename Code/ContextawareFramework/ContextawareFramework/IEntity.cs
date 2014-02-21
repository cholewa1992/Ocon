using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContextawareFramework
{
    public interface IEntity
    {
        List<IEntity> Entities { get; set; }
        int i { get; set; }
    }
}
