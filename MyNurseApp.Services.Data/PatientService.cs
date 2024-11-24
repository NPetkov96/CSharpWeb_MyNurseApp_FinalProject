﻿using Microsoft.AspNetCore.Http;
using MyNurseApp.Data.Models;
using MyNurseApp.Data.Repository.Interfaces;
using MyNurseApp.Web.ViewModels.PatientProfile;

namespace MyNurseApp.Services.Data
{
    public class PatientService
    {
        private readonly IRepository<PatientProfile, Guid> _patientRepository;
        private readonly IHttpContextAccessor _currentAccsessor;

        public PatientService(IRepository<PatientProfile, Guid> patientRepository, IHttpContextAccessor httpContextAccessor)
        {
            this._patientRepository = patientRepository;
            this._currentAccsessor = httpContextAccessor;
        }

        public async Task<PatientProfileViewModel> GetPatientProfileByUserIdAsync(IHttpContextAccessor httpContextAccessor)
        {
            var userId = _currentAccsessor.HttpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            PatientProfile existingProfile = await _patientRepository.FirstOrDefaultAsync(p => p.UserId == Guid.Parse(userId!));

            if (existingProfile != null)
            {
                PatientProfileViewModel patientProfileViewModel = new PatientProfileViewModel()
                {
                    Id = existingProfile.Id,
                    FirstName = existingProfile.FirstName,
                    LastName = existingProfile.LastName,
                    DateOfBirth = existingProfile.DateOfBirth,
                    HomeAddress = existingProfile.HomeAddress,
                    PhoneNumber = existingProfile.PhoneNumber,
                    EmergencyContactFullName = existingProfile.EmergencyContactFullName,
                    EmergencyContactPhone = existingProfile.EmergencyContactPhone,
                    Notes = existingProfile.Notes
                };

                return patientProfileViewModel;
            }
                return null!;
        }


        public async Task<bool> AddPatientAsync(PatientProfileViewModel inputModel, IHttpContextAccessor currentAccsessor)
        {

            var userId = _currentAccsessor.HttpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                throw new InvalidOperationException("User is not logged in.");
            }

            PatientProfile patient = new PatientProfile()
            {
                Id = Guid.Parse(userId),
                FirstName = inputModel.FirstName,
                LastName = inputModel.LastName,
                DateOfBirth = inputModel.DateOfBirth,
                HomeAddress = inputModel.HomeAddress,
                PhoneNumber = inputModel.PhoneNumber,
                EmergencyContactFullName = inputModel.EmergencyContactFullName,
                EmergencyContactPhone = inputModel.EmergencyContactPhone,
                Notes = inputModel.Notes,
                UserId = Guid.Parse(userId)
            };

            await _patientRepository.AddAsync(patient);

            return true;
        }
    }
}
