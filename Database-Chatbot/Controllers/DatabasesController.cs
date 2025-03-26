using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Domain.Domain;
using Repository;
using Service.Interface;
using System.Security.Claims;
using System.Reflection.Metadata.Ecma335;

namespace Database_Chatbot.Controllers
{
    public class DatabasesController : Controller
    {
        private readonly IDatabaseService _databaseService;

        public DatabasesController(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        // GET: Databases
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            List<Database> databases = _databaseService.GetAllDatabasesForUser(userId);

            return View(databases);
        }


        // GET: Databases/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Databases/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Host,Username,Password,Database_Name,Id")] Database database)
        {
            if (ModelState.IsValid)
            {
                var databaseConnection = await _databaseService.CheckDatabaseConnection(database.Host, database.Database_Name, database.Username, database.Password);
                if (databaseConnection.Equals(true))
                {
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    database.Id = Guid.NewGuid();
                    _databaseService.CreateNewDatabase(userId, database);
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Failed to connect to the database. Please check and re-enter your credentials");
                }
            }
            return View(database);
        }

        // GET: Databases/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            var database = _databaseService.GetDetailsForDatabase(id);
            return View(database);
        }

        // POST: Databases/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Name,Host,Username,Password,Database_Name,Id")] Database database)
        {
            if (id != database.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var databaseConnection = await _databaseService.CheckDatabaseConnection(database.Host, database.Database_Name, database.Username, database.Password);
                if (databaseConnection.Equals(true))
                {
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    _databaseService.UpdateDatabase(userId, database);
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Failed to connect to the database. Please check and re-enter your credentials");
                }

            }
            return View(database);
        }

        // GET: Databases/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {

            var database = _databaseService.GetDetailsForDatabase(id);
            return View(database);
        }

        // POST: Databases/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var database = _databaseService.GetDetailsForDatabase(id);
            _databaseService.DeleteDatabase(userId, id);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> SeeQuestions(Guid id)
        {
            return RedirectToAction("Index", "Questions", new { id = id });
        }
    }
}
