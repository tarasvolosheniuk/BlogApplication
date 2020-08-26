using BlogApplication.Data;
using BlogApplication.Models;
using BlogApplication.Repository;
using BlogApplication.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace BlogApplication.Controllers
{
    [Authorize(Roles = "admin")]

    public class PanelController:Controller
    {
        private IRepository _repo;
        private IFileManager _fileManager;

        public PanelController(IRepository repository, IFileManager fileManager)
        {
            _repo = repository;
            _fileManager = fileManager;

        }
        public IActionResult Index ()
        {
            var posts = _repo.GetAllPost();
            return View(posts);
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return View(new PostViewModel());

            }
            else
            {
                var post = _repo.GetPost((int)id);
                return View(new PostViewModel {
                    Id = post.Id,
                    Title = post.Title,
                    Body = post.Body,
                CurrentImage = post.Image,
                    Description = post.Description,
                    Category = post.Category,
                    Tags = post.Tags,

                }); ;

            }
        }
        [HttpPost]
        public async Task<IActionResult> Edit(PostViewModel vm)
        {
            var post = new Post
            {
                Id = vm.Id,
                Title = vm.Title,
                Body = vm.Body,
                Description = vm.Description,
                Category = vm.Category,
                Tags = vm.Tags,


            };
            if (vm.Image==null)
            {
                post.Image = vm.CurrentImage;
            }
            else
            {
                if (!string.IsNullOrEmpty(vm.CurrentImage))
                {
                    _fileManager.RemoveImage(vm.CurrentImage);
                }
                post.Image = await _fileManager.SaveImage(vm.Image);
            }
            if (post.Id > 0)
            {
                _repo.UpdatePost(post);
            }

            else
                _repo.AddPost(post);
            if (await _repo.SaveChangesAsync())
                return RedirectToAction("Index");
            else
                return View(post);
        }
        [HttpGet]
        public async Task<IActionResult> Remove(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");

            }
            else
            {
                _repo.RemovePost((int)id);
                await _repo.SaveChangesAsync();
                return RedirectToAction("Index");

            }
        }


    }
}
