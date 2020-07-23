using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TimeTableManagementAPI.Models;
using TimeTableManagementAPI.Repository;
using TimeTableManagementAPI.Services;
using TimeTableManagementAPI.Utility;
using TimeTableManagementAPI.VM;

namespace TimeTableManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TimeTableController : Controller
    {
        private ICommonRepository<Time_Table> _timeTableRepo;
        private ITimeTableServices _timeTableServices;
        private ICommonRepository<Slot> _slotRepo;
        public TimeTableController(ICommonRepository<Time_Table> timeTableRepo, ITimeTableServices timeTableServices, ICommonRepository<Slot> slotRepo)
        {
            _timeTableRepo = timeTableRepo;
            _timeTableServices = timeTableServices;
            _slotRepo = slotRepo;
        }

        public IActionResult GetAllTimeTables()
        {
            var result = _timeTableRepo.GetAll("Time_table");
            if (result != null)
                return Ok(result);
            else
                return BadRequest("No time tables Found");

        }

        [HttpPost]
        public IActionResult CreateATimeTable([FromBody]Time_Table time_Table)
        {
            var Result = _timeTableServices.Add(time_Table);
            if (Result.GetType() == typeof(Time_Table))
                return Ok(Result);
            else
                return BadRequest(Result);
        }

        [HttpPut]
        public IActionResult UpdateATimeTable([FromBody]Time_Table time_Table)
        {
            var Result = _timeTableServices.Add(time_Table);
            if (Result.GetType() == typeof(Time_Table))
                return Ok(Result);
            else
                return BadRequest(Result);

        }

        [HttpDelete]
        public IActionResult DeleteATimeTable(int Id)
        {
            var allSLots = _slotRepo.GetByOneParameter("Slot", "Time_Table_Id",Id.ToString());
            foreach(var slot in allSLots)
            {
                _slotRepo.DeleteRecord("Slot", slot.Id);
            };
            var result = _timeTableRepo.DeleteRecord("Time_Table", Id);
            if (result)
                return Ok("Time Table Successfully deleted");
            else
                return BadRequest("Record not deleted");
        }

        [Route("{Id}")]
        public IActionResult GetATimeTableById(int Id)
        {
            var result = _timeTableRepo.GetById("Time_Table", Id);
            return Ok(result);
        }


        [Route("GetTimeTableDetails/{id}")]
        public IActionResult GetAllDetailsRelatedToATimeTable(int Id)
        {
            var Result = _timeTableServices.GetTimeTableDetails(Id);
            if (Result.GetType() == typeof(string))
                return BadRequest("Time Table Not FOund");
            else
                return Ok(Result);
        }

        [Route("GetTimeTableDetailsByClassId/{id}")]
        public IActionResult GetTimeTableDetailsByClassId(int Id)
        {
            var Result = _timeTableServices.GetDetailsOfATimeTableByClassId(Id);
            if (Result.GetType() == typeof(string))
                return BadRequest("Time Table Not FOund");
            else
                return Ok(Result);
        }

        [HttpPost]
        [Route("AddSlot")]
        public IActionResult CreateATimeTableSlot([FromBody]Slot slot)
        {
            var Result = _timeTableServices.CreateAPeriodSlot(slot);
            if (Result.GetType() == typeof(Slot))
                return Ok(Result);
            else
                return BadRequest(Result);
        }

        [HttpDelete]
        [Route("DeleteSlot/{Id}")]
        public IActionResult DeleteSlot(int Id)
        {
            var result = _slotRepo.DeleteRecord("Slot", Id);
            if (result)
                return Ok("Slot Successfully deleted");
            else
                return BadRequest("Record not deleted");
        }

        [HttpPut]
        [Route("UpdateSlot")]
        public IActionResult UpateSlot([FromBody]Slot slot)
        {
            var Result = _timeTableServices.UpdatePeriodSlot(slot);
            if (Result.GetType() == typeof(string))
                return BadRequest(Result); 
            else
                return Ok(Result);
        }

        [Route("getASlotById/{id}")]
        public IActionResult getASlotById(int Id)
        {
            var Result = _slotRepo.GetById("Slot", Id);
            if (Result != null)
                return Ok(Result);
            else
                return BadRequest("Slot not found");
        }


        [Route("GetAvailableTeachers")]
        public IActionResult GetAllTeachersAvailableForSlotForASubject(string PeriodNo,int SubjectId)
        {

            var Result=_timeTableServices.GetAllTeachersAvailableForSlotForASubject(PeriodNo, SubjectId);
            if (Result.GetType() == typeof(string))
                return BadRequest(Result); 
            else
                return Ok(Result);
        }

        [Route("GetTeachersSlots")]
        public IActionResult GetAllTeachersSlots(int Id)
        {
            var Result = _timeTableServices.AllSlotsOfATeacher(Id);
            if (Result.GetType() == typeof(string))
                return BadRequest(Result);
            else
                return Ok(Result);
        }
    }
}