using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OwlNinja.Database.Models
{
    public class PostTag
    {
        public Guid Id { get; set; }

        [ForeignKey("Post")]
        public Guid PostId { get; set; }

        public virtual Post Post { get; set; }

        [MaxLength(64)]
        public string Tag { get; set; }
    }
}
