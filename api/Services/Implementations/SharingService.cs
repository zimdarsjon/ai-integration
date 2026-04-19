using Microsoft.EntityFrameworkCore;

public class SharingService : ISharingService
{
    private readonly AppDbContext _db;

    public SharingService(AppDbContext db) => _db = db;

    public async Task<IEnumerable<ContentShareDto>> GetSharesAsync(CustomContentType type, int contentId, int requestingUserId, CancellationToken ct = default)
    {
        await VerifyOwnershipAsync(type, contentId, requestingUserId, ct);

        return await _db.CustomContentShares
            .Include(s => s.SharedWithUser)
            .Include(s => s.SharedWithCampaign)
            .Where(s => s.ContentType == type && s.ContentId == contentId)
            .Select(s => new ContentShareDto
            {
                Id = s.Id,
                ContentType = s.ContentType,
                ContentId = s.ContentId,
                SharedWithUserId = s.SharedWithUserId,
                SharedWithUserName = s.SharedWithUser != null ? s.SharedWithUser.FirstName + " " + s.SharedWithUser.LastName : null,
                SharedWithCampaignId = s.SharedWithCampaignId,
                SharedWithCampaignName = s.SharedWithCampaign != null ? s.SharedWithCampaign.Name : null,
                CreatedAt = s.CreatedAt
            })
            .ToListAsync(ct);
    }

    public async Task<ContentShareDto> ShareWithUserAsync(ShareWithUserRequest request, int requestingUserId, CancellationToken ct = default)
    {
        await VerifyOwnershipAsync(request.ContentType, request.ContentId, requestingUserId, ct);

        if (request.TargetUserId == requestingUserId)
            throw new ValidationException("You cannot share content with yourself.");

        var targetExists = await _db.Users.AnyAsync(u => u.Id == request.TargetUserId, ct);
        if (!targetExists) throw new NotFoundException($"User {request.TargetUserId} not found.");

        var existing = await _db.CustomContentShares.FirstOrDefaultAsync(s =>
            s.ContentType == request.ContentType &&
            s.ContentId == request.ContentId &&
            s.SharedWithUserId == request.TargetUserId, ct);

        if (existing != null)
            return await MapShareAsync(existing, ct);

        var share = new CustomContentShare
        {
            ContentType = request.ContentType,
            ContentId = request.ContentId,
            CreatedByUserId = requestingUserId,
            SharedWithUserId = request.TargetUserId
        };
        _db.CustomContentShares.Add(share);
        await _db.SaveChangesAsync(ct);

        return await MapShareAsync(share, ct);
    }

    public async Task<ContentShareDto> ShareWithCampaignAsync(ShareWithCampaignRequest request, int requestingUserId, CancellationToken ct = default)
    {
        await VerifyOwnershipAsync(request.ContentType, request.ContentId, requestingUserId, ct);

        var isMember = await _db.CampaignMembers.AnyAsync(m => m.CampaignId == request.CampaignId && m.UserId == requestingUserId, ct)
                    || await _db.Campaigns.AnyAsync(c => c.Id == request.CampaignId && c.DMUserId == requestingUserId, ct);
        if (!isMember) throw new UnauthorizedException("You must be a member of the campaign to share content with it.");

        var existing = await _db.CustomContentShares.FirstOrDefaultAsync(s =>
            s.ContentType == request.ContentType &&
            s.ContentId == request.ContentId &&
            s.SharedWithCampaignId == request.CampaignId, ct);

        if (existing != null)
            return await MapShareAsync(existing, ct);

        var share = new CustomContentShare
        {
            ContentType = request.ContentType,
            ContentId = request.ContentId,
            CreatedByUserId = requestingUserId,
            SharedWithCampaignId = request.CampaignId
        };
        _db.CustomContentShares.Add(share);
        await _db.SaveChangesAsync(ct);

        return await MapShareAsync(share, ct);
    }

    public async Task RevokeShareAsync(int shareId, int requestingUserId, CancellationToken ct = default)
    {
        var share = await _db.CustomContentShares.FindAsync([shareId], ct)
            ?? throw new NotFoundException($"Share {shareId} not found.");

        if (share.CreatedByUserId != requestingUserId)
            throw new UnauthorizedException("Only the content owner can revoke shares.");

        _db.CustomContentShares.Remove(share);
        await _db.SaveChangesAsync(ct);
    }

