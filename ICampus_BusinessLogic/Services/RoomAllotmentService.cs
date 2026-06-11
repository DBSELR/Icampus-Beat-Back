using ICampus_BusinessLogic.Interfaces;
using ICampus_DataAccessLayer.Helpers;
using ICampus_DataAccessLayer.Interfaces;
using ICampus_Models.Requests;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Services
{
    public class RoomAllotmentService : IRoomAllotmentService
    {
        private readonly IGenericRepository<object> _repo;

        public RoomAllotmentService(IGenericRepository<object> repo)
        {
            _repo = repo;
        }

        // 1. Load sems — filtered by exam notification (EXAMMY) not all-time sems
        // SP: SPS_Get_Sems_ExamMY — same SP used by TimeTable module
        // Params: @EXAMMY, @COURSE, @REGULATION
        public async Task<IEnumerable<object>> LoadSemsAsync(string course, string examMy, string regulation)
        {
            var p1 = new SqlParameter("@EXAMMY", SqlDbType.VarChar) { Value = examMy ?? string.Empty };
            var p2 = new SqlParameter("@COURSE", SqlDbType.VarChar) { Value = course ?? string.Empty };
            var p3 = new SqlParameter("@REGULATION", SqlDbType.VarChar) { Value = regulation ?? string.Empty };
            var sql = StoredProcSql.Exec(StoredProcedures.SPS_Get_Sems_ExamMY, "@EXAMMY", "@COURSE", "@REGULATION");
            var raw = await _repo.QueryFromStoredProcAsync(sql, p1, p2, p3);
            return raw ?? Enumerable.Empty<object>();
        }

        // 2. Load sessions (Spr_Load_Session)
        public async Task<IEnumerable<object>> LoadSessionsAsync(string course, string sem, string examType)
        {
            var p1 = new SqlParameter("@Course", SqlDbType.VarChar) { Value = course ?? string.Empty };
            var p2 = new SqlParameter("@Sem", SqlDbType.VarChar) { Value = sem ?? string.Empty };
            var p3 = new SqlParameter("@ExamType", SqlDbType.VarChar) { Value = examType ?? string.Empty };

            var sql = StoredProcSql.Exec(StoredProcedures.Spr_Load_Session, "@Course", "@Sem", "@ExamType");
            var raw = await _repo.QueryFromStoredProcAsync(sql, p1, p2, p3);
            return raw ?? Enumerable.Empty<object>();
        }

        // 3. Load exam dates (SPS_GET_EDATE)
        public async Task<IEnumerable<object>> LoadExamDatesAsync(RoomAllotSemanticRequest req)
        {
            var p1 = new SqlParameter("@Course", SqlDbType.VarChar) { Value = req.Course ?? string.Empty };
            var p2 = new SqlParameter("@Sem", SqlDbType.VarChar) { Value = req.Sem ?? string.Empty };
            var p3 = new SqlParameter("@ESESS", SqlDbType.VarChar) { Value = req.DaySession ?? string.Empty }; // note: request uses DaySession or Section
            var p4 = new SqlParameter("@Exammy", SqlDbType.VarChar) { Value = req.ExamMy ?? string.Empty };
            var p5 = new SqlParameter("@Regulation", SqlDbType.VarChar) { Value = req.Regulation ?? string.Empty };
            var p6 = new SqlParameter("@ExamType", SqlDbType.VarChar) { Value = req.ExamType ?? string.Empty };

            var sql = StoredProcSql.Exec(StoredProcedures.SPS_GET_EDATE, "@Course", "@Sem", "@ESESS", "@Exammy", "@Regulation", "@ExamType");
            var raw = await _repo.QueryFromStoredProcAsync(sql, p1, p2, p3, p4, p5, p6);
            return raw ?? Enumerable.Empty<object>();
        }

        // 4. Load rooms (SPR_LOAD_ROOM)
        // Original ASPX: "SPR_LOAD_ROOM '" + Course + "','" + Section + "'"
        // Note: ASPX uses @Section parameter, but SP might accept @Session as well
        public async Task<IEnumerable<object>> LoadRoomsAsync(string course, string session)
        {
            var p1 = new SqlParameter("@Course", SqlDbType.VarChar) { Value = course ?? string.Empty };
            var p2 = new SqlParameter("@Section", SqlDbType.VarChar) { Value = session ?? string.Empty }; // Changed to @Section to match ASPX

            var sql = StoredProcSql.Exec(StoredProcedures.SPR_LOAD_ROOM, "@Course", "@Section");
            var raw = await _repo.QueryFromStoredProcAsync(sql, p1, p2);
            return raw ?? Enumerable.Empty<object>();
        }

        // 5. Get elective subjects / pcode (proc_Get_Elec_Subjects)
        // Original ASPX: "proc_Get_Elec_Subjects'" + Course + "','" + Regulation + "','" + ExamMY + "','" + EDate + "','" + Sem + "'"
        // Note: Date format must be 'yyyy-MM-dd'
        public async Task<IEnumerable<object>> LoadPcodesAsync(RoomAllotSemanticRequest req)
        {
            // Format date as yyyy-MM-dd if provided
            string formattedDate = string.Empty;
            if (!string.IsNullOrWhiteSpace(req.EDate))
            {
                if (DateTime.TryParse(req.EDate, out DateTime dateValue))
                {
                    formattedDate = dateValue.ToString("yyyy-MM-dd");
                }
                else
                {
                    formattedDate = req.EDate; // Assume already formatted
                }
            }

            var p1 = new SqlParameter("@COURSE", SqlDbType.VarChar) { Value = req.Course ?? string.Empty };
            var p2 = new SqlParameter("@REGULATION", SqlDbType.VarChar) { Value = req.Regulation ?? string.Empty };
            var p3 = new SqlParameter("@exammy", SqlDbType.VarChar) { Value = req.ExamMy ?? string.Empty };
            var p4 = new SqlParameter("@ExamDate", SqlDbType.VarChar) { Value = formattedDate };
            var p5 = new SqlParameter("@Sem", SqlDbType.VarChar) { Value = req.Sem ?? string.Empty }; // Changed to VarChar to match ASPX

            var sql = StoredProcSql.Exec(StoredProcedures.proc_Get_Elec_Subjects, "@COURSE", "@REGULATION", "@exammy", "@ExamDate", "@Sem");
            var raw = await _repo.QueryFromStoredProcAsync(sql, p1, p2, p3, p4, p5);
            return raw ?? Enumerable.Empty<object>();
        }

        // 6. Load regnos - Sp_RoomAllot_Load_Regno
        // Original ASPX: "Sp_RoomAllot_Load_Regno '" + Course + "','" + Regulation + "','" + ExamMY + "','" + ExamDate + "','" + Sem + "','" + RoomNo + "','" + Regsup + "','" + ExamType + "','" + Section + "',null" + "," + maxnum + ""
        // Note: Date format must be 'yyyy-MM-dd' as in ASPX
        public async Task<IEnumerable<object>> LoadRegnosAsync(RoomAllotSemanticRequest req)
        {
            // Format date as yyyy-MM-dd if provided
            string formattedDate = string.Empty;
            if (!string.IsNullOrWhiteSpace(req.EDate))
            {
                if (DateTime.TryParse(req.EDate, out DateTime dateValue))
                {
                    formattedDate = dateValue.ToString("yyyy-MM-dd");
                }
                else
                {
                    formattedDate = req.EDate; // Assume already formatted
                }
            }

            var p1 = new SqlParameter("@COURSE", SqlDbType.VarChar) { Value = req.Course ?? string.Empty };
            var p2 = new SqlParameter("@REGULATION", SqlDbType.VarChar) { Value = req.Regulation ?? string.Empty };
            var p3 = new SqlParameter("@exammy", SqlDbType.VarChar) { Value = req.ExamMy ?? string.Empty };
            var p4 = new SqlParameter("@ExamDate", SqlDbType.VarChar) { Value = formattedDate };
            var p5 = new SqlParameter("@Sem", SqlDbType.VarChar) { Value = req.Sem ?? string.Empty }; // Changed to VarChar to match ASPX
            var p6 = new SqlParameter("@RoomNo", SqlDbType.VarChar) { Value = req.RoomNo ?? string.Empty };
            var p7 = new SqlParameter("@REGSUP", SqlDbType.VarChar) { Value = req.Regsup ?? "REG" };
            var p8 = new SqlParameter("@ExamType", SqlDbType.VarChar) { Value = req.ExamType ?? string.Empty };
            var p9 = new SqlParameter("@Session", SqlDbType.VarChar) { Value = req.DaySession ?? string.Empty };
            var p10 = new SqlParameter("@Pcode", SqlDbType.VarChar) { Value = (object)req.Pcode ?? DBNull.Value };
            var p11 = new SqlParameter("@maxnum", SqlDbType.Int) { Value = req.MaxNum };

            var sql = StoredProcSql.Exec(StoredProcedures.Sp_RoomAllot_Load_Regno,
                "@COURSE", "@REGULATION", "@exammy", "@ExamDate", "@Sem", "@RoomNo", "@REGSUP", "@ExamType", "@Session", "@Pcode", "@maxnum");

            var raw = await _repo.QueryFromStoredProcAsync(sql, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11);
            return raw ?? Enumerable.Empty<object>();
        }

        // 7. Load allotted data (Sp_RoomAllotment_Load_Data)
        // Original ASPX: "Sp_RoomAllotment_Load_Data '" + Course + "','" + ExamMY + "','" + Sem + "','" + RoomNo + "','" + ExamDate + "','" + Section + "','" + Regsup + "','" + ExamType + "'"
        // Note: Date format must be 'yyyy-MM-dd'
        public async Task<IEnumerable<object>> LoadAllotedDataAsync(RoomAllotSemanticRequest req)
        {
            // Format date as yyyy-MM-dd if provided
            string formattedDate = string.Empty;
            if (!string.IsNullOrWhiteSpace(req.EDate))
            {
                if (DateTime.TryParse(req.EDate, out DateTime dateValue))
                {
                    formattedDate = dateValue.ToString("yyyy-MM-dd");
                }
                else
                {
                    formattedDate = req.EDate; // Assume already formatted
                }
            }

            var p1 = new SqlParameter("@Course", SqlDbType.VarChar) { Value = req.Course ?? string.Empty };
            var p2 = new SqlParameter("@Exammy", SqlDbType.VarChar) { Value = req.ExamMy ?? string.Empty };
            var p3 = new SqlParameter("@Sem", SqlDbType.VarChar) { Value = req.Sem ?? string.Empty };
            var p4 = new SqlParameter("@RoomNo", SqlDbType.VarChar) { Value = req.RoomNo ?? string.Empty };
            var p5 = new SqlParameter("@Exam_Date", SqlDbType.VarChar) { Value = formattedDate };
            var p6 = new SqlParameter("@DaySession", SqlDbType.VarChar) { Value = req.DaySession ?? string.Empty };
            var p7 = new SqlParameter("@Regsup", SqlDbType.VarChar) { Value = req.Regsup ?? "REG" };
            var p8 = new SqlParameter("@Examtype", SqlDbType.VarChar) { Value = req.ExamType ?? string.Empty };

            var sql = StoredProcSql.Exec(StoredProcedures.Sp_RoomAllotment_Load_Data,
                "@Course", "@Exammy", "@Sem", "@RoomNo", "@Exam_Date", "@DaySession", "@Regsup", "@Examtype");

            var raw = await _repo.QueryFromStoredProcAsync(sql, p1, p2, p3, p4, p5, p6, p7, p8);
            return raw ?? Enumerable.Empty<object>();
        }

        // 8. Save room allotment (Sp_Std_Exam_RoomAllotment) with TVP @RoomAllotment
        // Original ASPX: Checks unallocated students count before saving
        // Date format must be 'yyyy-MM-dd'
        public async Task<int> SaveRoomAllotmentAsync(RoomAllotmentSaveRequest req)
        {
            // Format date as yyyy-MM-dd if provided
            string formattedDate = string.Empty;
            if (!string.IsNullOrWhiteSpace(req.ExamDate))
            {
                if (DateTime.TryParse(req.ExamDate, out DateTime dateValue))
                {
                    formattedDate = dateValue.ToString("yyyy-MM-dd");
                }
                else
                {
                    formattedDate = req.ExamDate; // Assume already formatted
                }
            }

            // Create DataTable matching TVP type (Row_Col, Regno)
            // Original ASPX uses Session["RoomAllotment"] DataTable with columns: Row_Col, Regno
            var tvp = new DataTable();
            tvp.Columns.Add("Row_Col", typeof(string));
            tvp.Columns.Add("Regno", typeof(string));

            foreach (var r in req.RoomAllotment ?? Enumerable.Empty<RoomAllotmentRow>())
            {
                if (!string.IsNullOrWhiteSpace(r.Regno))
                {
                    tvp.Rows.Add(r.RowCol, r.Regno);
                }
            }

            var ps = new[]
            {
                new SqlParameter("@SEM", SqlDbType.VarChar) { Value = req.Sem ?? string.Empty },
                new SqlParameter("@Session", SqlDbType.VarChar) { Value = req.Session ?? string.Empty },
                new SqlParameter("@ExamDate", SqlDbType.VarChar) { Value = formattedDate },
                new SqlParameter("@RoomNo", SqlDbType.VarChar) { Value = req.RoomNo ?? string.Empty },
                new SqlParameter("@Course", SqlDbType.VarChar) { Value = req.Course ?? string.Empty },
                new SqlParameter("@ExamMy", SqlDbType.VarChar) { Value = req.ExamMy ?? string.Empty },
                new SqlParameter("@BP", SqlDbType.VarChar) { Value = req.BP ?? string.Empty },
                new SqlParameter("@ExamType", SqlDbType.VarChar) { Value = req.ExamType ?? string.Empty },
                // TVP param - Original ASPX uses @RoomAllotment parameter
                new SqlParameter("@RoomAllotment", SqlDbType.Structured)
                {
                    TypeName = "Send_RoomAllotment_Data", // TVP type name in DB
                    Value = tvp
                }
            };

            var sql = StoredProcSql.Exec(StoredProcedures.Sp_Std_Exam_RoomAllotment,
                "@SEM", "@Session", "@ExamDate", "@RoomNo", "@Course", "@ExamMy", "@BP", "@ExamType", "@RoomAllotment");

            return await _repo.ExecuteStoredProcAsync(sql, ps);
        }

        // 9. Reset room (Sp_Reset_Room)
        // Original ASPX: Date format 'yyyy-MM-dd'
        public async Task<int> ResetRoomAsync(RoomResetRequest req)
        {
            // Format date as yyyy-MM-dd if provided
            string formattedDate = string.Empty;
            if (!string.IsNullOrWhiteSpace(req.ExamDate))
            {
                if (DateTime.TryParse(req.ExamDate, out DateTime dateValue))
                {
                    formattedDate = dateValue.ToString("yyyy-MM-dd");
                }
                else
                {
                    formattedDate = req.ExamDate; // Assume already formatted
                }
            }

            var ps = new[]
            {
                new SqlParameter("@SEM", SqlDbType.VarChar) { Value = req.Sem ?? string.Empty },
                new SqlParameter("@Session", SqlDbType.VarChar) { Value = req.Session ?? string.Empty },
                new SqlParameter("@ExamDate", SqlDbType.VarChar) { Value = formattedDate }, // Changed to VarChar to match ASPX
                new SqlParameter("@RoomNo", SqlDbType.VarChar) { Value = req.RoomNo ?? string.Empty },
                new SqlParameter("@Course", SqlDbType.VarChar) { Value = req.Course ?? string.Empty },
                new SqlParameter("@ExamMy", SqlDbType.VarChar) { Value = req.ExamMy ?? string.Empty },
                new SqlParameter("@ExamType", SqlDbType.VarChar) { Value = req.ExamType ?? string.Empty },
                new SqlParameter("@BP", SqlDbType.VarChar) { Value = req.BP ?? string.Empty },
            };

            var sql = StoredProcSql.Exec(StoredProcedures.Sp_Reset_Room,
                "@SEM", "@Session", "@ExamDate", "@RoomNo", "@Course", "@ExamMy", "@ExamType", "@BP");

            return await _repo.ExecuteStoredProcAsync(sql, ps);
        }

        // 10. Excel export (Sp_room_ExcelExport_Dump)
        // Original ASPX: "Sp_room_ExcelExport_Dump '" + Course + "','" + ExamMY + "','" + Sem + "','" + ExamDate + "','" + Session + "','" + Room + "'"
        // Note: DaySession should be string (Session), not int
        public async Task<IEnumerable<object>> ExportExcelAsync(ExcelExportRequest req)
        {
            // Format date as yyyy-MM-dd if provided
            string formattedDate = string.Empty;
            if (!string.IsNullOrWhiteSpace(req.ExamDate))
            {
                if (DateTime.TryParse(req.ExamDate, out DateTime dateValue))
                {
                    formattedDate = dateValue.ToString("yyyy-MM-dd");
                }
                else
                {
                    formattedDate = req.ExamDate; // Assume already formatted
                }
            }

            var p1 = new SqlParameter("@Course", SqlDbType.VarChar) { Value = req.Course ?? string.Empty };
            var p2 = new SqlParameter("@ExamMy", SqlDbType.VarChar) { Value = req.ExamMy ?? string.Empty };
            var p3 = new SqlParameter("@Sem", SqlDbType.VarChar) { Value = req.Sem ?? string.Empty };
            var p4 = new SqlParameter("@Exam_Date", SqlDbType.VarChar) { Value = formattedDate }; // Changed to VarChar
            var p5 = new SqlParameter("@DaySession", SqlDbType.VarChar) { Value = req.DaySession ?? string.Empty }; // Changed to VarChar (Session is string in ASPX)
            var p6 = new SqlParameter("@RoomNo", SqlDbType.VarChar) { Value = req.RoomNo ?? string.Empty };

            var sql = StoredProcSql.Exec(StoredProcedures.Sp_room_ExcelExport_Dump,
                "@Course", "@ExamMy", "@Sem", "@Exam_Date", "@DaySession", "@RoomNo");

            var raw = await _repo.QueryFromStoredProcAsync(sql, p1, p2, p3, p4, p5, p6);
            return raw ?? Enumerable.Empty<object>();
        }

        // 11. Apply seating for all dates (proc_room_allotment_For_AllDates)
        public async Task<int> ApplyAllDatesAsync(ApplyAllDatesRequest req)
        {
            var ps = new[]
            {
                new SqlParameter("@course", SqlDbType.VarChar) { Value = req.Course ?? string.Empty },
                new SqlParameter("@exammy", SqlDbType.VarChar) { Value = req.ExamMy ?? string.Empty },
                new SqlParameter("@sem", SqlDbType.VarChar) { Value = req.Sem ?? string.Empty },
                new SqlParameter("@examtype", SqlDbType.VarChar) { Value = req.ExamType ?? string.Empty },
            };

            var sql = StoredProcSql.Exec(StoredProcedures.proc_room_allotment_For_AllDates,
                "@course", "@exammy", "@sem", "@examtype");

            return await _repo.ExecuteStoredProcAsync(sql, ps);
        }

        // 12. Get room master configuration (NOOFROWS, NOOFCOLUMNS)
        // Original ASPX: "select * from tbl_RoomMaster where ROOMNO = '" + room + "' and DaySession='" + session + "'"
        public async Task<IEnumerable<object>> GetRoomMasterConfigAsync(string roomNo, string daySession)
        {
            var sql = "SELECT * FROM tbl_RoomMaster WHERE ROOMNO = @ROOMNO AND DaySession = @DaySession";
            var p1 = new SqlParameter("@ROOMNO", SqlDbType.VarChar) { Value = roomNo ?? string.Empty };
            var p2 = new SqlParameter("@DaySession", SqlDbType.VarChar) { Value = daySession ?? string.Empty };
            var raw = await _repo.QueryFromStoredProcAsync(sql, p1, p2);
            return raw ?? Enumerable.Empty<object>();
        }

        // 13. Load course groups (Sp_RoomAllot_Load_Course)
        // Original ASPX: Used to populate ListBox for each column with groups (branches)
        public async Task<IEnumerable<object>> LoadCourseGroupsAsync(string course, string regulation)
        {
            var p1 = new SqlParameter("@Course", SqlDbType.VarChar) { Value = course ?? string.Empty };
            var p2 = new SqlParameter("@REGU", SqlDbType.VarChar) { Value = regulation ?? string.Empty };

            var sql = StoredProcSql.Exec(StoredProcedures.Sp_RoomAllot_Load_Course, "@Course", "@REGU");
            var raw = await _repo.QueryFromStoredProcAsync(sql, p1, p2);
            return raw ?? Enumerable.Empty<object>();
        }

        // 14. Load student data with row order (Sp_Get_Std_Data)
        // Original ASPX: "Sp_Get_Std_Data " + rowCount + ",'" + groups + "'"
        // Used when chkRowOrder is checked
        public async Task<IEnumerable<object>> LoadStudentDataWithRowOrderAsync(int rowCount, string groups)
        {
            var p1 = new SqlParameter("@RowCount", SqlDbType.Int) { Value = rowCount };
            var p2 = new SqlParameter("@Groups", SqlDbType.VarChar) { Value = groups ?? string.Empty };

            var sql = StoredProcSql.Exec(StoredProcedures.Sp_Get_Std_Data, "@RowCount", "@Groups");
            var raw = await _repo.QueryFromStoredProcAsync(sql, p1, p2);
            return raw ?? Enumerable.Empty<object>();
        }

        // 15. Load students from RoomAllotment table directly
        // Original ASPX: "Select top " + count + " * from RoomAllotment where grp in (" + groups + ") and regno_Status='' Order by rno,grp"
        public async Task<IEnumerable<object>> LoadStudentsFromRoomAllotmentAsync(int count, string groups)
        {
            // Build groups list for IN clause
            string groupsClause = groups ?? string.Empty;
            if (string.IsNullOrWhiteSpace(groupsClause))
            {
                return Enumerable.Empty<object>();
            }

            var sql = $"SELECT TOP {count} * FROM RoomAllotment WHERE grp IN ({groupsClause}) AND regno_Status = '' ORDER BY rno, grp";
            var raw = await _repo.QueryFromStoredProcAsync(sql);
            return raw ?? Enumerable.Empty<object>();
        }

        // 16. Update RoomAllotment status (reset to available)
        // Original ASPX: "Update R set regno_Status='' from RoomAllotment R where regno='" + regno + "'"
        public async Task<int> ResetStudentStatusAsync(string regno)
        {
            var sql = "UPDATE R SET regno_Status = '' FROM RoomAllotment R WHERE regno = @Regno";
            var p1 = new SqlParameter("@Regno", SqlDbType.VarChar) { Value = regno ?? string.Empty };
            return await _repo.ExecuteStoredProcAsync(sql, p1);
        }

        // 17. Mark student as temporarily allocated
        // Original ASPX: "Update R set regno_Status='T' from RoomAllotment R where regno='" + regno + "'"
        public async Task<int> MarkStudentAsAllocatedAsync(string regno)
        {
            var sql = "UPDATE R SET regno_Status = 'T' FROM RoomAllotment R WHERE regno = @Regno";
            var p1 = new SqlParameter("@Regno", SqlDbType.VarChar) { Value = regno ?? string.Empty };
            return await _repo.ExecuteStoredProcAsync(sql, p1);
        }

        // 18. Check unallocated students count
        // Original ASPX: "Select Count(*) from RoomAllotment where regno_Status=''"
        public async Task<int> GetUnallocatedStudentsCountAsync()
        {
            var sql = "SELECT COUNT(*) FROM RoomAllotment WHERE regno_Status = ''";
            var raw = await _repo.QueryFromStoredProcAsync(sql);
            if (raw != null && raw.Any())
            {
                var firstRow = raw.First();
                if (firstRow != null)
                {
                    // Try to get the count value
                    var props = firstRow.GetType().GetProperties();
                    if (props != null && props.Length > 0)
                    {
                        var countProp = props.FirstOrDefault(p => p.Name.Contains("Count") || p.Name.Contains("Column1") || p.Name == "Item");
                        if (countProp != null)
                        {
                            var value = countProp.GetValue(firstRow);
                            if (value != null && int.TryParse(value.ToString(), out int count))
                            {
                                return count;
                            }
                        }
                        // Try first property
                        var firstProp = props[0];
                        var firstValue = firstProp.GetValue(firstRow);
                        if (firstValue != null && int.TryParse(firstValue.ToString(), out int count2))
                        {
                            return count2;
                        }
                    }
                }
            }
            return 0;
        }
    }
}
