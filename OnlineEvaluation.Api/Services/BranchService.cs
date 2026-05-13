using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OnlineEvaluation.Api.Data;
using OnlineEvaluation.Api.Models.DTO;
using OnlineEvaluation.Api.Models.Entities;
using OnlineEvaluation.Api.Services.IServices;

namespace OnlineEvaluation.Api.Services
{
    public class BranchService : IBranchService
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;

        public BranchService(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }
        public async Task<BranchDto> CreateAsync(CreateBranchDto dto, string? actorUserId)
        {
            var branch = _mapper.Map<Branch>(dto);
            branch.CreatedBy = actorUserId ?? "System";
            branch.CreatedAt = DateTime.UtcNow;

            _db.Branches.Add(branch);
            await _db.SaveChangesAsync();

            return _mapper.Map<BranchDto>(branch);
        }

        public async Task<IEnumerable<BranchDto>> GetAllAsync(int page, int pageSize)
        {
            return _mapper.Map<IEnumerable<BranchDto>>(
                await _db.Branches
                    .AsNoTracking()
                    .OrderBy(b => b.Name)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync()
            );
        }

        public async Task<BranchDto?> GetByCodeAsync(string code)
        {
            var branch = await _db.Branches
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Code.ToLower() == code.ToLower());

            return _mapper.Map<BranchDto?>(branch);
        }

        public async Task<BranchDto?> GetByIdAsync(int id)
        {
            var branch = await _db.Branches
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == id);

            return _mapper.Map<BranchDto?>(branch);
        }

        public async Task<bool> HardDeleteAsync(int id)
        {
            var branch = await _db.Branches
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(b => b.Id == id);

            if (branch == null) return false;

            _db.Branches.Remove(branch);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SoftDeleteAsync(int id, string? actorUserId)
        {
            var branch = await _db.Branches.FindAsync(id);
            if (branch == null) return false;

            branch.IsDeleted = true;
            branch.DeletedBy = actorUserId;
            branch.DeletedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateAsync(int id, UpdateBranchDto dto, string? actorUserId)
        {
            var branch = await _db.Branches.FindAsync(id);
            if (branch == null) return false;

            _mapper.Map(dto, branch);
            branch.UpdatedBy = actorUserId;
            branch.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return true;
        }
    }
}
