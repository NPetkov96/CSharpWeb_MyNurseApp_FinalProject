using Microsoft.AspNetCore.Http;
using MyNurseApp.Common.Enums;
using MyNurseApp.Data;
using MyNurseApp.Data.Repository.Interfaces;
using MyNurseApp.Web.ViewModels.NurseProfile;

namespace MyNurseApp.Services.Data
{
    public class NurseService
    {
        private readonly IRepository<NurseProfile, Guid> _nurseRepository;
        private readonly IHttpContextAccessor _currentAccsessor;

        public NurseService(IRepository<NurseProfile, Guid> patientRepository, IHttpContextAccessor httpContextAccessor)
        {
            this._nurseRepository = patientRepository;
            this._currentAccsessor = httpContextAccessor;
        }

        public async Task<List<NurseProfileViewModel>> GetAllNursesAsync()
        {
            var profiles = await _nurseRepository.GetAllAsync();
            var viewProfiles = new List<NurseProfileViewModel>();

            foreach (var profile in profiles)
            {
               var model = ConvertToViewModel(profile);
                viewProfiles.Add(model);
            }

            return viewProfiles;
        }

        public async Task RegisterNurseAsync(NurseProfileViewModel viewModel)
        {
            var nurse = ConvertToModel(viewModel);

            await _nurseRepository.AddAsync(nurse);
        }

        private NurseProfileViewModel ConvertToViewModel(NurseProfile model)
        {
            var viewModel = new NurseProfileViewModel()
            {
                Id = model.Id,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Education = model.Education,
                PhoneNumber = model.PhoneNumber,
                Recommendations = model.Recommendations,
                YearsOfExperience = model.YearsOfExperience
            };

            return viewModel;
        }

        private NurseProfile ConvertToModel(NurseProfileViewModel viewModel)
        {
            var model = new NurseProfile()
            {
                Id = viewModel.Id,
                FirstName = viewModel.FirstName,
                LastName = viewModel.LastName,
                Education = viewModel.Education,
                PhoneNumber = viewModel.PhoneNumber,
                Recommendations = viewModel.Recommendations,
                YearsOfExperience = viewModel.YearsOfExperience
            };

            return model;
        }
    }
}
