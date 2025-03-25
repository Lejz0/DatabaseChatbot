using Domain.Domain;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Npgsql;
using Repository.Implementation;
using Repository.Interface;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Service.Implementation
{
    public class DatabaseService : IDatabaseService
    {
        private readonly IRepository<Database> _databaseRepository;
        private readonly IUserRepository _userRepository;

        public DatabaseService(IRepository<Database> databaseRepository, IUserRepository userRepository)
        {
            _userRepository = userRepository;
            _databaseRepository = databaseRepository;
        }

        public void CreateNewDatabase(string userId, Database database)
        {
            var user = _userRepository.Get(userId);

            if (user.Databases == null)
            {
                user.Databases = new List<Database>();
            }
            user.Databases.Add(database);

            _databaseRepository.Insert(database);
            _userRepository.Update(user);
        }

        public void DeleteDatabase(string userid, Guid id)
        {
            var user = _userRepository.Get(userid);
            var database = _databaseRepository.Get(id);

            user.Databases.Remove(database);
            _userRepository.Update(user);
            _databaseRepository.Delete(database);
        }

        public List<Database> GetAllDatabasesForUser(string userId)
        {
            var user = _userRepository.Get(userId);
            var userDatabases = user.Databases;

            if (userDatabases == null)
            {
                userDatabases = new List<Database>();
            }
            return userDatabases.ToList();

        }

        public Database GetDetailsForDatabase(Guid? id)
        {
            return _databaseRepository.Get(id);
        }

        public void UpdateDatabase(string userId, Database database)
        {
            _databaseRepository.Update(database);

            var user = _userRepository.Get(userId);
            var db = _databaseRepository.Get(database.Id);

            user.Databases.Remove(db);
            user.Databases.Add(db);

            _userRepository.Update(user);
        }

        public async Task<bool> CheckDatabaseConnection(string host, string databaseName, string username, string password)
        {
            try
            {
                using (var connection = new NpgsqlConnection($"Host={host};Database={databaseName};Username={username};Password={password}"))
                {
                    await connection.OpenAsync();
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }


        public async Task<string> GetDatabaseSchema(string host, string databaseName, string username, string password)
        {
            var schemaBuilder = new StringBuilder();

            using (var connection = new NpgsqlConnection($"Host={host};Database={databaseName};Username={username};Password={password}"))
            {
                connection.Open();

                string schemaQuery = @"
                    SELECT table_name
                    FROM infromation_schema.tables
                    WHERE table_scheme = 'public'";

                using (var cmd = new NpgsqlCommand(schemaQuery, connection))
                using (var reader = cmd.ExecuteReader())
                {
                    var tables = new List<string>();
                    while (reader.Read())
                    {
                        tables.Add(reader.GetString(0));
                    }

                    foreach (var table in tables)
                    {
                        schemaBuilder.Append(table);
                    }
                }
            }
            return schemaBuilder.ToString();
        }
    }
}
