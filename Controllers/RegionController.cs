using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Org.BouncyCastle.Asn1.Iana;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Route("[controller]")]
    public class RegionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RegionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // --- 1. HIỂN THỊ DANH SÁCH (INDEX) ---
        [HttpGet("index")]
        public async Task<IActionResult> Index()
        {
            var regions = await _context.Regions.ToListAsync();
            return View(regions);
        }

        // --- 2. TẠO MỚI (CREATE) ---
        [HttpGet("create")]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create(CreateRegionRequest request)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra trùng tên
                if (await _context.Regions.AnyAsync(r => r.Name == request.Name))
                {
                    ModelState.AddModelError(string.Empty, "Region with the same name already exists.");
                    return View(request);
                }

                var region = new Models.Region
                {
                    Name = request.Name
                };

                _context.Regions.Add(region);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(request);
        }

        // --- 3. CHỈNH SỬA (EDIT) - MỚI THÊM ---

        // GET: Hiển thị form sửa
        [HttpGet("edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            // Tìm region theo id trong database
            var region = await _context.Regions.FindAsync(id);

            if (region == null)
            {
                return NotFound();
            }

            // Trả về View cùng với dữ liệu region tìm được
            return View(region);
        }

        // POST: Xử lý lưu sau khi sửa
        [HttpPost("edit/{id}")]
        public async Task<IActionResult> Edit(int id, Models.Region region)
        {
            // Kiểm tra ID gửi lên có khớp với ID trong model không
            if (id != region.regionId)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                // Kiểm tra xem tên mới có bị trùng với region KHÁC không
                var existingRegionWithName = await _context.Regions
                    .FirstOrDefaultAsync(r => r.Name == region.Name && r.regionId != id);

                if (existingRegionWithName != null)
                {
                    ModelState.AddModelError("Name", "Region name already exists.");
                    return View(region);
                }

                try
                {
                    // Cập nhật dữ liệu
                    _context.Regions.Update(region);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _context.Regions.AnyAsync(e => e.regionId == id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                // Sửa xong thì quay về trang Index
                return RedirectToAction("Index");
            }
            return View(region);
        }

        // --- 4. XÓA (DELETE) ---
        [HttpGet("delete")]
        public async Task<IActionResult> Delete()
        {
            var regions = await _context.Regions.ToListAsync();
            return View(regions);
        }

        [HttpPost("delete/{id}")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var region = await _context.Regions.FindAsync(id);
            if (region == null)
            {
                return NotFound();
            }

            _context.Regions.Remove(region);
            await _context.SaveChangesAsync();
            return RedirectToAction("Delete");
        }
    }
}