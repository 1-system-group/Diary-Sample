using System.IO;
using System.Text;
using System.Threading.Tasks;
using Diary_Sample.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Diary_Sample.Controllers
{
    [Authorize]
    public class UserAdminAccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<UserAdminAccountController> _logger;

        private const string DeleteNgMessage = "エラーが発生して削除できませんでした。";

        public UserAdminAccountController(ILogger<UserAdminAccountController> logger,
                                    SignInManager<IdentityUser> signInManager,
                                    UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
            if (user == null)
            {
                // ログイン画面に戻る
                await _signInManager.SignOutAsync().ConfigureAwait(false);
                return RedirectToAction("Index", "Auth");
            }

            UserAdminAccountViewModel userAdminAccountViewModel = new UserAdminAccountViewModel
            {
                UserId = user.Id,
            };

            return View("Index", userAdminAccountViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Download()
        {
            var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
            if (user == null)
            {
                return UpadateError(user.Id, DeleteNgMessage);
            }
            var value = "{\"Id\":" + user.Id +
                        ",\"UserName\":" + user.UserName +
                        ",\"Email\":" + user.Email +
                        ",\"EmailConfirmed\":" + user.EmailConfirmed +
                        ",\"PhoneNumber\":" + user.PhoneNumber +
                        ",\"PhoneNumberConfirmed\":" + user.PhoneNumberConfirmed + "}";
            var filename = "PersonalData.json";

            return File(new MemoryStream(Encoding.UTF8.GetBytes(value)), "application/json", filename);
        }

        private IActionResult UpadateError(string userId, string message)
        {
            UserAdminAccountViewModel outUserAdminAccountViewModel = new UserAdminAccountViewModel
            {
                UserId = userId,
                Notification = message,
                UpdateResult = false,
            };

            return View("Index", outUserAdminAccountViewModel);
        }

    }

}