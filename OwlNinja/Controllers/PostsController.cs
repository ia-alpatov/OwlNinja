using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using OwlNinja.Database;
using OwlNinja.Models;

namespace OwlNinja.Controllers
{
    
    [AllowAnonymous]
    public class PostsController : Controller
    {
        private BlogContext db;

        public PostsController([FromServices] BlogContext db)
        {
            this.db = db;
        }

        // GET: api/posts get posts list
        [Route("api/posts")]
        [HttpGet]
        public JsonResult GetPosts([FromBody] int skip)
        {
            PostsResult result = new PostsResult();
            result.Posts = new List<PostResult>();
            var query = db.Posts.OrderByDescending(post => post.Time);
            result.CountLeft = query.Skip(skip + 10).Count();
            var posts = query.Skip(skip).Take(10);

            foreach (var post in posts)
            {
                result.Posts.Add(new PostResult() {
                    Id = post.Id.ToString(),
                    PostTitle = post.Title,
                    PostSubHeading = post.Summary,
                    HeaderPostImage = post.HeaderImage,
                    EnTitle = post.EnTitle,
                    PostHtml = post.Content,
                    PostDate = post.Time.ToString("HH:MM dd.mm.yyyy"),
                    Tags = post.Tags.Select(tag => tag.Tag).ToList()
                });
            }

            return Json(result);
        }

        // GET api/posts/1 get post N data
        [Route("api/posts")]
        [HttpGet("{url}")]
        public IActionResult GetPost(string url)
        {
            var post = db.Posts.SingleOrDefault(p => p.EnTitle == url);

            if (post != null)
            {
                var result = new PostResult()
                {
                    Id = post.Id.ToString(),
                    PostTitle = post.Title,
                    PostSubHeading = post.Summary,
                    HeaderPostImage = post.HeaderImage,
                    EnTitle = post.EnTitle,
                    PostHtml = post.Content,
                    PostDate = post.Time.ToString("HH:MM dd.mm.yyyy"),
                    Tags = post.Tags.Select(tag => tag.Tag).ToList()
                };

                return Json(result);
            }
            else
            {
                return NotFound();
            }
        }

        // GET api/tags get posts data by tag
        [Route("api/tags")]
        [HttpGet]
        public JsonResult GetPostsByTag([FromBody] string tag, [FromBody] int skip)
        {
            PostsResult result = new PostsResult();
            result.Posts = new List<PostResult>();
            var query = db.Posts.OrderByDescending(post => post.Time).Where(post => post.Tags.Any(t => t.Tag == tag));
            result.CountLeft = query.Skip(skip + 10).Count();
            var posts = query.Skip(skip).Take(10);

            foreach (var post in posts)
            {
                result.Posts.Add(new PostResult()
                {
                    Id = post.Id.ToString(),
                    PostTitle = post.Title,
                    PostSubHeading = post.Summary,
                    HeaderPostImage = post.HeaderImage,
                    EnTitle = post.EnTitle,
                    PostHtml = post.Content,
                    PostDate = post.Time.ToString("HH:MM dd.mm.yyyy"),
                    Tags = post.Tags.Select(t => t.Tag).ToList()
                });
            }

            return Json(result);
        }
    }
}
