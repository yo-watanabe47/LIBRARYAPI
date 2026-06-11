// using LibraryApi.Domains.Models;
// using LibraryApi.Domains.Repositories;
// using LibraryApi.Applications.Exceptions;
// using LibraryApi.Applications.Usecases.Products.Interfaces;
// namespace LibraryApi.Applications.Usecases.Products.Interactors;
// /// <summary>
// /// ユースケース:[新商品を登録する]を実現するインターフェイスの実装
// /// </summary>
// public class RegisterBookUsecase : IRegisterBookUsecase
// {
//     private readonly ICategoryRepository _productCategoryRepository;
//     private readonly IBookRepository _productRepository;
//     private readonly IUnitOfWork _unitOfWork;
//     /// <summary>
//     /// コンストラクタ
//     /// </summary>
//     /// <param name="productCategoryRepository">商品カテゴリCRUD操作リポジトリ</param>
//     /// <param name="productRepository">商品CRUD操作リポジトリ</param>
//     /// <param name="unitOfWork">トランザクション制御機能</param>
//     public RegisterBookUsecase(
//         ICategoryRepository productCategoryRepository,
//         IBookRepository productRepository,
//         IUnitOfWork unitOfWork)
//     {
//         _productCategoryRepository = productCategoryRepository;
//         _productRepository = productRepository;
//         _unitOfWork = unitOfWork;
//     }

//     /// <summary>
//     /// 指定ざれた商品の存在有無を調べる
//     /// </summary>
//     /// <param name="productName">商品目</param>
//     /// <returns>なし</returns>
//     /// <exception cref="ExistsException">同一商品名が存在する場合にスローされる</exception>
//     public async Task ExistsByTitleAsync(string title)
//     {
//         // 指定された商品の有無を調べる
//         var result = await _productRepository.ExistsByNameAsync(title);
//         if (result) // 商品が既に存在する
//         {
//             throw new ExistsException($"商品名:{title}は既に存在します。");
//         }
//     }

//     /// <summary>
//     /// すべての商品カテゴリを取得する
//     /// クライアント側の[入力画面]で利用するプルダウンを作成するため
//     /// </summary>
//     /// <returns>ProductCategoryのリスト</returns>
//     public async Task<List<Category>> GetCategoriesAsync()
//     {
//         return await _productCategoryRepository.SelectAllAsync();
//     }

//     /// <summary>
//     /// 指定された商品カテゴリIdの商品カテゴリを取得する
//     /// クライアント側の[確認画面]で利用するため
//     /// </summary>
//     /// <param name="id">商品カテゴリId</param>
//     /// <returns>該当商品カテゴリ</returns>
//     /// <exception cref="NotFoundException">該当データが存在しない場合にスローされる</exception>
//     public async Task<Category> GetCategoryByIdAsync(string id)
//     {
//         var result = await _productCategoryRepository.SelectByIdAsync(id);
//         if (result is null)
//         {
//             throw new NotFoundException($"商品カテゴリId:{id}の商品カテゴリは存在しません。");
//         }
//         return result!; 
//     }

//     /// <summary>
//     /// 新商品を登録する
//     /// </summary>
//     /// <param name="product">登録対象商品</param>
//     /// <returns>なし</returns>
//     /// <exception cref="NotFoundException">商品カテゴリが存在しない場合にスローされる</exception>
//     public async Task RegisterBookAsync(Book book)
//     {
//         // トランザクションを開始する
//         await _unitOfWork.BeginAsync();
//         try
//         {
//             // 商品カテゴリを取得する
//             await GetCategoryByIdAsync(book.Category!.CategoryUuid);
//             // 新商品を登録する
//             await _productRepository.CreateAsync(book);
//             // トランザクションをコミットする
//             await _unitOfWork.CommitAsync();
//         }
//         catch
//         {
//             // トランザクションをロールバックする
//             await _unitOfWork.RollbackAsync();
//             throw;
//         }
//     }
// }