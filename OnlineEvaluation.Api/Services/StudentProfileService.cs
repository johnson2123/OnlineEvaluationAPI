using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OnlineEvaluation.Api.Data;
using OnlineEvaluation.Api.Models.DTO;
using OnlineEvaluation.Api.Services.IServices;

namespace OnlineEvaluation.Api.Services
{
    public class StudentProfileService : IStudentProfileService
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        public StudentProfileService(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<StudentProfileDto?> GetProfileByUserIdAsync(string applicationUserId)
        {
            var student = await _db.Students
                .Include(s => s.User)           
                .Include(s => s.AcademicMap)
                    .ThenInclude(am => am.Branch) 
                .FirstOrDefaultAsync(s => s.ApplicationUserId == applicationUserId && !s.IsDeleted);

            if (student == null)
            {
                return null;
            }
            return _mapper.Map<StudentProfileDto>(student);
        }

        public async Task<bool> UpdateProfileAsync(string applicationUserId, UpdateStudentProfileDto dto)
        {
            var student = await _db.Students
                .FirstOrDefaultAsync(s => s.ApplicationUserId == applicationUserId && !s.IsDeleted);

            if (student == null)
            {
                throw new KeyNotFoundException("No active student record maps to the provided user session context.");
            }

            student.ContactNumber = dto.ContactNumber?.Trim();
            student.Address = dto.Address?.Trim();
            student.BloodGroup = dto.BloodGroup?.ToUpper().Trim();

            student.UpdatedAt = DateTime.UtcNow;
            student.UpdatedBy = applicationUserId;

            _db.Students.Update(student);

            return await _db.SaveChangesAsync() > 0;
        }
    }
}
