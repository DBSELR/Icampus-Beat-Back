using ICampus_BusinessLogic.Interfaces;
using ICampus_Models.Common;
using ICampus_Models.Requests;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace ICampus_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomWiseNominalRollsController : BaseApiController
    {
        private readonly IRoomWiseNominalRollsService _svc;

        public RoomWiseNominalRollsController(IRoomWiseNominalRollsService svc)
        {
            _svc = svc;
        }

        /// <summary>
        /// Get list of semesters for dropdown
        /// Query: SELECT DISTINCT cast( SEM as varchar(250)) SEM,cast(sem as int )sem1 
        ///        FROM tbl_sh WHERE COURSE = '{Course}' and ExamMY = '{ExamMy}' ORDER BY sem1
        /// </summary>
        [HttpGet("semesters")]
        public async Task<IActionResult> GetSemesters([FromQuery] string course, [FromQuery] string examMY)
        {
            if (string.IsNullOrWhiteSpace(course) || string.IsNullOrWhiteSpace(examMY))
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "course and examMY are required parameters"
                });
            }

            var data = await _svc.GetSemestersAsync(course, examMY);

            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Semesters loaded successfully" : "No semesters found",
                Data = data
            });
        }

        /// <summary>
        /// Get list of exam dates for dropdown (depends on Semester and ExamType)
        /// Stored Procedure: Sp_REP_Nominal_LoadEdate
        /// </summary>
        [HttpGet("examdates")]
        public async Task<IActionResult> GetExamDates(
            [FromQuery] string course,
            [FromQuery] string sem,
            [FromQuery] string examMY,
            [FromQuery] string regulation,
            [FromQuery] string examType)
        {
            if (string.IsNullOrWhiteSpace(course) || string.IsNullOrWhiteSpace(sem) ||
                string.IsNullOrWhiteSpace(examMY) || string.IsNullOrWhiteSpace(regulation) ||
                string.IsNullOrWhiteSpace(examType))
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "course, sem, examMY, regulation, and examType are required parameters"
                });
            }

            var data = await _svc.GetExamDatesAsync(course, sem, examMY, regulation, examType);

            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Exam dates loaded successfully" : "No exam dates found",
                Data = data
            });
        }

        /// <summary>
        /// Get list of branches for dropdown (depends on Exam Date)
        /// Stored Procedure: Sp_REP_Nominal_LoadBranch
        /// </summary>
        [HttpGet("branches")]
        public async Task<IActionResult> GetBranches(
            [FromQuery] string course,
            [FromQuery] string sem,
            [FromQuery] string examMY,
            [FromQuery] string regulation,
            [FromQuery] string edate,
            [FromQuery] string examType)
        {
            if (string.IsNullOrWhiteSpace(course) || string.IsNullOrWhiteSpace(sem) ||
                string.IsNullOrWhiteSpace(examMY) || string.IsNullOrWhiteSpace(regulation) ||
                string.IsNullOrWhiteSpace(edate) || string.IsNullOrWhiteSpace(examType))
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "course, sem, examMY, regulation, edate, and examType are required parameters"
                });
            }

            var data = await _svc.GetBranchesAsync(course, sem, examMY, regulation, edate, examType);

            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Branches loaded successfully" : "No branches found",
                Data = data
            });
        }

        /// <summary>
        /// Get room-wise nominal rolls data
        /// Stored Procedure: Sp_REP_NominalRolls_ROOMWISE
        /// </summary>
        [HttpGet("data")]
        public async Task<IActionResult> GetRoomWiseNominalRollsData(
            [FromQuery] string course,
            [FromQuery] string examMY,
            [FromQuery] string regulation,
            [FromQuery] string examType,
            [FromQuery] string sem = null,
            [FromQuery] string edate = null,
            [FromQuery] string branch = null)
        {
            // Validate required parameters
            if (string.IsNullOrWhiteSpace(course) || string.IsNullOrWhiteSpace(examMY) ||
                string.IsNullOrWhiteSpace(regulation) || string.IsNullOrWhiteSpace(examType))
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "course, examMY, regulation, and examType are required parameters"
                });
            }

            var request = new RoomWiseNominalRollsRequest
            {
                Course = course,
                ExamMY = examMY,
                Regulation = regulation,
                ExamType = examType,
                Sem = sem ?? string.Empty,
                Edate = edate ?? string.Empty,
                Branch = branch ?? string.Empty
            };

            var data = await _svc.GetRoomWiseNominalRollsDataAsync(request);

            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Room-wise nominal rolls data loaded successfully" : "No room-wise nominal rolls data found",
                Data = data
            });
        }
    }
}

