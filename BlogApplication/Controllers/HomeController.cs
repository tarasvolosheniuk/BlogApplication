using BlogApplication.Comments;
using BlogApplication.Data;
using BlogApplication.Models;
using BlogApplication.Repository;
using BlogApplication.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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
        public IActionResult Index(int pageNumber,string category)
        {
            if (pageNumber<1)
            {
                return RedirectToAction("Index", new { pageNumber=1, category});
            }

            var vm = _repo.GetAllPost(pageNumber,category);

            if (vm.PageCount<pageNumber)
            {
                return RedirectToAction("Index", new { pageNumber = 1, category });
            }

            return View(vm);
        }

        public IActionResult Post(int id)
        {
            var post = _repo.GetPost(id);
            return View(post);
        }
        [HttpGet("/Image/{image}")]
        [ResponseCache(CacheProfileName ="Monthly")]
        public IActionResult Image(string image)
        {
            var mime = image.Substring(image.LastIndexOf('.')+1);
            return new FileStreamResult(_fileManager.ImageStream(image),$"image/{mime}");

        }

        [HttpPost]
        public async Task<IActionResult> Comment(CommentViewModel vm)
        {
            var post = _repo.GetPost(vm.PostId);
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Post", new { id = post.Id }); ;
            }
            if (vm.MainCommentId==0)
            {
                post.MainComments = post.MainComments ?? new List<MainComment>();

                post.MainComments.Add(new MainComment { Message = vm.Message });

                _repo.UpdatePost(post);
            }
            else
            {
                var comment = new SubComment { 
                MainCommentId =vm.MainCommentId,
                Message = vm.Message,
                };
                _repo.AddSubComment(comment);
                //_repo.UpdatePost(post);

            }

            await _repo.SaveChangesAsync();
            return RedirectToAction("Post", new { id=post.Id }); ;
        }
       
    }
}
