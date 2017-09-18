using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OwlNinja.Models
{
    public class PostRequest
    {
        public string Title { get; set; }

        public string EnTitle { get; set; }

        public string Summary { get; set; }

        public string Content { get; set; }

        public DateTime Time { get; set; }

        public List<string> Tags { get; set; }
    }
}
