using ICampus_BusinessLogic.Interfaces;
using ICampus_Models.Common;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ICampus_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OmrNumberUpdateController : BaseApiController
    {
        private readonly IOmrNumberUpdateService _svc;

        public OmrNumberUpdateController(IOmrNumberUpdateService svc)
        {
            _svc = svc;
        }

        // GET api/omnumberupdate/load?regulation=&course=&exammy=
        // SP: PROC_REGNOVSOMR (@REGULATION, @COURSE, @EXAMMY)
        // Returns: aSHID, REGNO, GRP, SEM, TEMPCODE, PCODE, PNAME, OMRNUMBER, PKTNO, SCANNED_SNO, SNO
        [HttpGet("load")]
        public async Task<IActionResult> LoadOmrGrid(
            [FromQuery] string regulation,
            [FromQuery] string course,
            [FromQuery] string exammy)
        {
            if (string.IsNullOrWhiteSpace(regulation) || string.IsNullOrWhiteSpace(course) || string.IsNullOrWhiteSpace(exammy))
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Please provide regulation, course and exammy.",
                    Data = null
                });

            var data = await _svc.LoadOmrGridAsync(regulation, course, exammy);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "OMR grid loaded" : "No data found",
                Data = data
            });
        }

        // POST api/omnumberupdate/update
        // Iterates rows, updates OMRNUMBER per REGNO+PCODE+EXAMMY composite key
        // Validation: all rows must have a non-empty OmrNo ("PLEASE ENTER OMR NUMBER..")
        // SQL: UPDATE TBL_SH SET OMRNUMBER = @OmrNo WHERE REGNO=@Regno AND PCODE=@PCode AND EXAMMY=@ExamMY
        [HttpPost("update")]
        public async Task<IActionResult> UpdateOmrNumbers([FromBody] List<OmrUpdateRow> rows)
        {
            if (rows == null || !rows.Any())
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "No rows provided",
                    Data = null
                });

            foreach (var row in rows)
            {
                if (string.IsNullOrWhiteSpace(row.OmrNo))
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "PLEASE ENTER OMR NUMBER..",
                        Data = null
                    });
            }

            foreach (var row in rows)
                await _svc.UpdateOmrNumAsync(row.Regno, row.PCode, row.ExamMY, row.OmrNo);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "OMR NUMBER UPDATED SUCESSFULLY.",
                Data = null
            });
        }
    }

    public class OmrUpdateRow
    {
        public string Regno  { get; set; }
        public string PCode  { get; set; }
        public string ExamMY { get; set; }
        public string OmrNo  { get; set; }
    }
}
