using LibraryApi.Domains.Adapters;
using LibraryApi.Domains.Models;
using LibraryApi.Applications.Exceptions;
using LibraryApi.Infrastructures.Entities;
namespace LibraryApi.Infrastructures.Adapters;
/// <summary>
/// ドメインオブジェクト:ProductとProductEntityの相互変換クラス
/// </summary> 
/// <typeparam name="Product">ドメインオブジェクト:Product</typeparam>
/// <typeparam name="ProductEntity">EFCore:ProductEntity</typeparam>
public class BookEntityAdapter :
IConverter<Book, BookEntity>, IRestorer<Book, BookEntity>
{
    /// <summary>
    /// ドメインオブジェクト:ProductをProductEntityに変換する
    /// </summary>
    /// <param name="domain">ドメインオブジェクト:Product</param>
    /// <returns>EFCore:ProductEntity</returns>
    public Task<BookEntity> ConvertAsync(Book domain)
    {
        // 引数domainがnullの場合
        //nullがここまで来るのはエラーなので、InternalExceptionをスローする
        _ = domain ?? throw new InternalException("引数domainがnullです。");
        // ドメインオブジェクト:DepartmentをDepartmentEntityに変換する
        var entity = new BookEntity();
        entity.BookUuid = domain.BookUuid;
        entity.Title = domain.Title;
        entity.Author = domain.Author;
        return Task.FromResult(entity);
        //Task.FromResultは、非同期メソッドの戻り値を簡単に作成するためのメソッド
    }

    /// <summary>
    /// ProductEntityからドメインオブジェクト:Productを復元する
    /// </summary>
    /// <param name="target">>EFCore:ProductEntity</param>
    /// <returns>ドメインオブジェクト:Product</returns>
    public Task<Book> RestoreAsync(BookEntity target)
    {
        // 引数targetがnullの場合
        _ = target ?? throw new InternalException("引数targetがnullです。");
        // ProductEntityからドメインオブジェクト:Productを復元する
        var domain = new Book(target.BookUuid, target.Title, target.Author);
        return Task.FromResult(domain);
    }
}