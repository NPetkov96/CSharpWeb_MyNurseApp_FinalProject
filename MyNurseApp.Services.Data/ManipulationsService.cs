using Microsoft.AspNetCore.Http;
using MyNurseApp.Data.Models;
using MyNurseApp.Data.Repository.Interfaces;
using MyNurseApp.Web.ViewModels.Manipulations;

namespace MyNurseApp.Services.Data
{
    public class ManipulationsService
    {
        private readonly IRepository<MedicalManipulation, Guid> _manipulationRepository;
        private readonly IHttpContextAccessor _currentAccsessor;

        public ManipulationsService(IRepository<MedicalManipulation, Guid> manipulationRepository, IHttpContextAccessor httpContextAccessor)
        {
            this._manipulationRepository = manipulationRepository;
            this._currentAccsessor = httpContextAccessor;
        }

        public async Task<List<MedicalManipulationsViewModel>> GetAllManipulationsAsync()
        {

            var manipulations = await _manipulationRepository.GetAllAsync();

            List<MedicalManipulationsViewModel> viewMnipulations = new List<MedicalManipulationsViewModel>();

            foreach (var manipulation in manipulations)
            {
                MedicalManipulationsViewModel currenctManipulation = new MedicalManipulationsViewModel()
                {
                    Id = manipulation.Id,
                    Name = manipulation.Name,
                    Duration = manipulation.Duration,
                    Description = manipulation.Description,
                    Price = manipulation.Price
                };

                viewMnipulations.Add(currenctManipulation);
            }
            return viewMnipulations;
        }
    }
}
