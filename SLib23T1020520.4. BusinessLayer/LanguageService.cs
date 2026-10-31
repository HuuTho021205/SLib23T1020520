using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using SLib23T1020520.DataLayer.Interfaces;
using SLib23T1020520.Models.Entities;

namespace SLib23T1020520.BusinessLayer;

/// <summary>
/// Cung cấp các chức năng nghiệp vụ liên quan đến ngôn ngữ (<see cref="Language"/>):
/// kiểm tra tính hợp lệ của dữ liệu và điều phối các thao tác xử lý dữ liệu với
/// <see cref="ILanguageRepository"/>.
/// <para>
/// Quy ước xử lý lỗi: dữ liệu không hợp lệ hoặc vi phạm quy tắc nghiệp vụ thì ném
/// <see cref="ValidationException"/> với thông điệp tiếng Việt có thể hiển thị trực tiếp cho người dùng;
/// tham số bắt buộc là <c>null</c> thì ném <see cref="ArgumentNullException"/>; các lỗi hệ thống từ tầng dữ liệu
/// được để nguyên. Khi không tìm thấy bản ghi cần cập nhật hoặc cần xóa, <see cref="UpdateAsync(Language)"/>
/// và <see cref="DeleteAsync(int)"/> không ném exception mà trả về <c>false</c>.
/// </para>
/// </summary>
public class LanguageService
{
    // Độ dài tối đa của các cột trong bảng Languages
    private const int LanguageCodeMaxLength = 10;
    private const int LanguageNameMaxLength = 100;

    private readonly ILanguageRepository _repository;

    /// <summary>
    /// Khởi tạo dịch vụ nghiệp vụ ngôn ngữ.
    /// </summary>
    /// <param name="repository">Đối tượng thực hiện các phép xử lý dữ liệu trên bảng ngôn ngữ.</param>
    /// <exception cref="ArgumentNullException"><paramref name="repository"/> là <c>null</c>.</exception>
    public LanguageService(ILanguageRepository repository)
    {
        ArgumentNullException.ThrowIfNull(repository);
        _repository = repository;
    }

    /// <summary>
    /// Lấy danh sách toàn bộ ngôn ngữ.
    /// </summary>
    /// <returns>Tập hợp các ngôn ngữ; tập hợp rỗng nếu chưa có dữ liệu.</returns>
    public async Task<IEnumerable<Language>> ListAsync()
    {
        return await _repository.ListAsync();
    }

    /// <summary>
    /// Lấy thông tin của một ngôn ngữ theo mã.
    /// </summary>
    /// <param name="id">Mã ngôn ngữ (phải lớn hơn 0).</param>
    /// <returns>Bản ghi tìm được; <c>null</c> nếu không tồn tại bản ghi có mã tương ứng.</returns>
    /// <exception cref="ValidationException"><paramref name="id"/> không hợp lệ.</exception>
    public async Task<Language?> GetAsync(int id)
    {
        ValidateId(id);
        return await _repository.GetAsync(id);
    }

    /// <summary>
    /// Bổ sung một ngôn ngữ mới.
    /// </summary>
    /// <param name="data">
    /// Thông tin cần bổ sung. Dữ liệu được chuẩn hóa trước khi lưu (đối tượng <paramref name="data"/> bị thay đổi
    /// trực tiếp). Bản ghi mới luôn được tạo ở trạng thái đang sử dụng (<c>IsActive = true</c>).
    /// </param>
    /// <returns>Mã của bản ghi vừa được bổ sung.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="data"/> là <c>null</c>.</exception>
    /// <exception cref="ValidationException">Dữ liệu không hợp lệ.</exception>
    public async Task<int> AddAsync(Language data)
    {
        ArgumentNullException.ThrowIfNull(data);

        Normalize(data);
        await ValidateDataAsync(data);
        data.IsActive = true;

        return await _repository.AddAsync(data);
    }

