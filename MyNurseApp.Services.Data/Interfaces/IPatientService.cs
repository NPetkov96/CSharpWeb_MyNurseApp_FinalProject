using Microsoft.AspNetCore.Http;
using MyNurseApp.Web.ViewModels.PatientProfile;

namespace MyNurseApp.Services.Data.Interfaces
{
    public interface IPatientService
    {
        Task<PatientProfileViewModel> GetPatientProfileByUserIdAsync(IHttpContextAccessor _currentAccsessor);

        Task AddPatientAsync(PatientProfileViewModel inputModel);

        Task<PatientProfileViewModel> GetPatientProfileAync(Guid id);

        Task EditPatientProfileAync(PatientProfileViewModel model);

        Task<List<PatientProfileViewModel>> GetAllPatientsAsync();

        Task DeletePatientAync(Guid id);
    }
}
