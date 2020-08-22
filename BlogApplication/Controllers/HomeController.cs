﻿using BlogApplication.Data;
using BlogApplication.Models;
using BlogApplication.Repository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApplication.Controllers
{
    public class HomeController:Controller
    {
        private IRepository _repo;
        private IFileManager _fileManager;

        public HomeController(IRepository repo, IFileManager fileManager)
        {
            _repo = repo;
            _fileManager = fileManager;
        }
        public IActionResult Index(string category)
        {
            var posts = String.IsNullOrEmpty(category) ? _repo.GetAllPost():_repo.GetAllPost(category);
            //
            return View(posts);
        }

        public IActionResult Post(int id)
        {
            var post = _repo.GetPost(id);
            return View(post);
        }
        [HttpGet("/Image/{image}")]
        public IActionResult Image(string image)
        {
            var mime = image.Substring(image.LastIndexOf('.')+1);
            return new FileStreamResult(_fileManager.ImageStream(image),$"image/{mime}");

        }
       
    }
}
