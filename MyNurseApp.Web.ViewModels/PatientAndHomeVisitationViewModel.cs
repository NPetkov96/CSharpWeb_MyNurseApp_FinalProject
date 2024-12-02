using MyNurseApp.Web.ViewModels.HomeVisitation;
using MyNurseApp.Web.ViewModels.Manipulations;
using MyNurseApp.Web.ViewModels.PatientProfile;

namespace MyNurseApp.Web.ViewModels
{
    public class PatientAndHomeVisitationViewModel
    {
        public PatientProfileViewModel? PatientProfile { get; set; }
        public HomeVisitationViewModel HomeVisitation { get; set; } = null!;

        public IEnumerable<MedicalManipulationsViewModel> MedicalManipulations = new List<MedicalManipulationsViewModel>();
    }
}
