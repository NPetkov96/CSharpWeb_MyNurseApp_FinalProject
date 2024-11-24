namespace MyNurseApp.Web.ViewModels.Manipulations
{
    public class MedicalManipulationsViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public int Duration { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
    }
}
