namespace SLib23T1020520.Models.Entities;

/// <summary>
/// Thể loại/danh mục sách. Các danh mục được tổ chức theo dạng cây cha - con
/// thông qua <see cref="ParentCategoryId"/>.
/// Ánh xạ với bảng <c>dbo.Categories</c>.
/// </summary>
public class Category
{
    /// <summary>Mã danh mục (khóa chính, tự tăng).</summary>
    public int CategoryId { get; set; }

    /// <summary>Tên danh mục.</summary>
    public string CategoryName { get; set; } = string.Empty;

    /// <summary>
    /// Mã danh mục cha (khóa ngoại tham chiếu đến chính bảng <c>Categories</c>);
    /// <c>null</c> nếu là danh mục gốc. Không được trùng với <see cref="CategoryId"/>.
    /// </summary>
    public int? ParentCategoryId { get; set; }

    /// <summary>Mô tả về danh mục.</summary>
    public string? Description { get; set; }

    /// <summary>
    /// Trạng thái sử dụng: <c>true</c> là đang sử dụng,
    /// <c>false</c> là ngừng sử dụng (xóa mềm).
    /// </summary>
    public bool IsActive { get; set; }
}
