using LibraryApi.Domains.Models;
using LibraryApi.Domains.Repositories;
using LibraryApi.Applications.Exceptions;
using LibraryApi.Applications.Usecases.Books.Interfaces;
namespace LibraryApi.Applications.Usecases.Books.Interactors;
/// <summary>
/// ユースケース:[商品をキーワード検索する]を実現するインターフェイスの実装
/// </summary>
public class ViewBookUsecase : IViewBookUsecase
{
    private readonly IBookRepository _bookrepository;
    private readonly ICategoryRepository _categoryRepository;
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="repository">商品CRUD操作リポジトリ</param>
    public ViewBookUsecase(IBookRepository bookrepository, ICategoryRepository categoryRepository)
    {
        _bookrepository = bookrepository;
        _categoryRepository = categoryRepository;
    }

    // /// <summary>
    // /// 指定されたキーワードで商品を部分一致検索した結果を返す
    // /// </summary>
    // /// <param name="keyword">商品キーワード</param>
    // /// <returns>キーワード検索結果</returns>
    // /// <exception cref="NotFoundException">該当データが存在しない場合にスローされる</exception>
    // public async Task<List<Book>> ExecuteAsync(string keyword)
    // {
    //     var result = await _repository
    //         .SelectByNameLikeWithProductStockAndProductCategoryAsync(keyword);
    //     return result;
    // }

    // /// <summary>
    // /// カテゴリ一覧を表示する
    // /// </summary>
       public async Task<List<Category>> ViewCategoryAsync()
    {
       return await _categoryRepository.SelectAllAsync();
    }
}