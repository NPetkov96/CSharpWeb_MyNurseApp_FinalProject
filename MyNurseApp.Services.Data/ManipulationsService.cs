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


            return null;
        }
    }
}
