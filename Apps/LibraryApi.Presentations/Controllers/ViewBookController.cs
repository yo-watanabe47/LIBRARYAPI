using Microsoft.AspNetCore.Mvc;
using LibraryApi.Domains.Models;
using LibraryApi.Applications.Exceptions;
using LibraryApi.Applications.Usecases.Books.Interfaces;
using LibraryApi.Presentation.ViewModels;
using Microsoft.AspNetCore.Authorization;

using Swashbuckle.AspNetCore.Annotations;
namespace LibraryApi.Presentation.Controllers;
/// <summary>
/// ユースケース:[商品を表示する]を実現するコントローラ
/// </summary>
[ApiController]
[Route("api/Books/view")]
[SwaggerTag("商品表示API")]
public class ViewBookController : ControllerBase
{
    private readonly IViewBookUsecase _usecase;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="usecase">ユースケース:[商品を表示する]を実現するインターフェイス</param>
    /// <param name="adapter">RegisterProductViewModelからドメインオブジェクト:Productへ変換するアダプタ</param>
    public ViewBookController(
        IViewBookUsecase usecase)
    {
        _usecase = usecase;
    }

    /// <summary>
    /// 商品カテゴリ一覧の取得
    /// </summary>
    /// <returns></returns>
    [HttpGet("categories")]
    [SwaggerOperation(Summary = "商品カテゴリ一覧を取得", 
                      Description = "登録可能なすべての商品カテゴリを返します。")]
    [SwaggerResponse(StatusCodes.Status200OK, "カテゴリ一覧", typeof(List<Category>))]
    public async Task<IActionResult> ViewCategory()
    {
        var result = await _usecase.ViewCategoryAsync();
        return Ok(result);
    }

   
}