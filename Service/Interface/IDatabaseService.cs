using Domain.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface IDatabaseService
    {
        List<Database> GetAllDatabasesForUser(string userId);
        Database GetDetailsForDatabase(Guid? id);
        void CreateNewDatabase(string userId, Database database);

        void UpdateDatabase(string userId, Database database);
        void DeleteDatabase(string userid, Guid id);

        Task<bool> CheckDatabaseConnection(string host, string databaseName, string username, string password);

        Task<string> GetDatabaseSchema(string host, string databaseName, string username, string password);
    }
}
