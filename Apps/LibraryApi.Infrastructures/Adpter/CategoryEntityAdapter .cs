using LibraryApi.Domains.Adapters;
using LibraryApi.Domains.Models;
using LibraryApi.Domains.Exceptions;
using LibraryApi.Infrastructure.Entities;
namespace LibraryApi.Infrastructure.Adapters;
/// <summary>
/// ドメインオブジェクト:ProductCategoryとProductCategoryEntityの相互変換クラス
/// </summary> 
/// <typeparam name="ProductCategory">ドメインオブジェクト:ProductCategory</typeparam>
/// <typeparam name="ProductCategoryEntity">EFCore:ProductCategoryEntity</typeparam>
public class CategoryEntityAdapter :
IConverter<Category, CategoryEntity>, IRestorer<Category, CategoryEntity>
{
    /// <summary>
    /// ドメインオブジェクト:ProductCategoryをProductCategoryEntityに変換する
    /// </summary>
    /// <param name="domain">ドメインオブジェクト:ProductCategory</param>
    /// <returns>EFCore:ProductCategoryEntity</returns>
    public Task<CategoryEntity> ConvertAsync(Category domain)
    {
        // 引数domainがnullの場合
        _ = domain ?? throw new InternalException("引数domainがnullです。");
        // ドメインオブジェクト:ProductCategoryをProductCategoryEntityに変換する
        var entity = new CategoryEntity();
        entity.CategoryUuid = domain.CategoryUuid;
        entity.Name = domain.Name;
        return Task.FromResult(entity);
    }

    /// <summary>
    /// ProductCategoryEntityからドメインオブジェクト:ProductCategoryを復元する
    /// </summary>
    /// <param name="target">>EFCore:ProductCategoryEntity</param>
    /// <returns>ドメインオブジェクト:ProductCategory</returns>
    public Task<Category> RestoreAsync(CategoryEntity target)
    {
        // 引数targetがnullの場合
        _ = target ?? throw new InternalException("引数targetがnullです。");
        // ProductCategoryEntityからドメインオブジェクト:ProductCategoryを復元する
        var domain = new Category(target.CategoryUuid, target.Name);
        return Task.FromResult(domain);
    }
}