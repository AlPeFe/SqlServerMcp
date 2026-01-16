<div align="center">

# ??? SqlServerMcp

### **Model Context Protocol Server for SQL Server**

[![.NET 10](https://img.shields.io/badge/.NET-10.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![MCP](https://img.shields.io/badge/MCP-Protocol-00ADD8?style=for-the-badge&logo=data:image/svg+xml;base64,PHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHdpZHRoPSIyNCIgaGVpZ2h0PSIyNCIgdmlld0JveD0iMCAwIDI0IDI0IiBmaWxsPSJub25lIiBzdHJva2U9IndoaXRlIiBzdHJva2Utd2lkdGg9IjIiIHN0cm9rZS1saW5lY2FwPSJyb3VuZCIgc3Ryb2tlLWxpbmVqb2luPSJyb3VuZCI+PHBhdGggZD0iTTEyIDJhMTAgMTAgMCAxIDAgMTAgMTBIMTJWMnoiLz48L3N2Zz4=&logoColor=white)](https://modelcontextprotocol.io/)
[![SQL Server](https://img.shields.io/badge/SQL%20Server-CC2927?style=for-the-badge&logo=microsoft-sql-server&logoColor=white)](https://www.microsoft.com/sql-server)
[![License](https://img.shields.io/badge/License-MIT-green?style=for-the-badge)](LICENSE)

*Connect your SQL Server database with LLMs through the Model Context Protocol*

[Installation](#-installation) �
[Configuration](#-configuration) �
[Tools](#-available-tools) �
[Usage](#-usage)

---

</div>

## ?? Description

**SqlServerMcp** is an MCP (Model Context Protocol) server that enables language models (LLMs) to interact securely with SQL Server databases. It exposes tools for exploring schemas, executing SELECT queries, and managing business rules.

## ? Features

- ?? **Schema Exploration** - Get the complete structure of your database
- ?? **Safe Queries** - Only allows SELECT operations (read-only)
- ?? **Business Rules** - Define and expose domain rules for better context
- ?? **High Performance** - Published as a single, self-contained executable
- ?? **STDIO Transport** - Standard communication with MCP clients

## ??? Technologies Used

| Technology | Version | Description |
|------------|---------|-------------|
| **.NET** | 10.0 | Main framework |
| **ModelContextProtocol** | 0.5.0-preview.1 | Official MCP SDK for C# |
| **Microsoft.Data.SqlClient** | 7.0.0-preview3 | Modern SQL Server client |
| **Microsoft.Extensions.Hosting** | 8.0.1 | .NET generic hosting |

## ?? Installation

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- SQL Server (local or remote)

### Clone and Build

```bash
git clone https://github.com/AlPeFe/SqlServerMcp.git
cd SqlServerMcp
dotnet build
```

### Publish as Single Executable

```bash
# Windows
dotnet publish -c Release -r win-x64

# macOS (Apple Silicon)
dotnet publish -c Release -r osx-arm64

# Linux
dotnet publish -c Release -r linux-x64
```

## ?? Configuration

1. Copy `appsettings.example.json` to `appsettings.json`:

```bash
cp appsettings.example.json appsettings.json
```

2. Configure your connection string and business rules:

```json
{
  "ConnectionStrings": {
    "SqlServer": "Server=YOUR_SERVER;Initial Catalog=YOUR_DATABASE;User ID=YOUR_USER;Password=YOUR_PASSWORD;TrustServerCertificate=True"
  },
  "BusinessRules": {
    "Description": "Business Rules - Your Database",
    "Rules": [
      {
        "Name": "RuleName",
        "Description": "Description of when this rule applies",
        "Conditions": [
          "FIELD1 = 'VALUE'",
          "FIELD2 is not NULL",
          "FIELD3 > FIELD4"
        ],
        "ExampleSql": "SELECT * FROM TABLE WHERE FIELD1 = 'VALUE' AND FIELD2 IS NOT NULL"
      }
    ]
  }
}
```

## ?? Available Tools

| Tool | Description |
|------|-------------|
| `get_database_schema` | Gets the complete schema (tables, columns, PKs, constraints) |
| `get_business_rules` | Returns the configured business rules |
| `execute_read_query` | Executes SELECT queries safely |

## ?? MCP Resources

| Resource | URI | Description |
|----------|-----|-------------|
| Business Rules | `db://business-rules` | Domain rules in JSON format |

## ?? Usage

### Integration with MCP Clients

Configure your MCP client to use this server:

```json
{
  "mcpServers": {
    "sqlserver": {
      "command": "path/to/SqlServerMcp.exe",
      "args": []
    }
  }
}
```

### Example Workflow

1. The LLM calls `get_business_rules` to understand the domain
2. Then uses `get_database_schema` to explore the structure
3. Finally executes queries with `execute_read_query`

## ?? Project Structure

```
SqlServerMcp/
??? ?? Program.cs              # Entry point
??? ?? Tools/
?   ??? SqlTools.cs            # MCP tools
??? ?? Resources/
?   ??? SqlResources.cs        # MCP resources
??? ?? Models/
?   ??? BusinessRulesConfig.cs # Configuration models
??? ?? appsettings.json         # Configuration
??? ?? SqlServerMcp.csproj      # Project file
```

## ?? Security

- **Read-only access**: Only SELECT queries are allowed
- **No DDL/DML**: INSERT, UPDATE, DELETE, DROP operations are blocked
- **Business rules isolation**: Rules are defined in configuration, not exposed to modification

## ?? Contributing

Contributions are welcome! Please open an issue or pull request.

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## ?? License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

---

<div align="center">

**Made with ?? using .NET and MCP**

[? Back to top](#?-sqlservermcp)

</div>
