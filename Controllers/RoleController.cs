using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class RoleController : Controller
    {
        // GET: RoleController
        private ApplicationDbContext _context;
        public RoleController(ApplicationDbContext context)
        {
            _context = context;
        }
        public ActionResult Index()
        {
            var roles = _context.Roles.ToList();
            return View(roles);
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateRoleRequest request)
        {
            if (ModelState.IsValid)
            {
                if (await _context.Roles.AnyAsync(r => r.Name == request.RoleName))
                {
                    ModelState.AddModelError(string.Empty, "Role with the same name already exists.");
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

    }
}
