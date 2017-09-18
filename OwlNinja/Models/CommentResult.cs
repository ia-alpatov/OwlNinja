using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OwlNinja.Models
{
    public class CommentsResult
    {
        public List<CommentResult> Comments{ get; set; }
    }


    public class CommentResult
    {
        public string Id { get; set; }

        public DateTime Time { get; set; }

        public string Text { get; set; }
    }
}
