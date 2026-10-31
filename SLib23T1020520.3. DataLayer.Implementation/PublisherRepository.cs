using Dapper;
using SLib23T1020520.DataLayer.Interfaces;
using SLib23T1020520.Models.Entities;

namespace SLib23T1020520.DataLayer.SQLServer;

/// <summary>
/// Cài đặt các phép xử lý dữ liệu trên bảng <c>Publishers</c> (nhà xuất bản) bằng Dapper và SQL Server.
/// </summary>
public class PublisherRepository : IPublisherRepository
{
    private readonly DbHelper _db;

    /// <summary>
    /// Khởi tạo repository.
    /// </summary>
    /// <param name="db">Đối tượng cung cấp kết nối đến cơ sở dữ liệu SQL Server.</param>
    public PublisherRepository(DbHelper db)
    {
        _db = db;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Publisher>> ListAsync()
    {
        const string sql = @"SELECT * FROM dbo.Publishers ORDER BY PublisherName";

        using var connection = _db.GetConnection();
        return await connection.QueryAsync<Publisher>(sql);
    }

    /// <inheritdoc/>
    public async Task<Publisher?> GetAsync(int id)
    {
        const string sql = @"SELECT * FROM dbo.Publishers WHERE PublisherId = @id";

        using var connection = _db.GetConnection();
        return await connection.QueryFirstOrDefaultAsync<Publisher>(sql, new { id });
    }

    /// <inheritdoc/>
    public async Task<int> AddAsync(Publisher data)
    {
        // CreatedAt không đưa vào câu lệnh INSERT để CSDL tự gán giá trị mặc định (SYSDATETIME()).
        const string sql = @"
            INSERT INTO dbo.Publishers (PublisherName, Address, Phone, Email, Website, IsActive)
            VALUES (@PublisherName, @Address, @Phone, @Email, @Website, @IsActive);

            SELECT CAST(SCOPE_IDENTITY() AS int);";

        using var connection = _db.GetConnection();
        return await connection.ExecuteScalarAsync<int>(sql, data);
    }

    /// <inheritdoc/>
    public async Task<bool> UpdateAsync(Publisher data)
    {
        const string sql = @"
            UPDATE dbo.Publishers
            SET PublisherName = @PublisherName,
                Address = @Address,
                Phone = @Phone,
                Email = @Email,
                Website = @Website,
                IsActive = @IsActive
            WHERE PublisherId = @PublisherId;";

        using var connection = _db.GetConnection();
        return await connection.ExecuteAsync(sql, data) > 0;
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteAsync(int id)
    {
        const string sql = @"DELETE FROM dbo.Publishers WHERE PublisherId = @id";

        using var connection = _db.GetConnection();
        return await connection.ExecuteAsync(sql, new { id }) > 0;
    }

    /// <inheritdoc/>
    public async Task<bool> InUsedAsync(int id)
    {
        const string sql = @"
            SELECT CASE
                       WHEN EXISTS (SELECT 1 FROM dbo.Books WHERE PublisherId = @id)
                       THEN 1 ELSE 0
                   END;";

        using var connection = _db.GetConnection();
        return await connection.ExecuteScalarAsync<bool>(sql, new { id });
    }
}
