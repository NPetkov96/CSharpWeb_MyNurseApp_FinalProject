namespace MyNurseApp.Web.ViewModels.AdminInformation
{
    public class ApplicationUserViewModel
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; } = null!;
        public string Role { get; set; } = null!;
        public bool HasNurseProfile { get; set; }
        public bool HasPatientProfile { get; set; }
    }
}
