namespace ApiAnaForteProjetoFinal.Models
{
    //DTO (Data Transfer Object) para mapear a resposta de consulta ao moutebank
    public class InventoryResponseDto
    {
        public string? Sku { get; set; } = string.Empty; //identificador unico do livro para o inventario
        public int StockDisponivel { get; set; } //quantidade de stock disponivel do livro
        public string? LocalizacaoArmazem { get; set; } = string.Empty; //localizacao do armazem onde o livro se encontra
    }
}
