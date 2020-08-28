using BlogApplication.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApplication.Controllers
{
    public class AuthController:Controller
    {
        private SignInManager<IdentityUser> _signInManager;
        private UserManager<IdentityUser> _userManager;

        public AuthController(SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginViewModel());
        }
        [HttpPost]

        public async Task<IActionResult> Login(LoginViewModel vm)
        {
            await _signInManager.PasswordSignInAsync(vm.UserName, vm.Password, false, false);
            if (User.IsInRole("admin"))
            {
                return RedirectToAction("Index", "Panel");
            }
            else
                return RedirectToAction("Index", "Home");
        }
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        [HttpPost]

        public async Task<IActionResult> Register(RegisterViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }
                var user = new IdentityUser
                {
                    UserName = vm.Email,
                    Email = vm.Email,
                };
                var result = await _userManager.CreateAsync(user, "password");

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user,false);
                return RedirectToAction("Index", "Home");

            }
            return View(vm);
        }


        [HttpGet]
        public async Task <IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
