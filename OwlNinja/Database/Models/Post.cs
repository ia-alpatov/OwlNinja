using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OwlNinja.Database.Models
{
    public class Post 
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string EnTitle { get; set; }

        public string Summary { get; set; }

        public string Content { get; set; }

        public DateTime Time { get; set; }

        public virtual ICollection<PostTag> Tags { get; set; } = new List<PostTag>();

        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    }
}
