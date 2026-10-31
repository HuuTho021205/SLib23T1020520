using Dapper;
using SLib23T1020520.DataLayer.Interfaces;
using SLib23T1020520.Models.Entities;

namespace SLib23T1020520.DataLayer.SQLServer;

/// <summary>
/// Cài đặt các phép xử lý dữ liệu trên bảng <c>Categories</c> (thể loại sách) bằng Dapper và SQL Server.
/// </summary>
public class CategoryRepository : ICategoryRepository
{
    private readonly DbHelper _db;

    /// <summary>
    /// Khởi tạo repository.
    /// </summary>
    /// <param name="db">Đối tượng cung cấp kết nối đến cơ sở dữ liệu SQL Server.</param>
    public CategoryRepository(DbHelper db)
    {
        _db = db;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Category>> ListAsync()
    {
        const string sql = @"SELECT * FROM dbo.Categories ORDER BY CategoryName";

        using var connection = _db.GetConnection();
        return await connection.QueryAsync<Category>(sql);
    }

    /// <inheritdoc/>
    public async Task<Category?> GetAsync(int id)
    {
        const string sql = @"SELECT * FROM dbo.Categories WHERE CategoryId = @id";

        using var connection = _db.GetConnection();
        return await connection.QueryFirstOrDefaultAsync<Category>(sql, new { id });
    }

    /// <inheritdoc/>
    public async Task<int> AddAsync(Category data)
    {
        const string sql = @"
            INSERT INTO dbo.Categories (CategoryName, ParentCategoryId, Description, IsActive)
            VALUES (@CategoryName, @ParentCategoryId, @Description, @IsActive);

            SELECT CAST(SCOPE_IDENTITY() AS int);";

        using var connection = _db.GetConnection();
        return await connection.ExecuteScalarAsync<int>(sql, data);
    }

    /// <inheritdoc/>
    public async Task<bool> UpdateAsync(Category data)
    {
        const string sql = @"
            UPDATE dbo.Categories
            SET CategoryName = @CategoryName,
                ParentCategoryId = @ParentCategoryId,
                Description = @Description,
                IsActive = @IsActive
            WHERE CategoryId = @CategoryId;";

        using var connection = _db.GetConnection();
        return await connection.ExecuteAsync(sql, data) > 0;
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteAsync(int id)
    {
        const string sql = @"DELETE FROM dbo.Categories WHERE CategoryId = @id";

        using var connection = _db.GetConnection();
        return await connection.ExecuteAsync(sql, new { id }) > 0;
    }

    /// <inheritdoc/>
    public async Task<bool> InUsedAsync(int id)
    {
        const string sql = @"
            SELECT CASE
                       WHEN EXISTS (SELECT 1 FROM dbo.BookCategories WHERE CategoryId = @id)
                         OR EXISTS (SELECT 1 FROM dbo.Categories WHERE ParentCategoryId = @id)
                       THEN 1 ELSE 0
                   END;";

        using var connection = _db.GetConnection();
        return await connection.ExecuteScalarAsync<bool>(sql, new { id });
    }
}
