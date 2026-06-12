using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using LibraryApi.Domains.Models;
using LibraryApi.Applications.Usecases.Books.Interfaces;
using Microsoft.AspNetCore.Authorization;
using LibraryApi.Applications.Dtos;
namespace LibraryApi.Presentations.Controllers;
/// <summary>
/// ユースケース:[商品をキーワード検索する]を実現するコントローラ
/// </summary>
[ApiController]
[Route("api/books/search")]
// タググループに反映されるコントローラの概要
[SwaggerTag("本をキーワード検索API")]
public class SearchBookByKeywordController : ControllerBase
{
    private readonly ISearchBookByKeywordUsecase _usecase;
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="usecase">ユースケース:[商品をキーワード検索する]を実現するインターフェイス</param>
    public SearchBookByKeywordController(ISearchBookByKeywordUsecase usecase)
    {
        _usecase = usecase;
    }

    /// <summary>
    /// キーワードで商品を検索する
    /// </summary>
    /// <param name="keyword">検索キーワード</param>
    /// <returns>検索結果の商品一覧</returns>

    [HttpGet]
    // [ProducesResponseType]から[SwaggerResponse]に変更する
    [SwaggerResponse(StatusCodes.Status200OK, "検索に成功した場合、商品リストを返す", typeof(List<BookDto>))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "キーワード未入力など、リクエストが不正な場合")]
    public async Task<IActionResult> Search([FromQuery] string? keyword)
    {
        // 未入力チェック
        if (string.IsNullOrWhiteSpace(keyword))
        {
            return BadRequest(
            new { code = "ValidationError", message = "キーワードは1~50文字で入力してください" });
        }
        if (keyword.Length > 50)
        {
            return BadRequest(new
            {
                error = "ValidationError",
                message = "キーワードは1~50文字で入力してください。"
            });
        }
        // 商品キーワード検索する
        var result = await _usecase.ExecuteAsync(keyword.Trim());
        return Ok(result);
    }
}