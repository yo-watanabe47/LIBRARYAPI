using LibraryApi.Domains.Models;
using LibraryApi.Domains.Repositories;
using LibraryApi.Applications.Exceptions;
using LibraryApi.Applications.Usecases.Books.Interfaces;
namespace LibraryApi.Applications.Usecases.Books.Interactors;
/// <summary>
/// ユースケース:[書籍を削除する]を実現するインターフェイスの実装
/// </summary>
public class DeleteBookUsecase : IDeleteBookUsecase
{
    private readonly IBookRepository _bookRepository;
    private readonly IUnitOfWork _unitOfWork;
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="bookRepository">書籍CRUD操作リポジトリ</param>
    /// <param name="unitOfWork">トランザクション制御機能</param>
    public DeleteBookUsecase(
        IBookRepository bookRepository, IUnitOfWork unitOfWork)
    {
        _bookRepository = bookRepository;
        _unitOfWork = unitOfWork;
    }


    /// <summary>
    /// 指定された書籍Idの書籍を取得する
    /// クライアント側の[入力画面]で利用するため
    /// </summary>
    /// <param name="id">書籍Id</param>
    /// <returns>該当書籍、書籍在庫、書籍カテゴリ</returns>
    /// <exception cref="NotFoundException">該当データが存在しない場合にスローされる</exception>
    public async Task<Book> GetBookByIdAsync(string id)
    {
        var result = await _bookRepository
            .SelectByIdWithBookStockAndCategoryAsync(id);
        if (result is null)
        {
            throw new NotFoundException($"書籍Id:{id}の書籍は存在しません。");
        }
        return result;
    }

    /// <summary>
    /// 書籍を削除する
    /// </summary>
    /// <param name="book">削除対象書籍</param>
    /// <returns>なし</returns>
    /// <exception cref="NotFoundException">書籍が存在しない場合にスローされる</exception>
    public async Task DeleteByIdAsync(string id)
    {
        // トランザクションを開始する
        await _unitOfWork.BeginAsync();
        try
        {
            var result = await _bookRepository.DeleteByIdAsync(id);
            if (result == false)
            {
                throw new NotFoundException($"書籍Id:{id}の書籍は存在しないため削除できません。");
            }
            // トランザクションをコミットする
            await _unitOfWork.CommitAsync();
        }
        catch
        {
            // トランザクションをロールバックする
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }
}