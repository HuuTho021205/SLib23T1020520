namespace SLib23T1020520.Models.Constants;

/// <summary>
/// Tập hợp các hằng số tương ứng với các giá trị cố định được quy định (bằng ràng buộc
/// <c>CHECK</c> hoặc giá trị mặc định) cho các cột trong cơ sở dữ liệu <c>LibraryManagementDB</c>.
/// <para>
/// Quy tắc đặt tên hằng: <c>TENBANG_TENCOT_GIATRI</c> (viết hoa toàn bộ),
/// ví dụ <see cref="BOOKCOPIES_COPYCONDITION_NEW"/>.
/// </para>
/// </summary>
public static class DatabaseConstant
{
    // =====================================================================
    // BẢNG BookCopies
    // =====================================================================

    #region BookCopies.CopyCondition - Tình trạng vật lý của bản sao sách

    /// <summary>Bản sao sách còn mới (giá trị "New" của cột <c>BookCopies.CopyCondition</c>).</summary>
    public const string BOOKCOPIES_COPYCONDITION_NEW = "New";

    /// <summary>
    /// Bản sao sách ở tình trạng tốt (giá trị "Good" của cột <c>BookCopies.CopyCondition</c>).
    /// Đây là giá trị mặc định của cột.
    /// </summary>
    public const string BOOKCOPIES_COPYCONDITION_GOOD = "Good";

    /// <summary>Bản sao sách ở tình trạng trung bình, đã cũ hoặc mòn (giá trị "Fair" của cột <c>BookCopies.CopyCondition</c>).</summary>
    public const string BOOKCOPIES_COPYCONDITION_FAIR = "Fair";

    /// <summary>Bản sao sách bị hư hỏng (giá trị "Damaged" của cột <c>BookCopies.CopyCondition</c>).</summary>
    public const string BOOKCOPIES_COPYCONDITION_DAMAGED = "Damaged";

    #endregion

    #region BookCopies.CopyStatus - Trạng thái hiện tại của bản sao sách

    /// <summary>
    /// Bản sao sách sẵn sàng cho mượn (giá trị "Available" của cột <c>BookCopies.CopyStatus</c>).
    /// Đây là giá trị mặc định của cột.
    /// </summary>
    public const string BOOKCOPIES_COPYSTATUS_AVAILABLE = "Available";

    /// <summary>Bản sao sách đang được mượn (giá trị "OnLoan" của cột <c>BookCopies.CopyStatus</c>).</summary>
    public const string BOOKCOPIES_COPYSTATUS_ONLOAN = "OnLoan";

    /// <summary>Bản sao sách đã được đặt trước (giá trị "Reserved" của cột <c>BookCopies.CopyStatus</c>).</summary>
    public const string BOOKCOPIES_COPYSTATUS_RESERVED = "Reserved";

    /// <summary>Bản sao sách bị mất (giá trị "Lost" của cột <c>BookCopies.CopyStatus</c>).</summary>
    public const string BOOKCOPIES_COPYSTATUS_LOST = "Lost";

    /// <summary>
    /// Bản sao sách đã được rút khỏi kho, không còn lưu thông
    /// (giá trị "Withdrawn" của cột <c>BookCopies.CopyStatus</c>).
    /// </summary>
    public const string BOOKCOPIES_COPYSTATUS_WITHDRAWN = "Withdrawn";

    #endregion

    // =====================================================================
    // BẢNG LoanItems
    // =====================================================================

    #region LoanItems.ItemStatus - Trạng thái của từng dòng mượn

    /// <summary>
    /// Bản sao sách đang được mượn, chưa trả (giá trị "Borrowed" của cột <c>LoanItems.ItemStatus</c>).
    /// Đây là giá trị mặc định của cột; khi đó <c>ReturnedAt</c> phải là <c>null</c>.
    /// </summary>
    public const string LOANITEMS_ITEMSTATUS_BORROWED = "Borrowed";

    /// <summary>
    /// Bản sao sách đã được trả (giá trị "Returned" của cột <c>LoanItems.ItemStatus</c>).
    /// Khi đó <c>ReturnedAt</c> bắt buộc có giá trị.
    /// </summary>
    public const string LOANITEMS_ITEMSTATUS_RETURNED = "Returned";

