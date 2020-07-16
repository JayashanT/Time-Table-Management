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

namespace TimeTableManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TimeTableController : Controller
    {
        ICommonRepository<Time_Table> _timeTableRepo;
        ITimeTableServices _timeTableServices;
        ICommonRepository<Slot> _slotRepo;
        DBContext _dBContext;
        public TimeTableController(ICommonRepository<Time_Table> timeTableRepo,ITimeTableServices timeTableServices, ICommonRepository<Slot> slotRepo)
        {
            _timeTableRepo = timeTableRepo;
            _timeTableServices = timeTableServices;
            _slotRepo = slotRepo;
            _dBContext = new DBContext();
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
        public IActionResult CreateATimeTable(Time_Table time_Table)
        {
            var Result = _timeTableServices.Add(time_Table);
            if (Result.GetType()==typeof(Time_Table))
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

        [HttpPost]
        [Route("AddSlot")]
        public IActionResult CreateATimeTableSlot(Slot slot)
        {
            var Result = _timeTableServices.CreateAPeriodSlot(slot);
            if (Result == "Teacher is not available for this slot")
                return BadRequest("Teacher is not available for this slot");
            else
                return Ok("Slot Created");
        }

        [HttpDelete]
        public IActionResult DeleteATimeTable(int Id)
        {
            var result=_timeTableRepo.DeleteRecord("Time_Table",Id);
            if(result)
                return Ok();
            else
                return BadRequest("Record not deleted");
        }

        [Route("GetAvailableTeachers")]
        public IActionResult GetAllTeachersAvailableForSlotForASubject(string PeriodNo,int SubjectId)
        {

            return Ok(_timeTableServices.GetAllTeachersAvailableForSlotForASubject(PeriodNo, SubjectId));
        }

       
        [Route("GetTimeTableDetails/{id}")]
        public IActionResult GetAllDetailsRelatedToATimeTable(int Id)
        {
            return Ok(_timeTableServices.GetTimeTableDetails(Id));
        }

        [Route("getASlotById/{id}")]
        public IActionResult getASlotById(int Id)
        {
            return Ok(_slotRepo.GetById("Slot",Id));
        }


    }
}