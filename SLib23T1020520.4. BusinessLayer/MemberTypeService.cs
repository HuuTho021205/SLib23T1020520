using System.ComponentModel.DataAnnotations;
using SLib23T1020520.DataLayer.Interfaces;
using SLib23T1020520.Models.Entities;

namespace SLib23T1020520.BusinessLayer;

/// <summary>
/// Cung cấp các chức năng nghiệp vụ liên quan đến loại thành viên (<see cref="MemberType"/>):
/// kiểm tra tính hợp lệ của dữ liệu và điều phối các thao tác xử lý dữ liệu với
/// <see cref="IMemberTypeRepository"/>.
/// <para>
/// Quy ước xử lý lỗi: dữ liệu không hợp lệ hoặc vi phạm quy tắc nghiệp vụ thì ném
/// <see cref="ValidationException"/> với thông điệp tiếng Việt có thể hiển thị trực tiếp cho người dùng;
/// tham số bắt buộc là <c>null</c> thì ném <see cref="ArgumentNullException"/>; các lỗi hệ thống từ tầng dữ liệu
/// được để nguyên. Khi không tìm thấy bản ghi cần cập nhật hoặc cần xóa, <see cref="UpdateAsync(MemberType)"/>
/// và <see cref="DeleteAsync(int)"/> không ném exception mà trả về <c>false</c>.
/// </para>
/// </summary>
public class MemberTypeService
{
    // Độ dài tối đa của các cột trong bảng MemberTypes
    private const int TypeNameMaxLength = 100;

    private readonly IMemberTypeRepository _repository;

    /// <summary>
    /// Khởi tạo dịch vụ nghiệp vụ loại thành viên.
    /// </summary>
    /// <param name="repository">Đối tượng thực hiện các phép xử lý dữ liệu trên bảng loại thành viên.</param>
    /// <exception cref="ArgumentNullException"><paramref name="repository"/> là <c>null</c>.</exception>
    public MemberTypeService(IMemberTypeRepository repository)
    {
        ArgumentNullException.ThrowIfNull(repository);
        _repository = repository;
    }

    /// <summary>
    /// Lấy danh sách toàn bộ loại thành viên.
    /// </summary>
    /// <returns>Tập hợp các loại thành viên; tập hợp rỗng nếu chưa có dữ liệu.</returns>
    public async Task<IEnumerable<MemberType>> ListAsync()
    {
        return await _repository.ListAsync();
    }

    /// <summary>
    /// Lấy thông tin của một loại thành viên theo mã.
    /// </summary>
    /// <param name="id">Mã loại thành viên (phải lớn hơn 0).</param>
    /// <returns>Bản ghi tìm được; <c>null</c> nếu không tồn tại bản ghi có mã tương ứng.</returns>
    /// <exception cref="ValidationException"><paramref name="id"/> không hợp lệ.</exception>
    public async Task<MemberType?> GetAsync(int id)
    {
        ValidateId(id);
        return await _repository.GetAsync(id);
    }

    /// <summary>
    /// Bổ sung một loại thành viên mới.
    /// </summary>
    /// <param name="data">
    /// Thông tin cần bổ sung. Dữ liệu được chuẩn hóa trước khi lưu (đối tượng <paramref name="data"/> bị thay đổi
    /// trực tiếp). Bản ghi mới luôn được tạo ở trạng thái đang sử dụng (<c>IsActive = true</c>).
    /// </param>
    /// <returns>Mã của bản ghi vừa được bổ sung.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="data"/> là <c>null</c>.</exception>
    /// <exception cref="ValidationException">Dữ liệu không hợp lệ.</exception>
    public async Task<int> AddAsync(MemberType data)
    {
        ArgumentNullException.ThrowIfNull(data);

        Normalize(data);
        await ValidateDataAsync(data);
        data.IsActive = true;

        return await _repository.AddAsync(data);
    }

    /// <summary>
    /// Cập nhật thông tin của một loại thành viên đã có.
    /// </summary>
    /// <param name="data">
    /// Thông tin mới; bản ghi cần cập nhật được xác định bằng <c>MemberTypeId</c>.
    /// Dữ liệu được chuẩn hóa trước khi lưu (đối tượng <paramref name="data"/> bị thay đổi trực tiếp).
    /// </param>
    /// <returns>
    /// <c>true</c> nếu cập nhật thành công; <c>false</c> nếu không tìm thấy bản ghi cần cập nhật.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="data"/> là <c>null</c>.</exception>
    /// <exception cref="ValidationException">Mã không hợp lệ hoặc dữ liệu không hợp lệ.</exception>
    public async Task<bool> UpdateAsync(MemberType data)
    {
        ArgumentNullException.ThrowIfNull(data);

        ValidateId(data.MemberTypeId);
        Normalize(data);
        await ValidateDataAsync(data);

        return await _repository.UpdateAsync(data);
    }

