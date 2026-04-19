using Microsoft.EntityFrameworkCore;

public class CampaignService : ICampaignService
{
    private readonly AppDbContext _db;

    public CampaignService(AppDbContext db) => _db = db;

    public async Task<IEnumerable<CampaignSummaryDto>> GetUserCampaignsAsync(int userId, CancellationToken ct = default)
    {
        var asDM = await _db.Campaigns
            .Include(c => c.Members)
            .Where(c => c.DMUserId == userId)
            .Select(c => new CampaignSummaryDto
            {
                Id = c.Id, Name = c.Name, Description = c.Description,
                DMName = c.DM.FirstName + " " + c.DM.LastName,
                MemberCount = c.Members.Count, IsActive = c.IsActive,
                UserRole = CampaignRole.DM
            }).ToListAsync(ct);

        var asPlayer = await _db.CampaignMembers
            .Include(m => m.Campaign).ThenInclude(c => c.DM)
            .Include(m => m.Campaign).ThenInclude(c => c.Members)
            .Where(m => m.UserId == userId && m.Campaign.DMUserId != userId)
            .Select(m => new CampaignSummaryDto
            {
                Id = m.Campaign.Id, Name = m.Campaign.Name, Description = m.Campaign.Description,
                DMName = m.Campaign.DM.FirstName + " " + m.Campaign.DM.LastName,
                MemberCount = m.Campaign.Members.Count, IsActive = m.Campaign.IsActive,
                UserRole = m.Role
            }).ToListAsync(ct);

        return asDM.Concat(asPlayer);
    }

    public async Task<CampaignDetailDto> GetByIdAsync(int id, int requestingUserId, CancellationToken ct = default)
    {
        var campaign = await _db.Campaigns
            .Include(c => c.DM)
            .Include(c => c.Members).ThenInclude(m => m.User)
            .Include(c => c.Members).ThenInclude(m => m.Character)
            .Include(c => c.Sessions)
            .FirstOrDefaultAsync(c => c.Id == id, ct)
            ?? throw new NotFoundException($"Campaign {id} not found.");

        var isMember = campaign.Members.Any(m => m.UserId == requestingUserId) || campaign.DMUserId == requestingUserId;
        if (!isMember) throw new UnauthorizedException("Not a member of this campaign.");

        return new CampaignDetailDto
        {
            Id = campaign.Id,
            Name = campaign.Name,
            Description = campaign.Description,
            DMUserId = campaign.DMUserId,
            DMName = $"{campaign.DM.FirstName} {campaign.DM.LastName}",
            IsActive = campaign.IsActive,
            Setting = campaign.Setting,
            Notes = campaign.Notes,
            Members = campaign.Members.Select(m => new CampaignMemberDto
            {
                Id = m.Id,
                UserId = m.UserId,
                UserName = $"{m.User.FirstName} {m.User.LastName}",
                CharacterId = m.CharacterId,
                CharacterName = m.Character?.Name,
                Role = m.Role
            }).ToList(),
            Sessions = campaign.Sessions.OrderByDescending(s => s.SessionNumber).Select(s => new CampaignSessionDto
            {
                Id = s.Id, SessionNumber = s.SessionNumber, Title = s.Title, Date = s.Date, Summary = s.Summary
            }).ToList()
        };
    }

    public async Task<CampaignDetailDto> CreateAsync(CreateCampaignRequest request, int userId, CancellationToken ct = default)
    {
        var campaign = new Campaign
        {
            Name = request.Name,
            Description = request.Description,
            Setting = request.Setting,
            DMUserId = userId,
            IsActive = true
        };
        _db.Campaigns.Add(campaign);
        await _db.SaveChangesAsync(ct);
        return await GetByIdAsync(campaign.Id, userId, ct);
    }

    public async Task<CampaignDetailDto> UpdateAsync(int id, UpdateCampaignRequest request, int userId, CancellationToken ct = default)
    {
        var campaign = await _db.Campaigns.FindAsync([id], ct)
            ?? throw new NotFoundException($"Campaign {id} not found.");
        if (campaign.DMUserId != userId) throw new UnauthorizedException("Only the DM can update the campaign.");

        if (request.Name != null) campaign.Name = request.Name;
        if (request.Description != null) campaign.Description = request.Description;
        if (request.Setting != null) campaign.Setting = request.Setting;
        if (request.Notes != null) campaign.Notes = request.Notes;
        if (request.IsActive.HasValue) campaign.IsActive = request.IsActive.Value;

        await _db.SaveChangesAsync(ct);
        return await GetByIdAsync(id, userId, ct);
    }

    public async Task DeleteAsync(int id, int userId, CancellationToken ct = default)
    {
        var campaign = await _db.Campaigns.FindAsync([id], ct)
            ?? throw new NotFoundException($"Campaign {id} not found.");
        if (campaign.DMUserId != userId) throw new UnauthorizedException("Only the DM can delete the campaign.");
        _db.Campaigns.Remove(campaign);
        await _db.SaveChangesAsync(ct);
    }

    public async Task AddMemberAsync(int id, AddMemberRequest request, int requestingUserId, CancellationToken ct = default)
    {
        var campaign = await _db.Campaigns.FindAsync([id], ct)
            ?? throw new NotFoundException($"Campaign {id} not found.");
        if (campaign.DMUserId != requestingUserId) throw new UnauthorizedException("Only the DM can add members.");

        var alreadyMember = await _db.CampaignMembers.AnyAsync(m => m.CampaignId == id && m.UserId == request.UserId, ct);
        if (!alreadyMember)
        {
            _db.CampaignMembers.Add(new CampaignMember { CampaignId = id, UserId = request.UserId, Role = request.Role });
            await _db.SaveChangesAsync(ct);
        }
    }

    public async Task RemoveMemberAsync(int id, int memberId, int requestingUserId, CancellationToken ct = default)
    {
        var campaign = await _db.Campaigns.FindAsync([id], ct)
            ?? throw new NotFoundException($"Campaign {id} not found.");
        if (campaign.DMUserId != requestingUserId) throw new UnauthorizedException("Only the DM can remove members.");

        var member = await _db.CampaignMembers.FindAsync([memberId], ct)
            ?? throw new NotFoundException("Member not found.");
        _db.CampaignMembers.Remove(member);
        await _db.SaveChangesAsync(ct);
    }

    public async Task AssignCharacterAsync(int id, int memberId, int characterId, int userId, CancellationToken ct = default)
    {
        var member = await _db.CampaignMembers.FirstOrDefaultAsync(m => m.Id == memberId && m.CampaignId == id, ct)
            ?? throw new NotFoundException("Member not found.");
        if (member.UserId != userId) throw new UnauthorizedException("You can only assign your own character.");

        var character = await _db.Characters.FindAsync([characterId], ct)
            ?? throw new NotFoundException("Character not found.");
        if (character.UserId != userId) throw new UnauthorizedException("That is not your character.");

        member.CharacterId = characterId;
        await _db.SaveChangesAsync(ct);
    }
}
