using Microsoft.AspNetCore.Mvc;
using ProjetoFinal_API.Data;
using ProjetoFinal_API.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ProjetoFinal_API.Controllers
{
    [Route("api/[controller]/")]
    [ApiController]

    public class FuncionarioController : ControllerBase
    {
        private FuncionarioContext _context;
        public FuncionarioController(FuncionarioContext context)
        {
            _context = context;
        }
        
        [HttpGet]
        public ActionResult<List<Funcionario>> GetAll(){
            if(_context.Funcionario is not null){
                return _context.Funcionario.ToList();
            }
            else{
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Falha no acesso ao banco de dados.");
            }
        }

        
        [HttpGet("{FuncionarioId}")]
        public ActionResult<List<Funcionario>> Get(int FuncionarioId){
            try{
                var result = _context.Funcionario.Find(FuncionarioId);
                if (result == null){
                    return NotFound();
                }
                return Ok(result);
            }
            catch{
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Falha no acesso ao banco de dados.");
            }
        }

        
        [HttpGet("FuncionarioNome")]
        public ActionResult<List<Funcionario>> GetFuncionarioNome(string FuncionarioNome){
            if (_context.Funcionario is not null){
                var result = _context.Funcionario.Where(a => a.nome == FuncionarioNome);
                if (result == null){
                    return NotFound();
                }
                return Ok(result);
            }
            else{
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Falha no acesso ao banco de dados.");
            }
        }

        [HttpPost]
        public async Task<ActionResult> Post(Funcionario model){
            try{
                _context.Funcionario.Add(model);
                if(await _context.SaveChangesAsync() == 1){
                    return Created($"/api/funcionario/{model.id}", model);
                }
            }
            catch{
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Falha no acesso ao banco de dados.");
            }

            return BadRequest();
        }

        [HttpPut("{FuncionarioId}")]
        public async Task<IActionResult> Put(int FuncionarioId, Funcionario dadosFuncAlt){
            try{
                var result = await _context.Funcionario.FindAsync(FuncionarioId);
                if(FuncionarioId != result.id){
                    return BadRequest();
                }

                result.nome = dadosFuncAlt.nome;
                result.setor = dadosFuncAlt.setor;
                result.metas = dadosFuncAlt.metas;
                result.funcmes = dadosFuncAlt.funcmes;

                await _context.SaveChangesAsync();
                return Created($"/api/funcionario/{dadosFuncAlt.id}", dadosFuncAlt);
            }
            catch{
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Falha no acesso ao banco de dados.");
            }
        }

        [HttpDelete("{FuncionarioId}")]
        public async Task<ActionResult> Delete(int FuncionarioId){
            try{
                var funcionario = await _context.Funcionario.FindAsync(FuncionarioId);
                if (funcionario == null){
                    return NotFound();
                }
                _context.Remove(funcionario);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch{
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Falha no acesso ao banco de dados.");
            }
        }
    }
}