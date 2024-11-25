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
        private readonly List<MedicalManipulationsViewModel> manipulationsViewModellist;


        public ManipulationsService(IRepository<MedicalManipulation, Guid> manipulationRepository, IHttpContextAccessor httpContextAccessor)
        {
            this._manipulationRepository = manipulationRepository;
            this._currentAccsessor = httpContextAccessor;
            manipulationsViewModellist = new List<MedicalManipulationsViewModel>();
        }

        public async Task<List<MedicalManipulationsViewModel>> PatientBookManipulationAsync(Guid manipulationId)
        {
            var model = await _manipulationRepository.FirstOrDefaultAsync(m => m.Id == manipulationId);

            var viewModel = new MedicalManipulationsViewModel() 
            {
                Id = model.Id,
                Name = model.Name,
                Duration = model.Duration,
                Description = model.Description,
                Price = model.Price,
            };

            if (!manipulationsViewModellist.Any(m => m.Id == manipulationId))
            {
                manipulationsViewModellist.Add(viewModel);
            }
            return manipulationsViewModellist;
        }

        public async Task<MedicalManipulationsViewModel> GetByIdAsync(Guid id)
        {
            MedicalManipulation model = await _manipulationRepository.GetByIdAsync(id);

            var modelView = new MedicalManipulationsViewModel()
            {
                Id = model.Id,
                Name = model.Name,
                Duration = model.Duration,
                Description = model.Description,
                Price = model.Price,
            };

            return modelView;
        }

        public async Task AddManipulationAsync(MedicalManipulationsViewModel model)
        {
            var isManipulationExist = await _manipulationRepository.FirstOrDefaultAsync(m => m.Name == model.Name);

            if (isManipulationExist != null)
            {
                throw new ArgumentException("The manipulation already exist!"); //TODO better handling the exception.
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
            return viewMnipulations.OrderBy(m => m.Price);
        }

        public async Task RemoveManipulationAsync(Guid id)
        {
            var manipulationToDelete = await _manipulationRepository.FirstOrDefaultAsync(x => x.Id == id);
            if (manipulationToDelete == null)
            {
                throw new ArgumentException("The manipulation doestn exist!"); //TODO better handling the exception.
            }

            await _manipulationRepository.DeleteAsync(manipulationToDelete);
        }

        public async Task EditManipulationAsync(MedicalManipulationsViewModel model)
        {
            MedicalManipulation manipulation = new MedicalManipulation() 
            {
                Id = model.Id,
                Name = model.Name,
                Duration = model.Duration,
                Description = model.Description,
                Price = model.Price
            };

            await _manipulationRepository.UpdateAsync(manipulation);
        }

    }
}
