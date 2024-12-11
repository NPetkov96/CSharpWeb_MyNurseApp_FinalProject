using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyNurseApp.Data;
using MyNurseApp.Data.Models;
using MyNurseApp.Data.Repository.Interfaces;
using MyNurseApp.Services.Data.Interfaces;
using MyNurseApp.Web.ViewModels.AdminInformation;

namespace MyNurseApp.Services.Data
{
    public class AdminInformationService : IAdminInformationService
    {
        private readonly IRepository<NurseProfile, Guid> _nurseRepository;
        private readonly IRepository<PatientProfile, Guid> _patientRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminInformationService(UserManager<ApplicationUser> userManager, IRepository<NurseProfile, Guid> nurseRepository, IRepository<PatientProfile, Guid> patientRepository)
        {
            this._userManager = userManager;
            this._nurseRepository = nurseRepository;
            this._patientRepository = patientRepository;
        }

        public async Task<List<ApplicationUserViewModel>> GetAllUsersAsync()
        {
            var users = await _userManager.Users
                .Include(u => u.Nurse)
                .Include(u => u.Patient)
                .ToListAsync();


            var userViewModels = users
                .Select(user => new ApplicationUserViewModel
                {
                    Id = user.Id,
                    FirstName = user.Nurse?.FirstName ?? user.Patient?.FirstName ?? "N/A",
                    LastName = user.Nurse?.LastName ?? user.Patient?.LastName ?? "N/A",
                    Email = user.Email!,
                    Role = _userManager.GetRolesAsync(user).Result.FirstOrDefault() ?? "No Role",
                    HasNurseProfile = user.Nurse != null,
                    HasPatientProfile = user.Patient != null
                })
                .OrderBy(u => u.Role == "User")
                .ThenBy(u => u.Role == "Nurse")
                .ThenBy(p => p.Role == "Admin")
                .ToList();

            return userViewModels;
        }

        public async Task<bool> DeleteUserAsync(Guid id)
        {
            var user = await _userManager.Users
                .Include(u => u.Nurse)
                .Include(u => u.Patient)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return false;
            }

            if (user.Nurse != null)
            {
                await _nurseRepository.DeleteAsync(user.Nurse);
            }

            if (user.Patient != null)
            {
                await _patientRepository.DeleteAsync(user.Patient);
            }

            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded;
        }
    }
}
