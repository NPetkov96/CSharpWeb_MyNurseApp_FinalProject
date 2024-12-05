using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using MyNurseApp.Data.Models;
using MyNurseApp.Data.Repository.Interfaces;
using MyNurseApp.Web.ViewModels.PatientProfile;
using System.Security.Claims;

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
            var userId = _currentAccsessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            PatientProfile existingProfile = await _patientRepository.FirstOrDefaultAsync(p => p.UserId == Guid.Parse(userId!));

            if (existingProfile != null)
            {
                PatientProfileViewModel patientProfileViewModel = ConvertToViewModel(existingProfile);

                return patientProfileViewModel;
            }
            return null!;
        }


        public async Task AddPatientAsync(PatientProfileViewModel inputModel)
        {
            if (inputModel.DateOfBirth > DateTime.Now)
            {
                throw new InvalidOperationException("Date of birth cannot be in the future.");
            }

            int age = DateTime.Now.Year - inputModel.DateOfBirth!.Value.Year;
            if (inputModel.DateOfBirth.Value.Date > DateTime.Now.AddYears(-age)) age--;

            if (age < 18)
            {
                throw new InvalidOperationException("Patient must be at least 18 years old.");
            }

            var userId = _currentAccsessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                throw new InvalidOperationException("User is not logged in.");
            }

            if (_patientRepository.GetAllAttached().Any(p=>p.UIN == inputModel.UIN))
            {
                throw new InvalidOperationException($"Profile with {inputModel.UIN} already exist.");
            }
            PatientProfile patient = ConvertToModel(inputModel);

            await _patientRepository.AddAsync(patient);

            if(!_patientRepository.GetAllAttached().Any(p=>p.Id == patient.Id))
            {
                throw new InvalidOperationException("Profile could not be created.");
            }
        }

        public async Task<PatientProfileViewModel> GetPatientProfileAync(Guid id)
        {
            var model = await _patientRepository.GetByIdAsync(id);
            if(model == null)
            {
                throw new InvalidOperationException("No user found");
            }
            var viewModel = ConvertToViewModel(model);

            return viewModel;
        }

        public async Task EditPatientProfileAync(PatientProfileViewModel model)
        {
            var patient = await _patientRepository.GetByIdAsync(model.Id);
            if (patient == null)
            {
                throw new InvalidOperationException("Patient not found");
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

        public async Task<List<PatientProfileViewModel>> GetAllPatientsAsync()
        {
            var patientsProfiles = await _patientRepository.GetAllAsync();
            var viewProfiles = patientsProfiles.Select(p=> ConvertToViewModel(p)).ToList();

            return viewProfiles;
        }

        public async Task DeletePatientAync(Guid id)
        {
            var patientProfile = await _patientRepository.GetByIdAsync(id);
            await _patientRepository.DeleteAsync(patientProfile);

            if (_patientRepository.GetAllAttached().Any(u => u.Id == id))
            {
                throw new InvalidOperationException("User could not be deleted.");
            }
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
            var userId = _currentAccsessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
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

    }
}
