using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContextAwareScrumBoard.Model
{
    public class Column
    {
        public string Name { get; set; }
        public List<TaskItem> TaskItems { get; set; } 
    }
}
