using ICampus_BusinessLogic.Interfaces;
using ICampus_Models.Common;
using ICampus_Models.Requests;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ICampus_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomAbstractController : BaseApiController
    {
        private readonly IRoomAbstractService _svc;

        public RoomAbstractController(IRoomAbstractService svc)
        {
            _svc = svc;
        }

        /// <summary>
        /// Get list of semesters for dropdown
        /// Query: select distinct sem from tbl_sh where course='{Course}' order by sem
        /// </summary>
        [HttpGet("semesters")]
        public async Task<IActionResult> GetSemesters([FromQuery] string course)
        {
            if (string.IsNullOrWhiteSpace(course))
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "course is a required parameter"
                });
            }

            var data = await _svc.GetSemestersAsync(course);

            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Semesters loaded successfully" : "No semesters found",
                Data = data
            });
        }

        /// <summary>
        /// Get list of sessions for dropdown
        /// Stored Procedure: Spr_Load_Session
        /// </summary>
        [HttpGet("sessions")]
        public async Task<IActionResult> GetSessions(
            [FromQuery] string course,
            [FromQuery] string sem,
            [FromQuery] string examType)
        {
            if (string.IsNullOrWhiteSpace(course) || string.IsNullOrWhiteSpace(sem) || string.IsNullOrWhiteSpace(examType))
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "course, sem, and examType are required parameters"
                });
            }

            var data = await _svc.GetSessionsAsync(course, sem, examType);

            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Sessions loaded successfully" : "No sessions found",
                Data = data
            });
        }

        /// <summary>
        /// Get list of exam dates for dropdown
        /// Stored Procedure: Proc_Load_Edate
        /// </summary>
        [HttpGet("examdates")]
        public async Task<IActionResult> GetExamDates(
            [FromQuery] string course,
            [FromQuery] string sem,
            [FromQuery] string session,
            [FromQuery] string examMY,
            [FromQuery] string examType)
        {
            if (string.IsNullOrWhiteSpace(course) || string.IsNullOrWhiteSpace(sem) ||
                string.IsNullOrWhiteSpace(session) || string.IsNullOrWhiteSpace(examMY) ||
                string.IsNullOrWhiteSpace(examType))
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "course, sem, session, examMY, and examType are required parameters"
                });
            }

            var data = await _svc.GetExamDatesAsync(course, sem, session, examMY, examType);

            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Exam dates loaded successfully" : "No exam dates found",
                Data = data
            });
        }

        /// <summary>
        /// Get room abstract data for report
        /// Stored Procedures: SPR_LOAD_EXAMDATES (Regular) or SPR_LOAD_EXAMDATES_Supple (Supply)
        /// </summary>
        [HttpGet("data")]
        public async Task<IActionResult> GetRoomAbstractData(
            [FromQuery] string course,
            [FromQuery] string examMY,
            [FromQuery] string sem,
            [FromQuery] int session,
            [FromQuery] string edate,
            [FromQuery] string examType,
            [FromQuery] int regsup)
        {
            // Validate required parameters
            if (string.IsNullOrWhiteSpace(course) || string.IsNullOrWhiteSpace(examMY) ||
                string.IsNullOrWhiteSpace(sem) || session <= 0 ||
                string.IsNullOrWhiteSpace(edate) || string.IsNullOrWhiteSpace(examType))
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "course, examMY, sem, session (must be > 0), edate, and examType are required parameters"
                });
            }

            // Validate regsup value (should be 0, 1, or 2)
            if (regsup < 0 || regsup > 2)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "regsup must be 0 (Select/Reg), 1 (Reg), or 2 (Sup)"
                });
            }

            // Validate date format
            if (!DateTime.TryParse(edate, out _))
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "edate must be a valid date"
                });
            }

            var request = new RoomAbstractRequest
            {
                Course = course,
                ExamMY = examMY,
                Sem = sem,
                Session = session,
                EDate = edate,
                ExamType = examType,
                Regsup = regsup
            };

            var data = await _svc.GetRoomAbstractDataAsync(request);

            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() 
                    ? (regsup == 2 ? "Room abstract data (Supply) loaded successfully" : "Room abstract data (Regular) loaded successfully")
                    : "No room abstract data found",
                Data = data
            });
        }
    }
}

