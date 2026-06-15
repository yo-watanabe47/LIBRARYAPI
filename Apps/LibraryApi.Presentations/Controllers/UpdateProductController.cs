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
[Route("api/books/update")]
[SwaggerTag("書籍変更API")]
public class UpdateBookController : ControllerBase
{
    private readonly IUpdateBookUsecase _usecase;
    private readonly UpdateBookViewModelAdapter _adapter;
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="usecase">ユースケース:[商品を変更する]を実現するインターフェイス</param>
    /// <param name="adapter">UpdateBookViewModelからドメインオブジェクト:Bookへ変換するアダプタ</param>
    public UpdateBookController(
        IUpdateBookUsecase usecase,
        UpdateBookViewModelAdapter adapter)
    {
        _usecase = usecase;
        _adapter = adapter;
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
    /// 書籍が既に存在するかを検証する
    /// </summary>
    /// <param name="Title">検証対象の書籍名</param>
    /// <returns>
    /// 存在しない場合:Ok(200)、存在する場合:Conflict(409) 
    /// </returns>
    
    [Authorize]
    [HttpGet("validate")]
    [SwaggerOperation(
        Summary = "書籍名の存在確認",
        Description = "書籍名が既に存在するかを検証する"
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "存在しない場合 { exists=false } を返す")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "書籍名が未入力の場合")]
    [SwaggerResponse(StatusCodes.Status409Conflict, "書籍名が既に存在する場合")]
    public async Task<IActionResult> ValidateProduct([FromQuery] string Title)
    {
        // 書籍名がnullか空白
        if (string.IsNullOrWhiteSpace(Title))
        {
            return BadRequest(new
            { code = "INVALID_BOOK_NAME", message = "書籍名は必須です。" });
        }
        try
        {
            // 書籍名の存在有無を調べる
            await _usecase.ExistsByTitleAsync(Title);
            return Ok(new { exists = false });
        }
        catch (ExistsException ex)
        {
            // 書籍が既に存在する場合
            return Conflict(new
            { code = "BOOK_ALREADY_EXISTS", message = ex.Message });
        }
    }

    /// <summary>
    /// 書籍を変更する
    /// </summary>
    /// <param name="model">書籍変更用ViewModel</param>
    /// <returns></returns>
    [Authorize]
    [HttpPut]
    [SwaggerOperation(
        Summary = "書籍変更",
        Description = "書籍情報を更新します。書籍名の重複や存在しない書籍Idを受け取った場合はエラーを返す"
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "変更成功", typeof(Book))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "バリデーションエラーまたは業務ルール違反")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "指定された書籍Idが存在しない場合")]
    [SwaggerResponse(StatusCodes.Status409Conflict, "書籍名が既に存在する場合")]
    public async Task<IActionResult> Updated([FromBody] UpdateBookViewModel model)
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
            // 書籍名の存在有無を調べる
            await _usecase.ExistsByTitleAsync(model.Title);
            // UpdateProductViewModelからProductを復元する
            var book = await _adapter.RestoreAsync(model);
            // 書籍を変更する
            await _usecase.UpdateBookAsync(book);
            return Ok(book);
        }
        catch (NotFoundException ex)
        {
            // エラーレスポンスを返却する
            return NotFound(
                new { code = "BOOK_NOT_FOUND", message = ex.Message });
        }
        catch (ExistsException ex)
        {
            // 書籍が既に存在する場合
            return Conflict(
                new { code = "BOOK_ALREADY_EXISTS", message = ex.Message });
        }
        catch (DomainException ex)
        {
            return BadRequest(
                new { code = "DOMAIN_RULE_VIOLATION", message = ex.Message });
        }
    }
}