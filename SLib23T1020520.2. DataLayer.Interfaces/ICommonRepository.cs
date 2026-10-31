using SLib23T1020520.Models.Entities;

namespace SLib23T1020520.DataLayer.Interfaces;

/// <summary>
/// Định nghĩa các phép xử lý dữ liệu chung (liệt kê, lấy, thêm, sửa, xóa, kiểm tra ràng buộc)
/// trên các bảng dữ liệu có khóa chính là một số nguyên tự tăng.
/// <para>
/// Interface này được dùng cho các bảng:
/// <c>Authors</c>, <c>Publishers</c>, <c>Languages</c>, <c>Categories</c>,
/// <c>Locations</c> và <c>MemberTypes</c>.
/// </para>
/// </summary>
/// <typeparam name="T">
/// Kiểu dữ liệu (entity) biểu diễn một dòng của bảng, ví dụ:
/// <c>Author</c>, <c>Publisher</c>, <c>Language</c>, <c>Category</c>, <c>Location</c>, <c>MemberType</c>.
/// </typeparam>
public interface ICommonRepository<T> where T : class
{
    /// <summary>
    /// Lấy danh sách toàn bộ dữ liệu của bảng.
    /// </summary>
    /// <returns>
    /// Tập hợp tất cả các bản ghi; tập hợp rỗng (không phải <c>null</c>) nếu bảng chưa có dữ liệu.
    /// </returns>
    Task<IEnumerable<T>> ListAsync();

    /// <summary>
    /// Lấy thông tin của một bản ghi dựa vào mã (khóa chính).
    /// </summary>
    /// <param name="id">Mã của bản ghi cần lấy.</param>
    /// <returns>
    /// Bản ghi tìm được; <c>null</c> nếu không tồn tại bản ghi có mã tương ứng.
    /// </returns>
    Task<T?> GetAsync(int id);

    /// <summary>
    /// Bổ sung một bản ghi mới vào bảng.
    /// </summary>
    /// <param name="data">Dữ liệu của bản ghi cần bổ sung (không cần cung cấp giá trị khóa chính).</param>
    /// <returns>
    /// Mã (khóa chính) của bản ghi vừa được bổ sung, do cơ sở dữ liệu tự sinh.
    /// </returns>
    Task<int> AddAsync(T data);

    /// <summary>
    /// Cập nhật thông tin của một bản ghi đã có.
    /// </summary>
    /// <param name="data">
    /// Dữ liệu mới của bản ghi; bản ghi cần cập nhật được xác định bằng giá trị khóa chính chứa trong <paramref name="data"/>.
    /// </param>
    /// <returns>
    /// <c>true</c> nếu cập nhật thành công; <c>false</c> nếu không tìm thấy bản ghi cần cập nhật.
    /// </returns>
    Task<bool> UpdateAsync(T data);

    /// <summary>
    /// Xóa một bản ghi dựa vào mã (khóa chính).
    /// </summary>
    /// <param name="id">Mã của bản ghi cần xóa.</param>
    /// <returns>
    /// <c>true</c> nếu xóa thành công; <c>false</c> nếu không tìm thấy bản ghi cần xóa.
    /// </returns>
    /// <remarks>
    /// Phép xử lý này chỉ thực hiện việc xóa, không tự kiểm tra các dữ liệu liên quan.
    /// Tầng nghiệp vụ cần gọi <see cref="InUsedAsync(int)"/> trước để đảm bảo bản ghi
    /// không còn được sử dụng ở bảng khác.
    /// </remarks>
    Task<bool> DeleteAsync(int id);

    /// <summary>
    /// Kiểm tra xem một bản ghi có đang được sử dụng (có dữ liệu liên quan ở bảng khác) hay không.
    /// </summary>
    /// <param name="id">Mã của bản ghi cần kiểm tra.</param>
    /// <returns>
    /// <c>true</c> nếu bản ghi có dữ liệu liên quan (không nên xóa); ngược lại là <c>false</c>.
    /// </returns>
    /// <remarks>
    /// Các quan hệ cần kiểm tra theo từng bảng:
    /// <list type="bullet">
    /// <item><description><c>Authors</c>: bảng <c>BookAuthors</c>.</description></item>
    /// <item><description><c>Publishers</c>: bảng <c>Books</c> (cột <c>PublisherId</c>).</description></item>
    /// <item><description><c>Languages</c>: bảng <c>Books</c> (cột <c>LanguageId</c>).</description></item>
    /// <item><description><c>Categories</c>: bảng <c>BookCategories</c> và các danh mục con trong chính bảng <c>Categories</c> (cột <c>ParentCategoryId</c>).</description></item>
    /// <item><description><c>Locations</c>: bảng <c>BookCopies</c> (cột <c>LocationId</c>).</description></item>
    /// <item><description><c>MemberTypes</c>: bảng <c>Members</c> (cột <c>MemberTypeId</c>).</description></item>
    /// </list>
    /// </remarks>
    Task<bool> InUsedAsync(int id);
}