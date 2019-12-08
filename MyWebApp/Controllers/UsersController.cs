using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using MyWebApp.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MyWebApp.Controllers
{
    public class UsersController : Controller
    {
        private readonly MyContext _context;
        private readonly IConfiguration _configuration;

        public UsersController(MyContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            return View(await _context.Users.ToListAsync());
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Age,ProfileFileName")] User user)
        {
            if (!ModelState.IsValid)
            {
                return View(user);
            }

            IFormFile file = Request.Form.Files["profile"];
            if (file != null && file.Length > 0)
            {
                user.ProfileFileName = await SaveFile(file);
            }

            _context.Add(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Age,ProfileFileName")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(user);
            }

            IFormFile file = Request.Form.Files["profile"];
            if (file != null && file.Length > 0)
            {
                user.ProfileFileName = await SaveFile(file);
            }

            try
            {
                _context.Update(user);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(user.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }

        private async Task<string> SaveFile(IFormFile file)
        {
            string connectionString = _configuration.GetConnectionString("StorageConnectionString");
            CloudStorageAccount.TryParse(connectionString, out CloudStorageAccount storageAccount);

            CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference("mycontainer");
            await cloudBlobContainer.CreateIfNotExistsAsync();

            BlobContainerPermissions permissions = await cloudBlobContainer.GetPermissionsAsync();
            permissions.PublicAccess = BlobContainerPublicAccessType.Blob;
            await cloudBlobContainer.SetPermissionsAsync(permissions);

            string fileName = Path.GetFileName(file.FileName);
            CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(fileName);
            await cloudBlockBlob.UploadFromStreamAsync(file.OpenReadStream());

            return cloudBlockBlob.Uri.ToString();
        }
    }
}