    /// <summary>
    /// Bản sao sách bị mất trong thời gian mượn (giá trị "Lost" của cột <c>LoanItems.ItemStatus</c>).
    /// Khi đó <c>ReturnedAt</c> bắt buộc có giá trị (thời điểm ghi nhận mất sách).
    /// </summary>
    public const string LOANITEMS_ITEMSTATUS_LOST = "Lost";

    #endregion

    #region LoanItems.ReturnCondition - Tình trạng sách khi trả

    /// <summary>Sách còn mới khi trả (giá trị "New" của cột <c>LoanItems.ReturnCondition</c>).</summary>
    public const string LOANITEMS_RETURNCONDITION_NEW = "New";

    /// <summary>Sách ở tình trạng tốt khi trả (giá trị "Good" của cột <c>LoanItems.ReturnCondition</c>).</summary>
    public const string LOANITEMS_RETURNCONDITION_GOOD = "Good";

    /// <summary>Sách ở tình trạng trung bình, cũ hoặc mòn khi trả (giá trị "Fair" của cột <c>LoanItems.ReturnCondition</c>).</summary>
    public const string LOANITEMS_RETURNCONDITION_FAIR = "Fair";

    /// <summary>Sách bị hư hỏng khi trả (giá trị "Damaged" của cột <c>LoanItems.ReturnCondition</c>).</summary>
    public const string LOANITEMS_RETURNCONDITION_DAMAGED = "Damaged";

    #endregion

    // =====================================================================
    // BẢNG Loans
    // =====================================================================

    #region Loans.LoanStatus - Trạng thái của phiếu mượn

    /// <summary>
    /// Phiếu mượn đang hoạt động, còn sách chưa trả (giá trị "Active" của cột <c>Loans.LoanStatus</c>).
    /// Đây là giá trị mặc định của cột.
    /// </summary>
    public const string LOANS_LOANSTATUS_ACTIVE = "Active";

    /// <summary>Phiếu mượn đã hoàn tất, các sách đã được trả đầy đủ (giá trị "Completed" của cột <c>Loans.LoanStatus</c>).</summary>
    public const string LOANS_LOANSTATUS_COMPLETED = "Completed";

    /// <summary>Phiếu mượn đã bị hủy (giá trị "Cancelled" của cột <c>Loans.LoanStatus</c>).</summary>
    public const string LOANS_LOANSTATUS_CANCELLED = "Cancelled";

    #endregion

    // =====================================================================
    // BẢNG Members
    // =====================================================================

    #region Members.Gender - Giới tính của thành viên

    /// <summary>Giới tính nam (giá trị "Male" của cột <c>Members.Gender</c>).</summary>
    public const string MEMBERS_GENDER_MALE = "Male";

    /// <summary>Giới tính nữ (giá trị "Female" của cột <c>Members.Gender</c>).</summary>
    public const string MEMBERS_GENDER_FEMALE = "Female";

    /// <summary>Giới tính khác (giá trị "Other" của cột <c>Members.Gender</c>).</summary>
    public const string MEMBERS_GENDER_OTHER = "Other";

    #endregion

    #region Members.MemberStatus - Trạng thái của thành viên

    /// <summary>
    /// Thành viên đang hoạt động, được phép mượn sách (giá trị "Active" của cột <c>Members.MemberStatus</c>).
    /// Đây là giá trị mặc định của cột.
    /// </summary>
    public const string MEMBERS_MEMBERSTATUS_ACTIVE = "Active";

    /// <summary>Thẻ thành viên đã hết hạn (giá trị "Expired" của cột <c>Members.MemberStatus</c>).</summary>
    public const string MEMBERS_MEMBERSTATUS_EXPIRED = "Expired";

    /// <summary>Thành viên bị tạm khóa, không được mượn sách (giá trị "Suspended" của cột <c>Members.MemberStatus</c>).</summary>
    public const string MEMBERS_MEMBERSTATUS_SUSPENDED = "Suspended";

    /// <summary>Thẻ thành viên đã được đóng (giá trị "Closed" của cột <c>Members.MemberStatus</c>).</summary>
    public const string MEMBERS_MEMBERSTATUS_CLOSED = "Closed";

    #endregion
}
