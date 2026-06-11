using ICampus_BusinessLogic.Interfaces;
using ICampus_Models.Common;
using ICampus_Models.Requests;
using Microsoft.AspNetCore.Mvc;

namespace ICampus_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BranchPriorityMasterController : BaseApiController
    {
        private readonly IBranchPriorityMasterService _svc;
        public BranchPriorityMasterController(IBranchPriorityMasterService svc) => _svc = svc;

        // GET api/BranchPriorityMaster/course/branches?course=B.TECH&regu=24
        [HttpGet("course/branches")]
        public async Task<IActionResult> GetCourseBranches([FromQuery] string course, [FromQuery] string regu)
        {
            var data = await _svc.LoadCourseBranchesAsync(course, regu);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Branches loaded" : "No branches found",
                Data = data
            });
        }

        // POST api/BranchPriorityMaster/save
        [HttpPost("save")]
        public async Task<IActionResult> SaveBranch([FromBody] BranchSaveRequest request)
        {
            var rows = await _svc.SaveBranchMasterAsync(request);
            return Ok(new ApiResponse<object>
            {
                Success = rows > 0,
                Message = rows > 0 ? "Branch master saved" : "Save failed"
            });
        }

        // GET api/BranchPriorityMaster/roommaster?id=0&session=FN
        [HttpGet("roommaster")]
        public async Task<IActionResult> LoadRoomMaster([FromQuery] int? id, [FromQuery] string session)
        {
            var req = new RoomMasterListRequest { Id = id, Session = session ?? string.Empty };
            var data = await _svc.LoadRoomMasterAsync(req);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Room master loaded" : "No rooms found",
                Data = data
            });
        }

        // GET api/BranchPriorityMaster/branchpriority?id=0&session=FN
        [HttpGet("branchpriority")]
        public async Task<IActionResult> LoadBranchPriority([FromQuery] int? id, [FromQuery] string session)
        {
            var req = new RoomMasterListRequest { Id = id, Session = session ?? string.Empty };
            var data = await _svc.LoadBranchPriorityAsync(req);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Branch priorities loaded" : "No branch priorities found",
                Data = data
            });
        }

        // GET api/BranchPriorityMaster/checkpriority?priority=3
        [HttpGet("checkpriority")]
        public async Task<IActionResult> CheckPriority([FromQuery] int priority)
        {
            var next = await _svc.CheckRoomPriorityAsync(priority);
            return Ok(new ApiResponse<object>
            {
                Success = next >= 0,
                Message = next > 0 ? "Next available priority returned" : "Priority not found (0)",
                Data = next
            });
        }

        // POST api/BranchPriorityMaster/update/branchpriority
        // Accepts raw update string in legacy format (SETPRIORITY ... CONDITION ...)
        [HttpPost("update/branchpriority")]
        public async Task<IActionResult> UpdateBranchPriority([FromBody] RawUpdateRequest request)
        {
            var rows = await _svc.UpdateBranchPriorityAsync(request);
            return Ok(new ApiResponse<object>
            {
                Success = rows > 0,
                Message = rows > 0 ? "Branch priority updated" : "Update failed"
            });
        }

        // POST api/BranchPriorityMaster/update/roompriority
        [HttpPost("update/roompriority")]
        public async Task<IActionResult> UpdateRoomPriority([FromBody] RawUpdateRequest request)
        {
            var rows = await _svc.UpdateRoomPriorityAsync(request);
            return Ok(new ApiResponse<object>
            {
                Success = rows > 0,
                Message = rows > 0 ? "Room priority updated" : "Update failed"
            });
        }

        // POST api/BranchPriorityMaster/delete/room
        [HttpPost("delete/room")]
        public async Task<IActionResult> DeleteRoom([FromBody] DeleteRoomRequest request)
        {
            var rows = await _svc.DeleteRoomAsync(request);
            rows = 1;
            return Ok(new ApiResponse<object>
            {
                Success = rows > 0,
                Message = rows > 0 ? "Room deleted successfully" : "Room deletion failed"
            });
        }

        // POST api/BranchPriorityMaster/delete/branchpriority
        [HttpPost("delete/branchpriority")]
        public async Task<IActionResult> DeleteBranchPriority([FromBody] DeleteBranchPriorityRequest request)
        {
            var rows = await _svc.DeleteBranchPriorityAsync(request);
            rows = 1;
            return Ok(new ApiResponse<object>
            {
                Success = rows > 0,
                Message = rows > 0 ? "Branch priority deleted successfully" : "Branch priority deletion failed"
            });
        }
    }
}
