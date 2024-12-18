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
using Microsoft.AspNetCore.Identity;

namespace JobMatch.Controllers
{
    [Authorize(Roles = "Administrator")]

    public class UserViewModelsController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;

        public UserViewModelsController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> UserList()
        {
            var users = await _userManager.Users.ToListAsync();
            var userList = new List<UserViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                userList.Add(new UserViewModel
                {
                    Id = user.Id,
                    Username = user.UserName,
                    Email = user.Email,
                    //Role = roles.ToList()
                });
            }

            return View(userList);
        }

        [HttpGet]
        public async Task<IActionResult> ResetPass(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction("UserList");
            }

            // Generate a password reset token
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var newPassword = "123@Abc";

            // Reset the password using the token
            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

            if (result.Succeeded)
            {
                TempData["Message"] = $"Password for {user.Email} has been reset to default (123@Abc)";
            }
            else
            {
                TempData["Error"] = $"Failed to reset password for {user.Email}";
            }

            return RedirectToAction("UserList");
        }
        //private readonly ApplicationDbContext _context;

        //public UserViewModelsController(ApplicationDbContext context)
        //{
        //    _context = context;
        //}

        //// GET: UserViewModels
        //public async Task<IActionResult> Index()
        //{
        //    return View(await _context.UserViewModel.ToListAsync());
        //}

        //// GET: UserViewModels/Details/5
        //public async Task<IActionResult> Details(string id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var userViewModel = await _context.UserViewModel
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (userViewModel == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(userViewModel);
        //}

        //// GET: UserViewModels/Create
        //public IActionResult Create()
        //{
        //    return View();
        //}

        //// POST: UserViewModels/Create
        //// To protect from overposting attacks, enable the specific properties you want to bind to, for 
        //// more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Id,Username,Email,Role")] UserViewModel userViewModel)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(userViewModel);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(userViewModel);
        //}

        //// GET: UserViewModels/Edit/5
        //public async Task<IActionResult> Edit(string id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var userViewModel = await _context.UserViewModel.FindAsync(id);
        //    if (userViewModel == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(userViewModel);
        //}

        //// POST: UserViewModels/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to, for 
        //// more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(string id, [Bind("Id,Username,Email,Role")] UserViewModel userViewModel)
        //{
        //    if (id != userViewModel.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(userViewModel);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!UserViewModelExists(userViewModel.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(userViewModel);
        //}

        //// GET: UserViewModels/Delete/5
        //public async Task<IActionResult> Delete(string id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var userViewModel = await _context.UserViewModel
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (userViewModel == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(userViewModel);
        //}

        //// POST: UserViewModels/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(string id)
        //{
        //    var userViewModel = await _context.UserViewModel.FindAsync(id);
        //    _context.UserViewModel.Remove(userViewModel);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        //private bool UserViewModelExists(string id)
        //{
        //    return _context.UserViewModel.Any(e => e.Id == id);
        //}
    }
}
