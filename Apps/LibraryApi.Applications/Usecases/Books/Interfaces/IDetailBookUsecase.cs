using LibraryApi.Domains.Models;
namespace LibraryApi.Applications.Usecases.Books.Interfaces;
/// <summary>
/// ユースケース:[商品を詳細表示する]を実現するインターフェイス
/// </summary>
public interface IDetailBookUsecase
{
    /// <summary>
    /// 指定されたキーワードで商品を部分一致検索した結果を返す
    /// </summary>
    /// <param name="keyword">商品キーワード</param>
    /// <returns>キーワード検索結果</returns>
    Task<List<Book>> ExecuteDetailAsync(string keyword);

}