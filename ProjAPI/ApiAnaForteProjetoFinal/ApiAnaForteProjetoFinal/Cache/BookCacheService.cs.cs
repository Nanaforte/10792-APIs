using ApiAnaForteProjetoFinal.Models;
using Microsoft.Extensions.Caching.Memory;

namespace ApiAnaForteProjetoFinal.Cache
{
    //implementacao da gestao de cache in memory de curta duracao
    public class BookCacheService : IBookCacheService
    {
        private readonly IMemoryCache _memoryCache;
        //time de expiracao da cache 30s
        private static readonly TimeSpan CacheDuration = TimeSpan.FromSeconds(30);

        public BookCacheService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public async Task<Book?> GetOrSetBookAsync(int id, Func<Task<Book?>> getItemCallback)
        {
            string cacheKey = $"book_{id}";

            //1 - tenta obter o livro diretamente da cache local
            if (_memoryCache.TryGetValue(cacheKey, out Book? cachedBook))
            {
                Console.WriteLine($"[Polly Cache] LIVRO ID {id} RETORNADO DA CACHE LOCAL (RÁPIDO)!");
                return cachedBook;
            }

            //2 - se n estiver em cache, executa o metodo para ir buscar à fonte original bd/lista
            Console.WriteLine($"[Polly Cache] LIVRO ID {id} NÃO ENCONTRADO EM CACHE. A consultar a fonte de dados...");
            var book = await getItemCallback();

            //3 - guarda o resultado em cache se o livro for encontrado
            if (book != null)
            {
                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(CacheDuration); //expira exatamente em 30s

                _memoryCache.Set(cacheKey, book, cacheOptions);
            }

            return book;
        }

        public void RemoveBookFromCache(int id)
        {
            string cacheKey = $"book_{id}";
            _memoryCache.Remove(cacheKey);
            Console.WriteLine($"[Polly Cache] LIVRO ID {id} REMOVIDO DA CACHE.");
        }
    }
}
