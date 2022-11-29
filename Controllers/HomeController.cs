using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using ProjetoFinal_API.Data;
using ProjetoFinal_API.Models;
using System.Security.Claims;


namespace ProjetoFinal_API.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly FuncionarioContext? _context;

        public HomeController(
            IConfiguration configuration,
            FuncionarioContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        public ActionResult<dynamic> Login([FromBody] User usuario)
        {
            var user = _context.Usuario.Where(u => u.username == usuario.username && u.senha == usuario.senha).FirstOrDefault();
            if (user == null){
                return Unauthorized("Usuário ou senha inválidos");
            }
            var authClaims = new List<Claim>{
                new Claim(ClaimTypes.Name, user.username),
                new Claim(ClaimTypes.Role, user.role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),

            };

            var token = GetToken(authClaims);
            user.senha = "";

            return Ok(new{
                token = new JwtSecurityTokenHandler().WriteToken(token),
                user = user
            });

        }

        [HttpGet]
        [Route("anonymous")]
        [AllowAnonymous]
        public string Anonymous() => "Anônimo";

        [HttpGet]
        [Route("authenticated")]
        [Authorize]
        public string Authenticated() => String.Format("Autenticado - {0}", User.Identity.Name);

        [HttpGet]
        [Route("funcionario")]
        [Authorize(Roles = "funcionario,patrao")]
        public string Funcionario() => "Funcionario";

        [HttpGet]
        [Route("patrao")]
        [Authorize(Roles = "patrao")]
        public string Patrao() => "Patrao";

        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                expires: DateTime.Now.AddHours(3),
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return token;
        }
    }
}