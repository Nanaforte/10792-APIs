using ApiAnaForteProjetoFinal.Models;

namespace ApiAnaForteProjetoFinal.Cache
{
    public interface IBookCacheService
    {
        //obtem um livro da cache, ou executa a func de procura e guarda na cache se n existir
        Task<Book?> GetOrSetBookAsync(int id, Func<Task<Book?>> getItemCallback);

        //remove um item da cache se o livro dor alterado
        void RemoveBookFromCache(int id);
    }
}
