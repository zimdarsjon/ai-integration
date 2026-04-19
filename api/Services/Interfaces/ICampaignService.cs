public interface ICampaignService
{
    Task<IEnumerable<CampaignSummaryDto>> GetUserCampaignsAsync(int userId, CancellationToken ct = default);
    Task<CampaignDetailDto> GetByIdAsync(int id, int requestingUserId, CancellationToken ct = default);
    Task<CampaignDetailDto> CreateAsync(CreateCampaignRequest request, int userId, CancellationToken ct = default);
    Task<CampaignDetailDto> UpdateAsync(int id, UpdateCampaignRequest request, int userId, CancellationToken ct = default);
    Task DeleteAsync(int id, int userId, CancellationToken ct = default);
    Task AddMemberAsync(int id, AddMemberRequest request, int requestingUserId, CancellationToken ct = default);
    Task RemoveMemberAsync(int id, int memberId, int requestingUserId, CancellationToken ct = default);
    Task AssignCharacterAsync(int id, int memberId, int characterId, int userId, CancellationToken ct = default);
}
