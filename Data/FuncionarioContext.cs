using Microsoft.EntityFrameworkCore;
using ProjetoFinal_API.Models;
using System.Diagnostics.CodeAnalysis;

namespace ProjetoFinal_API.Data
{
    public class FuncionarioContext: DbContext
    {
        protected readonly IConfiguration Configuration;

        public FuncionarioContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        
        

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(Configuration.GetConnectionString("StringConexaoSQLServer"));
        }
        
        public DbSet<Funcionario>? Funcionario {get; set;}
        public DbSet<User>? Usuario {get; set;}
    }
}