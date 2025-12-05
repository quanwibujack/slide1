using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Route("[controller]")]
    public class RoleController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RoleController(ApplicationDbContext context)
        {
            _context = context;
        }

        // --- 1. DANH SÁCH (INDEX) ---
        [HttpGet("index")]
        public IActionResult Index()
        {
            var roles = _context.Roles.ToList();
            return View(roles);
        }

        // --- 2. TẠO MỚI (CREATE) ---
        [HttpGet("create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create(CreateRoleRequest request)
        {
            if (ModelState.IsValid)
            {
                if (await _context.Roles.AnyAsync(r => r.Name == request.RoleName))
                {
                    ModelState.AddModelError(string.Empty, "Role name already exists.");
                    return View(request);
                }

                var role = new Models.Role
                {
                    Name = request.RoleName
                };

                _context.Roles.Add(role);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(request);
        }

        // --- 3. SỬA (EDIT) ---
        [HttpGet("edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null)
            {
                return NotFound();
            }
            return View(role);
        }

        [HttpPost("edit/{id}")]
        public async Task<IActionResult> Edit(int id, Models.Role role)
        {
            // Kiểm tra ID (dùng roleId chữ thường theo model của bạn)
            if (id != role.roleId)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                // Kiểm tra trùng tên
                var exists = await _context.Roles
                    .AnyAsync(r => r.Name == role.Name && r.roleId != id);

                if (exists)
                {
                    ModelState.AddModelError("Name", "Role name already exists.");
                    return View(role);
                }

                try
                {
                    _context.Roles.Update(role);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _context.Roles.AnyAsync(e => e.roleId == id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            return View(role);
        }

        // --- 4. XÓA (DELETE) - MỚI THÊM VÀO ĐÂY ---

        // GET: Hiển thị trang xác nhận xóa
        [HttpGet("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null)
            {
                return NotFound();
            }
            return View(role);
        }

        // POST: Thực hiện xóa trong Database
        [HttpPost("delete/{id}")]
        [ActionName("DeleteConfirmed")] // Để khớp với form asp-action="DeleteConfirmed"
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null)
            {
                return NotFound();
            }

            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}