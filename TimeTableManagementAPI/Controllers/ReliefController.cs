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

        [Route("GetAvailableTeachersForReleif")]
        public IActionResult GetAvailableTeachersForReleif(string PeriodNo,int SubjectId)
        {
            var Result = _reliefServices.GetAllTeachersAvailableForSlotForASubject(PeriodNo, SubjectId);
            if (Result.GetType() == typeof(string))
                return BadRequest(Result);
            else
                return Ok(Result);
        }

        [HttpPost]
        public IActionResult AddAReleifToTeacher([FromBody]Updates updates)
        {
            var Result = _reliefServices.AddAReliefUpdate(updates);
            if (Result.GetType() == typeof(string))
                return BadRequest(Result);
            else
                return Ok(Result);
        }

        [HttpPut]
        public IActionResult UpdateAReleifToTeacher([FromBody]Updates updates)
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
        
        [Route("GetAllRelifAllacations")]
        public IActionResult GetAllRelifAllacations()
        {
            var Result = _reliefServices.GetAllReliefRequests();
            if (Result.GetType() == typeof(string))
                return BadRequest(Result);
            else
                return Ok(Result);
        }

        [Route("GetAllRelifAllacationsByTeacher")]
        public IActionResult GetAllRelifAllacations(int UserId)
        {
            var Result = _reliefServices.GetAllReleifAllocationsByTeacherId(UserId);
            if (Result.GetType() == typeof(string))
                return BadRequest(Result);
            else
                return Ok(Result);
        }
    }
}
