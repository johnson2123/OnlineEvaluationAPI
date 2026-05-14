using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using OnlineEvaluation.Api.Data;
using OnlineEvaluation.Api.Models.DTO;
using OnlineEvaluation.Api.Models.Entities;
using OnlineEvaluation.Api.Services.IServices;

namespace OnlineEvaluation.Api.Services
{
    public class AcademicMapService : IAcademicMapService
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;

        public AcademicMapService(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<AcademicMapDto> CreateAsync(CreateAcademicMapDto dto, string? actorUserId)
        {
            var college = await _db.Colleges.FindAsync(dto.CollegeId);
            var program = await _db.StudyPrograms.FindAsync(dto.StudyProgramId);
            var branch = await _db.Branches.FindAsync(dto.BranchId);

            if (college == null || program == null || branch == null)
                throw new Exception("Invalid master data selection.");

            var entity = _mapper.Map<AcademicMap>(dto);

            // Logic for Alias Name generation: COLLEGE-PROGRAM-BRANCH
            if (string.IsNullOrWhiteSpace(entity.AliasCode))
            {
                entity.AliasCode = $"{college.Code}-{program.ShortName}-{branch.Code}".ToUpper();
            }

            entity.CreatedBy = actorUserId;
            entity.CreatedAt = DateTime.UtcNow;

            _db.AcademicMaps.Add(entity);
            await _db.SaveChangesAsync();

            return await GetByIdAsync(entity.Id) ?? _mapper.Map<AcademicMapDto>(entity);
        }

        public async Task<IEnumerable<AcademicMapDto>> GetAllAsync(int page, int pageSize)
        {
            return await _db.AcademicMaps
                .Where(x => !x.IsDeleted)
                .Include(x => x.College)
                .Include(x => x.StudyProgram)
                .Include(x => x.Branch)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ProjectTo<AcademicMapDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<AcademicMapDto?> GetByIdAsync(int id)
        {
            return await _db.AcademicMaps
                .Include(x => x.College)
                .Include(x => x.StudyProgram)
                .Include(x => x.Branch)
                .ProjectTo<AcademicMapDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<AcademicMapInitDto> GetInitDataAsync()
        {
            return new AcademicMapInitDto
            {
                Colleges = await _db.Colleges
                    .Where(x => !x.IsDeleted)
                    .ProjectTo<LookUpDto>(_mapper.ConfigurationProvider)
                    .ToListAsync(),

                StudyPrograms = await _db.StudyPrograms
                    .Where(x => !x.IsDeleted && x.IsActive)
                    .ProjectTo<LookUpDto>(_mapper.ConfigurationProvider)
                    .ToListAsync(),

                Branches = await _db.Branches
                    .Where(x => !x.IsDeleted && x.IsActive)
                    .ProjectTo<LookUpDto>(_mapper.ConfigurationProvider)
                    .ToListAsync()
            };
        }

        public async Task<bool> HardDeleteAsync(int id)
        {
            var entity = await _db.AcademicMaps
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null) return false;

            _db.AcademicMaps.Remove(entity);
            return await _db.SaveChangesAsync() > 0;
        }

        public async Task<bool> SoftDeleteAsync(int id, string? actorUserId)
        {
            var entity = await _db.AcademicMaps.FindAsync(id);
            if (entity == null) return false;

            entity.IsDeleted = true;
            entity.UpdatedBy = actorUserId;
            entity.UpdatedAt = DateTime.UtcNow;

            return await _db.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateAsync(int id, UpdateAcademicMapDto dto, string? actorUserId)
        {
            var entity = await _db.AcademicMaps.FindAsync(id);
            if (entity == null) return false;

            if (entity.CollegeId != dto.CollegeId ||
                entity.StudyProgramId != dto.StudyProgramId ||
                entity.BranchId != dto.BranchId)
            {
                var college = await _db.Colleges.FindAsync(dto.CollegeId);
                var program = await _db.StudyPrograms.FindAsync(dto.StudyProgramId);
                var branch = await _db.Branches.FindAsync(dto.BranchId);

                if (college == null || program == null || branch == null)
                    throw new Exception("One or more Master Data IDs are invalid.");

                entity.AliasCode = $"{college.Code}-{program.ShortName}-{branch.Code}".ToUpper();
            }

            _mapper.Map(dto, entity);

            entity.UpdatedBy = actorUserId;
            entity.UpdatedAt = DateTime.UtcNow;

            return await _db.SaveChangesAsync() > 0;
        }
    }
}
