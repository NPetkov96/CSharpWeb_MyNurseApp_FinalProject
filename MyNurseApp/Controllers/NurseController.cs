﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyNurseApp.Services.Data;
using MyNurseApp.Web.ViewModels.NurseProfile;

namespace MyNurseApp.Controllers
{
    public class NurseController : Controller
    {
        private readonly NurseService _nurseService;

        public NurseController(NurseService nurseService)
        {
            this._nurseService = nurseService;
        }


        public async Task<IActionResult> Index()
        {
            await Task.CompletedTask;
            return View();
        }

        public async Task<IActionResult> Profile()
        {
            var viewModels = await _nurseService.GetNurseProfileAsync();
            return View(viewModels);
        }

        [HttpGet]
        public async Task<IActionResult> CreateNurseProfile()
        {
            var viewModels = await _nurseService.GetNurseProfileAsync();
            if (viewModels != null)
            {
                return RedirectToAction("Profile", viewModels);
            }
            await Task.CompletedTask;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateNurseProfile(NurseProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            await _nurseService.RegisterNurseAsync(model);

            return View("Profile");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllNursesProfiles()
        {
            var viewModels = await _nurseService.GetAllNursesAsync();
            return View(viewModels);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AprooveNurse(Guid id)
        {
            await _nurseService.AprooveNurseAync(id);
            await Task.CompletedTask;
            return RedirectToAction("GetAllNursesProfiles");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeclineNurse(Guid id)
        {
            await _nurseService.DeclineNurseAync(id);
            await Task.CompletedTask;
            return RedirectToAction("GetAllNursesProfiles");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteNurse(Guid id)
        {
            await _nurseService.DeleteNurseAync(id);
            await Task.CompletedTask;
            return RedirectToAction("GetAllNursesProfiles");
        }
    }
}
