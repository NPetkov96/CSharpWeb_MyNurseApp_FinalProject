using MyNurseApp.Web.ViewModels.HomeVisitation;
using MyNurseApp.Web.ViewModels.NurseProfile;

namespace MyNurseApp.Services.Data.Interfaces
{
    public interface INurseService
    {
        Task<NurseProfileViewModel> GetNurseProfileAsync();

        Task<List<NurseProfileViewModel>> GetAllNursesAsync();

        Task EditNurseProfileAync(NurseProfileViewModel model);

        Task RegisterNurseAsync(NurseProfileViewModel viewModel);

        Task AprooveNurseAync(Guid id);

        Task DeclineNurseAync(Guid id);

        Task DeleteNurseProfileAync(Guid id);

        Task<ICollection<HomeVisitationViewModel>?> GetNurseHomeVisitatonsAync();

        Task GetNurseHomeVisitatonsAync(Guid id);
    }
}
