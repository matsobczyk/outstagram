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

            if (_context.Posts.Count() == 0)
            {
                _context.Posts.Add(new Post { id=1, title = "Post 1", description="lalal", author="Autor" });;
                _context.SaveChanges();
            }
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
           // if (ModelState.IsValid)
            {   // saving pictures of posts to wwwroot/pictures 
                string wwwRootPath = _hostEnvironment.WebRootPath;
                string fileName = Path.GetFileNameWithoutExtension(post.pictureFile.FileName);
                string extension = Path.GetExtension(post.pictureFile.FileName);
                post.pictureUrl = fileName = fileName + DateTime.Now.ToString("yymmss") + extension;
                string path = Path.Combine(wwwRootPath + "/pictures/", fileName);

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await post.pictureFile.CopyToAsync(fileStream);
                }


                _context.Add(post);
                await _context.SaveChangesAsync();
                return Ok();
                // return Created("dodano post o id: ",post.id);
            }
            //return View(artPiece);
        }
    }
}