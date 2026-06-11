using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using ICampus_BusinessLogic.Interfaces;
using ICampus_Models.Requests;
using ICampus_Models.Common;

namespace ICampus_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExamRegistrationController : ControllerBase
    {
        private readonly IExamRegistrationService _svc;

        public ExamRegistrationController(IExamRegistrationService svc)
        {
            _svc = svc;
        }

        // 1. Get Sems for Exam Registration
        [HttpGet("sems")]
        public async Task<IActionResult> GetSems([FromQuery] string course, [FromQuery] string examMy, [FromQuery] string regulation)
        {
            var data = await _svc.GetSemsForExamRegAsync(course, examMy, regulation);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Sems loaded" : "No sem data",
                Data = data
            });
        }

        // 2. Get Student Details
        [HttpGet("student")]
        public async Task<IActionResult> GetStudent([FromQuery] string regNo)
        {
            var data = await _svc.GetStudentDetailsAsync(regNo);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Student loaded" : "Student not found",
                Data = data
            });
        }

        // 3. Get Failed Papers / Required Data for ExamReg
        [HttpGet("failed-papers")]
        public async Task<IActionResult> GetFailedPapers([FromQuery] string regNo, [FromQuery] string examMy = "", 
            [FromQuery] string course = "", [FromQuery] string regulation = "", [FromQuery] string sem = "")
        {
            // Check if student is already registered
            if (!string.IsNullOrWhiteSpace(sem))
            {
                var isRegistered = await _svc.CheckAlreadyRegisteredAsync(regNo, sem, examMy);
                if (isRegistered)
                {
                    return Ok(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "No Data", // Client expects "No Data" message
                        Data = null
                    });
                }
            }

            // Check exam notification date
            if (!string.IsNullOrWhiteSpace(course) && !string.IsNullOrWhiteSpace(regulation) && !string.IsNullOrWhiteSpace(sem))
            {
                var notificationStatus = await _svc.CheckExamNotificationAsync(examMy, course, regulation, sem);
                if (notificationStatus == "EXCEEDED")
                {
                    return Ok(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Please check the notification date", // Client expects this exact message
                        Data = null
                    });
                }
            }

            var data = await _svc.GetDataForExamRegAsync(regNo, examMy, course, regulation, sem);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Failed papers loaded" : "No failed papers",
                Data = data
            });
        }

        // 4. Check Regs Up for Exam
        // ExamRegistrationController.cs
        [HttpGet("regsup-check")]
        public async Task<IActionResult> CheckRegsUp([FromQuery] int regu, [FromQuery] int sem, [FromQuery] string examMy, [FromQuery] string course)
        {
            var status = await _svc.CheckRegsUpAsync(regu, sem, examMy, course);

            return Ok(new ApiResponse<string>
            {
                Success = string.Equals(status, "TRUE", StringComparison.OrdinalIgnoreCase),
                Message = "Status returned",
                Data = status // will be "TRUE" or "FALSE" (or fallback string)
            });
        }

        // 5. Get Fee Structure (Regular)
        [HttpGet("fee/regular")]
        public async Task<IActionResult> GetFeeStructureRegular([FromQuery] string regu, [FromQuery] string sem,
            [FromQuery] string course, [FromQuery] string grp, [FromQuery] string examMy,
            [FromQuery] string regulation, [FromQuery] string regNo)
        {
            var data = await _svc.GetFeeStructureRegularAsync(regu, sem, course, grp, examMy, regulation, regNo);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Fee structure loaded" : "No fee structure",
                Data = data
            });
        }

        // 6. Get Fee Structure (Supply)
        [HttpGet("fee/supply")]
        public async Task<IActionResult> GetFeeStructureSupply([FromQuery] int fCount, [FromQuery] string course,
            [FromQuery] string examMy, [FromQuery] string regulation, [FromQuery] string regNo, [FromQuery] int sem)
        {
            var data = await _svc.GetFeeStructureSupplyAsync(fCount, course, examMy, regulation, regNo, sem);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Supply fee loaded" : "No data",
                Data = data
            });
        }

        // 7. Register (Regular or Supply)
        // IMPORTANT: The RegSup value determines registration type:
        // - "REG" = Regular registration (uses sp_SH_ExamReg_Update)
        // - "SUP" = Supply/Supplementary registration (uses SP_SH_EXAMREG_SAVE)
        // The RegSup value should come from the failed papers data (SPM_REQ_DATA_FOR_EXAMREG returns REGSUP column)
        // If RegSup is not provided, the IsSupply flag will be used as fallback
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] ExamRegisterRequest req)
        {
            // Validate required fields
            if (req == null || string.IsNullOrWhiteSpace(req.RegNo) || string.IsNullOrWhiteSpace(req.ExamMy))
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "RegNo and ExamMy are required"
                });
            }

            // Check exam notification date
            if (!string.IsNullOrWhiteSpace(req.Course) && !string.IsNullOrWhiteSpace(req.Regulation) && !string.IsNullOrWhiteSpace(req.Sem))
            {
                var notificationStatus = await _svc.CheckExamNotificationAsync(req.ExamMy, req.Course, req.Regulation, req.Sem);
                if (notificationStatus == "EXCEEDED")
                {
                    return Ok(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Please check the notification date", // Client expects this exact message
                        Data = null
                    });
                }
            }

            var rows = await _svc.RegisterAsync(req);
            return Ok(new ApiResponse<object>
            {
                Success = rows > 0,
                Message = rows > 0 ? "Registration completed" : "Registration failed"
            });
        }

        // Check exam notification status
        [HttpGet("check-notification")]
        public async Task<IActionResult> CheckNotification([FromQuery] string examMy, [FromQuery] string course, 
            [FromQuery] string regulation, [FromQuery] string sem)
        {
            var status = await _svc.CheckExamNotificationAsync(examMy, course, regulation, sem);
            return Ok(new ApiResponse<object>
            {
                Success = status != "EXCEEDED",
                Message = status == "EXCEEDED" ? "Please check the notification date" : "Notification is valid",
                Data = new { Status = status }
            });
        }

        // Check if student is already registered
        [HttpGet("check-registered")]
        public async Task<IActionResult> CheckRegistered([FromQuery] string regNo, [FromQuery] string sem, [FromQuery] string examMy)
        {
            var isRegistered = await _svc.CheckAlreadyRegisteredAsync(regNo, sem, examMy);
            return Ok(new ApiResponse<object>
            {
                Success = !isRegistered,
                Message = isRegistered ? "No Data" : "Student is not registered",
                Data = new { IsRegistered = isRegistered }
            });
        }

        // 8. Unregister
        [HttpPost("unregister")]
        public async Task<IActionResult> UnRegister([FromBody] UnRegisterRequest req)
        {
            var rows = await _svc.UnRegisterAsync(req);
            return Ok(new ApiResponse<object>
            {
                Success = rows > 0,
                Message = rows > 0 ? "Un-registration completed" : "Unregister failed"
            });
        }

        // 9. Pay Exam Fee
        [HttpPost("fee/pay")]
        public async Task<IActionResult> PayExamFee([FromBody] ExamFeePayRequest req)
        {
            var dt = await _svc.PayExamFeeAsync(req);
            return Ok(new ApiResponse<object>
            {
                Success = dt != null,
                Message = dt != null ? "Fee payment processed" : "Payment failed",
                Data = dt
            });
        }

        // 10. Registered List
        [HttpGet("registered-list")]
        public async Task<IActionResult> RegisteredList([FromQuery] string examMy, [FromQuery] string course,
            [FromQuery] string regulation, [FromQuery] string status = "REG")
        {
            var data = await _svc.ExamRegisteredListAsync(examMy, course, regulation, status);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "List loaded" : "No records",
                Data = data
            });
        }

        // 11. Duplicate Receipt
        [HttpGet("receipt/duplicate")]
        public async Task<IActionResult> DuplicateReceipt([FromQuery] string receiptNo)
        {
            var data = await _svc.DuplicateReceiptAsync(receiptNo);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Duplicate receipt data" : "No receipts",
                Data = data
            });
        }

        // 12. Print Receipt
        [HttpGet("receipt/print")]
        public async Task<IActionResult> PrintReceipt([FromQuery] string receiptNo)
        {
            var data = await _svc.PrintReceiptAsync(receiptNo);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Receipt data loaded" : "No receipt found",
                Data = data
            });
        }

        // 13. Register All — bulk register all students in a batch-sem
        // Triggered by Ok_Click (btReg_All) after selecting from ddlBatch_Sem modal
        // Confirmed from DLL IL: DataAccessLayer.dll Regular_Exmas_Update
        // UPDATE TBL_SH SET REGD = 'Y' WHERE COURSE=? AND REGU=? AND SEM=?
        [HttpPost("register-all")]
        public async Task<IActionResult> RegisterAll([FromBody] RegisterAllRequest req)
        {
            if (req == null || string.IsNullOrWhiteSpace(req.Course) ||
                string.IsNullOrWhiteSpace(req.Regu) || string.IsNullOrWhiteSpace(req.Sem))
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Course, Regu and Sem are required"
                });
            }

            var rows = await _svc.RegisterAllAsync(req);
            return Ok(new ApiResponse<object>
            {
                Success = rows > 0,
                Message = rows > 0 ? $"Registered all — {rows} rows updated" : "No rows updated"
            });
        }

        // 14. Unregister All — bulk unregister all students in a batch-sem
        // Triggered by btnUnRege_All_Click after selecting from ddlBatch_Sem modal
        // Confirmed from DLL IL: DataAccessLayer.dll Regular_Exmas_Update_UnRegester
        // UPDATE TBL_SH SET REGD = null WHERE COURSE=? AND REGU=? AND SEM=?
        [HttpPost("unregister-all")]
        public async Task<IActionResult> UnRegisterAll([FromBody] RegisterAllRequest req)
        {
            if (req == null || string.IsNullOrWhiteSpace(req.Course) ||
                string.IsNullOrWhiteSpace(req.Regu) || string.IsNullOrWhiteSpace(req.Sem))
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Course, Regu and Sem are required"
                });
            }

            var rows = await _svc.UnRegisterAllAsync(req);
            return Ok(new ApiResponse<object>
            {
                Success = rows > 0,
                Message = rows > 0 ? $"Unregistered all — {rows} rows updated" : "No rows updated"
            });
        }

        // 16. Get Batch-Semester data for modal dropdown
        // Used when Register/Unregister button is clicked - opens modal with Batch-Semester dropdown
        [HttpGet("batch-semester")]
        public async Task<IActionResult> GetBatchSemester([FromQuery] string course, [FromQuery] string examMy)
        {
            if (string.IsNullOrWhiteSpace(course) || string.IsNullOrWhiteSpace(examMy))
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Course and ExamMy are required"
                });
            }

            var data = await _svc.GetBatchSemesterAsync(course, examMy);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Batch-Semester data loaded" : "No batch-semester data found",
                Data = data
            });
        }
    }
}
