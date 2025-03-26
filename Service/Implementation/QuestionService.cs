using Domain.Domain;
using Repository.Interface;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Implementation
{
    public class QuestionService : IQuestionService
    {
        private readonly IRepository<Question> _questionRepository;
        private readonly IRepository<Database> _databaseRepository;

        public QuestionService(IRepository<Question> questionRepository, IRepository<Database> databaseRepository)
        {
            _questionRepository = questionRepository;
            _databaseRepository = databaseRepository;
        }

        public void AskQuestion(string questionText, string questionAnswer, Guid databaseId)
        {
            var questionToAsk = new Question
            {
                Id = Guid.NewGuid(),
                QuestionText = questionText,
                QuestionAnswer = questionAnswer,
            };

            _questionRepository.Insert(questionToAsk);

            var database = _databaseRepository.Get(databaseId);
            var databaseQuestions = database?.Questions;

            if (databaseQuestions == null)
            {
                databaseQuestions = new List<Question>();
            }

            databaseQuestions.Add(questionToAsk);
            _databaseRepository.Update(database);
        }

        public void DeleteQuestion(Guid databaseId, Guid id)
        {
            var database = _databaseRepository.Get(databaseId);
            var question = _questionRepository.Get(id);

            _questionRepository.Delete(question);
            database.Questions?.Remove(question);
            _databaseRepository.Update(database);
            
        }

        public Question GetDetailsForQuestion(Guid? id)
        {
            return _questionRepository.Get(id);
        }

        public List<Question> GetQuestionsForDatabase(Guid databaseId)
        {
            var database = _databaseRepository.Get(databaseId);
            var databaseQuestions = database?.Questions;

            if (databaseQuestions == null)
            {
                databaseQuestions = new List<Question>();
            }

            return databaseQuestions.ToList();
        }
    }
}
