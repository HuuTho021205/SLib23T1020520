namespace SLib23T1020520.Models.Entities;

/// <summary>
/// Nhà xuất bản: đơn vị xuất bản/phát hành các đầu sách có trong thư viện.
/// Ánh xạ với bảng <c>dbo.Publishers</c>.
/// </summary>
public class Publisher
{
    /// <summary>Mã nhà xuất bản (khóa chính, tự tăng).</summary>
    public int PublisherId { get; set; }

    /// <summary>Tên nhà xuất bản.</summary>
    public string PublisherName { get; set; } = string.Empty;

    /// <summary>Địa chỉ trụ sở của nhà xuất bản.</summary>
    public string? Address { get; set; }

    /// <summary>Số điện thoại liên hệ.</summary>
    public string? Phone { get; set; }

    /// <summary>Địa chỉ email liên hệ.</summary>
    public string? Email { get; set; }

    /// <summary>Địa chỉ website của nhà xuất bản.</summary>
    public string? Website { get; set; }

    /// <summary>
    /// Trạng thái sử dụng: <c>true</c> là đang sử dụng,
    /// <c>false</c> là ngừng sử dụng (xóa mềm).
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>Thời điểm tạo bản ghi.</summary>
    public DateTime CreatedAt { get; set; }
}
