using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using OwlNinja.Database;
using OwlNinja.Database.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OwlNinja.Controllers
{
    
    [AllowAnonymous]
    public class PostsController : Controller
    {
        private BlogContext db;

        public PostsController(BlogContext db)
        {
            this.db = db;
        }

        // GET: api/posts get posts list
        [Route("api/posts")]
        [HttpGet]
        public JsonResult GetPosts([FromBody] int skip)
        {


            var posts = db.Posts.OrderByDescending(post => post.Time).Skip(skip).Take(10);

            foreach (var post in posts)
            {

            }

            return Json();
        }

        // GET api/posts/1 get post N data
        [Route("api/posts")]
        [HttpGet("{id}")]
        public JsonResult GetPost(int id)
        {
           
        }

        // GET api/posts/TAGVALUE get posts data by tag
        [Route("api/tags")]
        [HttpGet("{tag}")]
        public JsonResult GetPostsByTag(string tag)
        {

        }
    }
}
