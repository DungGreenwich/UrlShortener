using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.ComponentModel.DataAnnotations;
using URLShortener.Data;
using URLShortener.Models;
using System.Text;

namespace URLShortener.Controllers
{
	public class URLsController : Controller
	{
        private static Random random = new Random(DateTime.Now.Millisecond);
        private static readonly char[] chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789".ToCharArray();

		private readonly URLDbContext _context;

		public URLsController(URLDbContext context)
		{
			_context = context;
        }

		// GET: URLs
		public async Task<IActionResult> Index()
		{
			// List of all URLS then display it on the webpage
			ViewData["Host"] = $"{Request.Scheme}://{Request.Host}";
            return View(await _context.URLs.ToListAsync());
		}

		// DELETE: URLs
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Delete(Guid id)
		{
			var item = _context.URLs.Find(id);
			if (item != null)
			{
				_context.URLs.Remove(item);
				_context.SaveChanges();
			}
			return RedirectToAction("Index");
        }

        // Shortens the URL.
        // This will only succeed if the original link is an URL.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Shorten(URL url)
		{
			if (ModelState.IsValid)
			{
                string id = Guid.NewGuid().ToString();
                
				var builder = new StringBuilder(8);

				for (int i = 0; i < 8; i++)
					builder.Append(chars[random.Next() % chars.Length]);

                // Get first 8 chars of GUID + 8 random chars as link
                url.ShortCode = id.Substring(0, 8) + builder.ToString();

                // Add to database & save changes
                _context.URLs.Add(url);
                _context.SaveChanges();

                // Craft a link from the short code: https://host/shortCode.
                TempData["ResultMessage"] = $"Your shortened URL: {Request.Scheme}://{Request.Host}/{url.ShortCode}";
            }
			else
			{
				// Error
                TempData["ResultMessage"] = $"Input string is not an URL";
            }

			return RedirectToAction("Index");
        }

		// Redirect to original link when shortened URL is typed.
		[HttpGet("/{code}")]
		public IActionResult RedirectToOriginal(string code)
		{
			// Find destination URL from shortened URL
			var url = _context.URLs.FirstOrDefault(u => u.ShortCode == code);
			if (url == null)
				return NotFound();

			return Redirect(url.OriginalUrl);
		}
	}
}
