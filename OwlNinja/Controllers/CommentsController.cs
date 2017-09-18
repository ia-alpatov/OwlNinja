using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using OwlNinja.Database;

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
        public JsonResult Get(int id)
        {
            return "value";
        }

        // POST api/comments/1 send comment for post
        [HttpPost("{id}")]
        public JsonResult Put(int id, [FromBody]string comment)
        {

        }

    }
}
