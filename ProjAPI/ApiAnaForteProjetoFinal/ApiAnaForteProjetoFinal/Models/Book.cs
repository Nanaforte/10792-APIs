namespace ApiAnaForteProjetoFinal.Models
{
    //isto representa o livro guardado no catalogo local da minha API
    public class Book
    {
        public int Id { get; set; } //id unico do livro
        public string? Title { get; set; } = string.Empty; //titulo do livro
        public string? Author { get; set; } = string.Empty; //autor do livro
        public decimal Price { get; set; } //preco do livro
        public string? Sku { get; set; } = string.Empty; //identificador unico do livro para o inventario
    }
}
