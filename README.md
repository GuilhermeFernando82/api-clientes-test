🚀 Como rodar a API (.NET)
📌 Requisitos

Antes de começar, instale:

🟦 .NET SDK (ex.: 6 ou 8)

🐘 PostgreSQL instalado e rodando

📦 Ferramenta de migração (opcional se usar migrations automáticas)

🛠️ Configuração do Banco de Dados

Crie um banco no PostgreSQL (exemplo: meu_banco).

Configure a ConnectionString no arquivo:

📍 appsettings.json

{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=meu_banco;Username=postgres;Password=SUA_SENHA"
  }
}


⚠️ Nunca envie senhas reais para o GitHub.
Use variáveis de ambiente em produção.

🗂️ Criar as Tabelas

Se a API usa Entity Framework com migrations, execute:

dotnet ef database update


Se ainda não existem migrations, crie primeiro:

dotnet ef migrations add InitialCreate
dotnet ef database update

▶️ Compilar e Rodar a API

Dentro da pasta do projeto, execute:

dotnet build
dotnet run


A API deve iniciar e exibir algo assim:

Now listening on: https://localhost:7181/swagger/index.html

🔎 Testar via navegador ou Postman

Acesse:

https://localhost:5001/swagger