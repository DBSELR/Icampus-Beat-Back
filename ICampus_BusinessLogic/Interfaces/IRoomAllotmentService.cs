using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ICampus_Models.Requests;

namespace ICampus_BusinessLogic.Interfaces
{
    public interface IRoomAllotmentService
    {
        Task<IEnumerable<object>> LoadSemsAsync(string course, string examMy, string regulation);
        Task<IEnumerable<object>> LoadSessionsAsync(string course, string sem, string examType);
        Task<IEnumerable<object>> LoadExamDatesAsync(RoomAllotSemanticRequest req);
        Task<IEnumerable<object>> LoadRoomsAsync(string course, string session);
        Task<IEnumerable<object>> LoadPcodesAsync(RoomAllotSemanticRequest req);
        Task<IEnumerable<object>> LoadRegnosAsync(RoomAllotSemanticRequest req);
        Task<IEnumerable<object>> LoadAllotedDataAsync(RoomAllotSemanticRequest req);
        Task<int> SaveRoomAllotmentAsync(RoomAllotmentSaveRequest req);
        Task<int> ResetRoomAsync(RoomResetRequest req);
        Task<IEnumerable<object>> ExportExcelAsync(ExcelExportRequest req);
        Task<int> ApplyAllDatesAsync(ApplyAllDatesRequest req);
        Task<IEnumerable<object>> GetRoomMasterConfigAsync(string roomNo, string daySession);
        Task<IEnumerable<object>> LoadCourseGroupsAsync(string course, string regulation);
        Task<IEnumerable<object>> LoadStudentDataWithRowOrderAsync(int rowCount, string groups);
        Task<IEnumerable<object>> LoadStudentsFromRoomAllotmentAsync(int count, string groups);
        Task<int> ResetStudentStatusAsync(string regno);
        Task<int> MarkStudentAsAllocatedAsync(string regno);
        Task<int> GetUnallocatedStudentsCountAsync();
    }
}
