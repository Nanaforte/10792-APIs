namespace ApiAnaForteProjetoFinal.Models
{
    //DTO (Data Transfer Object) para receber as credenciais de login do user
    public class LoginDto
    {
        //nome de user introduzido no form
        public string? Username { get; set; } = string.Empty;

        //password introduzida
        public string? Password { get; set; } = string.Empty;
    }
}
