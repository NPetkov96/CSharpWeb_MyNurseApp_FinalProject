using Microsoft.AspNetCore.Http;
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

        public PatientService(IRepository<PatientProfile, Guid> patientRepository, IHttpContextAccessor httpContextAccessor)
        {
            this._patientRepository = patientRepository;
            this._currentAccsessor = httpContextAccessor;
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
            var userId = _currentAccsessor.HttpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            PatientProfile patient = new PatientProfile()
            {
                //Id = model.Id,
                FirstName = model.FirstName,
                LastName = model.LastName,
                DateOfBirth = model.DateOfBirth,
                UIN = model.UIN,
                HomeAddress = model.HomeAddress,
                PhoneNumber = model.PhoneNumber,
                EmergencyContactFullName = model.EmergencyContactFullName,
                EmergencyContactPhone = model.EmergencyContactPhone,
                Notes = model.Notes,
                //UserId = Guid.Parse(userId!)
            };
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
    }
}
