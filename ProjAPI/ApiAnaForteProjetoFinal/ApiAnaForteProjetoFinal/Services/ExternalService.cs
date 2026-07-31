using ApiAnaForteProjetoFinal.Models;
using ApiDarioProjetoFinal.Models;
using System.Net.Http.Json;

namespace ApiAnaForteProjetoFinal.Services
{
    public class ExternalService : IExternalService
    {
        private readonly HttpClient _httpClient;

        //o HttpClient injetado ja vem automaticamente protegido com as politicas de retry e circuit breaker da polly
        public ExternalService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<InventoryResponseDto?> GetInventoryBySkuAsync(string sku)
        {
            try
            {
                //faz a chamada http get /inventory/{sku} ao mountebank
                var response = await _httpClient.GetAsync($"inventory/{sku}");

                if(response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<InventoryResponseDto>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Serviço Externo] Erro ao consultar inventario: {ex.Message}");
            }
            return null;
        }

        public async Task<PaymentResponseDto?> ProcessPaymentAsync(PaymentRequestDto payment)
        {
            try
            {
                //faz a chamada http post /payment ao mountebank
                var response = await _httpClient.PostAsJsonAsync("/payments", payment);

                if(response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<PaymentResponseDto>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Serviço Externo] Erro ao processar pagamento: {ex.Message}");
            }
            return null;
        }
    }
}
