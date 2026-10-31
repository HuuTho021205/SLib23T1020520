namespace SLib23T1020520.Models.Entities;

/// <summary>
/// Vị trí lưu trữ sách trong thư viện (tầng, phòng, kệ).
/// Ánh xạ với bảng <c>dbo.Locations</c>.
/// </summary>
public class Location
{
    /// <summary>Mã vị trí (khóa chính, tự tăng).</summary>
    public int LocationId { get; set; }

    /// <summary>Mã định danh vị trí, duy nhất (ví dụ: "LOC-F1-R1-S1").</summary>
    public string LocationCode { get; set; } = string.Empty;

    /// <summary>Tên vị trí (ví dụ: "Kệ Sách Văn Học A1").</summary>
    public string LocationName { get; set; } = string.Empty;

    /// <summary>Tầng nơi đặt vị trí (ví dụ: "Tầng 1").</summary>
    public string? Floor { get; set; }

    /// <summary>Phòng nơi đặt vị trí (ví dụ: "Phòng Đọc 1").</summary>
    public string? Room { get; set; }

    /// <summary>Kệ sách (ví dụ: "Kệ A1").</summary>
    public string? Shelf { get; set; }

    /// <summary>Mô tả thêm về vị trí, ví dụ loại sách được xếp ở đây.</summary>
    public string? Description { get; set; }

    /// <summary>
    /// Trạng thái sử dụng: <c>true</c> là đang sử dụng,
    /// <c>false</c> là ngừng sử dụng (xóa mềm).
    /// </summary>
    public bool IsActive { get; set; }
}
