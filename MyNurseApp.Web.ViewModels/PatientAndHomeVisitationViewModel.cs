using MyNurseApp.Web.ViewModels.HomeVisitation;
using MyNurseApp.Web.ViewModels.PatientProfile;

namespace MyNurseApp.Web.ViewModels
{
    public class PatientAndHomeVisitationViewModel
    {
        public PatientProfileViewModel PatientProfile { get; set; } = null!;
        public HomeVisitationViewModel HomeVisitation { get; set; } = null!;
    }
}
