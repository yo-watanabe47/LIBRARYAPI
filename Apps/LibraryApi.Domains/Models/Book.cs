using LibraryApi.Domains.Exceptions;
namespace LibraryApi.Domains.Models;

/// <summary>
/// 商品を表すドメインオブジェクト(集約ルート)
/// </summary>
public class Book
{
    public string BookUuid { get; private set; } = string.Empty;
    public string Title { get; private set; } = string.Empty;
    public string Author { get; private set; } = string.Empty;
    public Category? Category { get; private set; }
    public BookStock? Stock { get; private set; }

    /// <summary>
    /// 再構築・復元用コンストラクタ（UUID指定）
    /// </summary>
    /// <param name="productUuid">商品UUID</param>
    /// <param name="name">商品名</param>
    /// <param name="price">価格</param>
    /// <param name="category">商品カテゴリ</param>
    /// <param name="stock">在庫</param>
    public Book(string bookUuid, string title, string author, Category category, BookStock stock)
    {
        ValidateUuid(bookUuid); // UUID形式検証
        BookUuid = bookUuid;
        ValidateTitle(title);        // 商品名検証
        Title = title;
        ValidateAuthor(author);      // 著者名検証
        Author = author;

        Category = category ?? throw new DomainException("カテゴリは必須です。");
        Stock = stock ?? throw new DomainException("在庫情報は必須です。");
    }

    /// <summary>
    /// 新規作成用コンストラクタ（UUID自動生成）
    /// </summary>
    /// <param name="name">商品名</param>
    /// <param name="price">価格</param>
    /// <param name="category">カテゴリ</param>
    /// <param name="stock">在庫</param>
    public Book(string title, string author, Category category, BookStock stock)
        : this(Guid.NewGuid().ToString(), title, author, category, stock)　{}

 /// <summary>
    /// 再構築・復元用コンストラクタ（UUID指定）
    /// </summary>
    /// <param name="productUuid">商品UUID</param>
    /// <param name="name">商品名</param>
    /// <param name="price">価格</param>
    /// <param name="category">商品カテゴリ</param>
    /// <param name="stock">在庫</param>
    public Book(string bookUuid, string title, string author)
    {
        ValidateUuid(bookUuid); // UUID形式検証
        BookUuid = bookUuid;
        ValidateTitle(title);        // 商品名検証
        Title = title;
        ValidateAuthor(author);      // 著者名検証
        Author = author;
    }


    /// <summary>
    /// UUIDの形式検証
    /// </summary>
    private void ValidateUuid(string uuid)
    {
        if (!Guid.TryParse(uuid, out _))
            throw new DomainException("UUIDの形式が正しくありません。");
    }

    // 商品名の最大長
    private const int MaxTitleLength = 50;

    private void ValidateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("商品名は必須です。");
        if (title.Length > MaxTitleLength)
            throw new DomainException($"商品名は{MaxTitleLength}文字以内である必要があります。");
    }


    private const int MaxAuthorLength = 30;

    private void ValidateAuthor(string author)
    {
        if (string.IsNullOrWhiteSpace(author))
            throw new DomainException("著者名は必須です。");
        if (author.Length > MaxAuthorLength)
            throw new DomainException($"著者名は{MaxAuthorLength}文字以内である必要があります。");
    }


    /// <summary>
    /// 商品名の変更
    /// </summary>
    public void ChangeTitle(string title)
    {
        ValidateTitle(title);
        Title = title;
    }

    /// <summary>
    /// 価格の変更
    /// </summary>
    public void ChangeAuthor(string author)
    {
        ValidateAuthor(author);
        Author = author;
    }

    /// <summary>
    /// カテゴリの変更
    /// </summary>
    public void ChangeCategory(Category category)
    {
        Category = category ?? throw new DomainException("カテゴリは必須です。");
    }

    /// <summary>
    /// 在庫の変更
    /// </summary>
    public void ChangeStock(BookStock stock)
    {
            Stock = stock ?? throw new DomainException("在庫情報は必須です。");
    }

    /// <summary>
    /// 識別子の等価性判定
    /// </summary>
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj)) return true;
        return obj is Book other && BookUuid == other.BookUuid;
    }
    public override int GetHashCode() => BookUuid.GetHashCode();

    /// <summary>
    /// インスタンスの内容
    /// </summary>
    /// <returns></returns>
    public override string ToString()
        => $"{BookUuid}: {Title} , {Author} / {Category?.Name ?? "未分類"} , 在庫: {Stock?.Stock ?? 0}";
}