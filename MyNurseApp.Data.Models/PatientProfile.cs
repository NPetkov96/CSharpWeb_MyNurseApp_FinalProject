using MyNurseApp.Data.Models;

namespace MyNurseApp.Data
{
    public class PatientProfile
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string HomeAddress { get; set; }
        public string PhoneNumber { get; set; }
        public string EmergencyContactName { get; set; }
        public string EmergencyContactPhone { get; set; }
        public string? Notes { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
