using Dapper;
using SLib23T1020520.DataLayer.Interfaces;
using SLib23T1020520.Models.Entities;

namespace SLib23T1020520.DataLayer.SQLServer;

/// <summary>
/// Cài đặt các phép xử lý dữ liệu trên bảng <c>Authors</c> (tác giả) bằng Dapper và SQL Server.
/// </summary>
public class AuthorRepository : IAuthorRepository
{
    private readonly DbHelper _db;

    /// <summary>
    /// Khởi tạo repository.
    /// </summary>
    /// <param name="db">Đối tượng cung cấp kết nối đến cơ sở dữ liệu SQL Server.</param>
    public AuthorRepository(DbHelper db)
    {
        _db = db;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Author>> ListAsync()
    {
        const string sql = @"SELECT * FROM dbo.Authors ORDER BY AuthorName";

        using var connection = _db.GetConnection();
        return await connection.QueryAsync<Author>(sql);
    }

    /// <inheritdoc/>
    public async Task<Author?> GetAsync(int id)
    {
        const string sql = @"SELECT * FROM dbo.Authors WHERE AuthorId = @id";

        using var connection = _db.GetConnection();
        return await connection.QueryFirstOrDefaultAsync<Author>(sql, new { id });
    }

    /// <inheritdoc/>
    public async Task<int> AddAsync(Author data)
    {
        const string sql = @"
            INSERT INTO dbo.Authors (AuthorName, Biography, BirthDate, DeathDate, IsActive)
            VALUES (@AuthorName, @Biography, @BirthDate, @DeathDate, @IsActive);

            SELECT CAST(SCOPE_IDENTITY() AS int);";

        using var connection = _db.GetConnection();
        return await connection.ExecuteScalarAsync<int>(sql, data);
    }

    /// <inheritdoc/>
    public async Task<bool> UpdateAsync(Author data)
    {
        const string sql = @"
            UPDATE dbo.Authors
            SET AuthorName = @AuthorName,
                Biography = @Biography,
                BirthDate = @BirthDate,
                DeathDate = @DeathDate,
                IsActive = @IsActive
            WHERE AuthorId = @AuthorId;";

        using var connection = _db.GetConnection();
        return await connection.ExecuteAsync(sql, data) > 0;
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteAsync(int id)
    {
        const string sql = @"DELETE FROM dbo.Authors WHERE AuthorId = @id";

        using var connection = _db.GetConnection();
        return await connection.ExecuteAsync(sql, new { id }) > 0;
    }

    /// <inheritdoc/>
    public async Task<bool> InUsedAsync(int id)
    {
        const string sql = @"
            SELECT CASE
                       WHEN EXISTS (SELECT 1 FROM dbo.BookAuthors WHERE AuthorId = @id)
                       THEN 1 ELSE 0
                   END;";

        using var connection = _db.GetConnection();
        return await connection.ExecuteScalarAsync<bool>(sql, new { id });
    }
}
