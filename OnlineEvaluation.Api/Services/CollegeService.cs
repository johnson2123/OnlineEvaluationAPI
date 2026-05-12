using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OnlineEvaluation.Api.Data;
using OnlineEvaluation.Api.Models.DTO;
using OnlineEvaluation.Api.Models.Entities;
using OnlineEvaluation.Api.Services.IServices;

namespace OnlineEvaluation.Api.Services
{
    public class CollegeService : ICollegeService
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;

        public CollegeService(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }
        public async Task<CollegeDto> CreateAsync(CreateCollegeDto dto, string? actorUserId)
        {
            var universityExists = await _db.Universities
                                        .AnyAsync(u => u.Code == dto.UniversityCode);

            if (!universityExists)
                throw new Exception($"University with code {dto.UniversityCode} does not exist.");

            var college = _mapper.Map<College>(dto);
            college.CreatedByUserId = actorUserId;
            college.CreatedAt = DateTime.UtcNow;

            _db.Colleges.Add(college);
            await _db.SaveChangesAsync();

            return _mapper.Map<CollegeDto>(college);
        }

        public async Task<IEnumerable<CollegeDto>> GetAllAsync(int page, int pageSize)
        {
            var colleges = await _db.Colleges
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return _mapper.Map<IEnumerable<CollegeDto>>(colleges);
        }

        public async Task<CollegeDto?> GetByCodeAsync(string universityCode, string collegeCode)
        {
            var college = await _db.Colleges
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.UniversityCode == universityCode && c.Code == collegeCode);

            return _mapper.Map<CollegeDto?>(college);
        }

        public async Task<CollegeDto?> GetByIdAsync(int id)
        {
            var college = await _db.Colleges.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
            return _mapper.Map<CollegeDto?>(college);
        }

        public async Task<IEnumerable<CollegeDto>> GetByUniversityAsync(string universityCode)
        {
            var colleges = await _db.Colleges
                .AsNoTracking()
                .Where(c => c.UniversityCode == universityCode)
                .OrderBy(c => c.Name)
                .ToListAsync();

            return _mapper.Map<IEnumerable<CollegeDto>>(colleges);
        }

        public async Task<bool> HardDeleteAsync(int id)
        {
            var college = await _db.Colleges
                .IgnoreQueryFilters() // Must ignore filters to find soft-deleted items for hard deletion
                .FirstOrDefaultAsync(c => c.Id == id);

            if (college == null) return false;

            _db.Colleges.Remove(college);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SoftDeleteAsync(int id, string? actorUserId)
        {
            var college = await _db.Colleges.FindAsync(id);
            if (college == null) return false;

            college.IsDeleted = true;
            college.DeletedAt = DateTime.UtcNow;
            college.DeletedByUserId = actorUserId;

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateAsync(int id, UpdateCollegeDto dto, string? actorUserId)
        {
            var college = await _db.Colleges.FindAsync(id);
            if (college == null) return false;

            _mapper.Map(dto, college);
            college.UpdatedByUserId = actorUserId;
            college.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return true;
        }
    }
}
