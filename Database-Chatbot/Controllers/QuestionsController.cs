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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Database_Chatbot.Controllers
{
    public class QuestionsController : Controller
    {
        private readonly IDatabaseService _databaseService;
        private readonly IQuestionService _questionService;
        private readonly IGroqService _groqService;
        private readonly IUserService _userService;

        public QuestionsController(IQuestionService questionService, IDatabaseService databaseService, IUserService userService, IGroqService groqService)
        {
            _questionService = questionService;
            _databaseService = databaseService;
            _userService = userService;
            _groqService = groqService;
        }

        // GET: Questions
        public async Task<IActionResult> Index(Guid id, string errorMessage)
        {
            ViewBag.DatabaseId = id;
            ViewBag.ErrorMessage = errorMessage;
            List<Question> questions = _questionService.GetQuestionsForDatabase(id);
            return View(questions);
        }

        // GET: Questions/Create
        public IActionResult Create(Guid id)
        {
            ViewBag.DatabaseId = id;
            return View();
        }

        // POST: Questions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Guid databaseId, [Bind("QuestionText")] Question question)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = _userService.GetUser(userId);

                var apiToken = user.APIToken;
                var database = _databaseService.GetDetailsForDatabase(databaseId);

                string schema = await _databaseService.GetDatabaseSchema(database.Host, database.Database_Name, database.Username, database.Password);

                string query = await _groqService.GenerateQuerry(schema, question.QuestionText, apiToken);

                if (query == "Error")
                {
                    var errorMessage = "Failed to get an answer from LLM";
                    return RedirectToAction(nameof(Index), new { id = databaseId, errorMessage = errorMessage });
                }

                string querryResponse = await _groqService.ExecuteQuery(database.Host, database.Database_Name, database.Username, database.Password, query);
                if (querryResponse == "Error")
                {
                    var errorMessage = "Failed to execute query";
                    return RedirectToAction(nameof(Index), new { id = databaseId, errorMessage = errorMessage });
                }

                string answer = await _groqService.GenerateNaturalLanguageResponse(schema, question.QuestionText, query, querryResponse, apiToken);
                if (answer == "Error")
                {
                    var errorMessage = "Failed to generate natural language response";
                    return RedirectToAction(nameof(Index), new { id = databaseId, errorMessage = errorMessage });
                }

                var questionAnswer = answer;

                _questionService.AskQuestion(question.QuestionText, questionAnswer, databaseId);

                return RedirectToAction(nameof(Index), new { id = databaseId});
            }
            return RedirectToAction(nameof(Index), new { id = databaseId });
        }


        // GET: Questions/Delete/5
        public async Task<IActionResult> Delete(Guid? id, Guid databaseId)
        {
            ViewBag.DatabaseId = databaseId;
            var question = _questionService.GetDetailsForQuestion(id);
            return View(question);
        }

        // POST: Questions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id, Guid databaseId)
        {
            _questionService.DeleteQuestion(databaseId, id);
            return RedirectToAction(nameof(Index), new { id = databaseId});
        }
    }
}
