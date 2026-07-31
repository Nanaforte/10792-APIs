using ApiAnaForteProjetoFinal.Cache;
using ApiAnaForteProjetoFinal.Models;
using ApiAnaForteProjetoFinal.Services;
using ApiDarioProjetoFinal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiAnaForteProjetoFinal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly IBookCacheService _cacheService;
        private readonly IExternalService _externalService;

        //bd de dados simulados
        private static readonly List<Book> _booksDB = new()
        {
            new Book { Id = 1, Title = "O Senhor dos Anéis", Author = "J.R.R. Tolkien", Price = 25.00m, Sku = "BOOK-123" },
            new Book { Id = 2, Title = "1984", Author = "George Orwell", Price = 15.50m, Sku = "BOOK-456" }
        };

        public BooksController(IBookCacheService cacheService, IExternalService externalService)
        {
            _cacheService = cacheService;
            _externalService = externalService;
        }


        //lista todos os livros do catalogo
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_booksDB);
        }


        //obtem um livro por id tirando partido do polly cache
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            //usa o Polly para devolver o dado rapidamente se ja existir em memoria
            var book = await _cacheService.GetOrSetBookAsync(id, () =>
            {
                var foundBook = _booksDB.FirstOrDefault(b => b.Id == id);
                return Task.FromResult(foundBook);
            });

            if (book == null)
                return NotFound(new { mensagem = $"Livro com o ID {id} não foi encontrado." });

            return Ok(book);
        }


        //consulta o stock no Mountebank para o sku de um livro especifico
        //protegido por JWT.
        [Authorize]
        [HttpGet("{id}/stock-externo")]
        public async Task<IActionResult> GetExternalStock(int id)
        {
            var book = _booksDB.FirstOrDefault(b => b.Id == id);
            if (book == null)
                return NotFound(new { mensagem = "Livro não encontrado." });

            //executa a chamada http com Polly retry e circuit breaker)
            var inventory = await _externalService.GetInventoryBySkuAsync(book.Sku);

            if (inventory == null)
                return StatusCode(503, new { mensagem = "Serviço externo de inventário indisponível de momento." });

            return Ok(new
            {
                Livro = book.Title,
                Sku = book.Sku,
                Stock = inventory.StockDisponivel,
                Armazem = inventory.LocalizacaoArmazem
            });
        }


        //realiza a compra de um livro enviando o pagamento para o mountebank
        //protegido por JWT.
        [Authorize]
        [HttpPost("comprar")]
        public async Task<IActionResult> BuyBook([FromBody] PaymentRequestDto paymentRequest)
        {
            var book = _booksDB.FirstOrDefault(b => b.Id == paymentRequest.BookId);
            if (book == null)
                return NotFound(new { mensagem = "Livro não encontrado para compra." });

            paymentRequest.Valor = book.Price;

            //envia o pedido ao mountebank
            var result = await _externalService.ProcessPaymentAsync(paymentRequest);

            if (result == null)
                return StatusCode(503, new { mensagem = "Não foi possível processar o pagamento com o serviço externo." });

            return Ok(new
            {
                mensagem = "Compra efetuada com sucesso!",
                detalhes = result
            });
        }
    }
}
