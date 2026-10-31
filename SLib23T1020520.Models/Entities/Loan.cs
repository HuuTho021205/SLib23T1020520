namespace SLib23T1020520.Models.Entities;

/// <summary>
/// Phiếu mượn sách: một lượt mượn của một thành viên do một nhân viên lập.
/// Mỗi phiếu gồm một hoặc nhiều dòng chi tiết <see cref="LoanItem"/>.
/// Ánh xạ với bảng <c>dbo.Loans</c>.
/// </summary>
public class Loan
{
    /// <summary>Mã phiếu mượn (khóa chính, tự tăng).</summary>
    public int LoanId { get; set; }

    /// <summary>Số phiếu mượn, duy nhất (ví dụ: "LN-202603-001").</summary>
    public string LoanCode { get; set; } = string.Empty;

    /// <summary>Mã thành viên mượn sách (khóa ngoại tham chiếu đến <see cref="Member"/>).</summary>
    public int MemberId { get; set; }

    /// <summary>
    /// Mã nhân viên lập phiếu (khóa ngoại tham chiếu đến <see cref="Employee"/>);
    /// <c>null</c> nếu không xác định được nhân viên lập phiếu.
    /// </summary>
    public int? EmployeeId { get; set; }

    /// <summary>Thời điểm mượn sách (thời điểm lập phiếu).</summary>
    public DateTime BorrowedAt { get; set; }

    /// <summary>
    /// Trạng thái của phiếu mượn, mặc định là "Active". Các giá trị hợp lệ:
    /// "Active" (đang mượn), "Completed" (đã trả đầy đủ), "Cancelled" (đã hủy).
    /// </summary>
    public string LoanStatus { get; set; } = string.Empty;

    /// <summary>Ghi chú về phiếu mượn.</summary>
    public string? Notes { get; set; }
}
