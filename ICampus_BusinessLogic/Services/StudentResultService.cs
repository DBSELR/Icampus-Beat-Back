using ICampus_BusinessLogic.Interfaces;
using ICampus_DataAccessLayer.Helpers;
using ICampus_DataAccessLayer.Interfaces;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Services
{
    public class StudentResultService : IStudentResultService
    {
        private readonly IGenericRepository<object> _repo;

        public StudentResultService(IGenericRepository<object> repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Load student header details — SPM_STUDENT_DETAILS
        /// Triggered by txtRegNo AutoPostBack (txtRegNo_TextChanged)
        /// Fills txtCourse, txtStudentName, txtGRP on the page
        /// </summary>
        public async Task<IEnumerable<object>> LoadStudentDetailsAsync(string regNo)
        {
            var sql = StoredProcSql.Exec(StoredProcedures.SPM_STUDENT_DETAILS, "@RegNo");

            var parameters = new[]
            {
                new SqlParameter("@RegNo", SqlDbType.VarChar, 20) { Value = regNo ?? string.Empty }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return result ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Load SGPA/CGPA grid — SP_STUDENT_SGPA_CGPA
        /// Triggered on RegNo entry; fills gvSGPA_CGPA
        /// Returns: SEM, TCR, SCR, REGNO, SGPA, CGPA, RNO, BACKLOGS
        /// </summary>
        public async Task<IEnumerable<object>> LoadSgpaCgpaAsync(string regNo)
        {
            var sql = "EXEC dbo.SP_STUDENT_SGPA_CGPA @REGNO";

            var parameters = new[]
            {
                new SqlParameter("@REGNO", SqlDbType.VarChar, 15) { Value = regNo ?? string.Empty }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return result ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Passed papers — SPM_REQ_DATA_FOR_PASSEDPAPLIST
        /// RBPassedList radio button click
        /// @REGNO, @EXAMMY, @CON ('true' = up to examMY, 'false' = all)
        /// Returns: REGNO, SEM, PNO, PCODE, PNAME, GR, EXAMMY, PAPRES, REGD, REGSUP, CR, Course, SName, GSUB
        /// </summary>
        public async Task<IEnumerable<object>> GetPassedPapersAsync(string regNo, string examMY, string con)
        {
            var sql = "EXEC dbo.SPM_REQ_DATA_FOR_PASSEDPAPLIST @REGNO, @EXAMMY, @CON";

            var parameters = new[]
            {
                new SqlParameter("@REGNO",  SqlDbType.VarChar, 15) { Value = regNo  ?? string.Empty },
                new SqlParameter("@EXAMMY", SqlDbType.VarChar, 20) { Value = examMY ?? string.Empty },
                new SqlParameter("@CON",    SqlDbType.VarChar, 10) { Value = con    ?? "true" }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return result ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Failed papers — SPM_REQ_DATA_FOR_FAILEDPAPLIST
        /// RBFailedList radio button click
        /// @REGNO, @EXAMMY, @CON ('true' = up to examMY, 'false' = all)
        /// Returns: REGNO, SEM, PNO, PCODE, PNAME, GR, EXAMMY, PAPRES, REGD, REGSUP, CR, SNAME, Course, GSUB
        /// </summary>
        public async Task<IEnumerable<object>> GetFailedPapersAsync(string regNo, string examMY, string con)
        {
            var sql = "EXEC dbo.SPM_REQ_DATA_FOR_FAILEDPAPLIST @REGNO, @EXAMMY, @CON";

            var parameters = new[]
            {
                new SqlParameter("@REGNO",  SqlDbType.VarChar, 15) { Value = regNo  ?? string.Empty },
                new SqlParameter("@EXAMMY", SqlDbType.VarChar, 20) { Value = examMY ?? string.Empty },
                new SqlParameter("@CON",    SqlDbType.VarChar, 10) { Value = con    ?? "true" }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return result ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// All papers (Rbtn_allFP) — both passed and failed — raw SQL on TBL_SH
        /// When examMY is provided, filters to that specific exam month/year only.
        /// Pass examMY = "" to return all papers across all semesters.
        /// Returns: REGNO, SEM, PCODE, PNAME, GR, EXAMMY, PAPRES, REGSUP, CR
        /// </summary>
        public async Task<IEnumerable<object>> GetAllPapersAsync(string regNo, string examMY)
        {
            string sql;
            object[] parameters;

            if (!string.IsNullOrWhiteSpace(examMY))
            {
                sql = @"SELECT DISTINCT H.REGNO, H.SEM, H.PCODE, H.PNAME, H.GR, H.EXAMMY, H.PAPRES, H.REGSUP, H.CR
                        FROM TBL_SH H
                        WHERE H.REGNO = @RegNo AND H.EXAMMY = @ExamMY
                        ORDER BY H.SEM, H.PCODE";

                parameters = new object[]
                {
                    new SqlParameter("@RegNo",  SqlDbType.VarChar, 15) { Value = regNo  ?? string.Empty },
                    new SqlParameter("@ExamMY", SqlDbType.VarChar, 20) { Value = examMY ?? string.Empty }
                };
            }
            else
            {
                sql = @"SELECT DISTINCT H.REGNO, H.SEM, H.PCODE, H.PNAME, H.GR, H.EXAMMY, H.PAPRES, H.REGSUP, H.CR
                        FROM TBL_SH H
                        WHERE H.REGNO = @RegNo
                        ORDER BY H.SEM, H.PCODE";

                parameters = new object[]
                {
                    new SqlParameter("@RegNo", SqlDbType.VarChar, 15) { Value = regNo ?? string.Empty }
                };
            }

            var result = await _repo.QueryFromStoredProcAsync(sql, parameters);
            return result ?? Enumerable.Empty<object>();
        }
    }
}
