public interface IBackgroundService
{
    Task<IEnumerable<BackgroundSummaryDto>> GetAllAsync(int? requestingUserId, CancellationToken ct = default);
    Task<BackgroundDetailDto> GetByIdAsync(int id, int? requestingUserId, CancellationToken ct = default);
    Task<BackgroundDetailDto> CreateAsync(CreateBackgroundRequest request, int userId, CancellationToken ct = default);
    Task<BackgroundDetailDto> UpdateAsync(int id, UpdateBackgroundRequest request, int userId, CancellationToken ct = default);
    Task DeleteAsync(int id, int userId, CancellationToken ct = default);
}
