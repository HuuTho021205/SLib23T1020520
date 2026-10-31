namespace SLib23T1020520.Models.Entities;

/// <summary>
/// Loại thành viên (bạn đọc), quy định các giới hạn mượn sách áp dụng cho
/// từng nhóm đối tượng (ví dụ: Sinh viên, Giảng viên, Khách đọc ngoại vi).
/// Ánh xạ với bảng <c>dbo.MemberTypes</c>.
/// </summary>
public class MemberType
{
    /// <summary>Mã loại thành viên (khóa chính, tự tăng).</summary>
    public int MemberTypeId { get; set; }

    /// <summary>Tên loại thành viên, duy nhất.</summary>
    public string TypeName { get; set; } = string.Empty;

    /// <summary>Số cuốn sách tối đa được mượn cùng lúc (chưa trả); không âm (mặc định 3).</summary>
    public int MaxActiveLoans { get; set; }

    /// <summary>Số ngày được mượn cho mỗi lần mượn (dùng để tính hạn trả); phải lớn hơn 0 (mặc định 14).</summary>
    public int LoanPeriodDays { get; set; }

    /// <summary>Số lần gia hạn tối đa cho mỗi lượt mượn; không âm, 0 là không được gia hạn (mặc định 0).</summary>
    public int MaxRenewals { get; set; }

    /// <summary>
    /// Trạng thái sử dụng: <c>true</c> là đang sử dụng,
    /// <c>false</c> là ngừng sử dụng (xóa mềm).
    /// </summary>
    public bool IsActive { get; set; }
}
