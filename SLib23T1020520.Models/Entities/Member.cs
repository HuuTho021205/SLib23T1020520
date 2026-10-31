namespace SLib23T1020520.Models.Entities;

/// <summary>
/// Thành viên (bạn đọc) của thư viện: người đăng ký thẻ và được phép mượn sách.
/// Ánh xạ với bảng <c>dbo.Members</c>.
/// </summary>
public class Member
{
    /// <summary>Mã thành viên (khóa chính, tự tăng).</summary>
    public int MemberId { get; set; }

    /// <summary>Mã số thẻ thành viên, duy nhất (ví dụ: "MB-2025-001").</summary>
    public string MemberCode { get; set; } = string.Empty;

    /// <summary>Mã loại thành viên (khóa ngoại tham chiếu đến <see cref="MemberType"/>).</summary>
    public int MemberTypeId { get; set; }

    /// <summary>Họ và tên đầy đủ của thành viên.</summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>Ngày sinh của thành viên.</summary>
    public DateTime? DateOfBirth { get; set; }

    /// <summary>Giới tính. Các giá trị hợp lệ: "Male" (nam), "Female" (nữ), "Other" (khác).</summary>
    public string? Gender { get; set; }

    /// <summary>Địa chỉ email của thành viên (tối đa 254 ký tự).</summary>
    public string? Email { get; set; }

    /// <summary>Số điện thoại của thành viên.</summary>
    public string? Phone { get; set; }

    /// <summary>Địa chỉ liên hệ của thành viên.</summary>
    public string? Address { get; set; }

    /// <summary>Đơn vị/tổ chức mà thành viên đang học tập hoặc công tác.</summary>
    public string? Organization { get; set; }

    /// <summary>Thời điểm đăng ký làm thành viên.</summary>
    public DateTime RegisteredAt { get; set; }

    /// <summary>
    /// Ngày hết hạn của thẻ thành viên; <c>null</c> nếu thẻ không giới hạn thời hạn.
    /// Nếu có giá trị thì không được sớm hơn ngày đăng ký.
    /// </summary>
    public DateTime? ExpiredAt { get; set; }

    /// <summary>
    /// Trạng thái của thành viên, mặc định là "Active". Các giá trị hợp lệ:
    /// "Active" (đang hoạt động), "Expired" (thẻ hết hạn),
    /// "Suspended" (bị tạm khóa), "Closed" (đã đóng thẻ).
    /// </summary>
    public string MemberStatus { get; set; } = string.Empty;

    /// <summary>Ghi chú về thành viên (ví dụ: lý do bị tạm khóa).</summary>
    public string? Notes { get; set; }
}
