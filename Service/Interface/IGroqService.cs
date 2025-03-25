using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface IGroqService
    {
        Task<string> GenerateQuerry(string schema, string question, string apiToken);
        Task<string> ExecuteQuery(string host, string databaseName, string username, string password, string query);

        Task<string> GenerateNaturalLanguageResponse(string schema, string question, string query, string sqlResponse, string apiToken);
    }
}
