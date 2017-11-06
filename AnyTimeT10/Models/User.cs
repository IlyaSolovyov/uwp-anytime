using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnyTimeT10.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string AvatarFilename { get; set; }

        public List <DailyTask> DailyTasks { get; set; }

        public User()
        {
            Name = null;
            Password = null;
            AvatarFilename = null;
        }
        public User(string name, string pass, string avatarFilename)
        {
            Name = name;
            Password = pass;
            AvatarFilename = avatarFilename;
        }
    }
}
