using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using outStagram.Models;

namespace outStagram.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly PostContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        public PostController(PostContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        [HttpGet]
        public IEnumerable<Post> GetPosts()
        {
            try
            {
                List<Post> posts = new List<Post>();
                return _context.Posts.ToList();
            }
            catch
            {
                Response.StatusCode = 500;
            }
            return null;
        }

        [HttpGet("/postimage/{id}")]
        public ActionResult<Post> GetPostImage(int id)
        {
            var image = System.IO.File.OpenRead(@"C:\sem4\outstagram\outstagram\outStagram\wwwroot\pictures\default.jpg");
            try
            {
                var Post = _context.Posts.Find(id);
                image = System.IO.File.OpenRead(@"C:\sem4\outstagram\outstagram\outStagram\wwwroot\pictures\" + Post.pictureUrl);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Something went wrong");
            }
            return File(image, "image/jpg");
        }

        [HttpGet("{id}")]
        public ActionResult<Post> GetPost(int id)
        {
            try
            {
                var Post = _context.Posts.Find(id);
                if (Post == null)
                {
                    return NotFound("There are no posts in database");
                }
                return Post;
            }
            catch (Exception ex)
            {
                return BadRequest(new { errorMessage = ex.Message });
            }
        }


        [HttpPost]
        public async Task<IActionResult> Create([Bind("id,title,description,author,pictureFile")][FromForm] Post post)
        {

            string wwwRootPath = _hostEnvironment.WebRootPath;
            string fileName;

            try
            {
                try
                {
                    fileName = Path.GetFileNameWithoutExtension(post.pictureFile.FileName);
                    string extension = Path.GetExtension(post.pictureFile.FileName);
                    post.pictureUrl = fileName = fileName + DateTime.Now.ToString("yymmss") + extension;
                    string path = Path.Combine(wwwRootPath + "/pictures/", fileName);

                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await post.pictureFile.CopyToAsync(fileStream);
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine("picture not found");
                    post.pictureUrl = "default.jpg";
                }


                if (post == null)
                {
                    return NotFound("Your post is empty!");
                }
                else
                {
                    _context.Add(post);
                    await _context.SaveChangesAsync();
                    return Created("", "dodano post o id: " + post.id);
                }

            }
            catch (Exception e)
            {
                return BadRequest(new { errorMessage = e.Message });
            }


        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch(int id, Post post)
        {
            try
            {
                Post postOld = _context.Posts
               .Where(l => l.id == id)
               .FirstOrDefault();

                if (postOld == null)
                {
                    throw new Exception("post not found");
                }else if (post == null)
                {
                    throw new Exception("wrong request");
                }

                postOld.title = post.title;
                postOld.description = post.description;
                postOld.author = post.author;

                _context.Posts.Update(postOld);
                _context.SaveChanges();
                return Ok("Zaktualizowano post " + postOld.id);
                
            }
            catch (Exception e)
            {
                return BadRequest(new { errorMessage = e.Message });
            }


        } 



        [HttpDelete("{id}")]
        public IActionResult DeletePost(int id)
        {
            try
            {
                var Post = _context.Posts.Find(id);

                if (Post == null)
                {
                    return NotFound();
                }

                _context.Posts.Remove(Post);
                _context.SaveChanges();

                return Ok(Post);
            }
            catch (Exception e)
            {
                return BadRequest(new { errorMessage = e.Message });
            }

        }
    }
}