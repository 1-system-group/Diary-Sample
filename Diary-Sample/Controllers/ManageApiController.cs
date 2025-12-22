using System.Text.Json;
using Diary_Sample.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Namotion.Reflection;

namespace Diary_Sample.Controllers
{

    // TODO 認証はいったん無しにしておく 
    //    [Authorize(Roles = "manager")]
    [ApiController]
    [Route("api/v1/[controller]/[action]")]
    [Produces("application/json")]
    public class ManageApiController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<ManageController> _logger;
        public ManageApiController(
            ILogger<ManageController> logger,
            UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
            _logger = logger;
        }
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BadRequestResult), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Index()
        {
            var manageViewModel = new ManageViewModel(_userManager, 1);
            // 画面表示に必要な部分だけJSON形式にして返す
            string json = JsonSerializer.Serialize(new { manageViewModel.Users, manageViewModel.Page });
            return Ok(json);
        }

        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BadRequestResult), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Paging(int page)
        {
            var manageViewModel = new ManageViewModel(_userManager, page);
            // 画面表示に必要な部分だけJSON形式にして返す
            string json = JsonSerializer.Serialize(new { manageViewModel.Users, manageViewModel.Page });
            return Ok(json);
        }

        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BadRequestResult), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Unlock([FromBody] string unlockId)
        {
            IdentityUser user = await _userManager.FindByIdAsync(unlockId).ConfigureAwait(false);
            // ロック解除
            await _userManager.SetLockoutEndDateAsync(user, null).ConfigureAwait(false);
            // 画面表示に必要な部分だけJSON形式にして返す
            var manageViewModel = new ManageViewModel(_userManager, user);
            // ロックを解除したユーザのページを表示
            string json = JsonSerializer.Serialize(new { manageViewModel.Users, manageViewModel.Page });
            return Ok(json);

        }
    }
}
