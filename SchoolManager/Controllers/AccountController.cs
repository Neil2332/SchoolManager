using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManager.Data.Entities;
using SchoolManager.Helpers;
using SchoolManager.Models;

namespace SchoolManager.Controllers
{
    public class AccountController : Controller
    {
    
        private readonly IUserHelper _userHelper;

        public AccountController(IUserHelper userHelper)
        {
            _userHelper = userHelper;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _userHelper.LoginAsync(
                model.Email,
                model.Password,
                model.RememberMe);

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, "Email ou palavra-passe inválidos.");
            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var existingUser = await _userHelper.GetUserByEmailAsync(model.Email);

            if (existingUser != null)
            {
                ModelState.AddModelError(nameof(model.Email), "Este email já está registado.");
                return View(model);
            }

            var user = new User
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                UserName = model.Email
            };

            var result = await _userHelper.AddUserAsync(user, model.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View(model);
            }

            const string defaultRole = "Utilizador";

            if (!await _userHelper.CheckRoleAsync(defaultRole))
            {
                await _userHelper.AddRoleAsync(defaultRole);
            }

            await _userHelper.AddUserToRoleAsync(user, defaultRole);

            await _userHelper.LoginAsync(model.Email, model.Password, false);

            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _userHelper.LogoutAsync();
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var email = User.Identity?.Name;

            if (string.IsNullOrWhiteSpace(email))
            {
                return RedirectToAction(nameof(Login));
            }

            var user = await _userHelper.GetUserByEmailAsync(email);

            if (user == null)
            {
                return NotFound();
            }

            var model = new ProfileViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email ?? string.Empty
            };

            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(ProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var currentEmail = User.Identity?.Name;

            if (string.IsNullOrWhiteSpace(currentEmail))
            {
                return RedirectToAction(nameof(Login));
            }

            var user = await _userHelper.GetUserByEmailAsync(currentEmail);

            if (user == null)
            {
                return NotFound();
            }

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Email = model.Email;
            user.UserName = model.Email;

            var result = await _userHelper.UpdateUserAsync(user);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View(model);
            }

            TempData["Success"] = "Perfil atualizado.";
            return RedirectToAction(nameof(Profile));
        }

        [Authorize]
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var email = User.Identity?.Name;

            if (string.IsNullOrWhiteSpace(email))
            {
                return RedirectToAction(nameof(Login));
            }

            var user = await _userHelper.GetUserByEmailAsync(email);

            if (user == null)
            {
                return NotFound();
            }

            var result = await _userHelper.ChangePasswordAsync(
                user,
                model.OldPassword,
                model.NewPassword);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View(model);
            }

            TempData["Success"] = "Palavra-passe alterada.";
            return RedirectToAction(nameof(Profile));
        }

        [HttpGet]
        public IActionResult RecoverPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RecoverPassword(RecoverPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userHelper.GetUserByEmailAsync(model.Email);

            if (user == null)
            {
                TempData["Success"] = "Se o email existir, será possível recuperar a palavra-passe.";
                return RedirectToAction(nameof(Login));
            }

            var token = await _userHelper.GeneratePasswordResetTokenAsync(user);

            return RedirectToAction(nameof(ResetPassword), new
            {
                email = model.Email,
                token
            });
        }

        [HttpGet]
        public IActionResult ResetPassword(string email, string token)
        {
            return View(new ResetPasswordViewModel
            {
                Email = email,
                Token = token
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userHelper.GetUserByEmailAsync(model.Email);

            if (user == null)
            {
                return NotFound();
            }

            var result = await _userHelper.ResetPasswordAsync(
                user,
                model.Token,
                model.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View(model);
            }

            TempData["Success"] = "Palavra-passe redefinida.";
            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}