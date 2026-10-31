namespace SLib23T1020520.Models.Entities;

/// <summary>
/// Nhân viên thư viện (thủ thư): người dùng hệ thống, đăng nhập để thực hiện
/// các nghiệp vụ như lập phiếu mượn, nhận trả sách.
/// Ánh xạ với bảng <c>dbo.Employees</c>.
/// </summary>
public class Employee
{
    /// <summary>Mã nhân viên (khóa chính, tự tăng).</summary>
    public int EmployeeId { get; set; }

    /// <summary>Tên của nhân viên (bao gồm tên đệm và tên, ví dụ "Văn A").</summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>Họ của nhân viên (ví dụ: "Nguyễn").</summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>Ngày sinh của nhân viên.</summary>
    public DateTime? DateOfBirth { get; set; }

    /// <summary>Địa chỉ email của nhân viên, dùng để liên hệ và đăng nhập.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Số điện thoại của nhân viên.</summary>
    public string? Phone { get; set; }

    /// <summary>Địa chỉ liên hệ của nhân viên.</summary>
    public string? Address { get; set; }

    /// <summary>
    /// Mật khẩu đăng nhập đã được mã hóa (hash); không lưu mật khẩu dạng văn bản thô.
    /// <c>null</c> nếu nhân viên chưa được thiết lập mật khẩu.
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// Tình trạng làm việc: <c>true</c> là đang làm việc,
    /// <c>false</c> là đã nghỉ việc (không được phép đăng nhập).
    /// </summary>
    public bool IsWorking { get; set; }
}
