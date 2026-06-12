using Microsoft.AspNetCore.Mvc;
using LibraryApi.Domains.Models;
using LibraryApi.Applications.Exceptions;
using LibraryApi.Domains.Exceptions;
using LibraryApi.Applications.Usecases.Books.Interfaces;
using LibraryApi.Presentations.Adapters;
using LibraryApi.Presentations.ViewModels;
using Microsoft.AspNetCore.Authorization;

using Swashbuckle.AspNetCore.Annotations;
namespace LibraryApi.Presentations.Controllers;
/// <summary>
/// ユースケース:[新書籍を登録する]を実現するコントローラ
/// </summary>
[ApiController]
[Route("api/books/register")]
[SwaggerTag("新書籍登録API")]
public class RegisterBookController : ControllerBase
{
    private readonly IRegisterBookUsecase _usecase;
    private readonly RegisterBookViewModelAdapter _adapter;
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="usecase">ユースケース:[新書籍を登録する]を実現するインターフェイス</param>
    /// <param name="adapter">RegisterBookViewModelからドメインオブジェクト:Bookへ変換するアダプタ</param>
    public RegisterBookController(
        IRegisterBookUsecase usecase,
        RegisterBookViewModelAdapter adapter)
    {
        _usecase = usecase;
        _adapter = adapter;
    }

    /// <summary>
    /// 商品カテゴリ一覧の取得
    /// </summary>
    /// <returns></returns>
    // [Authorize]
    [HttpGet("categories")]
    [SwaggerOperation(Summary = "カテゴリ一覧を取得", 
                      Description = "登録可能なすべてのカテゴリを返します。")]
    [SwaggerResponse(StatusCodes.Status200OK, "カテゴリ一覧", typeof(List<Category>))]
    public async Task<IActionResult> GetCategories()
    {
        var result = await _usecase.GetCategoriesAsync();
        return Ok(result);
    }

    /// <summary>
    /// 選択された商品カテゴリIdで商品カテゴリを取得する
    /// </summary>
    /// <param name="categoryId">商品カテゴリId(UUID)</param>
    /// <returns>該当するカテゴリが存在すればOK(200)、存在しなければNotFound(404)</returns>
    // [Authorize]
    [HttpGet("categories/{categoryId}")]
    [SwaggerOperation(Summary = "カテゴリの取得", 
                      Description = "指定されたカテゴリIdに一致するカテゴリを返します。")]
    [SwaggerResponse(StatusCodes.Status200OK, "カテゴリが見つかった場合", typeof(Category))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "該当カテゴリが存在しない場合")]
    public async Task<IActionResult> GetCategoryById(string categoryId)
    {
        //nullとスペースのチェックがない
        //UUID形式の確認してない
        try
        {
            var category = await _usecase.GetCategoryByIdAsync(categoryId);
            return Ok(category);
        }
        catch (NotFoundException ex)
        {
            // エラーレスポンスを返却する
            return NotFound(new
            { code = "CATEGORY_NOT_FOUND", message = ex.Message });
        }
    }

    /// <summary>
    /// 商品が既に存在するかを検証する
    /// </summary>
    /// <param name="productName">検証対象の商品名</param>
    /// <returns>
    /// 存在しない場合:Ok(200)、存在する場合:Conflict(409) 
    /// </returns>
    // [Authorize]
    [HttpGet("validate")]
    [SwaggerOperation(Summary = "書籍名の存在確認", 
                      Description = "書籍名が既に存在するかを検証する")]
    [SwaggerResponse(StatusCodes.Status200OK, "存在しない場合 { exists=false } を返す")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "書籍名が未入力の場合")]
    [SwaggerResponse(StatusCodes.Status409Conflict, "書籍名が既に存在する場合")]
    public async Task<IActionResult> ValidateProduct([FromQuery] string? productName)
    {
        // 書籍名がnullか空白
        if (string.IsNullOrWhiteSpace(productName))
        {
            return BadRequest(new
            { code = "INVALID_PRODUCT_NAME", message = "書籍名は必須です。" });
        }
        try
        {
            // 書籍名の存在有無を調べる
            await _usecase.ExistsByTitleAsync(productName);
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
    /// 新商品を登録する
    /// </summary>
    /// <param name="model">商品登録用ViewModel</param>
    /// <returns></returns>
    // [Authorize]
    [HttpPost]
    [SwaggerOperation(Summary = "新書籍を登録", 
                      Description = "書籍情報を受け取り、書籍を登録する")]
    [SwaggerResponse(StatusCodes.Status201Created, "登録成功", typeof(Book))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "バリデーションエラーまたは業務ルール違反")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "カテゴリIdが存在しない場合")]
    [SwaggerResponse(StatusCodes.Status409Conflict, "書籍が既に存在する場合")]
    public async Task<IActionResult> Register(
        // SwaggerRequestBodyを追加
        [FromBody, SwaggerRequestBody("新書籍登録用ViewModel", Required = true)]
        RegisterBookViewModel model)
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
            // 指定カテゴリが存在することを取得する（戻り値のCategoryを利用して設定する）
            var category = await _usecase.GetCategoryByIdAsync(model.CategoryId);
            // 既に登録済みの書籍を受信した(ミスしている)
            await _usecase.ExistsByTitleAsync(model.Title);
            // RegisterBookViewModelからBookを復元する
            var book = await _adapter.RestoreAsync(model);
            // 取得したカテゴリを設定してから永続化する
            book.ChangeCategory(category);
            await _usecase.RegisterBookAsync(book);
            return Created($"/api/books/{book.BookUuid}", book.BookUuid);
            //上の最後のProductUuidを消したほうがいいかも
        }
        catch (ExistsException ex)
        {
            // 既に存在する書籍を受信した
            return Conflict(new { code = "BOOK_ALREADY_EXISTS", message = ex.Message });
        }
        catch (NotFoundException ex)
        {
            // 存在しない商品カテゴリIdを受信した
            return NotFound(new { code = "CATEGORY_NOT_FOUND", message = ex.Message });
        }
        catch (DomainException ex)
        {
            // 業務ルール違反のデータを受信した
            return BadRequest(new { code = "DOMAIN_RULE_VIOLATION", message = ex.Message });
        }
    }
}