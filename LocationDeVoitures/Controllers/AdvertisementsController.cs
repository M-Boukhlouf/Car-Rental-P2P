﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using LocationDeVoitures.BindingModels;
using LocationDeVoitures.Helpers;
using LocationDeVoitures.Models;
using LocationDeVoitures.Models.Api;
using LocationDeVoitures.ViewModels;

namespace LocationDeVoitures.Controllers
{
    public class AdvertisementsController : Controller
    {
        private readonly ILogger<AdvertisementsController> _logger;

        public AdvertisementsController(ILogger<AdvertisementsController> logger)
        {
            _logger = logger;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<ActionResult> Index(int page = 1)
        {
            using var client = new ApiClient();
            User user;
            client.Token = Request.Cookies["token"]; ;
            user = await client.GetUser();

            int count = 10;
            var filter = new Filter
            {
                Start = (page - 1) * count,
                Count = count
            };

            var responseModel = await client.GetAdvertisements(filter);
            if (responseModel == null)
            {
                responseModel = new AdvertisementsBindingModel();
            }

            AdvertisementsViewModel model = new AdvertisementsViewModel
            {
                User = user,
                CurrentPage = page,
                MaxPage = (responseModel.Count / count) + 1,
                Advertisements = responseModel.Advertisements
            };
            return View(model);
        }
        public IActionResult Details()
        {
            return View();
        }
    }
}