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

        public async Task AddManipulationAsync(MedicalManipulationsViewModel model)
        {
            var isManipulationExist = _manipulationRepository.FirstOrDefaultAsync(m=>m.Name == model.Name);

            if (isManipulationExist != null)
            {
                throw new ArgumentException("The manipulation already exist!"); //TODO betther handling the exeption.
            }
            MedicalManipulation manipulation = new MedicalManipulation()
            {
                Id = Guid.NewGuid(),
                Name = model.Name,
                Duration = model.Duration,
                Description = model.Description,
                Price = model.Price
            };
           

            await _manipulationRepository.AddAsync(manipulation);
        }

        public async Task<IEnumerable<MedicalManipulationsViewModel>> GetAllManipulationsAsync()
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

        public async Task RemoveManipulationAsync(Guid id)
        {
            var manipulationToDelete = await _manipulationRepository.FirstOrDefaultAsync(x => x.Id == id);
            if (manipulationToDelete == null)
            {
                throw new ArgumentException("The manipulation doestn exist!"); //TODO betther handling the exeption.
            }

            await _manipulationRepository.DeleteAsync(manipulationToDelete);
        }
    }
}
