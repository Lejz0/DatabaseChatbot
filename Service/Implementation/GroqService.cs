using Domain.Domain;
using Npgsql;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Service.Implementation
{
    public class GroqService : IGroqService
    {
        public async Task<string> GenerateQuerry(string schema, string question, string apiToken)
        {
            var client = new HttpClient();

            var requestUrl = "https://api.groq.com/openai/v1/chat/completions";
            var requestData = new
            {
                model = "llama-3.3-70b-versatile",
                messages = new[]
                {
                    new { role = "user", content = "Based on the table schema below, write a SQL query that would answer the user's question and always only return the query and no aditional context:\r\n "+ schema +"\r\n\r\n    Question: "+ question +"\r\n    SQL Query:" }
                },
                temperature = 0.5,
                stream = false
            };

            var jsonPayload = JsonSerializer.Serialize(requestData);

            var requestContent = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + apiToken);

            var response = await client.PostAsync(requestUrl, requestContent);

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();

                var responseObject = JsonSerializer.Deserialize<JsonElement>(responseBody);

                var content = responseObject
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString();

                return content;
            }
            else
            {
                Console.WriteLine($"Error: {response.StatusCode}");
                return "Error";
            }
        }

        public async Task<string> ExecuteQuery(string host, string databaseName, string username, string password, string query)
        {
            var results = new List<string>();

            try
            {
                using (var connection = new NpgsqlConnection($"Host={host};Database={databaseName};Username={username};Password={password}"))
                {
                    await connection.OpenAsync();

                    using (var cmd = new NpgsqlCommand(query, connection))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var rowValues = new List<string>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                var value = reader[i] is string ? $"'{reader[i]}'" : reader[i].ToString();
                                rowValues.Add(value);
                            }
                            results.Add($"({string.Join(", ", rowValues)}");
                        }
                    }
                }

                return $"[{string.Join(", ", results)}]";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return "Error";
            }


        }

        public async Task<string> GenerateNaturalLanguageResponse(string schema, string question, string query, string sqlResponse, string apiToken)
        {
            var client = new HttpClient();

            var requestUrl = "https://api.groq.com/openai/v1/chat/completions";
            var requestData = new
            {
                model = "llama-3.3-70b-versatile",
                messages = new[]
                {
                    new { role = "user", content = "Based on the table schema below, question, sql query, and sql response, write a natural language response:\r\n "+ schema +"\r\n\r\n    Question: "+ question +"\r\n    SQL Query:"+ query +"\r\n    SQL Response:"+ sqlResponse }
                },
                temperature = 0.5,
                stream = false
            };

            var jsonPayload = JsonSerializer.Serialize(requestData);

            var requestContent = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + apiToken);

            var response = await client.PostAsync(requestUrl, requestContent);

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();

                var responseObject = JsonSerializer.Deserialize<JsonElement>(responseBody);

                var content = responseObject
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString();

                return content;
            }
            else
            {
                Console.WriteLine($"Error: {response.StatusCode}");
                return "Error";
            }
        }

    }
}
