using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using ReadyStatusApp.Data;
using ReadyStatusApp.Hubs;
using ReadyStatusApp.Models;

namespace ReadyStatusApp.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<StatusHub> _hubContext;

        public UserController(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context,
            IHubContext<StatusHub> hubContext)
        {
            _userManager = userManager;
            _context = context;
            _hubContext = hubContext;
        }

        public async Task<IActionResult> Dashboard()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (user.IsAdmin)
            {
                return RedirectToAction("Dashboard", "Admin");
            }

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Json(new { success = false, message = "User not found" });
            }

            user.IsReady = !user.IsReady;
            user.LastStatusChange = DateTime.UtcNow;

            await _userManager.UpdateAsync(user);

            // Log the status change
            var statusLog = new StatusLog
            {
                UserId = user.Id,
                IsReady = user.IsReady,
                Timestamp = DateTime.UtcNow
            };
            _context.StatusLogs.Add(statusLog);
            await _context.SaveChangesAsync();

            // Notify admin in real-time
            await _hubContext.Clients.Group("Admins").SendAsync(
                "ReceiveStatusUpdate",
                user.Id,
                user.FullName,
                user.IsReady,
                user.LastStatusChange?.ToString("yyyy-MM-dd HH:mm:ss"));

            return Json(new { success = true, isReady = user.IsReady });
        }

        [HttpGet]
        public async Task<IActionResult> GetStatus()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Json(new { success = false });
            }

            return Json(new { success = true, isReady = user.IsReady });
        }
    }
}
