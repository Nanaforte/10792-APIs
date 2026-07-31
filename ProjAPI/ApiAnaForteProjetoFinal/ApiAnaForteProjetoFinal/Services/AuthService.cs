using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ApiAnaForteProjetoFinal.Models;
using Microsoft.IdentityModel.Tokens;

namespace ApiAnaForteProjetoFinal.Services
{
    //servico responsavel pela validacao e geracao do token JWT
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;

        //injecao de dependencia para aceder as configs do appsettings.json
        public AuthService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        //lista simulada de users para teste em producao seria um bd
        private static readonly List<User> _users = new()
        {
            new User { Id = 1, Username = "admin", Password = "admin123", Role = "Admin" },
            new User { Id = 2, Username = "cliente", Password = "user123", Role = "User" }
        };


        public string Authenticate(LoginDto loginDto)
        {
            //1 - procura o user correspondente na "bd" simulada
            var user = _users.FirstOrDefault(u =>
                u.Username.Equals(loginDto.Username, StringComparison.OrdinalIgnoreCase) &&
                u.Password == loginDto.Password);

            //2 - se o user n existir ou a password estiver errada, devolve null
            if (user == null)
                return null!;

            //3 - le as configs do JWT do appsettings.json
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = Encoding.UTF8.GetBytes(jwtSettings["Secret"]!);

            //4 - define as Claims (afirmacoes/informacoes contidas dentro do Token)
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role) //define as permissoes do user
            };

            //5 - cria a chave de assinatura digital utilizando o algoritmo HMAC-SHA256
            var key = new SymmetricSecurityKey(secretKey);
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            //6 - constrooi o objeto do Token JWT com as suas propriedades
            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["ExpirationInMinutes"]!)),
                signingCredentials: creds
            );

            //7 - escreve e devolve o Token em formato de string codificada
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
