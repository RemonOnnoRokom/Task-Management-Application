using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleTaskManagementWebApplication.Data;
using SimpleTaskManagementWebApplication.Models;
using SimpleTaskManagementWebApplication.ViewModels;

namespace SimpleTaskManagementWebApplication.Controllers
{
    [Authorize]
    public class TaskItemController(ApplicationDbContext context, UserManager<AppUser> userManager) : Controller
    {
        private readonly ApplicationDbContext _context = context;
        private readonly UserManager<AppUser> _userManager = userManager;

        public IActionResult Index()
        {
            try
            {
                int userId = Convert.ToInt32(_userManager.GetUserId(User));
                var tasks = _context.TaskItems.Where(t => t.AppUserId == userId).ToList();
                return View(tasks);
            }
            catch (Exception)
            {
                TempData["message"] = "something went wrong when accessing the tasks";
                return View();
            }
            
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind("Title, Description, DueDate")] TaskItem task)
        {
            try
            {
                int userId = Convert.ToInt32(_userManager.GetUserId(User));
                task.AppUserId = userId;
                if (ModelState.IsValid)
                {
                    _context.TaskItems.Add(task);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index");
                }

                return View(task);
            }
            catch(Exception)
            {
                ViewData["message"] = "Something went wrong";
                return View(task);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                int userId = Convert.ToInt32(_userManager.GetUserId(User));
                var task = await _context.TaskItems.FirstOrDefaultAsync(t => t.Id == id && t.AppUserId == userId);
                if (task == null)
                {
                    return RedirectToAction("Index");
                }
                return View(task);
            }
            catch(Exception)
            {
                TempData["message"] = "Something went wrong";
                return RedirectToAction(nameof(Index));
            }
            
        }

        [HttpPost]
        public  async Task<IActionResult> Edit([Bind("Id, Title, Description, DueDate, IsCompleted")] TaskItem task)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    int userId = Convert.ToInt32((_userManager.GetUserId(User)));
                    var taskToUpdate = await _context.TaskItems.FirstOrDefaultAsync(t => t.Id == task.Id && t.AppUserId == userId);

                    if (taskToUpdate is null)
                    {
                        return RedirectToAction(nameof(Index));
                    }

                    taskToUpdate.Title = task.Title;
                    taskToUpdate.Description = task.Description;
                    taskToUpdate.DueDate = task.DueDate;
                    taskToUpdate.IsCompleted = task.IsCompleted;

                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }

                return View(task);
            }
            catch (Exception)
            {
                ViewData["message"] = "Something went wrong";
                return View(task);
            }
            
        }

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                int userId = Convert.ToInt32((_userManager.GetUserId(User)));
                var task = await _context.TaskItems.FirstOrDefaultAsync(t => t.Id == id && t.AppUserId == userId);           
                if(task is null)
                {
                    return RedirectToAction(nameof(Index));
                }

                _context.TaskItems.Remove(task);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch(Exception)
            {
                TempData["message"] = "Something went wrong";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public IActionResult FilterAndSort(SearchHelperViewModel search)
        {
            try
            {
                int userId = Convert.ToInt32(_userManager.GetUserId(User));
                
                if (search.SortValue == null && search.FilterValue == null)
                {
                    return RedirectToAction(nameof(Index));
                }

                IQueryable<TaskItem> query = _context.TaskItems.Where(t => t.AppUserId == userId);

                if (search.SortValue != null && search.FilterValue == null)
                {
                    if (search.SortValue == "ASC")
                    {
                        query = query.OrderBy(u => u.DueDate);
                    }
                    else
                    {
                        query = query.OrderByDescending(u => u.DueDate);
                    }
                }
                else if (search.SortValue == null && search.FilterValue != null)
                {
                    query = query.Where(t => t.IsCompleted == search.FilterValue);
                }
                else if (search.SortValue != null && search.FilterValue != null)
                {
                    if (search.SortValue == "ASC")
                    {
                        query = query.Where(t => t.IsCompleted == search.FilterValue).OrderBy(u => u.DueDate);
                    }
                    else
                    {
                        query = query.Where(t => t.IsCompleted == search.FilterValue).OrderByDescending(u => u.DueDate);
                    }
                }

                var tasks = query.ToList();
                TempData["SortValue"] = search.SortValue;
                TempData["FilterValue"] = search.FilterValue;

                return View(nameof(Index), query.ToList());
            }
            catch (Exception)
            {
                TempData["message"] = "Something went wrong while processing";

                return RedirectToAction(nameof(Index));
            }
            
        }
    }
}
