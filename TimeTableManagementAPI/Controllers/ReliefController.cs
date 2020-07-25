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

        public IActionResult GetReleifSlots()
        {
            var Result = _reliefServices.FindVanantSlotsInADay();
            if (Result.GetType() == typeof(string))
                return BadRequest(Result);
            else
                return Ok(Result);
        }

        [HttpPost]
        public IActionResult AddAReleifToTeacher(Updates updates)
        {
            var Result = _reliefServices.AddAReliefUpdate(updates);
            if (Result.GetType() == typeof(string))
                return BadRequest(Result);
            else
                return Ok(Result);
        }

        [HttpPut]
        public IActionResult UpdateAReleifToTeacher(Updates updates)
        {
            var Result = _reliefServices.UpdateARelief(updates);
            if (Result.GetType() == typeof(string))
                return BadRequest(Result);
            else
                return Ok(Result);
        }

        [Route("ApproveReliefRequest")]
        public IActionResult ApproveReliefRequest(int Id)
        {
            var Result = _reliefServices.ApproveAreliefRequest(Id);
            if (Result == "Relief Updated")
                return Ok(Result);
            else
                return BadRequest(Result);
        }
        

    }
}