    public async Task SetPublicAsync(CustomContentType type, int contentId, bool isPublic, int requestingUserId, CancellationToken ct = default)
    {
        await VerifyOwnershipAsync(type, contentId, requestingUserId, ct);

        switch (type)
        {
            case CustomContentType.Race:
                var race = await _db.Races.FindAsync([contentId], ct) ?? throw new NotFoundException("Race not found.");
                race.IsPublic = isPublic;
                break;
            case CustomContentType.Class:
                var cls = await _db.Classes.FindAsync([contentId], ct) ?? throw new NotFoundException("Class not found.");
                cls.IsPublic = isPublic;
                break;
            case CustomContentType.Spell:
                var spell = await _db.Spells.FindAsync([contentId], ct) ?? throw new NotFoundException("Spell not found.");
                spell.IsPublic = isPublic;
                break;
            case CustomContentType.Item:
                var item = await _db.Items.FindAsync([contentId], ct) ?? throw new NotFoundException("Item not found.");
                item.IsPublic = isPublic;
                break;
            case CustomContentType.Monster:
                var monster = await _db.Monsters.FindAsync([contentId], ct) ?? throw new NotFoundException("Monster not found.");
                monster.IsPublic = isPublic;
                break;
            case CustomContentType.AbilityScore:
                var score = await _db.AbilityScores.FindAsync([contentId], ct) ?? throw new NotFoundException("Ability score not found.");
                score.IsPublic = isPublic;
                break;
        }

        await _db.SaveChangesAsync(ct);
    }

    public async Task<HashSet<int>> GetAccessibleContentIdsAsync(CustomContentType type, int userId, CancellationToken ct = default)
    {
        // Campaign IDs the user belongs to
        var campaignIds = await _db.CampaignMembers
            .Where(m => m.UserId == userId)
            .Select(m => m.CampaignId)
            .Union(_db.Campaigns.Where(c => c.DMUserId == userId).Select(c => c.Id))
            .ToListAsync(ct);

        var sharedIds = await _db.CustomContentShares
            .Where(s => s.ContentType == type && (
                s.SharedWithUserId == userId ||
                (s.SharedWithCampaignId != null && campaignIds.Contains(s.SharedWithCampaignId.Value))
            ))
            .Select(s => s.ContentId)
            .ToListAsync(ct);

        return [.. sharedIds];
    }

    private async Task VerifyOwnershipAsync(CustomContentType type, int contentId, int userId, CancellationToken ct)
    {
        bool isOwner = type switch
        {
            CustomContentType.Race => await _db.Races.AnyAsync(r => r.Id == contentId && r.CreatedByUserId == userId, ct),
            CustomContentType.Class => await _db.Classes.AnyAsync(c => c.Id == contentId && c.CreatedByUserId == userId, ct),
            CustomContentType.Spell => await _db.Spells.AnyAsync(s => s.Id == contentId && s.CreatedByUserId == userId, ct),
            CustomContentType.Item => await _db.Items.AnyAsync(i => i.Id == contentId && i.CreatedByUserId == userId, ct),
            CustomContentType.Monster => await _db.Monsters.AnyAsync(m => m.Id == contentId && m.CreatedByUserId == userId, ct),
            CustomContentType.AbilityScore => await _db.AbilityScores.AnyAsync(a => a.Id == contentId && a.CreatedByUserId == userId, ct),
            _ => false
        };

        if (!isOwner) throw new UnauthorizedException("Only the content owner can manage shares.");
    }

    private async Task<ContentShareDto> MapShareAsync(CustomContentShare share, CancellationToken ct)
    {
        await _db.Entry(share).Reference(s => s.SharedWithUser).LoadAsync(ct);
        await _db.Entry(share).Reference(s => s.SharedWithCampaign).LoadAsync(ct);

        return new ContentShareDto
        {
            Id = share.Id,
            ContentType = share.ContentType,
            ContentId = share.ContentId,
            SharedWithUserId = share.SharedWithUserId,
            SharedWithUserName = share.SharedWithUser != null ? $"{share.SharedWithUser.FirstName} {share.SharedWithUser.LastName}" : null,
            SharedWithCampaignId = share.SharedWithCampaignId,
            SharedWithCampaignName = share.SharedWithCampaign?.Name,
            CreatedAt = share.CreatedAt
        };
    }
}
