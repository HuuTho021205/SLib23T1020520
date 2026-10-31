namespace SLib23T1020520.Models.Entities;

/// <summary>
/// Đầu sách: thông tin chung của một ấn bản sách (tên, ISBN, nhà xuất bản, ngôn ngữ...).
/// Mỗi đầu sách có thể có nhiều bản sao vật lý, được quản lý bởi <see cref="BookCopy"/>.
/// Ánh xạ với bảng <c>dbo.Books</c>.
/// </summary>
public class Book
{
    /// <summary>Mã đầu sách (khóa chính, tự tăng).</summary>
    public int BookId { get; set; }

    /// <summary>Mã số sách tiêu chuẩn quốc tế (ISBN, tối đa 20 ký tự); <c>null</c> nếu sách không có ISBN.</summary>
    public string? ISBN { get; set; }

    /// <summary>Nhan đề chính của sách.</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>Phụ đề của sách; <c>null</c> nếu sách không có phụ đề.</summary>
    public string? Subtitle { get; set; }

    /// <summary>
    /// Mã nhà xuất bản (khóa ngoại tham chiếu đến <see cref="Publisher"/>);
    /// <c>null</c> nếu chưa xác định nhà xuất bản.
    /// </summary>
    public int? PublisherId { get; set; }

    /// <summary>
    /// Mã ngôn ngữ của sách (khóa ngoại tham chiếu đến <see cref="Language"/>);
    /// <c>null</c> nếu chưa xác định ngôn ngữ.
    /// </summary>
    public int? LanguageId { get; set; }

    /// <summary>Năm xuất bản (trong khoảng 1000 đến 9999); <c>null</c> nếu chưa có thông tin.</summary>
    public short? PublicationYear { get; set; }

    /// <summary>Thông tin lần xuất bản/tái bản (ví dụ: "Tái bản lần 1", "1st Edition").</summary>
    public string? Edition { get; set; }

    /// <summary>Mô tả, tóm tắt nội dung của sách.</summary>
    public string? Description { get; set; }

    /// <summary>Số trang của sách (phải lớn hơn 0 nếu có giá trị).</summary>
    public int? PageCount { get; set; }

    /// <summary>Đường dẫn đến ảnh bìa của sách.</summary>
    public string? CoverImagePath { get; set; }

    /// <summary>
    /// Trạng thái sử dụng: <c>true</c> là đang sử dụng,
    /// <c>false</c> là ngừng sử dụng (xóa mềm).
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>Thời điểm tạo bản ghi.</summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>Thời điểm cập nhật gần nhất; <c>null</c> nếu bản ghi chưa từng được cập nhật.</summary>
    public DateTime? UpdatedAt { get; set; }
}
