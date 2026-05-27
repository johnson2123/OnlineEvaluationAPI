using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OnlineEvaluation.Api.Data;
using OnlineEvaluation.Api.Models.DTO;
using OnlineEvaluation.Api.Services.IServices;

namespace OnlineEvaluation.Api.Services
{
    public class StaffProfileService : IStaffProfileService
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;

        public StaffProfileService(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }
        public async Task<StaffProfileDto?> GetProfileByUserIdAsync(string applicationUserId)
        {
            var staff = await _db.StaffProfiles
                .Include(s => s.ApplicationUser)
                .Include(s => s.CollegeDepartment)
                    .ThenInclude(cd => cd.Department)
                .Include(s => s.CollegeDepartment)
                    .ThenInclude(cd => cd.College)
                .FirstOrDefaultAsync(s => s.ApplicationUserId == applicationUserId && !s.IsDeleted);

            if (staff == null)
            {
                return null;
            }

            return _mapper.Map<StaffProfileDto>(staff);
        }

        public async Task<bool> UpdateProfileAsync(string applicationUserId, UpdateStaffProfileDto dto)
        {
            var staff = await _db.StaffProfiles
                .FirstOrDefaultAsync(s => s.ApplicationUserId == applicationUserId && !s.IsDeleted);

            if (staff == null)
            {
                throw new KeyNotFoundException("No active staff record maps to the provided user session context.");
            }

            staff.PhoneNumber = dto.PhoneNumber?.Trim();
            staff.Address = dto.Address.Trim();

            staff.UpdatedAt = DateTime.UtcNow;
            staff.UpdatedBy = applicationUserId;

            _db.StaffProfiles.Update(staff);

            return await _db.SaveChangesAsync() > 0;
        }
    }
}
