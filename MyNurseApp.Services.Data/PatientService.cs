using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyNurseApp.Data;
using MyNurseApp.Data.Models;
using MyNurseApp.Data.Repository.Interfaces;
using MyNurseApp.Web.ViewModels.NurseProfile;
using MyNurseApp.Web.ViewModels.PatientProfile;

namespace MyNurseApp.Services.Data
{
    public class PatientService
    {
        private readonly IRepository<PatientProfile, Guid> _patientRepository;
        private readonly IHttpContextAccessor _currentAccsessor;
        private readonly UserManager<ApplicationUser> _userManager;

        public PatientService(IRepository<PatientProfile, Guid> patientRepository, IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager)
        {
            this._patientRepository = patientRepository;
            this._currentAccsessor = httpContextAccessor;
            this._userManager = userManager;
        }

        public async Task<PatientProfileViewModel> GetPatientProfileByUserIdAsync(IHttpContextAccessor httpContextAccessor)
        {
            var userId = _currentAccsessor.HttpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            PatientProfile existingProfile = await _patientRepository.FirstOrDefaultAsync(p => p.UserId == Guid.Parse(userId!));

            if (existingProfile != null)
            {
                PatientProfileViewModel patientProfileViewModel = ConvertToViewModel(existingProfile);

                return patientProfileViewModel;
            }
            return null!;
        }


        public async Task<bool> AddPatientAsync(PatientProfileViewModel inputModel)
        {

            var userId = _currentAccsessor.HttpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                throw new InvalidOperationException("User is not logged in.");
            }

            PatientProfile patient = ConvertToModel(inputModel);

            await _patientRepository.AddAsync(patient);

            return true;
        }

        public async Task<PatientProfileViewModel> GetPatientProfileAync(Guid id)
        {
            var model = await _patientRepository.GetByIdAsync(id);

            var viewModel = ConvertToViewModel(model);

            return viewModel;
        }

        public async Task EditPatientProfileAync(PatientProfileViewModel model)
        {
            var patient = await _patientRepository.GetByIdAsync(model.Id);
            if (patient == null)
            {
                throw new ArgumentException("Patient not found");
            }

            patient.FirstName = model.FirstName;
            patient.LastName = model.LastName;
            patient.DateOfBirth = model.DateOfBirth;
            patient.UIN = model.UIN;
            patient.HomeAddress = model.HomeAddress;
            patient.PhoneNumber = model.PhoneNumber;
            patient.EmergencyContactFullName = model.EmergencyContactFullName;
            patient.EmergencyContactPhone = model.EmergencyContactPhone;
            patient.Notes = model.Notes;

            await _patientRepository.UpdateAsync(patient);
        }

        private PatientProfileViewModel ConvertToViewModel(PatientProfile model)
        {
            var viewModel = new PatientProfileViewModel()
            {
                Id = model.Id,
                FirstName = model.FirstName,
                LastName = model.LastName,
                DateOfBirth = model.DateOfBirth,
                UIN = model.UIN,
                HomeAddress = model.HomeAddress,
                PhoneNumber = model.PhoneNumber,
                EmergencyContactFullName = model.EmergencyContactFullName,
                EmergencyContactPhone = model.EmergencyContactPhone,
                Notes = model.Notes
            };

            return viewModel;
        }

        private PatientProfile ConvertToModel(PatientProfileViewModel viewModel)
        {
            var userId = _currentAccsessor.HttpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var model = new PatientProfile()
            {
                Id = Guid.NewGuid(),
                FirstName = viewModel.FirstName,
                LastName = viewModel.LastName,
                DateOfBirth = viewModel.DateOfBirth,
                UIN = viewModel.UIN,
                HomeAddress = viewModel.HomeAddress,
                PhoneNumber = viewModel.PhoneNumber,
                EmergencyContactFullName = viewModel.EmergencyContactFullName,
                EmergencyContactPhone = viewModel.EmergencyContactPhone,
                Notes = viewModel.Notes,
                UserId = Guid.Parse(userId!)
            };
            return model;
        }

        public async Task<List<PatientProfileViewModel>> GetAllPatientsAsync()
        {
            var patientsProfiles = await _patientRepository.GetAllAsync();
            var viewProfiles = new List<PatientProfileViewModel>();

            foreach (var profile in patientsProfiles)
            {
                var model = ConvertToViewModel(profile);
                viewProfiles.Add(model);
            }

            return viewProfiles;
        }

        public async Task DeletePatientAync(Guid id)
        {
            var user = await _userManager.Users
               .Include(u => u.Patient)
               .FirstOrDefaultAsync(u => u.Patient!.Id == id);

            await _userManager.DeleteAsync(user!);
        }
    }
}
