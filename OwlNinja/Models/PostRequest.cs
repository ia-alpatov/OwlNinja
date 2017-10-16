using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OwlNinja.Models
{
    public class PostRequest
    {
        public string EnTitle { get; set; }
        public string HeaderPostImage { get; set; }
        public string PostSubHeading { get; set; }
        public string PostTitle { get; set; }
        public string PostHtml { get; set; }
        public string PostDate { get; set; }

        public List<string> Tags { get; set; }
    }
}
