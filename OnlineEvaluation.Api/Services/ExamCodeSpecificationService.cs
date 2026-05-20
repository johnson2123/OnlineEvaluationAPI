using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OnlineEvaluation.Api.Data;
using OnlineEvaluation.Api.Models.DTO;
using OnlineEvaluation.Api.Models.Entities;
using OnlineEvaluation.Api.Services.IServices;

namespace OnlineEvaluation.Api.Services
{
    public class ExamCodeSpecificationService : IExamCodeSpecificationService
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;

        public ExamCodeSpecificationService(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<ExamSpecDto> CreateAsync(CreateExamSpecDto dto, string? actorUserId)
        {
            var subject = await _db.Subjects
                .FirstOrDefaultAsync(s => s.Id == dto.SubjectId)
                ?? throw new KeyNotFoundException($"Subject with ID {dto.SubjectId} does not exist in the master ledger.");

            var academicMap = await _db.AcademicMaps
                .FirstOrDefaultAsync(a => a.Id == dto.AcademicMapId)
                ?? throw new KeyNotFoundException($"Academic Mapping with ID {dto.AcademicMapId} does not exist.");

            string generatedCode = $"{academicMap.AliasCode}-SEM{dto.Semester}-{subject.Code}".ToUpper();

            bool codeExists = await _db.ExamCodeSpecifications
                .AnyAsync(e => e.ExamSpecCode == generatedCode);

            if (codeExists)
            {
                throw new InvalidOperationException($"An active specification mapping already exists for code: '{generatedCode}'");
            }

            var entity = _mapper.Map<ExamCodeSpecification>(dto);
            entity.ExamSpecCode = generatedCode;

            entity.CreatedBy = actorUserId ?? "System";
            entity.CreatedAt = DateTime.UtcNow;

            await _db.ExamCodeSpecifications.AddAsync(entity);
            await _db.SaveChangesAsync();

            entity.Subject = subject;
            return _mapper.Map<ExamSpecDto>(entity);
        }

        public async Task<IEnumerable<ExamSpecDto>> GetAllAsync(int page, int pageSize)
        {
            var entities = await _db.ExamCodeSpecifications
                .Include(e => e.Subject)
                .Include(e => e.AcademicMap)
                .OrderByDescending(e => e.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ExamSpecDto>>(entities);
        }

        public async Task<ExamSpecDto?> GetByIdAsync(int id)
        {
            var entity = await _db.ExamCodeSpecifications
                .Include(e => e.Subject)
                .Include(e => e.AcademicMap)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (entity == null) return null;

            return _mapper.Map<ExamSpecDto>(entity);
        }

        public async Task<bool> HardDeleteAsync(int id)
        {
            var entity = await _db.ExamCodeSpecifications
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(e => e.Id == id)
                ?? throw new KeyNotFoundException($"Exam specification record not found for absolute hard deletion with ID: {id}");

            //bool isReferenced = await _db.ExaminationSchedules
            //    .AnyAsync(s => s.ExamCodeMatrixId == entity.Id);
            //if (isReferenced)
            //{
            //    throw new InvalidOperationException("Absolute hard-delete denied. This specification blueprint is linked to active operational schedules.");
            //}

            _db.ExamCodeSpecifications.Remove(entity);
            return await _db.SaveChangesAsync() > 0;

        }

        public async Task<bool> SoftDeleteAsync(int id, string? actorUserId)
        {
            var entity = await _db.ExamCodeSpecifications
                .FirstOrDefaultAsync(e => e.Id == id)
                ?? throw new KeyNotFoundException($"Exam specification record not found for removal with ID: {id}");

            // Relational guard check: prevent deleting if locked by a calendar schedule
            //bool isReferencedInSchedules = await _db.ExaminationSchedules
            //    .AnyAsync(s => s.ExamCodeMatrixId == entity.Id);

            entity.IsDeleted = true;
            entity.DeletedBy = actorUserId;
            entity.DeletedAt = DateTime.UtcNow;

            _db.ExamCodeSpecifications.Update(entity);
            return await _db.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateAsync(int id, UpdateExamSpecDto dto, string? actorUserId)
        {
            var entity = await _db.ExamCodeSpecifications
                .FirstOrDefaultAsync(e => e.Id == id)
                ?? throw new KeyNotFoundException($"Exam specification record not found for updates with ID: {id}");

            _mapper.Map(dto, entity);

            entity.UpdatedBy = actorUserId;
            entity.UpdatedAt = DateTime.UtcNow;

            _db.ExamCodeSpecifications.Update(entity);
            return await _db.SaveChangesAsync() > 0;
        }
    }
}
