using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SimpleTaskManagementWebApplication.Data;
using SimpleTaskManagementWebApplication.Models;
using SimpleTaskManagementWebApplication.ViewModels;

namespace SimpleTaskManagementWebApplication.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register([Bind("FirstName, LastName, UserName, Email, Password, ConfirmPassword")] RegistrationViewModel rvm)
        {
            if (ModelState.IsValid)
            {
                AppUser appUser = new AppUser
                {
                    FirstName = rvm.FirstName,
                    LastName = rvm.LastName,
                    Email = rvm.Email,
                    UserName = rvm.Email
                };
                IdentityResult result = await _userManager.CreateAsync(appUser, rvm.Password);

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
            return View(rvm);
        }

        public IActionResult Login(string returnUrl = "/")
        {
            LoginViewModel login = new LoginViewModel();
            login.ReturnUrl = returnUrl;
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
