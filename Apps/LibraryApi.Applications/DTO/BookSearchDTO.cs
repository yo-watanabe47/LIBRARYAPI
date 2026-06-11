using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryApi.Applications.Dtos;

/// <summary>
/// 商品キーワード検索結果を格納する DTO
/// </summary>
public class BookSearchDto
{
    public string BookUuid { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty; 
    public int Stock { get; set; } 
}