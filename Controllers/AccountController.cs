using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SimpleTaskManagementWebApplication.Models;
using SimpleTaskManagementWebApplication.ViewModels;

namespace SimpleTaskManagementWebApplication.Controllers
{
    public class AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager) : Controller
    {
        private readonly UserManager<AppUser> _userManager = userManager;
        private readonly SignInManager<AppUser> _signInManager = signInManager;

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register([Bind("FirstName, LastName, UserName, Email, Password, ConfirmPassword")] RegistrationViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var appUser = new AppUser
                {
                    FirstName = viewModel.FirstName,
                    LastName = viewModel.LastName,
                    Email = viewModel.Email,
                    UserName = viewModel.Email
                };

                var result = await _userManager.CreateAsync(appUser, viewModel.Password);

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(appUser, false);

                    return RedirectToAction(nameof(Index), nameof(TaskItem));
                }
                else
                {
                    ViewData["errors"] = result.Errors;
                }
            }

            return View(viewModel);
        }

        public IActionResult Login(string returnUrl = "/")
        {
            var login = new LoginViewModel
            {
                ReturnUrl = returnUrl
            };

            return View(login);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, isPersistent: false, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    return Redirect(model.ReturnUrl ?? "/");
                }
                else
                {
                    ModelState.AddModelError("ReturnUrl", "Invalid login attempt.");
                }
            }

            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction(nameof(Login));
        }
    }
}
