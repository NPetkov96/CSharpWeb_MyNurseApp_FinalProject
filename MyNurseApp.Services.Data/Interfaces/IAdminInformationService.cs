using MyNurseApp.Web.ViewModels.AdminInformation;

namespace MyNurseApp.Services.Data.Interfaces
{
    public interface IAdminInformationService
    {
        Task<List<ApplicationUserViewModel>> GetAllUsersAsync();
        Task<bool> DeleteUserAsync(Guid id);
    }
}
