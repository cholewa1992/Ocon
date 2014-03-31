using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ContextAwareScrumBoard.Model
{
    public class ProductItem
    {

        public string Name { get; set; }
        public string Description { get; set; }
        public string SizeEstimate { get; set; }
        public List<Column> Columns { get; set; }
        public SolidColorBrush BackgroundColor { get; set; }
    }
}
