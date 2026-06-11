using ICampus_BusinessLogic.Interfaces;
using ICampus_DataAccessLayer.Helpers;
using ICampus_DataAccessLayer.Interfaces;
using ICampus_Models.Requests;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ICampus_BusinessLogic.Services
{
    public class CondonationService : ICondonationService
    {
        private readonly IGenericRepository<object> _repo;

        public CondonationService(IGenericRepository<object> repo)
        {
            _repo = repo;
        }

        // 1) Load Semesters
        public async Task<IEnumerable<object>> LoadSemsAsync(
            string course, string regulation, string examMy, string regsup, string regno)
        {
            string sql = @"
                SELECT DISTINCT SH.SEM 
                FROM TBL_SH SH
                WHERE SH.course = @Course
                  AND SH.Regulation = @Regulation
                  AND SH.exammy = @ExamMy
                  AND SH.Regsup = @Regsup
                  AND SH.regno = @Regno
                ORDER BY SH.SEM";

            SqlParameter[] p =
            {
                new("@Course", course),
                new("@Regulation", regulation),
                new("@ExamMy", examMy),
                new("@Regsup", regsup),
                new("@Regno", regno)
            };

            return await _repo.QueryFromStoredProcAsync(sql, p);
        }

        // 2) Load Student Details
        public async Task<object> GetStudentDetailsAsync(string regno)
        {
            var p = new SqlParameter("@REGNO", regno);

            string sql = StoredProcSql.Exec(StoredProcedures.SP_GETDETAILS, "@REGNO");

            var raw = await _repo.QueryFromStoredProcAsync(sql, new[] { p });

            return raw?.FirstOrDefault();
        }

        // 3) Grid
        public async Task<IEnumerable<object>> GetCondonationGridAsync(
    string regno, string examMy, string course, string sem)
        {
            var p = new[]
            {
        new SqlParameter("@Course", course),
        new SqlParameter("@Exammy", examMy),
        new SqlParameter("@Regno", regno),
        new SqlParameter("@Sem", sem)
    };

            string sql = StoredProcSql.Exec(StoredProcedures.SP_Condotion_Grid,
                "@Course", "@Exammy", "@Regno", "@Sem");

            return await _repo.QueryFromStoredProcAsync(sql, p);
        }


        // 4) Check Dates
        public async Task<IEnumerable<object>> CheckCondonationDatesAsync(
            string regno, string examMy, string course, string regulation, string sem)
        {
            var p = new[]
            {
                new SqlParameter("@Regno", regno),
                new SqlParameter("@ExamMy", examMy),
                new SqlParameter("@Course", course),
                new SqlParameter("@Regulation", regulation),
                new SqlParameter("@Sem", sem)
            };

            string sql = StoredProcSql.Exec(StoredProcedures.SP_Condinatiom_date_CHECK,
                "@Regno", "@ExamMy", "@Course", "@Regulation", "@Sem");

            return await _repo.QueryFromStoredProcAsync(sql, p);
        }

        // 5) Save
        public async Task<int> SaveCondonationAsync(CondonationSaveRequest req)
        {
            var p = new[]
            {
        new SqlParameter("@REGULATION", req.Regulation),
        new SqlParameter("@REGNO", req.Regno),
        new SqlParameter("@SEM", req.Sem),
        new SqlParameter("@CondationAMOUNT", req.CondonationAmount),  // exact SP spelling
        new SqlParameter("@Regu", req.Regu),
        new SqlParameter("@Exammy", req.ExamMy)  // exact SP spelling
    };

            string sql = StoredProcSql.Exec(StoredProcedures.SP_CONDONATION_SAVE,
                "@REGULATION", "@REGNO", "@SEM", "@CondationAMOUNT", "@Regu", "@Exammy");

            return await _repo.ExecuteStoredProcAsync(sql, p);
        }


        // 6) Delete
        public async Task<int> DeleteCondonationAsync(int id)
        {
            string sql = "DELETE FROM tbl_CondonationFee WHERE ID=@ID";

            var p = new SqlParameter("@ID", id);

            return await _repo.ExecuteStoredProcAsync(sql, new[] { p });
        }

        // 7) Format Row
        public async Task<object> GetCondonationFormatAsync()
        {
            string sql = "SELECT TOP 1 * FROM tbl_CondonationFee ORDER BY ID DESC";

            var raw = await _repo.QueryFromStoredProcAsync(sql, null);

            return raw?.FirstOrDefault();
        }

        // 8) Export
        public async Task<IEnumerable<object>> ExportCondonationAsync(string examMy, string regulation)
        {
            var p = new[]
            {
                new SqlParameter("@ExamMy", examMy),
                new SqlParameter("@Regulation", regulation)
            };

            string sql = StoredProcSql.Exec(StoredProcedures.PROC_CONDONATION_EXPORT, "@ExamMy", "@Regulation");

            return await _repo.QueryFromStoredProcAsync(sql, p);
        }
    }
}
