﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyNurseApp.Common.Enums;
using MyNurseApp.Data;
using MyNurseApp.Data.Models;
using MyNurseApp.Data.Repository.Interfaces;
using MyNurseApp.Web.ViewModels.HomeVisitation;
using MyNurseApp.Web.ViewModels.NurseProfile;
using MyNurseApp.Web.ViewModels.PatientProfile;
using System.Security.Claims;

namespace MyNurseApp.Services.Data
{
    public class NurseService
    {
        private readonly IRepository<NurseProfile, Guid> _nurseRepository;
        private readonly IRepository<HomeVisitation, Guid> _visitationRepository;
        private readonly IHttpContextAccessor _currentAccsessor;
        private readonly UserManager<ApplicationUser> _userManager;


        public NurseService(IRepository<HomeVisitation, Guid> visitationRepository, IRepository<NurseProfile, Guid> patientRepository, IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager)
        {
            this._nurseRepository = patientRepository;
            this._currentAccsessor = httpContextAccessor;
            this._userManager = userManager;
            this._visitationRepository = visitationRepository;
        }

        public async Task<NurseProfileViewModel> GetNurseProfileAsync()
        {
            var userId = _currentAccsessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var model = await _nurseRepository.FirstOrDefaultAsync(m => m.UserId == Guid.Parse(userId!));

            if (model == null)
            {
                return null!;
            }
            return ConvertToViewModel(model);
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
        public async Task EditNurseProfileAync(NurseProfileViewModel model)
        {
            var nurse = await _nurseRepository.GetByIdAsync(model.Id);
            if (nurse == null)
            {
                throw new ArgumentException("Nurse profile not found.");
            }

            // Проверка дали UserId е валиден
            if (!await _userManager.Users.AnyAsync(u => u.Id == model.UserId))
            {
                throw new ArgumentException("Invalid UserId. User does not exist.");
            }

            nurse.FirstName = model.FirstName;
            nurse.LastName = model.LastName;
            nurse.MedicalLicenseNumber = model.MedicalLicenseNumber;
            nurse.PhoneNumber = model.PhoneNumber;
            nurse.Education = model.Education;
            nurse.Recommendations = model.Recommendations;
            nurse.YearsOfExperience = model.YearsOfExperience;

            await _nurseRepository.UpdateAsync(nurse);
        }

        public async Task RegisterNurseAsync(NurseProfileViewModel viewModel)
        {
            bool isMedicalLicenseExist = _nurseRepository.GetAllAttached().Any(m => m.MedicalLicenseNumber == viewModel.MedicalLicenseNumber);
            if (isMedicalLicenseExist)
            {
                throw new ArgumentException("Medical License number exist"); //TODO better exception handling
            }
            var nurse = ConvertToModel(viewModel);
            nurse.IsRegistrated = true;
            await _nurseRepository.AddAsync(nurse);
        }

        public async Task AprooveNurseAync(Guid id)
        {

            var user = await _userManager.Users
                .Include(u => u.Nurse)
                .FirstOrDefaultAsync(u => u.Nurse!.Id == id);

            user!.IsPending = false;
            user.Nurse!.IsConfirmed = NurseStatus.Approved;
            await _userManager.UpdateAsync(user);
        }
        public async Task DeclineNurseAync(Guid id)
        {

            var user = await _userManager.Users
                .Include(u => u.Nurse)
                .FirstOrDefaultAsync(u => u.Nurse!.Id == id);

            user!.IsPending = true;
            user.Nurse!.IsConfirmed = NurseStatus.Declined;
            await _userManager.UpdateAsync(user);
        }
        public async Task DeleteNurseAync(Guid id)
        {
            var user = await _userManager.Users
                .Include(u => u.Nurse)
                .FirstOrDefaultAsync(u => u.Nurse!.Id == id);

            await _userManager.DeleteAsync(user!);
        }

        private NurseProfileViewModel ConvertToViewModel(NurseProfile model)
        {
            var viewModel = new NurseProfileViewModel()
            {
                Id = model.Id,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Education = model.Education,
                MedicalLicenseNumber = model.MedicalLicenseNumber,
                PhoneNumber = model.PhoneNumber,
                Recommendations = model.Recommendations,
                YearsOfExperience = model.YearsOfExperience,
                UserId = model.UserId,
                IsConfirmed = model.IsConfirmed,
            };

            return viewModel;
        }

        private NurseProfile ConvertToModel(NurseProfileViewModel viewModel)
        {
            var userId = _currentAccsessor.HttpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var model = new NurseProfile()
            {
                Id = Guid.NewGuid(),
                FirstName = viewModel.FirstName,
                LastName = viewModel.LastName,
                Education = viewModel.Education,
                MedicalLicenseNumber = viewModel.MedicalLicenseNumber,
                PhoneNumber = viewModel.PhoneNumber,
                Recommendations = viewModel.Recommendations,
                YearsOfExperience = viewModel.YearsOfExperience,
                UserId = Guid.Parse(userId!)
            };

            return model;
        }

        public async Task<ICollection<HomeVisitationViewModel>?> GetNurseHomeVisitatonsAync()
        {
            var userId = _currentAccsessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var nurse = await _nurseRepository.FirstOrDefaultAsync(n => n.UserId == Guid.Parse(userId!));
            var models = await _visitationRepository
                .GetAllAttached()
                .Where(v => v.Nurse == nurse && v.IsComplete == false)
                .Include(p => p.Patient)
                .Include(p => p.MedicalManipulations)
                .ToListAsync();

            var viewModelList = models.Select(v => new HomeVisitationViewModel()
            {
                Id = v.Id,
                DateTimeManipulation = v.DateTimeManipulation,
                MedicalManipulations = v.MedicalManipulations,
                Note = v.Note,
                IsHomeVisitationConfirmed = v.IsHomeVisitationConfirmed,
                PatientId = v.PatientId,
                PriceForVisitation = v.MedicalManipulations.Sum(p => p.Price) + v.PriceForVisitation,
                PaymentMethod = v.PaymentMethod,
                Patient = new PatientProfileViewModel
                {
                    Id = v.Patient.Id,
                    FirstName = v.Patient.FirstName,
                    LastName = v.Patient.LastName,
                    PhoneNumber = v.Patient.PhoneNumber,
                    HomeAddress = v.Patient.HomeAddress
                },
            })
                .ToList();

            return viewModelList;
        }

        public async Task GetNurseHomeVisitatonsAync(Guid id)
        {
            var currentVisitation = await _visitationRepository.FirstOrDefaultAsync(v => v.Id == id);
            currentVisitation.IsComplete = true;
            await _visitationRepository.UpdateAsync(currentVisitation);
            await GetNurseHomeVisitatonsAync();
        }
    }
}
