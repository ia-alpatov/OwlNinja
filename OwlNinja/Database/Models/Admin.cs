using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OwlNinja.Database.Models
{
    public class Admin
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
    }
}
