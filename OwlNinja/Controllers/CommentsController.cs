using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using OwlNinja.Database;
using OwlNinja.Models;
using OwlNinja.Database.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OwlNinja.Controllers
{
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class CommentsController : Controller
    {

        private BlogContext db;

        public CommentsController(BlogContext db)
        {
            this.db = db;
        }

        // GET api/comments/1 get comments for post N
        [HttpGet("{id}")]
        public JsonResult GetCommentForPost(string id)
        {
            CommentsResult result = new CommentsResult();
            result.Comments = new List<CommentResult>();
            var query = db.Comments.Where(comment => comment.PostId.ToString() == id);
            
            foreach (var comment in query)
            {
                result.Comments.Add(new CommentResult()
                {
                    Id = comment.Id.ToString(),
                    Text = comment.Text,
                    Time = comment.Time
                });
            }

            return Json(result);
        }

        // POST api/comments/1 send comment for post
        [HttpPost("{id}")]
        [ServiceFilter(typeof(ValidateReCaptchaAttribute))]
        public IActionResult PostComment(string id, [FromBody]string comment)
        {
            if (!ModelState.IsValid)
                return Unauthorized();

            var post = db.Posts.SingleOrDefault(p => p.Id.ToString() == id);

            if (post != null)
            {
                post.Comments.Add(new Comment() {  Time = DateTime.Now, Text = comment});
                db.SaveChanges();
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }

    }
}
