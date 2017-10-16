using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Net.Http;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using OwlNinja.Database.Models;
using System.IO;
using OwlNinja.Database;
using System.Security.Cryptography;
using System.Text;
using OwlNinja.Models;
using Microsoft.AspNetCore.Hosting;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using Microsoft.EntityFrameworkCore;

namespace OwlNinja.Controllers
{

    public class AdminsController : Controller
    {
        private BlogContext db;
        private IHostingEnvironment hostingEnvironment;

        public AdminsController([FromServices] BlogContext db, IHostingEnvironment hostingEnvironment)
        {
            this.db = db;
            this.hostingEnvironment = hostingEnvironment;
        }


        //POST /api/admins login username and password
        [Route("api/admins/auth")]
        [HttpPost]
        [AllowAnonymous]
        [ServiceFilter(typeof(ValidateReCaptchaAttribute))]
        public IActionResult Login([FromForm]string username, [FromForm]string password)
        {
            if (!ModelState.IsValid)
                return Unauthorized();
            
            var user = db.Admins.SingleOrDefault(admin => admin.Username.ToLower() == username.ToLower());

            if (user != null)
            {
                string passwordHash = string.Empty;

                //sha256(sha256(password)+salt)
                using (var sha256 = SHA256.Create())
                {
                    var passwordBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

                    string prePassword = BitConverter.ToString(passwordBytes).Replace("-", "").ToLower();

                    var withSaltBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(prePassword + user.Salt));
                    passwordHash = BitConverter.ToString(withSaltBytes).Replace("-", "").ToLower();
                }

                if (user.Password == passwordHash)
                {
                    var token = new JwtTokenBuilder()
                                   .AddSecurityKey(JwtSecurityKey.Create())
                                   .AddSubject(user.Username + " " + user.Password)
                                   .AddIssuer("OwlNinja.Security.Bearer")
                                   .AddAudience("OwlNinja.Security.Bearer")
                                   .AddClaim("AdminId", user.Id.ToString())
                                   .AddExpiry(1)
                                   .Build();

                    return Ok(token.Value);
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                return NotFound();
            }
        }


        //PATCH api/admins Change site settings 
        [Authorize]
        [Route("api/admins/settings")]
        [HttpPut]
        public IActionResult ChangeSiteSettings([FromForm]string sets)
        {
            Settings settings = JsonConvert.DeserializeObject<Settings>(sets);


            var setting = db.Settings.FirstOrDefault();

            setting.HomeTitle = settings.HomeTitle;

            setting.HomeSubHeading = settings.HomeSubHeading;
            setting.PostsByTagsTitle = settings.PostsByTagsTitle;
            setting.AboutMeTitle = settings.AboutMeTitle;
            setting.AboutMeSubHeading = settings.AboutMeSubHeading;
            setting.AboutMeHtml = settings.AboutMeHtml;

            setting.HomeBgUrl = (String.IsNullOrWhiteSpace(settings.HomeBgUrl))? setting.HomeBgUrl : settings.HomeBgUrl;
            setting.PostsByTagsBgUrl = (String.IsNullOrWhiteSpace(settings.PostsByTagsBgUrl)) ? setting.PostsByTagsBgUrl : settings.PostsByTagsBgUrl;
            setting.AboutMeBgUrl = (String.IsNullOrWhiteSpace(settings.AboutMeBgUrl)) ? setting.AboutMeBgUrl : settings.AboutMeBgUrl;

            setting.SettingsImage = (String.IsNullOrWhiteSpace(settings.SettingsImage)) ? setting.SettingsImage : settings.SettingsImage;
            setting.CreatePostImage = (String.IsNullOrWhiteSpace(settings.CreatePostImage)) ? setting.CreatePostImage : settings.CreatePostImage;
            setting.AdminImage = (String.IsNullOrWhiteSpace(settings.AdminImage)) ? setting.AdminImage : settings.AdminImage;

            db.SaveChanges();

            return Ok();
        }

        //GET api/admins Change site settings 
        [AllowAnonymous]
        [Route("api/admins/settings")]
        [HttpGet]
        public JsonResult GetSiteSettings()
        {
            if (!db.Settings.Any())
            {
                var setting = new Settings();

                setting.HomeTitle = "Заглавная страница";
                setting.HomeSubHeading = "Подзаголовок заглавной страницы";

                setting.PostsByTagsTitle = "Посты по тэгам";

                setting.AboutMeTitle = "Обо мне"; ;
                setting.AboutMeSubHeading = "Подзаголовок страницы обо мне"; ;
                setting.AboutMeHtml = @"<h3>Тут может быть ваш текст</>";

                setting.HomeBgUrl = "img/home-bg.jpg";
                setting.PostsByTagsBgUrl = "img/home-bg.jpg";
                setting.AboutMeBgUrl = "img/home-bg.jpg";

                setting.SettingsImage = "img/home-bg.jpg";
                setting.CreatePostImage = "img/home-bg.jpg";
                setting.AdminImage = "img/home-bg.jpg";

                db.Settings.Add(setting);
                db.SaveChanges();
            }


            var settings = db.Settings.FirstOrDefault();
            return Json(settings);
        }

