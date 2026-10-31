using Dapper;
using SLib23T1020520.DataLayer.Interfaces;
using SLib23T1020520.Models.Entities;

namespace SLib23T1020520.DataLayer.SQLServer;

/// <summary>
/// Cài đặt các phép xử lý dữ liệu trên bảng <c>Languages</c> (ngôn ngữ) bằng Dapper và SQL Server.
/// </summary>
public class LanguageRepository : ILanguageRepository
{
    private readonly DbHelper _db;

    /// <summary>
    /// Khởi tạo repository.
    /// </summary>
    /// <param name="db">Đối tượng cung cấp kết nối đến cơ sở dữ liệu SQL Server.</param>
    public LanguageRepository(DbHelper db)
    {
        _db = db;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Language>> ListAsync()
    {
        const string sql = @"SELECT * FROM dbo.Languages ORDER BY LanguageName";

        using var connection = _db.GetConnection();
        return await connection.QueryAsync<Language>(sql);
    }

    /// <inheritdoc/>
    public async Task<Language?> GetAsync(int id)
    {
        const string sql = @"SELECT * FROM dbo.Languages WHERE LanguageId = @id";

        using var connection = _db.GetConnection();
        return await connection.QueryFirstOrDefaultAsync<Language>(sql, new { id });
    }

    /// <inheritdoc/>
    public async Task<int> AddAsync(Language data)
    {
        const string sql = @"
            INSERT INTO dbo.Languages (LanguageCode, LanguageName, IsActive)
            VALUES (@LanguageCode, @LanguageName, @IsActive);

            SELECT CAST(SCOPE_IDENTITY() AS int);";

        using var connection = _db.GetConnection();
        return await connection.ExecuteScalarAsync<int>(sql, data);
    }

    /// <inheritdoc/>
    public async Task<bool> UpdateAsync(Language data)
    {
        const string sql = @"
            UPDATE dbo.Languages
            SET LanguageCode = @LanguageCode,
                LanguageName = @LanguageName,
                IsActive = @IsActive
            WHERE LanguageId = @LanguageId;";

        using var connection = _db.GetConnection();
        return await connection.ExecuteAsync(sql, data) > 0;
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteAsync(int id)
    {
        const string sql = @"DELETE FROM dbo.Languages WHERE LanguageId = @id";

        using var connection = _db.GetConnection();
        return await connection.ExecuteAsync(sql, new { id }) > 0;
    }

    /// <inheritdoc/>
    public async Task<bool> InUsedAsync(int id)
    {
        const string sql = @"
            SELECT CASE
                       WHEN EXISTS (SELECT 1 FROM dbo.Books WHERE LanguageId = @id)
                       THEN 1 ELSE 0
                   END;";

        using var connection = _db.GetConnection();
        return await connection.ExecuteScalarAsync<bool>(sql, new { id });
    }
}
