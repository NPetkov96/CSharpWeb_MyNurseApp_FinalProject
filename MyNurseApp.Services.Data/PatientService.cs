using Microsoft.AspNetCore.Http;
using MyNurseApp.Data.Models;
using MyNurseApp.Data.Repository.Interfaces;
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
        

        public async Task<bool> AddPatientAsync(PatientProfileinputModel inputModel, IHttpContextAccessor currentAccsessor)
        {

            var userId = _currentAccsessor.HttpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                throw new InvalidOperationException("User is not logged in.");
            }

            PatientProfile patient = new PatientProfile()
            {
                FirstName = inputModel.FirstName,
                LastName = inputModel.LastName,
                DateOfBirth = inputModel.DateOfBirth,
                HomeAddress = inputModel.HomeAddress,
                PhoneNumber = inputModel.PhoneNumber,
                EmergencyContactFullName = inputModel.EmergencyContactFullName,
                EmergencyContactPhone = inputModel.EmergencyContactPhone,
                Notes = inputModel.Notes,
                UserId = Guid.Parse(userId)
            };

            await _patientRepository.AddAsync(patient);

            return true;
        }
    }
}