        //POST api/admins/image upload image and get url
        [Authorize]
        [Route("api/admins/image")]
        [HttpPost]
        public IActionResult UploadImage([FromForm]string data)
        {
            try
            {
                var base64Data = Regex.Match(data, @"data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;
                var base64array = Convert.FromBase64String(base64Data);
                var pathUrl = $"uploads/admins/{ Guid.NewGuid()}.jpg";
                var filePath = Path.Combine($"{hostingEnvironment.ContentRootPath}/wwwroot/" + pathUrl);
                System.IO.File.WriteAllBytes(filePath, base64array);

                return Ok(pathUrl);
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }


        // POST api/posts create new post from admin panel
        [Route("api/admins/post")]
        [Authorize]
        [HttpPost]
        public IActionResult CreateNewPost([FromForm]string post)
        {
            PostRequest Post = JsonConvert.DeserializeObject<PostRequest>(post);

            Post dbPost = new Post()
            {
                Title = Post.PostTitle,
                EnTitle = Post.EnTitle,
                Time = DateTime.Now,
                Content = Post.PostHtml,
                Summary = Post.PostSubHeading,
                HeaderImage = Post.HeaderPostImage
            };

            db.Posts.Add(dbPost);
            db.SaveChanges();

            foreach (var tag in Post.Tags)
                dbPost.Tags.Add(new PostTag() { Tag = tag});

            db.SaveChanges();

            return Ok();
        }

        // POST api/posts create new post from admin panel
        [Route("api/admins/tags")]
        [Authorize]
        [HttpGet]
        public IActionResult GetTags()
        {
            var tags = db.PostTags.Select(tag => tag.Tag).Distinct().ToList();
            return Json(tags);
        }


        // PATCH api/posts/1 edit post N from admin panel
        [Route("api/admins/post")]
        [Authorize]
        [HttpPatch]
        public IActionResult EditPost([FromForm]string post)
        {
            PostRequest Post = JsonConvert.DeserializeObject<PostRequest>(post);

            var dbPost = db.Posts.Include(p=>p.Tags).FirstOrDefault(p => p.EnTitle == Post.EnTitle);

            if (dbPost != null)
            {
                dbPost.Summary = Post.PostSubHeading;
                dbPost.Content = Post.PostHtml;
                dbPost.Title = Post.PostTitle;
                dbPost.EnTitle = Post.EnTitle;
                dbPost.HeaderImage = (String.IsNullOrWhiteSpace(Post.HeaderPostImage))? dbPost.HeaderImage : Post.HeaderPostImage;

                List<PostTag> tagsToDelete = new List<PostTag>();
                foreach (var tag in dbPost.Tags)
                {
                    if (!Post.Tags.Contains(tag.Tag))
                    {
                        tagsToDelete.Add(tag);
                    }
                }

                foreach (var tag in Post.Tags)
                {
                    if (!dbPost.Tags.Any(t => t.Tag == tag))
                    {
                        dbPost.Tags.Add(new PostTag() { Tag = tag });
                    }
                }

                foreach (var tag in tagsToDelete)
                    db.Remove(tag);

                db.SaveChanges();

                return Ok();
            }
            else
            {
                return NotFound();
            }
        }

        // DELETE api/admins/post/1 delete post N from admin panel
        [Route("api/admins/post")]
        [Authorize]
        [HttpDelete]
        public IActionResult DeletePost([FromForm]string url)
        {
            var post = db.Posts.SingleOrDefault(c => c.EnTitle == url);

            if (post != null)
            {
                db.Posts.Remove(post);
                db.SaveChanges();
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }

        // DELETE api/admins/comment/1 delete comment N from admin panel
        [Route("api/admins/comment")]
        [Authorize]
        [HttpDelete]
        public IActionResult DeleteComment([FromForm]string id)
        {
            var comment = db.Comments.SingleOrDefault(c => c.Id.ToString() == id);

            if (comment != null)
            {
                db.Comments.Remove(comment);
                db.SaveChanges();
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }
    }

    public sealed class JwtToken
    {
        private JwtSecurityToken token;

        internal JwtToken(JwtSecurityToken token)
        {
            this.token = token;
        }

        public DateTime ValidTo => token.ValidTo;
        public string Value => new JwtSecurityTokenHandler().WriteToken(this.token);
    }

    public class ValidateReCaptchaAttribute : ActionFilterAttribute
    {
        public const string ReCaptchaModelErrorKey = "ReCaptcha";
        private const string RecaptchaResponseTokenKey = "g-recaptcha-response";
        private const string ApiVerificationEndpoint = "https://www.google.com/recaptcha/api/siteverify";
        private readonly IConfiguration m_configuration;
        private readonly Lazy<string> m_reCaptchaSecret;
        public ValidateReCaptchaAttribute(IConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }
            this.m_configuration = configuration;
            this.m_reCaptchaSecret = new Lazy<string>(() => m_configuration["ReCaptcha:Secret"]);
        }
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            await DoReCaptchaValidation(context);
            await base.OnActionExecutionAsync(context, next);
        }
        private async Task DoReCaptchaValidation(ActionExecutingContext context)
        {
            if (!context.HttpContext.Request.HasFormContentType)
            {
                // Get request? 
                AddModelError(context, "No reCaptcha Token Found");
                return;
            }
            string token = context.HttpContext.Request.Form[RecaptchaResponseTokenKey];
            if (string.IsNullOrWhiteSpace(token))
            {
                AddModelError(context, "No reCaptcha Token Found");
            }
            else
            {
                await ValidateRecaptcha(context, token);
            }
        }
        private static void AddModelError(ActionExecutingContext context, string error)
        {
            context.ModelState.AddModelError(ReCaptchaModelErrorKey, error.ToString());
        }
        private async Task ValidateRecaptcha(ActionExecutingContext context, string token)
        {
            using (var webClient = new HttpClient())
            {
                var content = new FormUrlEncodedContent(new[]
                {
                        new KeyValuePair<string, string>("secret", this.m_reCaptchaSecret.Value),
                        new KeyValuePair<string, string>("response", token)
                    });
                HttpResponseMessage response = await webClient.PostAsync(ApiVerificationEndpoint, content);
                string json = await response.Content.ReadAsStringAsync();
                var reCaptchaResponse = JsonConvert.DeserializeObject<ReCaptchaResponse>(json);
                if (reCaptchaResponse == null)
                {
                    AddModelError(context, "Unable To Read Response From Server");
                }
                else if (!reCaptchaResponse.success)
                {
                    AddModelError(context, "Invalid reCaptcha");
                }
            }
        }
    }
    public class ReCaptchaResponse
    {
        public bool success { get; set; }
        public string challenge_ts { get; set; }
        public string hostname { get; set; }
        public string[] errorcodes { get; set; }
    }



    public sealed class JwtTokenBuilder
    {
        private SecurityKey securityKey = null;
        private string subject = "";
        private string issuer = "";
        private string audience = "";
        private Dictionary<string, string> claims = new Dictionary<string, string>();
        private int expiryInMinutes = 5;

        public JwtTokenBuilder AddSecurityKey(SecurityKey securityKey)
        {
            this.securityKey = securityKey;
            return this;
        }

        public JwtTokenBuilder AddSubject(string subject)
        {
            this.subject = subject;
            return this;
        }

        public JwtTokenBuilder AddIssuer(string issuer)
        {
            this.issuer = issuer;
            return this;
        }

        public JwtTokenBuilder AddAudience(string audience)
        {
            this.audience = audience;
            return this;
        }

        public JwtTokenBuilder AddClaim(string type, string value)
        {
            this.claims.Add(type, value);
            return this;
        }

        public JwtTokenBuilder AddClaims(Dictionary<string, string> claims)
        {
            this.claims.Union(claims);
            return this;
        }

        public JwtTokenBuilder AddExpiry(int expiryInMinutes)
        {
            this.expiryInMinutes = expiryInMinutes;
            return this;
        }

        public JwtToken Build()
        {
            EnsureArguments();

            var claims = new List<Claim>
            {
              new Claim(JwtRegisteredClaimNames.Sub, this.subject),
              new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            }
            .Union(this.claims.Select(item => new Claim(item.Key, item.Value)));

            var token = new JwtSecurityToken(
                              issuer: this.issuer,
                              audience: this.audience,
                              claims: claims,
                              expires: DateTime.UtcNow.AddMinutes(expiryInMinutes),
                              signingCredentials: new SigningCredentials(
                                                        this.securityKey,
                                                        SecurityAlgorithms.HmacSha256));

            return new JwtToken(token);
        }

        #region " private "

        private void EnsureArguments()
        {
            if (this.securityKey == null)
                throw new ArgumentNullException("Security Key");

            if (string.IsNullOrEmpty(this.subject))
                throw new ArgumentNullException("Subject");

            if (string.IsNullOrEmpty(this.issuer))
                throw new ArgumentNullException("Issuer");

            if (string.IsNullOrEmpty(this.audience))
                throw new ArgumentNullException("Audience");
        }

        #endregion
    }
}
