using ApiAnaForteProjetoFinal.Models;
using ApiAnaForteProjetoFinal.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApiAnaForteProjetoFinal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        //injecao do service de autenticacao
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }


        //endpoint para autenticar users e gerar token JWT
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto loginDto)
        {
            //executa a validacao das credenciais
            var token = _authService.Authenticate(loginDto);

            //se o token for nulo as credenciais estao erradas
            if(token == null)
            {
                return Unauthorized(new {mensagem = "Utilizador ou palavra-passe inválidos." });
            }

            //devolve o token assinado com sucesso
            return Ok(new {mensagem= "Autenticação bem-sucedida.", token=token });
        }
    }
}
