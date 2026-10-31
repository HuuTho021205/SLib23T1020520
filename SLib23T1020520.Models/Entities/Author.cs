namespace SLib23T1020520.Models.Entities;

/// <summary>
/// Tác giả: người sáng tác hoặc đóng góp cho các đầu sách trong thư viện.
/// Ánh xạ với bảng <c>dbo.Authors</c>.
/// </summary>
public class Author
{
    /// <summary>Mã tác giả (khóa chính, tự tăng).</summary>
    public int AuthorId { get; set; }

    /// <summary>Tên tác giả (hoặc bút danh).</summary>
    public string AuthorName { get; set; } = string.Empty;

    /// <summary>Tiểu sử, giới thiệu ngắn về tác giả.</summary>
    public string? Biography { get; set; }

    /// <summary>Ngày sinh của tác giả.</summary>
    public DateTime? BirthDate { get; set; }

    /// <summary>
    /// Ngày mất của tác giả; <c>null</c> nếu tác giả còn sống hoặc chưa có thông tin.
    /// Nếu có giá trị thì không được sớm hơn <see cref="BirthDate"/>.
    /// </summary>
    public DateTime? DeathDate { get; set; }

    /// <summary>
    /// Trạng thái sử dụng: <c>true</c> là đang sử dụng,
    /// <c>false</c> là ngừng sử dụng (xóa mềm).
    /// </summary>
    public bool IsActive { get; set; }
}
