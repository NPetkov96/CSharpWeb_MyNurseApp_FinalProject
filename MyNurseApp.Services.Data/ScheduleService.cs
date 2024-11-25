using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MyNurseApp.Data.Models;
using MyNurseApp.Data.Repository.Interfaces;
using MyNurseApp.Web.ViewModels;
using MyNurseApp.Web.ViewModels.HomeVisitation;
using MyNurseApp.Web.ViewModels.Manipulations;
using MyNurseApp.Web.ViewModels.PatientProfile;

namespace MyNurseApp.Services.Data
{
    public class ScheduleService
    {
        private readonly IRepository<HomeVisitation, Guid> _visitationRepository;
        private readonly IRepository<PatientProfile, Guid> _patientRepository;
        private readonly IRepository<MedicalManipulation, Guid> _manipulationRepository;
        private readonly IHttpContextAccessor _currentAccsessor;

        public ScheduleService(IRepository<MedicalManipulation, Guid> manipulationRepository, IRepository<HomeVisitation, Guid> scheduleRepository, IRepository<PatientProfile, Guid> patientRepository, IHttpContextAccessor httpContextAccessor)
        {
            this._visitationRepository = scheduleRepository;
            this._currentAccsessor = httpContextAccessor;
            this._patientRepository = patientRepository;
            this._manipulationRepository = manipulationRepository;
        }

        public async Task<IEnumerable<PatientAndHomeVisitationViewModel>> GetAllVisitationsAsync()
        {

            var visitationsListModel = await _visitationRepository.GetAllAttached()
                        .Include(hv => hv.Patient)
                        .Include(hv => hv.MedicalManipulations)
                        .ToListAsync();


            return visitationsListModel.Select(item => new PatientAndHomeVisitationViewModel
            {
                PatientProfile = item.Patient != null ? new PatientProfileViewModel
                {
                    Id = item.Patient.Id,
                    FirstName = item.Patient.FirstName,
                    LastName = item.Patient.LastName,
                    DateOfBirth = item.Patient.DateOfBirth,
                    UIN = item.Patient.UIN,
                    HomeAddress = item.Patient.HomeAddress,
                    PhoneNumber = item.Patient.PhoneNumber,
                    EmergencyContactFullName = item.Patient.EmergencyContactFullName,
                    EmergencyContactPhone = item.Patient.EmergencyContactPhone,
                    Notes = item.Patient.Notes
                } : new PatientProfileViewModel(),

                HomeVisitation = new HomeVisitationViewModel
                {
                    Id = item.Id,
                    DateTimeManipulation = item.DateTimeManipulation,
                    IsHomeVisitationConfirmed = item.IsHomeVisitationConfirmed,
                    Note = item.Note,
                    PaymentMethod = item.PaymentMethod,
                    PatientId = item.PatientId,
                    PriceForVisitation = item.PriceForVisitation
                },

                MedicalManipulations = item.MedicalManipulations?.Select(manipulation => new MedicalManipulationsViewModel
                {
                    Id = manipulation.Id,
                    Name = manipulation.Name,
                    Duration = manipulation.Duration,
                    Description = manipulation.Description,
                    Price = manipulation.Price
                }).ToList() ?? new List<MedicalManipulationsViewModel>()
            });
        }


        public async Task<PatientProfileViewModel> GetPatientAync()
        {
            var userId = _currentAccsessor.HttpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var patient = await _patientRepository.FirstOrDefaultAsync(p => p.UserId == Guid.Parse(userId!));

            if (patient == null)
            {
                throw new ArgumentException("No user found"); //make better exception handling....
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

        public async Task AddHomeVisitationAsync(PatientAndHomeVisitationViewModel model)
        {
            var isManipulationExist = await _visitationRepository.FirstOrDefaultAsync(m => m.Id == model.HomeVisitation.Id);
            if (isManipulationExist != null)
            {
                throw new ArgumentException("The manipulation already exist!"); //TODO better handling the exception.
            }
            decimal priceForVisitation = model.HomeVisitation.PriceForVisitation + model.MedicalManipulations.Sum(m => m.Price);

            List<MedicalManipulation> medicalManipulations = new List<MedicalManipulation>();

            foreach (var item in model.MedicalManipulations)
            {
                var existingManipulation = await _manipulationRepository.GetByIdAsync(item.Id);
                if (existingManipulation != null)
                {
                    medicalManipulations.Add(existingManipulation);
                }
            }

            HomeVisitation homeVisitation = new HomeVisitation()
            {
                Id = new Guid(),
                DateTimeManipulation = model.HomeVisitation.DateTimeManipulation,
                IsHomeVisitationConfirmed = false,
                Note = model.HomeVisitation.Note,
                PaymentMethod = model.HomeVisitation.PaymentMethod,
                PriceForVisitation = priceForVisitation,
                MedicalManipulations = medicalManipulations,
                PatientId = model.PatientProfile.Id
            };
            await _visitationRepository.AddAsync(homeVisitation);
        }

    }
}
