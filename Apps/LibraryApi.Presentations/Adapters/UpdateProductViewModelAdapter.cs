using LibraryApi.Domains.Adapters;
using LibraryApi.Domains.Models;
using LibraryApi.Presentations.ViewModels;
namespace LibraryApi.Presentations.Adapters;
/// <summary>
/// UpdateBookViewModelからドメインオブジェクト:Bookへ変換するアダプタ
/// </summary>
public class UpdateBookViewModelAdapter : IRestorer<Book, UpdateBookViewModel>
{
    /// <summary>
    /// UpdateBookViewModelからドメインオブジェクト:Bookを復元する
    /// </summary>
    /// <param name="target">ユースケース:[書籍を変更する]を実現するViewModel</param>
    /// <returns></returns>
    public Task<Book> RestoreAsync(UpdateBookViewModel target)
    {
        // 書籍在庫を生成する
        var bookStock = new BookStock(target.Stock);
        // 書籍を生成する
        var book = new Book(target.BookId, target.Title, target.Author);
        // 書籍在庫を設定する
        book.ChangeStock(bookStock);
        return Task.FromResult(book);
    }
}