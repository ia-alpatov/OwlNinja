using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using OwlNinja.Database;
using OwlNinja.Models;
using OwlNinja.Database.Models;

using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OwlNinja.Controllers
{
    [Route("api/comments")]
    [AllowAnonymous]
    public class CommentsController : Controller
    {

        private BlogContext db;

        public CommentsController([FromServices] BlogContext db)
        {
            this.db = db;
        }

        // GET api/comments/1 get comments for post N
        [HttpGet]
        [AllowAnonymous]
        public JsonResult GetCommentForPost([FromQuery]string url)
        {
            CommentsResult result = new CommentsResult();
            result.Comments = new List<CommentResult>();
            var query = db.Posts.Include(p=>p.Comments).SingleOrDefault(p => p.EnTitle == url);
            
            foreach (var comment in query.Comments)
            {
                result.Comments.Add(new CommentResult()
                {
                    Id = comment.Id.ToString(),
                    Text = comment.Text,
                    Time = comment.Time.ToString("HH:MM dd.mm.yyyy"),
                });
            }

            return Json(result);
        }

        // POST api/comments/1 send comment for post
        [HttpPost]
        [ServiceFilter(typeof(ValidateReCaptchaAttribute))]
        [AllowAnonymous]
        public IActionResult PostComment([FromForm]string url, [FromForm]string text)
        {
            if (!ModelState.IsValid)
                return Unauthorized();

            var post = db.Posts.Include(p=>p.Comments).SingleOrDefault(p => p.EnTitle == url);

            if (post != null)
            {
                post.Comments.Add(new Comment() {  Time = DateTime.Now, Text = System.Net.WebUtility.HtmlEncode(text) });
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
