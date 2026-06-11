using ICampus_Api.Controllers;
using ICampus_BusinessLogic.Interfaces;
using ICampus_Models.Common;
using ICampus_Models.Requests;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class MasterController : BaseApiController
{
    private readonly IMasterService _svc;
    public MasterController(IMasterService svc) => _svc = svc;

    // GET api/master/regular-data
    [HttpGet("regular-data")]
    public async Task<IActionResult> GetRegularData([FromQuery] string course, [FromQuery] string examMy, [FromQuery] string regulation)
    {
        var data = await _svc.GetRegularDataAsync(course, examMy, regulation);
        return Ok(new ApiResponse<object>
        {
            Success = data != null && data.Any(),
            Message = data != null && data.Any() ? "Regular data loaded" : "No regular data found",
            Data = data
        });
    }

    // POST api/master/update-pap
    [HttpPost("update-pap")]
    public async Task<IActionResult> UpdatePap([FromBody] UpdatePapRequest request)
    {
        var rows = await _svc.UpdatePapDataAsync(request);
        return Ok(new ApiResponse<object>
        {
            Success = rows > 0,
            Message = rows > 0 ? "Paper updated successfully" : "Update failed"
        });
    }

    // GET api/master/master-data
    [HttpGet("master-data")]
    public async Task<IActionResult> GetMasterData([FromQuery] string course, [FromQuery] string examMy, [FromQuery] string regulation)
    {
        var data = await _svc.LoadMasterDataAsync(course, examMy, regulation);
        return Ok(new ApiResponse<object>
        {
            Success = data != null && data.Any(),
            Message = data != null && data.Any() ? "Master data loaded" : "No master data found",
            Data = data
        });
    }

    // GET api/master/exists?course=...&examMy=...&batch=...&sem=...
    [HttpGet("exists")]
    public async Task<IActionResult> MasterExists([FromQuery] string course, [FromQuery] string examMy, [FromQuery] string batch, [FromQuery] string sem)
    {
        var exists = await _svc.MasterExistsAsync(course, examMy, batch, sem);
        return Ok(new ApiResponse<object>
        {
            Success = true,
            Message = "exists result",
            Data = exists // returns int 0/1
        });
    }

    // POST api/master/create
    [HttpPost("create")]
    public async Task<IActionResult> CreateMaster([FromBody] CreateMasterRequest request)
    {
        var rows = await _svc.CreateMasterAsync(request);
        return Ok(new ApiResponse<object>
        {
            Success = rows > 0,
            Message = rows > 0 ? "Master created successfully" : "Create failed"
        });
    }

    // Optional: export endpoint (returns binary file stream) — mirrors old btnExport behavior
    // GET api/master/export?course=...&examMy=...&regulation=...
    [HttpGet("export")]
    public async Task<IActionResult> Export([FromQuery] string course, [FromQuery] string examMy, [FromQuery] string regulation)
    {
        var excelBytes = await _svc.ExportPapDataExcelAsync(course, examMy, regulation); // returns byte[] or null
        if (excelBytes == null || excelBytes.Length == 0) return NotFound(new ApiResponse<object> { Success = false, Message = "No data to export" });

        return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "SubjectsData.xlsx");
    }
}
