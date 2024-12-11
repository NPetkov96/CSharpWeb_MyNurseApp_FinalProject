using MyNurseApp.Web.ViewModels.PatientProfile;
using MyNurseApp.Web.ViewModels;

namespace MyNurseApp.Services.Data.Interfaces
{
    public interface IScheduleService
    {
        Task<IEnumerable<PatientAndHomeVisitationViewModel>> GetVisitationsForUserAsync();

        Task<IEnumerable<PatientAndHomeVisitationViewModel>> GetAllVisitationsAsync();

        Task<PatientProfileViewModel> GetPatientAync();

        Task AddHomeVisitationAsync(PatientAndHomeVisitationViewModel model);

        Task AssignVisitationToNurseAsync(Guid visitationId, Guid nurseId);

        Task DeleteHomeVisitationAsync(Guid visitationId);
    }
}
