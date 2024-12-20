using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using JobMatch.Data;
using JobMatch.Models;
using Microsoft.AspNetCore.Authorization;
using static System.Net.Mime.MediaTypeNames;
using System.IO;
using Microsoft.AspNetCore.Http;
using JobMatch.Data.Migrations;

namespace JobMatch.Controllers
{
    public class JobsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public JobsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Jobs
        public async Task<IActionResult> index ()
        {
            var jobs = _context.Job.Include(j => j.Company);
            return View(await jobs.ToListAsync());
        }
        public async Task<IActionResult> ListJob()
        {
            var jobs = _context.Job.Include(j => j.Company);
            return View(await jobs.ToListAsync());

        }
        public async Task<IActionResult> Status()
        {
            var jobs = _context.Job.Include(j => j.Company);
            return View(await jobs.ToListAsync());

        }
        public async Task<IActionResult> Search()
        {
            var jobs = _context.Job.Include(j => j.Company.Name);
            return View(await jobs.ToListAsync());
        }


        // Thêm phương thức tìm kiếm
        // search by key word Name, Description, company 
        public async Task<IActionResult> Search(string keyword)
        {
            var jobs = from j in _context.Job.Include(j => j.Company)
                       select j;

            if (!string.IsNullOrEmpty(keyword))
            {
                jobs = jobs.Where(j => j.Name.Contains(keyword) ||
                                       j.Description.Contains(keyword) ||
                                       j.Company.Name.Contains(keyword));
            }

            return View("ListJob", await jobs.ToListAsync());
        }

        //public IActionResult Status(int id)
        //{
        //    // Lấy công việc theo ID
        //    var job = _context.Job.FirstOrDefault(j => j.Id == id);

        //    if (job == null)
        //    {
        //        return NotFound("Không tìm thấy công việc này.");
        //    }

        //    return View(job); // Truyền công việc sang View Status
        //}


        // GET: Jobs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var job = await _context.Job
                .Include(j => j.Company)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (job == null)
            {
                return NotFound();
            }

            return View(job);
        }
      
        // GET: Jobs/Create
        [Authorize]

        public IActionResult Create()
        {
            ViewData["CompanyId"] = new SelectList(_context.Company, "Id", "Name");
            return View();
        }

        // POST: Jobs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create([Bind("Id,Name,Description,Salary,Image,CompanyId")] Job job, IFormFile Image)
        {
            if (ModelState.IsValid)
            {
                //validate image is valid or not
                if (Image != null && Image.Length > 0)
                {
                    //set image file name => ensure file name is unique
                    var fileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(Image.FileName);
                    //set image file location
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        //copy (upload) image file from orignal location to project folder
                        Image.CopyTo(stream);
                    }

                    //set image file name for book cover
                    job.Image = "/images/" + fileName;
                }
                _context.Add(job);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CompanyId"] = new SelectList(_context.Set<Company>(), "Id", "Id", job.CompanyId);
            return View(job);
        }
        [Authorize(Roles = "Admin")]
        // GET: Jobs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var job = await _context.Job.FindAsync(id);
            if (job == null)
            {
                return NotFound();
            }
            ViewData["CompanyId"] = new SelectList(_context.Set<Company>(), "Id", "Id", job.CompanyId);
            return View(job);
        }

        // POST: Jobs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Salary,Image,CompanyId")] Job job)
        {
            if (id != job.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(job);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JobExists(job.Id))
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
            //ViewData["CompanyId"] = new SelectList(_context.Set<Company>(), "Id", "Id", job.CompanyId);
            //return View(job);
            ViewData["CompanyId"] = new SelectList(_context.Company, "Id", "Id", job.CompanyId);
            return View(job);
        }

        // GET: Jobs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var job = await _context.Job
                .Include(j => j.Company)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (job == null)
            {
                return NotFound();
            }

            return View(job);
        }

        // POST: Jobs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var job = await _context.Job.FindAsync(id);
            _context.Job.Remove(job);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
      


        private bool JobExists(int id)
        {
            return _context.Job.Any(e => e.Id == id);
        }

        //[HttpPost]
        //public async Task<IActionResult> Search (string model)
        //{
        //    var jobs = _context.Job.Include(j => j.Company).Where(jo => jo.Name.Contains(model));
        //    return View("List", await jobs.ToListAsync());

        //}
        [HttpPost]
        public IActionResult HandleStatus(int id, string action)
        {
            var job = _context.Job.FirstOrDefault(j => j.Id == id);

            if (job == null)
            {
                return NotFound("Không tìm thấy công việc này.");
            }

            if (action == "Accept")
            {
                TempData["Message"] = $"Bạn đã chấp nhận công việc: {job.Name}";
            }
            else if (action == "NotAccept")
            {
                TempData["Message"] = $"Bạn đã từ chối công việc: {job.Name}";
            }

            return RedirectToAction("JobList"); // Quay lại danh sách công việc
        }




    }
}
