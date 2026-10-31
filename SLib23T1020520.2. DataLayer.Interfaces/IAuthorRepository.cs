using SLib23T1020520.Models.Entities;

namespace SLib23T1020520.DataLayer.Interfaces;

/// <summary>
/// Định nghĩa các phép xử lý dữ liệu trên bảng <c>Authors</c> (tác giả).
/// <para>
/// Kế thừa các phép xử lý chung từ <see cref="ICommonRepository{T}"/>
/// (liệt kê, lấy, thêm, sửa, xóa, kiểm tra ràng buộc). Các phép xử lý riêng
/// của tác giả nếu có trong tương lai sẽ được khai báo bổ sung tại đây.
/// </para>
/// </summary>
public interface IAuthorRepository : ICommonRepository<Author>
{
}
