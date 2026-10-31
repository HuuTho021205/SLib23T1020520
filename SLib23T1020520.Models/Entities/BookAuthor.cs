namespace SLib23T1020520.Models.Entities;

/// <summary>
/// Liên kết giữa đầu sách và tác giả (quan hệ nhiều - nhiều giữa
/// <see cref="Book"/> và <see cref="Author"/>).
/// Khóa chính là khóa phức hợp gồm (<see cref="BookId"/>, <see cref="AuthorId"/>).
/// Ánh xạ với bảng <c>dbo.BookAuthors</c>.
/// </summary>
public class BookAuthor
{
    /// <summary>Mã đầu sách (một phần của khóa chính, khóa ngoại tham chiếu đến <see cref="Book"/>).</summary>
    public int BookId { get; set; }

    /// <summary>Mã tác giả (một phần của khóa chính, khóa ngoại tham chiếu đến <see cref="Author"/>).</summary>
    public int AuthorId { get; set; }

    /// <summary>Thứ tự của tác giả trong sách, phải lớn hơn 0 (1 là tác giả đứng đầu; mặc định là 1).</summary>
    public short AuthorOrder { get; set; }

    /// <summary>Vai trò/đóng góp của tác giả đối với sách (ví dụ: "Tác giả chính", "Dịch giả").</summary>
    public string? Contribution { get; set; }
}
