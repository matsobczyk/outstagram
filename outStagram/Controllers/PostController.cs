using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using outStagram.Models;

namespace outStagram.Controllers
{
    [ApiController]
    public class ErrorController : ControllerBase
    {
        [Route("/error")]
        public IActionResult Error() => Problem();
    }

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
            return _context.Posts.ToList();
        }

        [HttpGet("{id}")]
        public ActionResult<Post> GetPost(int id)
        {
            var Post = _context.Posts.Find(id);

            if (Post == null)
            {
                return NotFound();
            }

            return Post;
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind("id,title,description,author,pictureFile")][FromForm] Post post)
        {
                try
                {
                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    string fileName;
                     fileName = Path.GetFileNameWithoutExtension(post.pictureFile.FileName);
                    string extension = Path.GetExtension(post.pictureFile.FileName);
                    post.pictureUrl = fileName = fileName + DateTime.Now.ToString("yymmss") + extension;
                    string path = Path.Combine(wwwRootPath + "/pictures/", fileName);

                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await post.pictureFile.CopyToAsync(fileStream);
                    }
                }
                catch (NullReferenceException  e )
                {
                    return Ok(e.Message);
                }
                catch (Exception e)
                {
                    return Ok(e.Message);
                }

                _context.Add(post);
                    await _context.SaveChangesAsync();
                    //return Ok();
                    return Created("","dodano post o id: " +  post.id);
                
                
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch([FromRoute] int id, [Bind("id,title,description,author,pictureFile")] Post post)
        {
            if (id != post.id)
            {
                return BadRequest();
            }

            string wwwRootPath = _hostEnvironment.WebRootPath;
            string fileName = Path.GetFileNameWithoutExtension(post.pictureFile.FileName);
            string extension = Path.GetExtension(post.pictureFile.FileName);
            post.pictureUrl = fileName = fileName + DateTime.Now.ToString("yymmss") + extension;
            string path = Path.Combine(wwwRootPath + "/pictures/", fileName);

            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                post.pictureFile.CopyTo(fileStream);
            }

            _context.Posts.Update(post);
            _context.SaveChanges();

            return Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult DeletePost(int id)
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
    }
}