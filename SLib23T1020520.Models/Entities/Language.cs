namespace SLib23T1020520.Models.Entities;

/// <summary>
/// Ngôn ngữ của sách (ví dụ: Tiếng Việt, Tiếng Anh, Tiếng Nhật).
/// Ánh xạ với bảng <c>dbo.Languages</c>.
/// </summary>
public class Language
{
    /// <summary>Mã ngôn ngữ (khóa chính, tự tăng).</summary>
    public int LanguageId { get; set; }

    /// <summary>Mã viết tắt của ngôn ngữ, duy nhất, tối đa 10 ký tự (ví dụ: "VIE", "ENG", "JPN").</summary>
    public string LanguageCode { get; set; } = string.Empty;

    /// <summary>Tên đầy đủ của ngôn ngữ.</summary>
    public string LanguageName { get; set; } = string.Empty;

    /// <summary>
    /// Trạng thái sử dụng: <c>true</c> là đang sử dụng,
    /// <c>false</c> là ngừng sử dụng (xóa mềm).
    /// </summary>
    public bool IsActive { get; set; }
}