    /// <summary>
    /// Xóa một loại thành viên theo mã. Chỉ được xóa khi bản ghi chưa được sử dụng ở nơi khác (có thành viên nào thuộc loại này).
    /// </summary>
    /// <param name="id">Mã cần xóa (phải lớn hơn 0).</param>
    /// <returns>
    /// <c>true</c> nếu xóa thành công; <c>false</c> nếu không tìm thấy bản ghi cần xóa.
    /// </returns>
    /// <exception cref="ValidationException">Mã không hợp lệ hoặc bản ghi đang được sử dụng.</exception>
    public async Task<bool> DeleteAsync(int id)
    {
        ValidateId(id);

        if (await _repository.InUsedAsync(id))
            throw new ValidationException("Không thể xóa loại thành viên này vì đang có thành viên thuộc loại này.");

        return await _repository.DeleteAsync(id);
    }

    /// <summary>
    /// Kiểm tra xem một loại thành viên có đang được sử dụng (có thành viên nào thuộc loại này) hay không.
    /// </summary>
    /// <param name="id">Mã cần kiểm tra (phải lớn hơn 0).</param>
    /// <returns><c>true</c> nếu đang được sử dụng (không thể xóa); ngược lại là <c>false</c>.</returns>
    /// <exception cref="ValidationException"><paramref name="id"/> không hợp lệ.</exception>
    public async Task<bool> InUsedAsync(int id)
    {
        ValidateId(id);
        return await _repository.InUsedAsync(id);
    }

    // ---------------------------------------------------------------------
    // Các hàm hỗ trợ kiểm tra và chuẩn hóa dữ liệu
    // ---------------------------------------------------------------------

    /// <summary>Kiểm tra mã loại thành viên hợp lệ (lớn hơn 0).</summary>
    /// <exception cref="ValidationException">Mã không hợp lệ.</exception>
    private static void ValidateId(int id)
    {
        if (id <= 0)
            throw new ValidationException("Mã loại thành viên không hợp lệ.");
    }

    /// <summary>
    /// Chuẩn hóa dữ liệu trước khi kiểm tra và lưu: cắt khoảng trắng thừa của tên loại thành viên.
    /// </summary>
    private static void Normalize(MemberType data)
    {
        data.TypeName = (data.TypeName ?? string.Empty).Trim();
    }

    /// <summary>
    /// Kiểm tra tính hợp lệ của dữ liệu loại thành viên (đã được chuẩn hóa), phù hợp với các ràng buộc của CSDL.
    /// Việc kiểm tra trùng tên chỉ để báo lỗi sớm bằng thông điệp thân thiện; ràng buộc UNIQUE
    /// trong CSDL vẫn là chốt chặn cuối cùng.
    /// </summary>
    /// <exception cref="ValidationException">Dữ liệu không hợp lệ.</exception>
    private async Task ValidateDataAsync(MemberType data)
    {
        if (data.TypeName.Length == 0)
            throw new ValidationException("Tên loại thành viên không được để trống.");

        if (data.TypeName.Length > TypeNameMaxLength)
            throw new ValidationException($"Tên loại thành viên không được vượt quá {TypeNameMaxLength} ký tự.");

        if (data.MaxActiveLoans < 0)
            throw new ValidationException("Số sách được mượn tối đa không được âm.");

        if (data.LoanPeriodDays <= 0)
            throw new ValidationException("Số ngày được mượn phải lớn hơn 0.");

        if (data.MaxRenewals < 0)
            throw new ValidationException("Số lần gia hạn tối đa không được âm.");

        var all = await _repository.ListAsync();
        bool duplicated = all.Any(x => x.MemberTypeId != data.MemberTypeId
                                       && string.Equals(x.TypeName, data.TypeName, StringComparison.OrdinalIgnoreCase));
        if (duplicated)
            throw new ValidationException("Tên loại thành viên đã tồn tại.");
    }
}
