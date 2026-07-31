using ApiAnaForteProjetoFinal.Models;
using ApiDarioProjetoFinal.Models;

namespace ApiAnaForteProjetoFinal.Services
{
    public interface IExternalService
    {
        //consulta o stock no mountebank GET /inventory/:sku
        Task<InventoryResponseDto?> GetInventoryBySkuAsync(string sku);

        //envia o pedido de pagamento ao mountebank POST /payment
        Task<PaymentResponseDto?> ProcessPaymentAsync(PaymentRequestDto payment);
    }
}