    /// <summary>
    /// Cập nhật thông tin của một ngôn ngữ đã có.
    /// </summary>
    /// <param name="data">
    /// Thông tin mới; bản ghi cần cập nhật được xác định bằng <c>LanguageId</c>.
    /// Dữ liệu được chuẩn hóa trước khi lưu (đối tượng <paramref name="data"/> bị thay đổi trực tiếp).
    /// </param>
    /// <returns>
    /// <c>true</c> nếu cập nhật thành công; <c>false</c> nếu không tìm thấy bản ghi cần cập nhật.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="data"/> là <c>null</c>.</exception>
    /// <exception cref="ValidationException">Mã không hợp lệ hoặc dữ liệu không hợp lệ.</exception>
    public async Task<bool> UpdateAsync(Language data)
    {
        ArgumentNullException.ThrowIfNull(data);

        ValidateId(data.LanguageId);
        Normalize(data);
        await ValidateDataAsync(data);

        return await _repository.UpdateAsync(data);
    }

    /// <summary>
    /// Xóa một ngôn ngữ theo mã. Chỉ được xóa khi bản ghi chưa được sử dụng ở nơi khác (gắn với đầu sách nào).
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
            throw new ValidationException("Không thể xóa ngôn ngữ này vì đang có sách sử dụng.");

        return await _repository.DeleteAsync(id);
    }

    /// <summary>
    /// Kiểm tra xem một ngôn ngữ có đang được sử dụng (gắn với đầu sách nào) hay không.
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

    /// <summary>Kiểm tra mã ngôn ngữ hợp lệ (lớn hơn 0).</summary>
    /// <exception cref="ValidationException">Mã không hợp lệ.</exception>
    private static void ValidateId(int id)
    {
        if (id <= 0)
            throw new ValidationException("Mã ngôn ngữ không hợp lệ.");
    }

    /// <summary>
    /// Chuẩn hóa dữ liệu trước khi kiểm tra và lưu: cắt khoảng trắng thừa;
    /// mã ngôn ngữ được viết hoa toàn bộ (ví dụ "vie" thành "VIE").
    /// </summary>
    private static void Normalize(Language data)
    {
        data.LanguageCode = (data.LanguageCode ?? string.Empty).Trim().ToUpperInvariant();
        data.LanguageName = (data.LanguageName ?? string.Empty).Trim();
    }

    /// <summary>
    /// Kiểm tra tính hợp lệ của dữ liệu ngôn ngữ (đã được chuẩn hóa), phù hợp với các ràng buộc của CSDL.
    /// Việc kiểm tra trùng mã chỉ để báo lỗi sớm bằng thông điệp thân thiện; ràng buộc UNIQUE
    /// trong CSDL vẫn là chốt chặn cuối cùng.
    /// </summary>
    /// <exception cref="ValidationException">Dữ liệu không hợp lệ.</exception>
    private async Task ValidateDataAsync(Language data)
    {
        if (data.LanguageCode.Length == 0)
            throw new ValidationException("Mã ngôn ngữ không được để trống.");

        if (data.LanguageCode.Length > LanguageCodeMaxLength)
            throw new ValidationException($"Mã ngôn ngữ không được vượt quá {LanguageCodeMaxLength} ký tự.");

        if (!Regex.IsMatch(data.LanguageCode, "^[A-Z0-9_-]+$"))
            throw new ValidationException("Mã ngôn ngữ chỉ được gồm chữ cái không dấu, chữ số, dấu gạch ngang và gạch dưới.");

        if (data.LanguageName.Length == 0)
            throw new ValidationException("Tên ngôn ngữ không được để trống.");

        if (data.LanguageName.Length > LanguageNameMaxLength)
            throw new ValidationException($"Tên ngôn ngữ không được vượt quá {LanguageNameMaxLength} ký tự.");

        var all = await _repository.ListAsync();
        bool duplicated = all.Any(x => x.LanguageId != data.LanguageId
                                       && string.Equals(x.LanguageCode, data.LanguageCode, StringComparison.OrdinalIgnoreCase));
        if (duplicated)
            throw new ValidationException("Mã ngôn ngữ đã tồn tại.");
    }
}
