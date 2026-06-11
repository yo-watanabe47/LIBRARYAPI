using LibraryApi.Domains.Adapters;
using LibraryApi.Domains.Models;
using LibraryApi.Presentations.ViewModels;
namespace LibraryApi.Presentations.Adapters;
/// <summary>
/// UpdateProductViewModelからドメインオブジェクト:Productへ変換するアダプタ
/// </summary>
public class UpdateBookViewModelAdapter : IRestorer<Book, UpdateBookViewModel>
{
    /// <summary>
    /// UpdateProductViewModelからドメインオブジェクト:Productを復元する
    /// </summary>
    /// <param name="target">ユースケース:[商品を変更する]を実現するViewModel</param>
    /// <returns></returns>
    public Task<Book> RestoreAsync(UpdateBookViewModel target)
    {
        // 商品在庫を生成する
        var bookStock = new BookStock(target.Stock);
        // 商品を生成する
        var book = new Book(target.ProductId, target.Title, target.Author);
        // 商品在庫を設定する
        book.ChangeStock(bookStock);
        return Task.FromResult(book);
    }
}