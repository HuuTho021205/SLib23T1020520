namespace SLib23T1020520.Models.Entities;

/// <summary>
/// Liên kết giữa đầu sách và thể loại (quan hệ nhiều - nhiều giữa
/// <see cref="Book"/> và <see cref="Category"/>).
/// Khóa chính là khóa phức hợp gồm (<see cref="BookId"/>, <see cref="CategoryId"/>).
/// Ánh xạ với bảng <c>dbo.BookCategories</c>.
/// </summary>
public class BookCategory
{
    /// <summary>Mã đầu sách (một phần của khóa chính, khóa ngoại tham chiếu đến <see cref="Book"/>).</summary>
    public int BookId { get; set; }

    /// <summary>Mã danh mục (một phần của khóa chính, khóa ngoại tham chiếu đến <see cref="Category"/>).</summary>
    public int CategoryId { get; set; }
}
