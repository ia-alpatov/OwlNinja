using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OwlNinja.Database.Models
{
    public class Comment
    {
        public Guid Id { get; set; }

        [ForeignKey("Post")]
        public Guid PostId { get; set; }

        public virtual Post Post { get; set; }

        public DateTime Time { get; set; }

        public string Text { get; set; }
    }
}
