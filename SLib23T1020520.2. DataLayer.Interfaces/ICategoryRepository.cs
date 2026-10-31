using SLib23T1020520.Models.Entities;

namespace SLib23T1020520.DataLayer.Interfaces;

/// <summary>
/// Định nghĩa các phép xử lý dữ liệu trên bảng <c>Categories</c> (thể loại sách).
/// <para>
/// Kế thừa các phép xử lý chung từ <see cref="ICommonRepository{T}"/>
/// (liệt kê, lấy, thêm, sửa, xóa, kiểm tra ràng buộc). Các phép xử lý riêng
/// của thể loại sách nếu có trong tương lai sẽ được khai báo bổ sung tại đây.
/// </para>
/// </summary>
public interface ICategoryRepository : ICommonRepository<Category>
{
}
