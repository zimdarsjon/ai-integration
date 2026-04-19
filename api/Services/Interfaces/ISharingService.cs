public interface ISharingService
{
    Task<IEnumerable<ContentShareDto>> GetSharesAsync(CustomContentType type, int contentId, int requestingUserId, CancellationToken ct = default);
    Task<ContentShareDto> ShareWithUserAsync(ShareWithUserRequest request, int requestingUserId, CancellationToken ct = default);
    Task<ContentShareDto> ShareWithCampaignAsync(ShareWithCampaignRequest request, int requestingUserId, CancellationToken ct = default);
    Task RevokeShareAsync(int shareId, int requestingUserId, CancellationToken ct = default);
    Task SetPublicAsync(CustomContentType type, int contentId, bool isPublic, int requestingUserId, CancellationToken ct = default);

    // Used internally by other services to filter visible content
    Task<HashSet<int>> GetAccessibleContentIdsAsync(CustomContentType type, int userId, CancellationToken ct = default);
}
