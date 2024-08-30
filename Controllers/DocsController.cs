using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NewTextEditorProject.Data;
using NewTextEditorProject.Models;

namespace NewTextEditorProject.Controllers
{
    public class DocsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DocsController> _logger;

        public DocsController(ApplicationDbContext context, ILogger<DocsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Docs
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = from d in _context.Docs select d;
            applicationDbContext = applicationDbContext.Where(a => a.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier));
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Docs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Docs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Context,UserId")] Doc doc)
        {
            if (ModelState.IsValid)
            {
                _context.Add(doc);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Documento criado: {@Doc}", doc);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        _logger.LogError("Erro no campo {Field}: {ErrorMessage}", state.Key, error.ErrorMessage);
                    }
                }
                return View(doc);
            }

            return View(doc);
        }

        // GET: Docs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doc = await _context.Docs.FindAsync(id);
            if (doc == null)
            {
                return NotFound();
            }

            if (doc.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return NotFound();
            }

            return View(doc);
        }

        // POST: Docs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Context,UserId")] Doc doc)
        {
            if (id != doc.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(doc);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DocExists(doc.Id))
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
            return View(doc);
        }

        // GET: Docs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doc = await _context.Docs
                .Include(d => d.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (doc == null)
            {
                return NotFound();
            }

            if (doc.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return NotFound();
            }

            return View(doc);
        }

        // POST: Docs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var doc = await _context.Docs.FindAsync(id);
            if (doc != null)
            {
                _context.Docs.Remove(doc);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DocExists(int id)
        {
            return _context.Docs.Any(e => e.Id == id);
        }
    }
}
