using System.Security.Claims;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace YourProject.Pages
{
    public class LoginModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly IPasswordHasher<string> _passwordHasher;

        public LoginModel(
            IConfiguration configuration,
            IPasswordHasher<string> passwordHasher)
        {
            _configuration = configuration;
            _passwordHasher = passwordHasher;
        }


        [BindProperty]
        public string Username { get; set; } = string.Empty;


        [BindProperty]
        public string Password { get; set; } = string.Empty;


        public string ErrorMessage { get; set; } = string.Empty;


        public void OnGet()
        {
        }


        public async Task<IActionResult> OnPostAsync()
        {
            var adminUsername =
                _configuration["Admin:Username"];

            var adminPasswordHash =
                _configuration["Admin:PasswordHash"];


            if (string.IsNullOrWhiteSpace(adminUsername) ||
                string.IsNullOrWhiteSpace(adminPasswordHash))
            {
                ErrorMessage =
                    "Администратор не настроен.";

                return Page();
            }


            if (!string.Equals(
                    Username,
                    adminUsername,
                    StringComparison.Ordinal))
            {
                ErrorMessage =
                    "Неверный логин или пароль!";

                return Page();
            }


            var passwordResult =
                _passwordHasher.VerifyHashedPassword(
                    Username,
                    adminPasswordHash,
                    Password);


            if (passwordResult ==
                PasswordVerificationResult.Failed)
            {
                ErrorMessage =
                    "Неверный логин или пароль!";

                return Page();
            }


            var claims = new List<Claim>
            {
                new Claim(
                    ClaimTypes.Name,
                    Username),

                new Claim(
                    ClaimTypes.Role,
                    "Admin")
            };


            var identity =
                new ClaimsIdentity(
                    claims,
                    "CookieAuth");


            var principal =
                new ClaimsPrincipal(identity);


            await HttpContext.SignInAsync(
                "CookieAuth",
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = false,

                    ExpiresUtc =
                        DateTimeOffset.UtcNow
                            .AddMinutes(30)
                });


            return RedirectToPage(
                "/Admin/ManagePosts");
        }
    }
}