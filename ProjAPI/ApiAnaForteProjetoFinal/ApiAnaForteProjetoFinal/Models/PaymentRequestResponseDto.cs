namespace ApiDarioProjetoFinal.Models
{
    //dados enviados para processar pagamento
    public class PaymentRequestDto
    {
        public int BookId { get; set; }
        public decimal Valor { get; set; }
        public string MetodoPagamento { get; set; } = "CartaoCredito";
    }

    //resposta devolvida pelo servico externo de pagamentos
    public class PaymentResponseDto
    {
        public string PagamentoId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Mensagem { get; set; } = string.Empty;
    }
}