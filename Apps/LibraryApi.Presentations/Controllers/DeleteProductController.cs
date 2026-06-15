using Microsoft.AspNetCore.Mvc;
using LibraryApi.Domains.Models;
using LibraryApi.Applications.Exceptions;
using LibraryApi.Domains.Exceptions;
using LibraryApi.Applications.Usecases.Books.Interfaces;
using LibraryApi.Presentations.Adapters;
using LibraryApi.Presentations.ViewModels;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Authorization;
namespace LibraryApi.Presentations.Controllers;
/// <summary>
/// ユースケース:[書籍を変更する]を実現するコントローラ
/// </summary>
[ApiController]
[Route("api/books/delete")]
[SwaggerTag("書籍削除API")]
public class DeleteBookController : ControllerBase
{
    private readonly IDeleteBookUsecase _usecase;
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="usecase">ユースケース:[商品を変更する]を実現するインターフェイス</param>
    public DeleteBookController(
        IDeleteBookUsecase usecase)
    {
        _usecase = usecase;
    }

    /// <summary>
    /// 選択された書籍Idで書籍を取得する
    /// </summary>
    /// <param name="bookId">書籍Id(UUID)</param>
    /// <returns>該当する書籍が存在すればOK(200)、存在しなければNotFound(404)</returns>
    [Authorize]
    [HttpGet("book/{bookId}")]
    [SwaggerOperation(
        Summary = "書籍の取得",
        Description = "指定された書籍Id(UUID)で書籍を取得する"
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "書籍が存在する場合", typeof(Book))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "指定された書籍が存在しない場合")]
    public async Task<IActionResult> GetBookById(string bookId)
    {
        try
        {
            var book = await _usecase.GetBookByIdAsync(bookId);
            return Ok(book);
        }
        catch (NotFoundException ex)
        {
            // エラーレスポンスを返却する
            return NotFound(new
            { code = "BOOK_NOT_FOUND", message = ex.Message });
        }
    }


    /// <summary>
    /// 書籍を削除する
    /// </summary>
    /// <param name="bookId">書籍削除用ViewModel</param>
    /// <returns></returns>
    [Authorize]
    [HttpDelete]
    [SwaggerOperation(
        Summary = "書籍削除",
        Description = "書籍を削除します。存在しない書籍Idを受け取った場合はエラーを返す"
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "削除成功", typeof(Book))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "バリデーションエラーまたは業務ルール違反")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "指定された書籍Idが存在しない場合")]
    public async Task<IActionResult> Delete([FromBody] string bookId)
    {
        // サーバーサイドバリデーション
        if (!ModelState.IsValid)
        {
            // プロパティ名をキー、エラーメッセージ配列を値とするディクショナリに変換する
            var details = ModelState
                .Where(kv => kv.Value?.Errors.Count > 0) // エラーがある項目だけを抽出する
                .ToDictionary( // Dictionaryに変換する
                               // キー:プロパティ名 ("Name", "Price" など)
                    kv => kv.Key,
                    // 値: 当該プロパティのエラーメッセージ一覧
                    kv => kv.Value!.Errors
                        // エラーメッセージが空やnullの場合は "Invalid value."に置換する
                        .Select(e => string.IsNullOrWhiteSpace(e.ErrorMessage)
                            ? "Invalid value." : e.ErrorMessage)
                        .ToArray()
                );
            return BadRequest(new
            { code = "VALIDATION_ERROR", message = "入力内容に誤りがあります。", details });
        }
        try
        {
            // 書籍を削除する
            await _usecase.DeleteByIdAsync(bookId);
            return Ok();
        }
        catch (NotFoundException ex)
        {
            // エラーレスポンスを返却する
            return NotFound(
                new { code = "BOOK_NOT_FOUND", message = ex.Message });
        }
        catch (DomainException ex)
        {
            return BadRequest(
                new { code = "DOMAIN_RULE_VIOLATION", message = ex.Message });
        }
    }
}