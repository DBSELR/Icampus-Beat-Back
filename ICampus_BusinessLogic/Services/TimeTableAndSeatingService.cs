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
    public class TimeTableAndSeatingService : ITimeTableAndSeatingService
    {
        private readonly IGenericRepository<object> _repo;
        public TimeTableAndSeatingService(IGenericRepository<object> repo) => _repo = repo;

        public async Task<IEnumerable<object>> GetSemsExamMyAsync(string examMy, string course, string regulation)
        {
            var p1 = new SqlParameter("@EXAMMY", SqlDbType.VarChar) { Value = examMy ?? string.Empty };
            var p2 = new SqlParameter("@COURSE", SqlDbType.VarChar) { Value = course ?? string.Empty };
            var p3 = new SqlParameter("@REGULATION", SqlDbType.VarChar) { Value = regulation ?? string.Empty };

            var sql = StoredProcSql.Exec(StoredProcedures.SPS_Get_Sems_ExamMY, "@EXAMMY", "@COURSE", "@REGULATION");
            var raw = await _repo.QueryFromStoredProcAsync(sql, p1, p2, p3);
            return raw ?? Enumerable.Empty<object>();
        }

        public async Task<IEnumerable<object>> GetExamTimeTableDataAsync(string examMy, string course, int sem, string regulation)
        {
            var p1 = new SqlParameter("@EXAMMY", SqlDbType.VarChar) { Value = examMy ?? string.Empty };
            var p2 = new SqlParameter("@COURSE", SqlDbType.VarChar) { Value = course ?? string.Empty };
            var p3 = new SqlParameter("@SEM", SqlDbType.Int) { Value = sem };
            var p4 = new SqlParameter("@REGULATION", SqlDbType.VarChar) { Value = regulation ?? string.Empty };

            var sql = StoredProcSql.Exec(StoredProcedures.SPS_Get_ExamTimeTableData, "@EXAMMY", "@COURSE", "@SEM", "@REGULATION");
            var raw = await _repo.QueryFromStoredProcAsync(sql, p1, p2, p3, p4);
            return raw ?? Enumerable.Empty<object>();
        }

        public async Task<IEnumerable<object>> GetPapersWithCodeAsync(string examMy, string course, int sem, string eDate, string regulation)
        {
            var p1 = new SqlParameter("@EXAMMY", SqlDbType.VarChar) { Value = examMy ?? string.Empty };
            var p2 = new SqlParameter("@COURSE", SqlDbType.VarChar) { Value = course ?? string.Empty };
            var p3 = new SqlParameter("@SEM", SqlDbType.Int) { Value = sem };
            var p4 = new SqlParameter("@EDATE", SqlDbType.VarChar) { Value = eDate ?? string.Empty };
            var p5 = new SqlParameter("@REGULATION", SqlDbType.VarChar) { Value = regulation ?? string.Empty };

            var sql = StoredProcSql.Exec(StoredProcedures.SPS_GetPapersWithCode, "@EXAMMY", "@COURSE", "@SEM", "@EDATE", "@REGULATION");
            var raw = await _repo.QueryFromStoredProcAsync(sql, p1, p2, p3, p4, p5);
            return raw ?? Enumerable.Empty<object>();
        }

        public async Task<IEnumerable<object>> GetPapersDataAsync(string examMy, string course, int sem, string pcode, string regulation, string examType)
        {
            var ps = new[]
            {
                new SqlParameter("@EXAMMY", SqlDbType.VarChar) { Value = examMy ?? string.Empty },
                new SqlParameter("@COURSE", SqlDbType.VarChar) { Value = course ?? string.Empty },
                new SqlParameter("@SEM", SqlDbType.Int) { Value = sem },
                new SqlParameter("@PCODE", SqlDbType.VarChar) { Value = pcode ?? string.Empty },
                new SqlParameter("@REGULATION", SqlDbType.VarChar) { Value = regulation ?? string.Empty },
                new SqlParameter("@ExamType", SqlDbType.VarChar) { Value = examType ?? string.Empty }
            };

            var sql = StoredProcSql.Exec(StoredProcedures.SPS_Get_Papers_Data, "@EXAMMY", "@COURSE", "@SEM", "@PCODE", "@REGULATION", "@ExamType");
            var raw = await _repo.QueryFromStoredProcAsync(sql, ps);
            return raw ?? Enumerable.Empty<object>();
        }

        public async Task<IEnumerable<object>> GetExamDatesAsync(string examMy, string course, int sem, string regulation)
        {
            var p1 = new SqlParameter("@EXAMMY", SqlDbType.VarChar) { Value = examMy ?? string.Empty };
            var p2 = new SqlParameter("@COURSE", SqlDbType.VarChar) { Value = course ?? string.Empty };
            var p3 = new SqlParameter("@SEM", SqlDbType.Int) { Value = sem };
            var p4 = new SqlParameter("@REGULATION", SqlDbType.VarChar) { Value = regulation ?? string.Empty };

            var sql = StoredProcSql.Exec(StoredProcedures.SPS_Get_ExamDates, "@EXAMMY", "@COURSE", "@SEM", "@REGULATION");
            var raw = await _repo.QueryFromStoredProcAsync(sql, p1, p2, p3, p4);
            return raw ?? Enumerable.Empty<object>();
        }

        public async Task<IEnumerable<object>> GetRAPapersListAsync(string examMy, string course, int sem, string eDate, string regulation)
        {
            var p1 = new SqlParameter("@EXAMMY", SqlDbType.VarChar) { Value = examMy ?? string.Empty };
            var p2 = new SqlParameter("@COURSE", SqlDbType.VarChar) { Value = course ?? string.Empty };
            var p3 = new SqlParameter("@SEM", SqlDbType.Int) { Value = sem };
            var p4 = new SqlParameter("@EDATE", SqlDbType.VarChar) { Value = eDate ?? string.Empty };
            var p5 = new SqlParameter("@REGULATION", SqlDbType.VarChar) { Value = regulation ?? string.Empty };

            var sql = StoredProcSql.Exec(StoredProcedures.SPS_GetPapersWithCode, "@EXAMMY", "@COURSE", "@SEM", "@EDATE", "@REGULATION");
            var raw = await _repo.QueryFromStoredProcAsync(sql, p1, p2, p3, p4, p5);
            return raw ?? Enumerable.Empty<object>();
        }

        public async Task<IEnumerable<object>> GetRAPapersDataAsync(string examMy, string course, int sem, string pcode, string eDate, string regulation)
        {
            var ps = new[]
            {
                new SqlParameter("@EXAMMY", SqlDbType.VarChar) { Value = examMy ?? string.Empty },
                new SqlParameter("@COURSE", SqlDbType.VarChar) { Value = course ?? string.Empty },
                new SqlParameter("@SEM", SqlDbType.Int) { Value = sem },
                new SqlParameter("@PCODE", SqlDbType.VarChar) { Value = pcode ?? string.Empty },
                new SqlParameter("@EDATE", SqlDbType.VarChar) { Value = eDate ?? string.Empty },
                new SqlParameter("@REGULATION", SqlDbType.VarChar) { Value = regulation ?? string.Empty }
            };

            var sql = StoredProcSql.Exec(StoredProcedures.SPS_Get_RAPapers_Data, "@EXAMMY", "@COURSE", "@SEM", "@PCODE", "@EDATE", "@REGULATION");
            var raw = await _repo.QueryFromStoredProcAsync(sql, ps);
            return raw ?? Enumerable.Empty<object>();
        }

        public async Task<IEnumerable<object>> GetExamBranchAsync(string examMy, string course, int sem, string regulation)
        {
            // Original DAL used inline SQL: SELECT DISTINCT GRP FROM TBL_SH WHERE EXAMMY = '{ExamMY}' AND COURSE = '{Course}' AND SEM = {Sem} AND REGULATION = '{Regulation}' ORDER BY GRP
            var sql = "SELECT DISTINCT GRP FROM TBL_SH WHERE EXAMMY = @EXAMMY AND COURSE = @COURSE AND SEM = @SEM AND REGULATION = @REGULATION ORDER BY GRP";
            var p1 = new SqlParameter("@EXAMMY", SqlDbType.VarChar) { Value = examMy ?? string.Empty };
            var p2 = new SqlParameter("@COURSE", SqlDbType.VarChar) { Value = course ?? string.Empty };
            var p3 = new SqlParameter("@SEM", SqlDbType.Int) { Value = sem };
            var p4 = new SqlParameter("@REGULATION", SqlDbType.VarChar) { Value = regulation ?? string.Empty };
            
            var raw = await _repo.QueryFromStoredProcAsync(sql, p1, p2, p3, p4);
            return raw ?? Enumerable.Empty<object>();
        }

        public async Task<int> UpdateExamSessionAsync(UpdateExamSessionRequest req)
        {
            var ps = new[]
            {
                new SqlParameter("@EXAMMY", SqlDbType.VarChar) { Value = req.ExamMy ?? string.Empty },
                new SqlParameter("@COURSE", SqlDbType.VarChar) { Value = req.Course ?? string.Empty },
                new SqlParameter("@SEM", SqlDbType.Int) { Value = req.Sem },
                new SqlParameter("@Session", SqlDbType.VarChar) { Value = req.Session ?? string.Empty },
                new SqlParameter("@ExamTime", SqlDbType.VarChar) { Value = req.ExamTime ?? string.Empty },
                new SqlParameter("@REGULATION", SqlDbType.VarChar) { Value = req.Regulation ?? string.Empty }
            };

            var sql = StoredProcSql.Exec(StoredProcedures.SPS_Update_ExamSession, "@EXAMMY", "@COURSE", "@SEM", "@Session", "@ExamTime", "@REGULATION");
            return await _repo.ExecuteStoredProcAsync(sql, ps);
        }

        public async Task<int> UpdateExamDateAsync(UpdateExamDateRequest req)
        {
            var ps = new[]
            {
                new SqlParameter("@EXAMMY", SqlDbType.VarChar) { Value = req.ExamMy ?? string.Empty },
                new SqlParameter("@COURSE", SqlDbType.VarChar) { Value = req.Course ?? string.Empty },
                new SqlParameter("@SEM", SqlDbType.Int) { Value = req.Sem },
                new SqlParameter("@PCODE", SqlDbType.VarChar) { Value = req.PCode ?? string.Empty },
                new SqlParameter("@EDATE", SqlDbType.VarChar) { Value = req.EDate ?? string.Empty },
                new SqlParameter("@SESS", SqlDbType.VarChar) { Value = req.Session ?? string.Empty },
                new SqlParameter("@TIME", SqlDbType.VarChar) { Value = req.ExamTime ?? string.Empty },
                new SqlParameter("@REGULATION", SqlDbType.VarChar) { Value = req.Regulation ?? string.Empty },
                new SqlParameter("@GRP", SqlDbType.VarChar) { Value = req.Branch ?? "ALL BRANCHES" },
                new SqlParameter("@ExamType", SqlDbType.VarChar) { Value = req.ExamType ?? string.Empty },
                new SqlParameter("@Remarks", SqlDbType.VarChar) { Value = req.Remarks ?? string.Empty }
            };

            var sql = StoredProcSql.Exec(StoredProcedures.SPS_Update_ExamDate,
                "@EXAMMY", "@COURSE", "@SEM", "@PCODE", "@EDATE", "@SESS", "@TIME", "@REGULATION", "@GRP", "@ExamType", "@Remarks");

            return await _repo.ExecuteStoredProcAsync(sql, ps);
        }

        public async Task<int> UpdateRoomNumbersAsync(UpdateRoomNumbersRequest req)
        {
            var ps = new[]
            {
                new SqlParameter("@EXAMMY", SqlDbType.VarChar) { Value = req.ExamMy ?? string.Empty },
                new SqlParameter("@COURSE", SqlDbType.VarChar) { Value = req.Course ?? string.Empty },
                new SqlParameter("@SEM", SqlDbType.Int) { Value = req.Sem },
                new SqlParameter("@PCODE", SqlDbType.VarChar) { Value = req.PCode ?? string.Empty },
                new SqlParameter("@FREGNO", SqlDbType.VarChar) { Value = req.FromRegNo ?? string.Empty },
                new SqlParameter("@TREGNO", SqlDbType.VarChar) { Value = req.ToRegNo ?? string.Empty },
                new SqlParameter("@ROOMNO", SqlDbType.VarChar) { Value = req.Room ?? string.Empty },
                new SqlParameter("@REGULATION", SqlDbType.VarChar) { Value = req.Regulation ?? string.Empty }
            };

            var sql = StoredProcSql.Exec(StoredProcedures.SPS_Update_RoomNumbers,
                "@EXAMMY", "@COURSE", "@SEM", "@PCODE", "@FREGNO", "@TREGNO", "@ROOMNO", "@REGULATION");

            return await _repo.ExecuteStoredProcAsync(sql, ps);
        }

        public async Task<IEnumerable<object>> RoomsSearchAsync(string prefixText)
        {
            // The DAL used inline SQL: SELECT * FROM TBL_ROOMMASTER WHERE ROOMNO LIKE '%{prefix}%'
            // We'll use inline SQL here for search — consistent with your DAL.
            var sql = $"SELECT TOP(20) ROOMNO FROM TBL_ROOMMASTER WHERE ROOMNO LIKE @P";
            var p = new SqlParameter("@P", SqlDbType.VarChar) { Value = $"%{prefixText}%" };
            var raw = await _repo.QueryFromStoredProcAsync(sql, p);
            return raw ?? Enumerable.Empty<object>();
        }

        public async Task<IEnumerable<object>> ExamDatesFormatAsync(string regulation, string course, string examMy)
        {
            var p1 = new SqlParameter("@Regulation", SqlDbType.VarChar) { Value = regulation ?? string.Empty };
            var p2 = new SqlParameter("@course", SqlDbType.VarChar) { Value = course ?? string.Empty };
            var p3 = new SqlParameter("@exammy", SqlDbType.VarChar) { Value = examMy ?? string.Empty };

            var sql = StoredProcSql.Exec(StoredProcedures.SP_EXAMSESSIONDATEFORMAT, "@Regulation", "@course", "@exammy");
            var raw = await _repo.QueryFromStoredProcAsync(sql, p1, p2, p3);
            return raw ?? Enumerable.Empty<object>();
        }
    }
}
