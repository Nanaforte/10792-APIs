using ApiAnaForteProjetoFinal.Models;

namespace ApiAnaForteProjetoFinal.Services
{
    //interface que contrata os metodos de autenticacao da API
    public interface IAuthService
    {
        //metodo que recebe as credenciais e devolve o token JWT se forem validas
        string Authenticate(LoginDto loginDto);
    }
}
