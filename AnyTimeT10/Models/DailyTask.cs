using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnyTimeT10.Models
{
    public class DailyTask
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Completed { get; set; }
        public DateTime? Date { get; set; }
        public DateTime? Deadline { get; set; }

        public int CategoryId { get; set; }
        public TaskCategory Category { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
    }
}
