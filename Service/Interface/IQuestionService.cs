using Domain.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface IQuestionService
    {
        List<Question> GetQuestionsForDatabase(Guid databaseId);
        Question GetDetailsForQuestion(Guid? id);
        void AskQuestion(string questionText, string questionAnswer, Guid databaseId);
        void DeleteQuestion(Guid databaseId, Guid id);
    }
}
