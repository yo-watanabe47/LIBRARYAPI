using LibraryApi.Domains.Models;
namespace LibraryApi.Applications.Usecases.Books.Interfaces;
/// <summary>
/// ユースケース:[書籍を変更する]を実現するインターフェイス
/// </summary>
public interface IUpdateBookUsecase
{
    /// <summary>
    /// 指定された書籍Idの書籍を取得する
    /// クライアント側の[入力画面]で利用するため
    /// </summary>
    /// <param name="id">書籍Id</param>
    /// <returns>該当書籍、書籍在庫、書籍カテゴリ</returns>
    /// <exception cref="NotFoundException">該当データが存在しない場合にスローされる</exception>
    Task<Book> GetBookByIdAsync(string id);   

    /// <summary>
    /// 指定ざれた書籍の存在有無を調べる
    /// </summary>
    /// <param name="title">書籍名</param>
    /// <returns>なし</returns>
    /// <exception cref="ExistsException">同一書籍名が存在する場合にスローされる</exception>
    Task ExistsByTitleAsync(string title);

    /// <summary>
    /// 書籍を変更する
    /// </summary>
    /// <param name="book">変更対象対象書籍</param>
    /// <returns>なし</returns>
    /// <exception cref="NotFoundException">書籍が存在しない場合にスローされる</exception>
    Task UpdateBookAsync(Book book);
}