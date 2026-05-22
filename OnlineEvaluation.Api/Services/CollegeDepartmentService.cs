using AutoMapper;
using AutoMapper.QueryableExtensions;
using Azure;
using Microsoft.EntityFrameworkCore;
using OnlineEvaluation.Api.Data;
using OnlineEvaluation.Api.Models.DTO;
using OnlineEvaluation.Api.Models.Entities;
using OnlineEvaluation.Api.Services.IServices;

namespace OnlineEvaluation.Api.Services
{
    public class CollegeDepartmentService : ICollegeDepartmentService
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        public CollegeDepartmentService(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CollegeDepartmentDto>> GetAllAsync(int page, int pageSize)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;

            return await _db.CollegeDepartments
                .OrderBy(cd => cd.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ProjectTo<CollegeDepartmentDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<IEnumerable<CollegeDepartmentDto>> GetByCollegeAsync(int collegeId, int page, int pageSize)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;

            return await _db.CollegeDepartments
                .Where(cd => cd.CollegeId == collegeId)
                .OrderBy(cd => cd.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ProjectTo<CollegeDepartmentDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<CollegeDepartmentDto?> GetByIdAsync(int id)
        {
            return await _db.CollegeDepartments
                .Where(cd => cd.Id == id)
                .ProjectTo<CollegeDepartmentDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }

        public async Task<CollegeDepartmentDto> MapDepartmentAsync(CreateCollegeDepartmentDto dto, string userId)
        {
            var existingMapping = await _db.CollegeDepartments
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(cd => cd.CollegeId == dto.CollegeId && cd.DepartmentId == dto.DepartmentId);

            if (existingMapping != null)
            {
                if (!existingMapping.IsDeleted)
                {
                    throw new InvalidOperationException("This department mapping already exists for this college.");
                }

                // Reactivate previous record instead of inserting a duplicate
                existingMapping.IsDeleted = false;
                existingMapping.DeletedAt = null;
                existingMapping.DeletedBy = null;
                existingMapping.IsActive = true;
                existingMapping.UpdatedAt = DateTime.UtcNow;
                existingMapping.UpdatedBy = userId;

                await _db.SaveChangesAsync();
                return (await GetByIdAsync(existingMapping.Id))!;
            }

            var collegeCode = await _db.Colleges
                .Where(c => c.Id == dto.CollegeId && !c.IsDeleted)
                .Select(c => c.Code)
                .FirstOrDefaultAsync();

            if (string.IsNullOrEmpty(collegeCode))
            {
                throw new KeyNotFoundException("The specified College does not exist or has been deleted.");
            }

            var departmentCode = await _db.Departments
                .Where(d => d.Id == dto.DepartmentId && !d.IsDeleted)
                .Select(d => d.Code)
                .FirstOrDefaultAsync();

            if (string.IsNullOrEmpty(departmentCode))
            {
                throw new KeyNotFoundException("The specified Department does not exist or has been deleted.");
            }

            string computedAlias = $"{collegeCode.Trim().ToUpper()}-{departmentCode.Trim().ToUpper()}";

            var newMapping = new CollegeDepartment
            {
                CollegeId = dto.CollegeId,
                DepartmentId = dto.DepartmentId,
                AliasCode = computedAlias,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId
            };

            _db.CollegeDepartments.Add(newMapping);
            await _db.SaveChangesAsync();

            return (await GetByIdAsync(newMapping.Id))!;
        }

        public async Task<bool> RemoveMappingAsync(int id, string userId, bool hardDelete = false)
        {
            var mapping = await _db.CollegeDepartments.FindAsync(id);
            if (mapping == null) return false;

            if (hardDelete)
            {
                _db.CollegeDepartments.Remove(mapping);
            }
            else
            {
                mapping.IsDeleted = true;
                mapping.DeletedAt = DateTime.UtcNow;
                mapping.DeletedBy = userId;
            }

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateStatusAsync(int id, bool isActive, string userId)
        {
            var mapping = await _db.CollegeDepartments.FindAsync(id);
            if (mapping == null) return false;

            mapping.IsActive = isActive;
            mapping.UpdatedAt = DateTime.UtcNow;
            mapping.UpdatedBy = userId;

            await _db.SaveChangesAsync();
            return true;
        }
    }
}
