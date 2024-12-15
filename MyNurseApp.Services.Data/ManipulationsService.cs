using Microsoft.EntityFrameworkCore;
using MyNurseApp.Data.Models;
using MyNurseApp.Data.Repository.Interfaces;
using MyNurseApp.Services.Data.Interfaces;
using MyNurseApp.Web.ViewModels.Manipulations;


namespace MyNurseApp.Services.Data
{
    public class ManipulationsService : IManipulationsService
    {
        private readonly IRepository<MedicalManipulation, Guid> _manipulationRepository;
        private readonly List<MedicalManipulationsViewModel> manipulationsViewModellist;


        public ManipulationsService(IRepository<MedicalManipulation, Guid> manipulationRepository)
        {
            this._manipulationRepository = manipulationRepository;
            this.manipulationsViewModellist = new List<MedicalManipulationsViewModel>();
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
            var isManipulationExist = await _manipulationRepository.FirstOrDefaultAsync(m => m.Id == model.Id);

            if (isManipulationExist != null)
            {
                throw new InvalidOperationException("The manipulation already exist!");
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

        public async Task<IEnumerable<MedicalManipulationsViewModel>> GetAllManipulationsAsync(int pageNumber, int pageSize)
        {

            var manipulations = await _manipulationRepository
              .GetAllAttached()
              .OrderBy(m => m.Name)
              .Skip((pageNumber - 1) * pageSize)
              .Take(pageSize)
              .ToListAsync();

            return manipulations.Select(m => new MedicalManipulationsViewModel
            {
                Id = m.Id,
                Name = m.Name,
                Duration = m.Duration,
                Description = m.Description,
                Price = m.Price
            });
        }

        public int GetTotalCount()
        {
            return _manipulationRepository.GetAllAttached().Count();
        }

        public async Task RemoveManipulationAsync(Guid id)
        {
            var manipulationToDelete = await _manipulationRepository.FirstOrDefaultAsync(x => x.Id == id);
            if (manipulationToDelete == null)
            {
                throw new InvalidOperationException("The manipulation doestn exist!");
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

        public async Task<IEnumerable<MedicalManipulationsViewModel>> SearchManipulationsAsync(string query)
        {
            int pageNumber = 1;
            int pageSize = 7;
            if (string.IsNullOrWhiteSpace(query))
            {
                return await GetAllManipulationsAsync(pageNumber, pageSize);
            }

            var manipulations = await _manipulationRepository.GetAllAsync();

            return manipulations
                .Where(m => m.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                            (m.Description != null && m.Description.Contains(query, StringComparison.OrdinalIgnoreCase)))
                .Select(m => new MedicalManipulationsViewModel
                {
                    Id = m.Id,
                    Name = m.Name,
                    Description = m.Description,
                    Duration = m.Duration,
                    Price = m.Price
                })
                .OrderBy(m => m.Name)
                .ToList();
        }

        double IManipulationsService.GetTotalCount()
        {
            return _manipulationRepository.GetAllAttached().Count();
        }
    }
}
