using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace LibraryApi.Infrastructure.Entities;
/// <summary>
/// productテーブルに対応するEntity Framework Coreのエンティティ
/// </summary>
[Table("book")]
public class BookEntity
{
    [Key] // 主キーをマッピング
    [Column("id")]
    // 列名と同じ名称のプロパティなので[Column]は使わない
    public int Id { get; set; }

    [Required] // NOT NUll
    [StringLength(36)] // データ長は36文字
    [Column("book_uuid")]// マッピングする列名
    public string BookUuid { get; set; } = string.Empty;

    [Column("title")]
    [Required] // NOT NULL
    [StringLength(50)]// データ長は50文字
    // 列名と同じ名称のプロパティなので[Column]は使わない
    public string Title { get; set; } = string.Empty;

    [Column("author")]
    [Required] // NOT NULL
    [StringLength(30)]// データ長は30文字
    // 列名と同じ名称のプロパティなので[Column]は使わない
    public string Author { get; set; } = string.Empty;

    [Column("category_id")]// マッピングする列名
    [ForeignKey("CategoryId")]
    public int? CategoryId { get; set; }

    [Column("created_at")]
    [Required] // NOT NULL
    public DateTime CreatedAt { get; set; }
    [Column("updated_at")]
    [Required] // NOT NULL
    public DateTime UpdatedAt { get; set; }

    // ProductCategroyエンティティへのナビゲーションプロパティ
    // ProductCategoryIdプロパティの値と外部キー関係にある
    // null許容にし、商品のカテゴリを含めないケースも許可する
   
    public CategoryEntity? Category { get; set; }

    // 在庫情報（1:1 関係を想定）
    public BookStockEntity? BookStock { get; set; }

    public override string ToString()
    {
        return $"Id={Id}, ProductUuid={BookUuid}, Name={Title}, Author={Author},CreratedAt={CreatedAt},UpdatedAt={UpdatedAt}," +
               $"Category={Category?.Name}, Stock={BookStock?.Stock}" ;
    }
}