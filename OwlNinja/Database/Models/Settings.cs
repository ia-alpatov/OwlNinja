using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OwlNinja.Database.Models
{
    public class Settings
    {

        public Guid Id { get; set; }

        public string HomeTitle { get; set; }
        public string HomeSubHeading { get; set; }
        public string PostsByTagsTitle { get; set; }
        public string AboutMeTitle { get; set; }
        public string AboutMeSubHeading { get; set; }
        public string AboutMeHtml { get; set; }
        public string HomeBgUrl { get; set; }
        public string PostsByTagsBgUrl { get; set; }
        public string AboutMeBgUrl { get; set; }
        public string SettingsImage { get; set; }
        public string CreatePostImage { get; set; }
        public string AdminImage { get; set; }
    }

}
