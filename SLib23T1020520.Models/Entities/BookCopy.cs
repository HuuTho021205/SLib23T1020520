namespace SLib23T1020520.Models.Entities;

/// <summary>
/// Bản sao sách: một cuốn sách vật lý cụ thể của một đầu sách, có mã vạch,
/// vị trí, tình trạng và trạng thái riêng. Đây là đối tượng được mượn/trả.
/// Ánh xạ với bảng <c>dbo.BookCopies</c>.
/// </summary>
public class BookCopy
{
    /// <summary>Mã bản sao (khóa chính, tự tăng).</summary>
    public int BookCopyId { get; set; }

    /// <summary>Mã đầu sách mà bản sao thuộc về (khóa ngoại tham chiếu đến <see cref="Book"/>).</summary>
    public int BookId { get; set; }

    /// <summary>Mã vạch dán trên cuốn sách, duy nhất (ví dụ: "BC-00001-01").</summary>
    public string Barcode { get; set; } = string.Empty;

    /// <summary>
    /// Mã vị trí lưu trữ hiện tại (khóa ngoại tham chiếu đến <see cref="Location"/>);
    /// <c>null</c> nếu chưa xếp vị trí.
    /// </summary>
    public int? LocationId { get; set; }

    /// <summary>Ngày nhập sách vào thư viện.</summary>
    public DateTime? AcquisitionDate { get; set; }

    /// <summary>Giá nhập (giá mua) của cuốn sách; không âm nếu có giá trị.</summary>
    public decimal? AcquisitionPrice { get; set; }

    /// <summary>
    /// Tình trạng vật lý của cuốn sách, mặc định là "Good". Các giá trị hợp lệ:
    /// "New" (mới), "Good" (tốt), "Fair" (trung bình/cũ), "Damaged" (hư hỏng).
    /// </summary>
    public string CopyCondition { get; set; } = string.Empty;

    /// <summary>
    /// Trạng thái hiện tại của bản sao, mặc định là "Available". Các giá trị hợp lệ:
    /// "Available" (sẵn sàng cho mượn), "OnLoan" (đang được mượn),
    /// "Reserved" (đã được đặt trước), "Lost" (bị mất), "Withdrawn" (đã rút khỏi kho).
    /// </summary>
    public string CopyStatus { get; set; } = string.Empty;

    /// <summary>Ghi chú về bản sao.</summary>
    public string? Notes { get; set; }
}
