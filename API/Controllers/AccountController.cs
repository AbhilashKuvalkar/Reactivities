using System.Text;
using API.DTOs;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly SignInManager<User> _signInManager;
        private readonly IEmailSender<User> _emailSender;
        private readonly IConfiguration _configuration;

        public AccountController(SignInManager<User> signInManager, IEmailSender<User> emailSender, IConfiguration configuration)
        {
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            _emailSender = emailSender ?? throw new ArgumentNullException(nameof(emailSender));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult> Register(RegisterDto registerDto)
        {
            var user = new User
            {
                UserName = registerDto.Email,
                Email = registerDto.Email,
                DisplayName = registerDto.DisplayName
            };

            var result = await _signInManager.UserManager.CreateAsync(user, registerDto.Password);

            if (result.Succeeded)
            {
                await SendConfirmationEmailAsync(user, registerDto.Email);
                return Ok();
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(error.Code, error.Description);
            }

            return ValidationProblem();
        }

        [AllowAnonymous]
        [HttpGet("user-info")]
        public async Task<ActionResult> GetUserInfo()
        {
            if (User.Identity?.IsAuthenticated == false)
                return NoContent();

            var user = await _signInManager.UserManager.GetUserAsync(User);

            if (user == null)
                return Unauthorized();

            return Ok(new
            {
                user.DisplayName,
                user.Email,
                user.Id,
                user.ImageUrl
            });
        }

        [HttpPost("logout")]
        public async Task<ActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return NoContent();
        }

        [AllowAnonymous]
        [HttpGet("resendConfirmEmail")]
        public async Task<IActionResult> ResendConfirmEmail(string? email, string? userId)
        {
            if (string.IsNullOrEmpty(email) && string.IsNullOrEmpty(userId))
                return BadRequest("Email or UserId must be provided");

            var user = await _signInManager.UserManager.Users
                .FirstOrDefaultAsync(x => x.Email == email || x.Id == userId);

            if (user is null || string.IsNullOrEmpty(user.Email))
                return BadRequest("User not found");

            await SendConfirmationEmailAsync(user, user.Email);

            return Ok();
        }

        private async Task SendConfirmationEmailAsync(User user, string email)
        {
            var code = await _signInManager.UserManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedCode = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var confirmationUrl = $"{_configuration["ClientAppUrl"]}/confirm-email?userId={user.Id}&code={encodedCode}";

            await _emailSender.SendConfirmationLinkAsync(user, email, confirmationUrl);
        }

    }
}
