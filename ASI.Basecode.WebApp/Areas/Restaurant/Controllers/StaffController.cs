using System;
using System.Linq;
using System.Threading.Tasks;
using ASI.Basecode.Data;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Manager;
using ASI.Basecode.WebApp.Areas.Restaurant.Models;
using ASI.Basecode.WebApp.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ASI.Basecode.WebApp.Areas.Restaurant.Controllers
{
    [Authorize(Policy = "Restaurant")]
    [Area("Restaurant")]
    [Route("Restaurant/[controller]/[action]")]
    [IgnoreAntiforgeryToken]
    public class StaffController : ControllerBase<StaffController>
    {
        private readonly AsiBasecodeDBContext _db;

        public StaffController(
            IHttpContextAccessor httpContextAccessor,
            ILoggerFactory loggerFactory,
            IConfiguration configuration,
            IMapper mapper,
            AsiBasecodeDBContext db
        ) : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult Index() => View();

        // Helpers
        private static string Ago(DateTime? utc)
        {
            if (utc == null) return "—";
            var s = DateTime.UtcNow - utc.Value;
            if (s.TotalMinutes < 1) return "just now";
            if (s.TotalMinutes < 60) return $"{(int)s.TotalMinutes} min ago";
            if (s.TotalHours < 24) return $"{(int)s.TotalHours} hours ago";
            return $"{(int)s.TotalDays} days ago";
        }

        // API 
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var data = await _db.Staff
                .Include(s => s.User)
                .Select(s => new StaffListItemDto
                {
                    Id = s.Id,
                    FirstName = s.FirstName,
                    LastName = s.LastName,
                    Email = s.User.Email,
                    Role = s.Role,
                    Status = s.Status,
                    LastLogin = Ago(s.LastLoginUtc)
                })
                .ToListAsync();

            return Json(new { ok = true, data });
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var s = await _db.Staff.Include(x => x.User).FirstOrDefaultAsync(x => x.Id == id);
            if (s == null) return NotFound();

            var dto = new StaffDetailsDto
            {
                Id = s.Id,
                FirstName = s.FirstName,
                LastName = s.LastName,
                Email = s.User.Email,
                Phone = s.Phone,
                Address = s.Address,
                Role = s.Role,
                Status = s.Status,
                HireDate = s.HireDate,
                AnnualSalary = s.AnnualSalary,
                LastLogin = Ago(s.LastLoginUtc)
            };
            return Json(new { ok = true, data = dto });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] StaffUpsertDto vm)
        {
            if (!ModelState.IsValid) return BadRequest(new { ok = false, message = "Invalid data" });
            if (string.IsNullOrWhiteSpace(vm.Password))
                return BadRequest(new { ok = false, message = "Password is required" });

            // 1) Create User nga naka hash ang pass
            var exists = await _db.Users.AnyAsync(u => u.Email == vm.Email);
            if (exists) return BadRequest(new { ok = false, message = "Email already exists" });

            var encryptedPassword = PasswordManager.EncryptPassword(vm.Password);

            var user = new User
            {
                Email = vm.Email,
                Password = encryptedPassword,
                FirstName = vm.FirstName,
                LastName = vm.LastName,
                Role = "Restaurant",
                IsEmailVerified = true,
                CreatedTime = DateTime.UtcNow,
                UpdatedTime = DateTime.UtcNow
            };
            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            // 2) Create Staff profile linked to User para makuha ang login info sa User 
            var staff = new Staff
            {
                UserID = user.UserID,
                FirstName = vm.FirstName,
                LastName = vm.LastName,
                Phone = vm.Phone,
                Address = vm.Address,
                Role = vm.Role,
                Status = vm.Status,
                HireDate = vm.HireDate,
                AnnualSalary = vm.AnnualSalary
            };
            _db.Staff.Add(staff);
            await _db.SaveChangesAsync();

            var dto = new StaffListItemDto
            {
                Id = staff.Id,
                FirstName = staff.FirstName,
                LastName = staff.LastName,
                Email = user.Email,
                Role = staff.Role,
                Status = staff.Status,
                LastLogin = "—"
            };
            return Json(new { ok = true, data = dto });
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] StaffUpsertDto vm)
        {
            var s = await _db.Staff.Include(x => x.User).FirstOrDefaultAsync(x => x.Id == id);
            if (s == null) return NotFound();

            // Update User
            if (!string.Equals(s.User.Email, vm.Email, StringComparison.OrdinalIgnoreCase))
                s.User.Email = vm.Email;
            if (!string.IsNullOrWhiteSpace(vm.Password))
                s.User.Password = PasswordManager.EncryptPassword(vm.Password);
            s.User.FirstName = vm.FirstName;
            s.User.LastName = vm.LastName;
            s.User.UpdatedTime = DateTime.UtcNow;

            // Update Staff
            s.FirstName = vm.FirstName;
            s.LastName = vm.LastName;
            s.Phone = vm.Phone;
            s.Address = vm.Address;
            s.Role = vm.Role;
            s.Status = vm.Status;
            s.HireDate = vm.HireDate;
            s.AnnualSalary = vm.AnnualSalary;

            await _db.SaveChangesAsync();

            var dto = new StaffListItemDto
            {
                Id = s.Id,
                FirstName = s.FirstName,
                LastName = s.LastName,
                Email = s.User.Email,
                Role = s.Role,
                Status = s.Status,
                LastLogin = Ago(s.LastLoginUtc)
            };
            return Json(new { ok = true, data = dto });
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var s = await _db.Staff.FirstOrDefaultAsync(x => x.Id == id);
            if (s == null) return NotFound();

            var user = await _db.Users.FirstOrDefaultAsync(u => u.UserID == s.UserID);

            _db.Staff.Remove(s);
            await _db.SaveChangesAsync();

            if (user != null)
            {
                _db.Users.Remove(user);
                await _db.SaveChangesAsync();
            }

            return Json(new { ok = true });
        }
    }
}
