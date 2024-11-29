using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyNurseApp.Common.Enums;
using MyNurseApp.Data;
using MyNurseApp.Data.Models;
using MyNurseApp.Data.Repository.Interfaces;
using MyNurseApp.Web.ViewModels.NurseProfile;

namespace MyNurseApp.Services.Data
{
    public class NurseService
    {
        private readonly IRepository<NurseProfile, Guid> _nurseRepository;
        private readonly IRepository<ApplicationUser, Guid> _applicationRepository;
        private readonly IHttpContextAccessor _currentAccsessor;
        private readonly UserManager<ApplicationUser> _userManager;


        public NurseService(IRepository<ApplicationUser, Guid> applicationRepository, IRepository<NurseProfile, Guid> patientRepository, IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager)
        {
            this._nurseRepository = patientRepository;
            this._currentAccsessor = httpContextAccessor;
            this._userManager = userManager;
            this._applicationRepository = applicationRepository;
        }

        public async Task<NurseProfileViewModel> GetNurseProfileAsync()
        {
            var userId = _currentAccsessor.HttpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var model = await _nurseRepository.FirstOrDefaultAsync(m => m.UserId == Guid.Parse(userId!));

            if (model == null)
            {
                return null!;
            }
            return ConvertToViewModel(model);
        }

        public async Task<List<NurseProfileViewModel>> GetAllNursesAsync()
        {
            var profiles = await _nurseRepository.GetAllAsync();
            var viewProfiles = new List<NurseProfileViewModel>();

            foreach (var profile in profiles)
            {
                var model = ConvertToViewModel(profile);
                viewProfiles.Add(model);
            }

            return viewProfiles;
        }

        public async Task RegisterNurseAsync(NurseProfileViewModel viewModel)
        {
            var nurse = ConvertToModel(viewModel);
            nurse.IsRegistrated = true;
            await _nurseRepository.AddAsync(nurse);
        }

        private NurseProfileViewModel ConvertToViewModel(NurseProfile model)
        {
            var viewModel = new NurseProfileViewModel()
            {
                Id = model.Id,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Education = model.Education,
                MedicalLicenseNumber = model.MedicalLicenseNumber,
                PhoneNumber = model.PhoneNumber,
                Recommendations = model.Recommendations,
                YearsOfExperience = model.YearsOfExperience,
                IsConfirmed = model.IsConfirmed,
            };

            return viewModel;
        }

        private NurseProfile ConvertToModel(NurseProfileViewModel viewModel)
        {
            var userId = _currentAccsessor.HttpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var model = new NurseProfile()
            {
                Id = Guid.NewGuid(),
                FirstName = viewModel.FirstName,
                LastName = viewModel.LastName,
                Education = viewModel.Education,
                MedicalLicenseNumber = viewModel.MedicalLicenseNumber,
                PhoneNumber = viewModel.PhoneNumber,
                Recommendations = viewModel.Recommendations,
                YearsOfExperience = viewModel.YearsOfExperience,
                UserId = Guid.Parse(userId!)
            };

            return model;
        }

        public async Task AprooveNurseAync(Guid id)
        {

            var user = await _userManager.Users
                .Include(u => u.Nurse) 
                .FirstOrDefaultAsync(u => u.Nurse!.Id == id);

            user!.IsPending = false;
            user.Nurse!.IsConfirmed = NurseStatus.Approved;
            await _userManager.UpdateAsync(user);
        }
    }
}
