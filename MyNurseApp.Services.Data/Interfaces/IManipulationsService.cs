using MyNurseApp.Web.ViewModels.Manipulations;

namespace MyNurseApp.Services.Data.Interfaces
{
    public interface IManipulationsService
    {
        Task<List<MedicalManipulationsViewModel>> PatientBookManipulationAsync(Guid manipulationId);
        Task<MedicalManipulationsViewModel> GetByIdAsync(Guid id);
        Task AddManipulationAsync(MedicalManipulationsViewModel model);
        Task<IEnumerable<MedicalManipulationsViewModel>> GetAllManipulationsAsync(int pageNumber, int pageSize);
        Task RemoveManipulationAsync(Guid id);
        Task EditManipulationAsync(MedicalManipulationsViewModel model);
        Task<IEnumerable<MedicalManipulationsViewModel>> SearchManipulationsAsync(string query);
        double GetTotalCount();
    }
}
