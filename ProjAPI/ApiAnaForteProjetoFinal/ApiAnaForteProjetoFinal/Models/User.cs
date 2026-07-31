namespace ApiAnaForteProjetoFinal.Models
{
    //classe que representa a estrutura de um user
    public class User
    {
        //id unico do user
        public int Id { get; set; }

        //nome do user para login
        public string? Username { get; set; } = string.Empty;

        //password do user 
        public string? Password { get; set; } = string.Empty;

        //papel do user no sistema (ex: admin, user)
        public string? Role { get; set; } = string.Empty;
    }
}
