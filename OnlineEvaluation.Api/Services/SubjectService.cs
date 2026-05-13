using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OnlineEvaluation.Api.Data;
using OnlineEvaluation.Api.Models.DTO;
using OnlineEvaluation.Api.Models.Entities;
using OnlineEvaluation.Api.Services.IServices;

namespace OnlineEvaluation.Api.Services
{
    public class SubjectService : ISubjectService
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;

        public SubjectService(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }
        public async Task<SubjectDto> CreateAsync(CreateSubjectDto dto, string? actorUserId)
        {
            var subject = _mapper.Map<Subject>(dto);

            subject.Guid = Guid.NewGuid();
            subject.CreatedAt = DateTime.UtcNow;
            subject.CreatedBy = actorUserId;

            _db.Subjects.Add(subject);
            await _db.SaveChangesAsync();

            return _mapper.Map<SubjectDto>(subject);
        }

        public async Task<IEnumerable<SubjectDto>> GetAllAsync(int page, int pageSize)
        {
            var subjects = await _db.Subjects
                .AsNoTracking()
                .OrderBy(s => s.Code)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return _mapper.Map<IEnumerable<SubjectDto>>(subjects);
        }

        public async Task<SubjectDto?> GetByCodeAsync(string code)
        {
            var subject = await _db.Subjects
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Code == code);

            return _mapper.Map<SubjectDto>(subject);
        }

        public async Task<SubjectDto?> GetByIdAsync(int id)
        {
            var subject = await _db.Subjects
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id);

            return _mapper.Map<SubjectDto>(subject);
        }

        public async Task<bool> HardDeleteAsync(int id)
        {
            var subject = await _db.Subjects
                                .IgnoreQueryFilters()
                                .FirstOrDefaultAsync(s => s.Id == id);

            if (subject == null) return false;

            _db.Subjects.Remove(subject);
            return await _db.SaveChangesAsync() > 0;
        }

        public async Task<bool> SoftDeleteAsync(int id, string? actorUserId)
        {
            var subject = await _db.Subjects.FindAsync(id);
            if (subject == null) return false;

            subject.IsDeleted = true;
            subject.DeletedAt = DateTime.UtcNow;
            subject.DeletedBy = actorUserId;

            _db.Subjects.Update(subject);
            return await _db.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateAsync(int id, UpdateSubjectDto dto, string? actorUserId)
        {
            var subject = await _db.Subjects.FindAsync(id);
            if (subject == null) return false;

            // Map DTO to existing entity
            _mapper.Map(dto, subject);

            subject.UpdatedAt = DateTime.UtcNow;
            subject.UpdatedBy = actorUserId;

            _db.Subjects.Update(subject);
            return await _db.SaveChangesAsync() > 0;
        }
    }
}
