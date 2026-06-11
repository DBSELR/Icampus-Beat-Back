using ICampus_Models.Requests;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IBranchPriorityMasterService
{
    Task<IEnumerable<object>> LoadCourseBranchesAsync(string course, string regu);
    Task<int> SaveBranchMasterAsync(BranchSaveRequest req);
    Task<IEnumerable<object>> LoadRoomMasterAsync(RoomMasterListRequest req);
    Task<IEnumerable<object>> LoadBranchPriorityAsync(RoomMasterListRequest req);
    Task<int> CheckRoomPriorityAsync(int priority);
    Task<int> UpdateBranchPriorityAsync(RawUpdateRequest req);
    Task<int> UpdateRoomPriorityAsync(RawUpdateRequest req);
    Task<int> DeleteRoomAsync(DeleteRoomRequest req);
    Task<int> DeleteBranchPriorityAsync(DeleteBranchPriorityRequest req);
}
