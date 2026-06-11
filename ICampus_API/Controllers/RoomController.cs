using ICampus_BusinessLogic.Interfaces;
using ICampus_Models.Requests;
using ICampus_Models.Common;
using Microsoft.AspNetCore.Mvc;

namespace ICampus_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomController : BaseApiController
    {
        private readonly IRoomService _svc;
        public RoomController(IRoomService svc) => _svc = svc;

        // GET api/room/list?session=2&id=0
        [HttpGet("list")]
        public async Task<IActionResult> GetRooms([FromQuery] string session, [FromQuery] int id = 0)
        {
            var data = await _svc.LoadRoomMasterAsync(id, session);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Rooms loaded" : "No rooms found",
                Data = data
            });
        }

        // POST api/room/save
        [HttpPost("save")]
        public async Task<IActionResult> SaveRoom([FromBody] RoomSaveRequest request)
        {
            var rows = await _svc.SaveRoomAsync(request);
            return Ok(new ApiResponse<object>
            {
                Success = rows > 0,
                Message = rows > 0 ? "Room saved successfully" : "Save failed"
            });
        }

        // GET api/room/check-priority?priority=3
        [HttpGet("check-priority")]
        public async Task<IActionResult> CheckPriority([FromQuery] int priority)
        {
            var next = await _svc.CheckRoomPriorityAsync(priority);
            var ok = next > 0;
            return Ok(new ApiResponse<object>
            {
                Success = ok,
                Message = ok ? $"Suggested priority: {next}" : "No rooms exist (priority 0)",
                Data = next
            });
        }

        // POST api/room/update-priority
        [HttpPost("update-priority")]
        public async Task<IActionResult> UpdatePriority([FromBody] UpdatePriorityRequest request)
        {
            var rows = await _svc.UpdateRoomPriorityAsync(request);
            return Ok(new ApiResponse<object>
            {
                Success = rows > 0 ? true : false,
                Message = rows > 0 ? "Priority updated" : "Update failed"
            });
        }

        // POST api/room/delete
        [HttpPost("delete")]
        public async Task<IActionResult> DeleteRoom([FromBody] DeleteRoomRequest request)
        {
            var rows = await _svc.DeleteRoomAsync(request.RoomNo);
            return Ok(new ApiResponse<object>
            {
                Success = rows > 0 ? true : false,
                Message = rows > 0 ? "Room deleted successfully" : "Delete failed"
            });
        }

        // GET api/room/branch-priority?session=2&id=0
        [HttpGet("branch-priority")]
        public async Task<IActionResult> GetBranchPriority([FromQuery] string session, [FromQuery] int id = 0)
        {
            var data = await _svc.LoadBranchPriorityAsync(id, session);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Branch priority loaded" : "No branch priority found",
                Data = data
            });
        }

        // POST api/room/save-branch-priority
        [HttpPost("save-branch-priority")]
        public async Task<IActionResult> SaveBranchPriority([FromBody] BranchPrioritySaveRequest request)
        {
            var rows = await _svc.SaveBranchPriorityAsync(request);
            return Ok(new ApiResponse<object>
            {
                Success = rows > 0,
                Message = rows > 0 ? "Branch priority saved" : "Save failed"
            });
        }

        // POST api/room/delete-branch-priority
        [HttpPost("delete-branch-priority")]
        public async Task<IActionResult> DeleteBranchPriority([FromBody] BranchPriorityDeleteRequest request)
        {
            var rows = await _svc.DeleteBranchPriorityAsync(request.Priority, request.Branch, request.Session);
            return Ok(new ApiResponse<object>
            {
                Success = rows > 0,
                Message = rows > 0 ? "Branch priority deleted" : "Delete failed"
            });
        }
    }
}
