using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using ModelContextProtocol.Server;
using SqlServerMcp.Models;
using System.ComponentModel;
using System.Text;
using System.Text.Json;

namespace SqlServerMcp.Tools
{
    public class SqlServerTools
    {
        private readonly string _connectionString;
        private readonly BusinessRulesConfig _businessRules;

        public SqlServerTools(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("SqlServer")
                ?? throw new InvalidOperationException("Connection string 'SqlServer' not found in configuration.");
            
            _businessRules = configuration.GetSection("BusinessRules").Get<BusinessRulesConfig>() 
                ?? new BusinessRulesConfig();
        }

        [McpServerTool(Name = "get_database_schema")]
        [Description("Gets the complete database schema with tables, columns, relationships and constraints")]
        public async Task<string> GetDatabaseSchema()
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var schemaQuery = @"
            SELECT 
                t.TABLE_SCHEMA,
                t.TABLE_NAME,
                c.COLUMN_NAME,
                c.DATA_TYPE,
                c.CHARACTER_MAXIMUM_LENGTH,
                c.IS_NULLABLE,
                CASE WHEN pk.COLUMN_NAME IS NOT NULL THEN 'PK' ELSE '' END as KEY_TYPE
            FROM INFORMATION_SCHEMA.TABLES t
            INNER JOIN INFORMATION_SCHEMA.COLUMNS c 
                ON t.TABLE_NAME = c.TABLE_NAME
            LEFT JOIN (
                SELECT ku.TABLE_NAME, ku.COLUMN_NAME
                FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc
                INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE ku
                    ON tc.CONSTRAINT_NAME = ku.CONSTRAINT_NAME
                WHERE tc.CONSTRAINT_TYPE = 'PRIMARY KEY'
            ) pk ON c.TABLE_NAME = pk.TABLE_NAME AND c.COLUMN_NAME = pk.COLUMN_NAME
            ORDER BY t.TABLE_NAME, c.ORDINAL_POSITION";

            var schema = new StringBuilder();
            using var command = new SqlCommand(schemaQuery, connection);
            using var reader = await command.ExecuteReaderAsync();

            string currentTable = "";
            while (await reader.ReadAsync())
            {
                var tableName = reader.GetString(1);
                if (tableName != currentTable)
                {
                    schema.AppendLine($"\nTABLE: {tableName}");
                    currentTable = tableName;
                }

                var columnName = reader.GetString(2);
                var dataType = reader.GetString(3);
                var nullable = reader.GetString(5);
                var keyType = reader.GetString(6);

                schema.AppendLine($"  - {columnName} ({dataType}) {keyType} {(nullable == "YES" ? "NULL" : "NOT NULL")}");
            }

            return schema.ToString();
        }


        [McpServerTool(Name = "get_business_rules")]
        [Description("Gets the business rules and domain logic for interpreting database data. Call this FIRST before writing queries to understand the data semantics.")]
        public string GetBusinessRules()
        {
            return JsonSerializer.Serialize(_businessRules, new JsonSerializerOptions 
            { 
                WriteIndented = true 
            });
        }

        [McpServerTool(Name ="execute_read_query")]
        [Description("Executes a SELECT query on the database. Only SELECT statements are allowed.")]
        public async Task<string> ExecuteQuery(
        [Description("SQL SELECT query to execute")] string query)
        {
            if (!query.TrimStart().StartsWith("SELECT", StringComparison.OrdinalIgnoreCase))
            {
                return "Error: Only SELECT queries are allowed";
            }

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand(query, connection);
            command.CommandTimeout = 30;

            using var reader = await command.ExecuteReaderAsync();

            // Formatear resultados como JSON o tabla
            var results = new List<Dictionary<string, object>>();
            while (await reader.ReadAsync())
            {
                var row = new Dictionary<string, object>();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    row[reader.GetName(i)] = reader.GetValue(i);
                }
                results.Add(row);
            }

            return JsonSerializer.Serialize(results, new JsonSerializerOptions { WriteIndented = true });
        }

    }
}
