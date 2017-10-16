using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using OwlNinja.Database;
using OwlNinja.Models;
using Microsoft.EntityFrameworkCore;
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
        [AllowAnonymous]
        public JsonResult GetPosts([FromQuery] int skip,[FromQuery] string tag)
        {
            PostsResult result = new PostsResult();
            result.Posts = new List<PostResult>();

            IQueryable<Database.Models.Post> query;
            if (string.IsNullOrWhiteSpace(tag))
            {
                query = db.Posts.OrderByDescending(post => post.Time);
            }
            else
            {
                query = db.Posts.OrderByDescending(post => post.Time).Include(p=>p.Tags).Where(post => post.Tags.Any(t => t.Tag == tag));
            }
            
            result.CountLeft = query.Skip(skip + 10).Count();
            var posts = query.Skip(skip).Take(10).Include(p => p.Tags);

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
                    Tags = post.Tags.Select(t => t.Tag).ToList()
                });
            }

            return Json(result);
        }

        // GET api/posts/1 get post N data
        [Route("api/post")]
        [AllowAnonymous]
        [HttpGet]
        public IActionResult GetPost([FromQuery]string url)
        {
            var post = db.Posts.Include(p=>p.Tags).SingleOrDefault(p => p.EnTitle == url);

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

       
    }
}
