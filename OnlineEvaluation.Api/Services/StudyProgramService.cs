using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OnlineEvaluation.Api.Data;
using OnlineEvaluation.Api.Models.DTO;
using OnlineEvaluation.Api.Models.Entities;
using OnlineEvaluation.Api.Services.IServices;

namespace OnlineEvaluation.Api.Services
{
    public class StudyProgramService : IStudyProgramService
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;

        public StudyProgramService(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }
        public async Task<StudyProgramDto> CreateAsync(CreateStudyProgramDto createDto, string userId)
        {
            var program = _mapper.Map<StudyProgram>(createDto);
            program.CreatedByUserId = userId;
            program.CreatedAt = DateTime.UtcNow;

            _db.StudyPrograms.Add(program);
            await _db.SaveChangesAsync();
            return _mapper.Map<StudyProgramDto>(program);
        }

        public async Task<IEnumerable<StudyProgramDto>> GetAllAsync(int page, int pageSize)
        {
            var skip = (page - 1) * pageSize;
            var programs = await _db.StudyPrograms
                .OrderBy(p => p.Name)
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            return _mapper.Map<IEnumerable<StudyProgramDto>>(programs);
        }

        public async Task<StudyProgramDto?> GetByIdAsync(int id)
        {
            var program = await _db.StudyPrograms.FindAsync(id);
            return _mapper.Map<StudyProgramDto>(program);
        }

        public async Task<bool> HardDeleteAsync(int id)
        {
            var program = await _db.StudyPrograms
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(p => p.Id == id);

            if (program == null) return false;

            _db.StudyPrograms.Remove(program);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RestoreAsync(int id)
        {
            var program = await _db.StudyPrograms
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(p => p.Id == id && p.IsDeleted);

            if (program == null) return false;

            program.IsDeleted = false;
            program.DeletedAt = null;
            program.DeletedByUserId = null;

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SoftDeleteAsync(int id, string userId)
        {
            var program = await _db.StudyPrograms.FindAsync(id);
            if (program == null) return false;

            program.IsDeleted = true;
            program.DeletedAt = DateTime.UtcNow;
            program.DeletedByUserId = userId;

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateAsync(UpdateStudyProgramDto updateDto, string userId)
        {
            var existing = await _db.StudyPrograms.FindAsync(updateDto.Id);
            if (existing == null) return false;

            _mapper.Map(updateDto, existing);
            existing.UpdatedByUserId = userId;
            existing.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return true;
        }
    }
}
