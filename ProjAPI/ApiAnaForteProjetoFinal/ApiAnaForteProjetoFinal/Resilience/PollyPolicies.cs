using Polly;
using Polly.Extensions.Http;

namespace ApiAnaForteProjetoFinal.Resilience
{
    // Classe utilitaia para centralizar e reutilizar as politicas de resiliencia da Polly
    public class PollyPolicies
    {
        //============================================================================================
        //politica retry
        //em carro de erro http, tenta novamente 3 vezes com um delay
        //============================================================================================
        public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                //define quais os erros HTTP a monitorizar
                .HandleTransientHttpError()
                //tenta 3 vezes antes de desistir
                .WaitAndRetryAsync(3, retryAttempt =>
                    //espera 2 segundos na 1 tentativa, 4s na 2ª e 8s na 3ª
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetry: (outcome, timespan, retryCount, context) =>
                    {
                        //registo no terminal sempre que uma re-tentativa acontece
                        Console.WriteLine($"[Polly Retry] Tentativa {retryCount} falhou. A aguardar {timespan.TotalSeconds}s antes de tentar novamente...");
                    });
        }




        //============================================================================================
        //politica de circuit breaker
        //se ocorrerem 2 falhas consecutivas, o circuito abre por 15 segundos
        //durante esses 15s qualquer new pedido falha imediatamente sem tentar
        //============================================================================================
        public static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                //Se falhar 2 vezes seguidas abre o circuito
                .CircuitBreakerAsync(
                    handledEventsAllowedBeforeBreaking: 2,
                    durationOfBreak: TimeSpan.FromSeconds(15),
                    onBreak: (outcome, timespan) =>
                    {
                        Console.WriteLine($"[Polly Circuit Breaker] CIRCUITO ABERTO! Pedidos bloqueados durante {timespan.TotalSeconds} segundos devido a falhas continuadas.");
                    },
                    onReset: () =>
                    {
                        Console.WriteLine($"[Polly Circuit Breaker] CIRCUITO FECHADO! O serviço externo voltou ao normal.");
                    });
        }
    }
}
