using Microsoft.AspNetCore.Http;
using MyNurseApp.Data.Models;
using MyNurseApp.Data.Repository.Interfaces;
using MyNurseApp.Web.ViewModels.PatientProfile;

namespace MyNurseApp.Services.Data
{
    public class ScheduleService
    {
        private readonly IRepository<HomeVisitation, Guid> _visitationRepository;
        private readonly IRepository<PatientProfile, Guid> _patientRepository;
        private readonly IHttpContextAccessor _currentAccsessor;

        public ScheduleService(IRepository<HomeVisitation, Guid> scheduleRepository, IRepository<PatientProfile, Guid> patientRepository, IHttpContextAccessor httpContextAccessor)
        {
            this._visitationRepository = scheduleRepository;
            this._currentAccsessor = httpContextAccessor;
            this._patientRepository = patientRepository;
        }

        public async Task<PatientProfileViewModel> GetPatient()
        {
            var userId = _currentAccsessor.HttpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var patient = await _patientRepository.FirstOrDefaultAsync(p => p.UserId == Guid.Parse(userId!));

            if (patient == null)
            {
                throw new ArgumentException("No user found"); //make better exception handlig....
            }

            PatientProfileViewModel viewModel = new PatientProfileViewModel() 
            {
                Id = patient.Id,
                FirstName = patient.FirstName,
                LastName = patient.LastName,
                DateOfBirth = patient.DateOfBirth,
                UIN = patient.UIN,
                PhoneNumber = patient.PhoneNumber,
                EmergencyContactFullName = patient.EmergencyContactFullName,
                EmergencyContactPhone = patient.EmergencyContactPhone,
                HomeAddress = patient.HomeAddress,
                Notes = patient.Notes
            };

            return viewModel;
        }

    }
}
