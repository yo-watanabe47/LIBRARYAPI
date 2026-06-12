using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace LibraryApi.Applications.Dtos;

/// <summary>
/// 画面の表示仕様（理想の形式）に完全に合わせた DTO
/// </summary>
public class BookDto
{
    // bookUuid から bookId に名称を変更
    public string BookId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    
    // ★文字列ではなく、オブジェクト階層にする
    public BookCategoryDto? Category { get; set; }

    public int Stock { get; set; }
}

/// <summary>
/// カテゴリ情報用のネストされた DTO
/// </summary>
public class BookCategoryDto
{
    // categoryUuid から categoryId に名称を変更し、不要な日時は含めない
    public string CategoryId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}