using AutoMapper;
using Microsoft.EntityFrameworkCore;

public class BackgroundService : IBackgroundService
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public BackgroundService(AppDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<IEnumerable<BackgroundSummaryDto>> GetAllAsync(int? requestingUserId, CancellationToken ct = default)
    {
        var query = _db.Backgrounds.AsQueryable();

        query = query.Where(b =>
            !b.IsCustom ||
            b.IsPublic ||
            (requestingUserId.HasValue && b.CreatedByUserId == requestingUserId));

        var backgrounds = await query.OrderBy(b => b.Name).ToListAsync(ct);
        return _mapper.Map<IEnumerable<BackgroundSummaryDto>>(backgrounds);
    }

    public async Task<BackgroundDetailDto> GetByIdAsync(int id, int? requestingUserId, CancellationToken ct = default)
    {
        var background = await _db.Backgrounds
            .Include(b => b.Features)
            .Include(b => b.SkillProficiencies)
            .FirstOrDefaultAsync(b => b.Id == id, ct)
            ?? throw new NotFoundException($"Background {id} not found.");

        if (background.IsCustom && !background.IsPublic && background.CreatedByUserId != requestingUserId)
            throw new UnauthorizedException("You do not have access to this background.");

        return _mapper.Map<BackgroundDetailDto>(background);
    }

    public async Task<BackgroundDetailDto> CreateAsync(CreateBackgroundRequest request, int userId, CancellationToken ct = default)
    {
        var background = new Background
        {
            Name = request.Name,
            Description = request.Description,
            LanguagesGranted = request.LanguagesGranted,
            ToolProficiencyDescription = request.ToolProficiencyDescription,
            StartingEquipmentDescription = request.StartingEquipmentDescription,
            IsCustom = true,
            IsPublic = request.IsPublic,
            CreatedByUserId = userId,
            SourceBook = "Custom"
        };

        _db.Backgrounds.Add(background);
        await _db.SaveChangesAsync(ct);

        foreach (var f in request.Features)
        {
            _db.BackgroundFeatures.Add(new BackgroundFeature
            {
                BackgroundId = background.Id,
                Name = f.Name,
                Description = f.Description
            });
        }

        foreach (var skill in request.SkillProficiencies)
        {
            _db.BackgroundSkillProficiencies.Add(new BackgroundSkillProficiency
            {
                BackgroundId = background.Id,
                SkillName = skill
            });
        }

        await _db.SaveChangesAsync(ct);
        return await GetByIdAsync(background.Id, userId, ct);
    }

    public async Task<BackgroundDetailDto> UpdateAsync(int id, UpdateBackgroundRequest request, int userId, CancellationToken ct = default)
    {
        var background = await _db.Backgrounds.FindAsync([id], ct)
            ?? throw new NotFoundException($"Background {id} not found.");

        if (!background.IsCustom || background.CreatedByUserId != userId)
            throw new UnauthorizedException("You can only edit your own custom backgrounds.");

        if (request.Name != null) background.Name = request.Name;
        if (request.Description != null) background.Description = request.Description;
        if (request.LanguagesGranted.HasValue) background.LanguagesGranted = request.LanguagesGranted.Value;
        if (request.ToolProficiencyDescription != null) background.ToolProficiencyDescription = request.ToolProficiencyDescription;
        if (request.StartingEquipmentDescription != null) background.StartingEquipmentDescription = request.StartingEquipmentDescription;
        if (request.IsPublic.HasValue) background.IsPublic = request.IsPublic.Value;

        await _db.SaveChangesAsync(ct);
        return await GetByIdAsync(id, userId, ct);
    }

    public async Task DeleteAsync(int id, int userId, CancellationToken ct = default)
    {
        var background = await _db.Backgrounds.FindAsync([id], ct)
            ?? throw new NotFoundException($"Background {id} not found.");

        if (!background.IsCustom || background.CreatedByUserId != userId)
            throw new UnauthorizedException("You can only delete your own custom backgrounds.");

        _db.Backgrounds.Remove(background);
        await _db.SaveChangesAsync(ct);
    }
}
