using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using project.Data;
using project.Models;

namespace project.Controllers
{
    public class namesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public namesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: names
        public async Task<IActionResult> Index()
        {
            return _context.names != null ?
                      View(await _context.names.ToListAsync()) :
                      Problem("Entity set 'ApplicationDbContext.Registercs'  is null.");
        }
        public async Task<IActionResult> ShowSearchForm()
        {
            return _context.names != null ?
                        View("ShowSearchForm") :
                        Problem("Entity set 'ApplicationDbContext.names'  is null.");
        }
        public async Task<IActionResult> ShowSearchResults(string SearchPhrase,string fieldsearch)
        {if (fieldsearch == null)
            {
                return _context.names != null ?
                            View("index", await _context.names.Where(j => j.namedoc.Contains(SearchPhrase)).ToListAsync()) :
                            Problem("Entity set 'ApplicationDbContext.names'  is null.");
            }
        if(SearchPhrase == null)
            {
                return _context.names != null ?
                          View("index", await _context.names.Where(j => j.field.Contains(fieldsearch)).ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.names'  is null.");
            }
            return RedirectToAction(nameof(ShowSearchForm));
        }
        // GET: names/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.names == null)
            {
                return NotFound();
            }

            var names = await _context.names
                .FirstOrDefaultAsync(m => m.id == id);
            if (names == null)
            {
                return NotFound();
            }

            return View(names);
        }

        // GET: names/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: names/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,namedoc")] names names)
        {
            if (ModelState.IsValid)
            {
                _context.Add(names);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(names);
        }

        // GET: names/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.names == null)
            {
                return NotFound();
            }

            var names = await _context.names.FindAsync(id);
            if (names == null)
            {
                return NotFound();
            }
            return View(names);
        }

        // POST: names/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,namedoc")] names names)
        {
            if (id != names.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(names);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!namesExists(names.id))
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
            return View(names);
        }

        // GET: names/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.names == null)
            {
                return NotFound();
            }

            var names = await _context.names
                .FirstOrDefaultAsync(m => m.id == id);
            if (names == null)
            {
                return NotFound();
            }

            return View(names);
        }

        // POST: names/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.names == null)
            {
                return Problem("Entity set 'ApplicationDbContext.names'  is null.");
            }
            var names = await _context.names.FindAsync(id);
            if (names != null)
            {
                _context.names.Remove(names);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool namesExists(int id)
        {
          return (_context.names?.Any(e => e.id == id)).GetValueOrDefault();
        }
    }
}
