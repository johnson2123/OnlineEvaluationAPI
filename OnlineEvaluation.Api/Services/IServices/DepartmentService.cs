using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OnlineEvaluation.Api.Data;
using OnlineEvaluation.Api.Models.DTO;
using OnlineEvaluation.Api.Models.Entities;

namespace OnlineEvaluation.Api.Services.IServices
{
    public class DepartmentService : IDepartmentService
    {
        private readonly ApplicationDbContext _db; 
        private readonly IMapper _mapper;

        public DepartmentService(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }
        public async Task<DepartmentDto> CreateAsync(CreateDepartmentDto dto, string? actorUserId)
        {
            var codeExists = await _db.Departments
                                .AnyAsync(d => d.Code == dto.Code);

            if (codeExists)
            {
                throw new InvalidOperationException($"A department with the code '{dto.Code}' already exists.");
            }

            var department = _mapper.Map<Department>(dto);

            department.CreatedBy = actorUserId ?? "System";
            department.CreatedAt = DateTime.UtcNow;

            _db.Departments.Add(department);
            await _db.SaveChangesAsync();

            return _mapper.Map<DepartmentDto>(department);
        }

        public async Task<IEnumerable<DepartmentDto>> GetAllAsync(int page, int pageSize)
        {
            var departments = await _db.Departments
                .AsNoTracking()
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return _mapper.Map<IEnumerable<DepartmentDto>>(departments);
        }

        public async Task<DepartmentDto?> GetByCodeAsync(string code)
        {
            var department = await _db.Departments
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Code == code);

            return department == null ? null : _mapper.Map<DepartmentDto>(department);
        }

        public async Task<DepartmentDto?> GetByIdAsync(int id)
        {
            var department = await _db.Departments
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == id);

            return department == null ? null : _mapper.Map<DepartmentDto>(department);
        }

        public async Task<bool> HardDeleteAsync(int id)
        {
            var department = await _db.Departments
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(d => d.Id == id);

            if (department == null) return false;

            _db.Departments.Remove(department);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SoftDeleteAsync(int id, string? actorUserId)
        {
            var department = await _db.Departments.FirstOrDefaultAsync(d => d.Id == id);
            if (department == null) return false;

            department.IsDeleted = true;
            department.DeletedBy = actorUserId;
            department.DeletedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateAsync(int id, UpdateDepartmentDto dto, string? actorUserId)
        {
            var department = await _db.Departments.FirstOrDefaultAsync(d => d.Id == id);
            if (department == null) return false;

            var codeExistsOnOtherRecord = await _db.Departments
                .AnyAsync(d => d.Code == dto.Code && d.Id != id);

            if (codeExistsOnOtherRecord)
            {
                throw new InvalidOperationException($"Another department with the code '{dto.Code}' already exists.");
            }

            _mapper.Map(dto, department);

            department.UpdatedBy = actorUserId;
            department.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return true;
        }
    }
}
