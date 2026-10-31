namespace SLib23T1020520.Models.Entities;

/// <summary>
/// Chi tiết phiếu mượn: từng bản sao sách được mượn trong một phiếu mượn,
/// kèm hạn trả và thông tin trả sách của riêng bản sao đó.
/// Ánh xạ với bảng <c>dbo.LoanItems</c>.
/// </summary>
public class LoanItem
{
    /// <summary>Mã dòng chi tiết phiếu mượn (khóa chính, tự tăng).</summary>
    public int LoanItemId { get; set; }

    /// <summary>Mã phiếu mượn chứa dòng này (khóa ngoại tham chiếu đến <see cref="Loan"/>).</summary>
    public int LoanId { get; set; }

    /// <summary>Mã bản sao sách được mượn (khóa ngoại tham chiếu đến <see cref="BookCopy"/>).</summary>
    public int BookCopyId { get; set; }

    /// <summary>Hạn trả của bản sao sách này.</summary>
    public DateTime DueAt { get; set; }

    /// <summary>
    /// Thời điểm trả sách thực tế (hoặc thời điểm ghi nhận mất sách).
    /// <c>null</c> khi <see cref="ItemStatus"/> là "Borrowed"; bắt buộc có giá trị khi
    /// <see cref="ItemStatus"/> là "Returned" hoặc "Lost".
    /// </summary>
    public DateTime? ReturnedAt { get; set; }

    /// <summary>
    /// Trạng thái của dòng mượn, mặc định là "Borrowed". Các giá trị hợp lệ:
    /// "Borrowed" (đang mượn), "Returned" (đã trả), "Lost" (bị mất).
    /// </summary>
    public string ItemStatus { get; set; } = string.Empty;

    /// <summary>
    /// Tình trạng sách khi trả; <c>null</c> nếu chưa trả. Các giá trị hợp lệ:
    /// "New" (mới), "Good" (tốt), "Fair" (trung bình/cũ), "Damaged" (hư hỏng).
    /// </summary>
    public string? ReturnCondition { get; set; }

    /// <summary>Ghi chú về dòng mượn.</summary>
    public string? Notes { get; set; }
}
