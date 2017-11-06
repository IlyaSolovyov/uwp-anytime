using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnyTimeT10.Models
{
    public class TaskCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public string ColorCode { get; set; }

        List<DailyTask> DailyTasks { get; set; }

        public override String ToString()
        {
            return Name;
        }
    }
}
