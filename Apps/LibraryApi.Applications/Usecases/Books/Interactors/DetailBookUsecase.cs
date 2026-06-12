using LibraryApi.Domains.Models;
using LibraryApi.Domains.Repositories;
using LibraryApi.Applications.Exceptions;
using LibraryApi.Applications.Usecases.Books.Interfaces;
using LibraryApi.Applications.Dtos;
namespace LibraryApi.Applications.Usecases.Books.Interfaces;
/// <summary>
/// ユースケース:[商品を詳細表示する]を実現するインターフェイス
/// </summary>
public class DetailBookUsecase : IDetailBookUsecase
{
    private readonly IBookRepository _repository;
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="repository">商品CRUD操作リポジトリ</param>
    public DetailBookUsecase(IBookRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// 指定されたキーワードで商品を部分一致検索した結果を返す
    /// </summary>
    /// <param name="keyword">商品キーワード</param>
    /// <returns>キーワード検索結果</returns>
    /// <exception cref="NotFoundException">該当データが存在しない場合にスローされる</exception>
    public async Task<List<BookDto>> ExecuteDetailAsync(string keyword)
    {
        var result = await _repository
            .SelectByBookDetailAsync(keyword);
        return result.Select(b => new BookDto
        {
            BookId = b.BookUuid,
            Title = b.Title,
            Author = b.Author,
            Category = b.Category != null ? new BookCategoryDto
            {
                CategoryId = b.Category.CategoryUuid,
                Name = b.Category.Name
            } : null,
            Stock = b.Stock?.Stock ?? 0
        }).ToList();
    }
}