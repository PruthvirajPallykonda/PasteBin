using Microsoft.AspNetCore.Mvc;
using PastebinApi.Data;
using PastebinApi.Models;

namespace PastebinApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PastesController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _config;

        public PastesController(AppDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        // POST: /api/pastes
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePasteRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Content))
                return BadRequest(new { error = "content is required" });

            if (request.Ttl_Seconds.HasValue && request.Ttl_Seconds <= 0)
                return BadRequest(new { error = "ttl_seconds must be >= 1" });

            if (request.Max_Views.HasValue && request.Max_Views <= 0)
                return BadRequest(new { error = "max_views must be >= 1" });

            var id = Guid.NewGuid().ToString("N")[..8];
            var now = GetNow();

            DateTimeOffset? expiresAt = null;
            if (request.Ttl_Seconds.HasValue)
                expiresAt = now.AddSeconds(request.Ttl_Seconds.Value);

            var paste = new Paste
            {
                Id = id,
                Content = request.Content,
                Views = 0,
                MaxViews = request.Max_Views,
                ExpiresAt = expiresAt
            };

            _db.Pastes.Add(paste);
            await _db.SaveChangesAsync();

            var url = $"{Request.Scheme}://{Request.Host}/p/{id}";
            return Ok(new { id, url });
        }

        // GET: /api/pastes/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var paste = await _db.Pastes.FindAsync(id);
            if (paste == null)
                return NotFound(new { error = "Not found" });

            var now = GetNow();

            // Expiry check
            if (paste.ExpiresAt.HasValue && now > paste.ExpiresAt.Value)
            {
                _db.Pastes.Remove(paste);
                await _db.SaveChangesAsync();
                return NotFound(new { error = "Expired" });
            }

            // View limit check (BEFORE increment)
            if (paste.MaxViews.HasValue && paste.Views >= paste.MaxViews.Value)
                return NotFound(new { error = "View limit exceeded" });

            // Increment views
            paste.Views++;
            await _db.SaveChangesAsync();

            int? remainingViews = paste.MaxViews.HasValue
                ? Math.Max(paste.MaxViews.Value - paste.Views, 0)
                : null;

            return Ok(new
            {
                content = paste.Content,
                remaining_views = remainingViews, // ✅ correct now
                expires_at = paste.ExpiresAt?.UtcDateTime.ToString("o")
            });
        }

        private DateTimeOffset GetNow()
        {
            var testMode = _config["TEST_MODE"];

            if (testMode == "1" &&
                Request.Headers.TryGetValue("x-test-now-ms", out var headerVal) &&
                long.TryParse(headerVal.FirstOrDefault(), out var ms))
            {
                return DateTimeOffset.FromUnixTimeMilliseconds(ms);
            }

            return DateTimeOffset.UtcNow;
        }
    }
}
