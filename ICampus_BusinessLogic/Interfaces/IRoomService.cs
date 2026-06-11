using System.Collections.Generic;
using System.Threading.Tasks;
using ICampus_Models.Requests;

public interface IRoomService
{
    Task<IEnumerable<object>> LoadRoomMasterAsync(int id, string session);
    Task<int> SaveRoomAsync(RoomSaveRequest req);
    Task<int> CheckRoomPriorityAsync(int priority);
    Task<int> UpdateRoomPriorityAsync(UpdatePriorityRequest req);
    Task<int> DeleteRoomAsync(string roomNo);

    Task<IEnumerable<object>> LoadBranchPriorityAsync(int id, string session);
    Task<int> SaveBranchPriorityAsync(BranchPrioritySaveRequest req);
    Task<int> DeleteBranchPriorityAsync(string priority, string branch, string session);
}
