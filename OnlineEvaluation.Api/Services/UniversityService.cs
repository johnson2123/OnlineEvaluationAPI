using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OnlineEvaluation.Api.Data;
using OnlineEvaluation.Api.Models.DTO;
using OnlineEvaluation.Api.Models.Entities;
using OnlineEvaluation.Api.Services.IServices;

namespace OnlineEvaluation.Api.Services
{
    public class UniversityService : IUniversityService
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;

        public UniversityService(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }
        public async Task<UniversityDto> CreateAsync(CreateUniversityDto dto, string? actorUserId)
        {
            var university = _mapper.Map<University>(dto);

            university.CreatedByUserId = actorUserId;
            university.CreatedAt = DateTime.UtcNow;

            _db.Universities.Add(university);
            await _db.SaveChangesAsync();

            return _mapper.Map<UniversityDto>(university);
        }

        public async Task<IEnumerable<UniversityDto>> GetAllAsync(int page, int pageSize)
        {
            var universities = await _db.Universities
                .AsNoTracking()
                .OrderBy(u => u.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return _mapper.Map<IEnumerable<UniversityDto>>(universities);
        }

        public async Task<UniversityDto?> GetByCodeAsync(string code)
        {
            return _mapper.Map<UniversityDto?>(
                await _db.Universities.AsNoTracking().FirstOrDefaultAsync(u => u.Code == code)
            );
        }

        public async Task<UniversityDto?> GetByIdAsync(int id)
        {
            return _mapper.Map<UniversityDto?>(
                await _db.Universities.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id)
            );
        }

        public async Task<bool> HardDeleteAsync(int id)
        {
            var university = await _db.Universities
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(u => u.Id == id);

            if (university == null) return false;

            _db.Universities.Remove(university);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SoftDeleteAsync(int id, string? actorUserId)
        {
            var university = await _db.Universities.FindAsync(id);
            if (university == null) return false;

            university.IsDeleted = true;
            university.DeletedAt = DateTime.UtcNow;
            university.DeletedByUserId = actorUserId;

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateAsync(int id, UpdateUniversityDto dto, string? actorUserId)
        {
            var university = await _db.Universities.FindAsync(id);
            if (university == null) return false;

            _mapper.Map(dto, university);

            university.UpdatedByUserId = actorUserId;
            university.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return true;
        }
    }
}
