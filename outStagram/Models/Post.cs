using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace outStagram.Models
{
    public class Post
    {
        public int id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string author { get; set; }
        public string pictureUrl { get; set; }
        [NotMapped]
        public IFormFile pictureFile { get; set; }
    }
}
