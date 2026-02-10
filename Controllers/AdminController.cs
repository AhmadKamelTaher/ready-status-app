using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReadyStatusApp.Data;
using ReadyStatusApp.Models;
using ReadyStatusApp.ViewModels;

namespace ReadyStatusApp.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public AdminController(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Dashboard()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null || !currentUser.IsAdmin)
            {
                return RedirectToAction("Dashboard", "User");
            }

            var users = await _userManager.Users
                .Where(u => !u.IsAdmin)
                .Select(u => new UserStatusViewModel
                {
                    UserId = u.Id,
                    FullName = u.FullName,
                    Email = u.Email ?? string.Empty,
                    IsReady = u.IsReady,
                    LastStatusChange = u.LastStatusChange
                })
                .ToListAsync();

            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsersStatus()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null || !currentUser.IsAdmin)
            {
                return Forbid();
            }

            var users = await _userManager.Users
                .Where(u => !u.IsAdmin)
                .Select(u => new UserStatusViewModel
                {
                    UserId = u.Id,
                    FullName = u.FullName,
                    Email = u.Email ?? string.Empty,
                    IsReady = u.IsReady,
                    LastStatusChange = u.LastStatusChange
                })
                .ToListAsync();

            return Json(users);
        }

        [HttpGet]
        public async Task<IActionResult> StatusHistory()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null || !currentUser.IsAdmin)
            {
                return RedirectToAction("Dashboard", "User");
            }

            var logs = await _context.StatusLogs
                .Include(s => s.User)
                .OrderByDescending(s => s.Timestamp)
                .Take(50)
                .ToListAsync();

            return View(logs);
        }
    }
}
