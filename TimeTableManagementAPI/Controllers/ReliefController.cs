using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TimeTableManagementAPI.Models;
using TimeTableManagementAPI.Repository;
using TimeTableManagementAPI.Services;

namespace TimeTableManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReliefController:Controller
    {
        private IReliefServices _reliefServices;
        private ICommonRepository<Updates> _updatesRepository;
        public ReliefController(IReliefServices reliefServices,ICommonRepository<Updates> updatesRepository)
        {
            _reliefServices = reliefServices;
            _updatesRepository = updatesRepository;
        }

   
    }
}
