using Microsoft.Extensions.Configuration;
using ModelContextProtocol.Server;
using SqlServerMcp.Models;
using System.ComponentModel;
using System.Text.Json;

namespace SqlServerMcp.Resources
{
    public class SqlResources
    {
        private readonly BusinessRulesConfig _businessRules;

        public SqlResources(IConfiguration configuration)
        {
            _businessRules = configuration.GetSection("BusinessRules").Get<BusinessRulesConfig>() 
                ?? new BusinessRulesConfig();
        }

        [McpServerResource(Name = "db://business-rules")]
        [Description("Business rules and domain logic for interpreting database data")]
        public string GetBusinessRules()
        {
            return JsonSerializer.Serialize(_businessRules, new JsonSerializerOptions 
            { 
                WriteIndented = true 
            });
        }
    }
}

