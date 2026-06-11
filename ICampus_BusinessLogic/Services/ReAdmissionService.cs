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
    public class ReAdmissionService : IReAdmissionService
    {
        private readonly IGenericRepository<object> _repo;

        public ReAdmissionService(IGenericRepository<object> repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Load student personal details (Name, Programme, Branch)
        /// SP: SPM_STUDENT_DETAILS
        /// params: @RegNo (varchar)
        /// Confirmed from: DataAccessLayer.dll UTF-16LE "SPM_STUDENT_DETAILS '"
        ///   Same as StudentHistory — txtStudentName, txtCourse, txtGRP display
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
        /// Load the full subject-wise history grid for a student
        /// SP: SPM_STUDENTHISTORY
        /// params: @RegNo (varchar)
        /// Returns: ASHID, PCODE, PNAME, MMARKS, RVMARKS, MRK_FIN, SMARKS, PMARKS, MARKS,
        ///          SEM, GR, Papres, Exammy
        /// Confirmed from: DataAccessLayer.dll UTF-16LE "SPM_STUDENTHISTORY '"
        ///   Same SP as Student History module (dgvStudentHistory grid)
        /// </summary>
        public async Task<IEnumerable<object>> LoadHistoryAsync(string regNo)
        {
            // SPM_STUDENTHISTORY requires both @REGNO and @SEM (no default) — use inline SQL for all sems
            var sql = "SELECT ASHID, REGNO, SEM, PNO, PCODE, TEMPCODE, PNAME, CR, MMARKS, " +
                      "dbo.val(MRK_FIN) + dbo.val(PMarks) AS MRK_FIN, dbo.val(SMARKS) AS SMARKS, " +
                      "PMARKS, MARKS, PAPRES, REGSUP, " +
                      "CASE WHEN RVMARKS IS NULL THEN 'NA' ELSE CAST(RVMARKS AS VARCHAR) END AS RVMARKS, " +
                      "GR, GRPTS, EXAMMY " +
                      "FROM TBL_SH WHERE REGNO = @REGNO ORDER BY SEM, PNO, ASHID";

            var parameters = new[]
            {
                new SqlParameter("@REGNO", SqlDbType.VarChar, 15) { Value = regNo ?? string.Empty }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return result ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Load Entry Type dropdown — all distinct PTYPE values from TBL_PAP
        /// Raw SQL: SELECT DISTINCT Cast(PTYPE as varchar(50)) PTYPE FROM TBL_PAP
        /// No params. Confirmed from: DataAccessLayer.dll UTF-16LE offset 75957
        ///   BAL method: loadPaperType → binds cmbEntryType dropdown
        /// </summary>
        public async Task<IEnumerable<object>> LoadPaperTypesAsync()
        {
            var sql = "SELECT DISTINCT Cast(PTYPE as varchar(50)) PTYPE FROM TBL_PAP";

            var result = await _repo.QueryFromStoredProcAsync(sql, System.Array.Empty<object>());
            return result ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Load marks for one TBL_SH record into the edit modal (getReAdmissionStudentMarks)
        /// SP: SPM_ReAdmissionsStudentMarks
        /// params: @ASHID (varchar — ASHID from CommandArgument when PNAME LinkButton clicked)
        /// Returns: PCODE, PNAME, TMARKS, MMARKS, RVMARKS, SMARKS, PMARKS, PTYPE,
        ///          CR, SGPA_CR, TMAX, TPASS, SMAX, SPASS, PMAX, PPASS, MAXMRK, PASS
        /// Confirmed: DataAccessLayer.dll UTF-16LE "SPM_ReAdmissionsStudentMarks ?"
        ///   BAL method: getReAdmissionStudentMarks (called after set_aSHID in RowCommand)
        ///   Differs from StudentHistory getStudentMarks — returns additional max/pass/type fields
        /// </summary>
        public async Task<IEnumerable<object>> GetMarksByAshIdAsync(string ashId)
        {
            var sql = StoredProcSql.Exec(StoredProcedures.SPM_ReAdmissionsStudentMarks, "@ASHID");

            var parameters = new[]
            {
                new SqlParameter("@ASHID", SqlDbType.VarChar, 20) { Value = ashId ?? string.Empty }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return result ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Load paper structure details when paper code is typed (getPaperDetails)
        /// SP: SPM_PaperDetails_ReAdmission
        /// params: @PCode (varchar — from txtPCode AutoPostBack TextChanged)
        /// Returns: PNAME, PTYPE, CR, SGPA_CR, TMAX, TPASS, SMAX, SPASS, PMAX, PPASS,
        ///          MAXMRK, PASS, P1 (CT Max), P2 (MID Max), ASG (Assignment Max)
        /// Confirmed: DataAccessLayer.dll UTF-16LE "SPM_PaperDetails_ReAdmission '"
        ///   BAL method: getPaperDetails (txtPCode_TextChanged → auto-populates modal fields)
        ///   Also used to populate Theory/Internal/Practical max & pass boxes
        /// </summary>
        public async Task<IEnumerable<object>> GetPaperDetailsAsync(string pCode, string regulation, string sem)
        {
            var sql = StoredProcSql.Exec(StoredProcedures.SPM_PaperDetails_ReAdmission, "@Regulation", "@PCODE", "@SEM");

            var parameters = new[]
            {
                new SqlParameter("@Regulation", SqlDbType.VarChar, 5)  { Value = regulation ?? string.Empty },
                new SqlParameter("@PCODE",      SqlDbType.VarChar, 15) { Value = pCode      ?? string.Empty },
                new SqlParameter("@SEM",        SqlDbType.VarChar, 2)  { Value = sem        ?? string.Empty }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return result ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Update marks for a readmit TBL_SH record (SetReAdmissionsStudentMarks / btnSave_Click)
        /// Raw SQL UPDATE on TBL_SH WHERE ASHID = @ASHID
        /// Confirmed from: DataAccessLayer.dll UTF-16LE fragment (offset 75884)
        ///   "elec =case when (pcode!= tempcode and elec is null) then 'R' else elec end
        ///    WHERE ASHID = ?" — this is the tail of the SET clause
        /// Key notes:
        ///   - elec field is set to 'R' (readmit) only when PCODE differs from tempcode AND elec was NULL
        ///     (tempcode = original paper code stored at time of SH creation)
        ///   - All max/pass fields (TMAX, TPASS, SMAX, SPASS, PMAX, PPASS, MAXMRK, PASS) are updated
        ///     because the readmit regulation may have different mark structures
        ///   - PTYPE (EntryType) controls how marks flow in result process
        ///   - Credits (CR) and SGPA_CR may differ from original regulation
        /// BOL props used: Regulations, Credits, SGPA_Credits, PMax/SMax/TMax, PPass/SPass/TPass/Pass
        ///   App_Web_m2jhophz.dll: set_TMax, set_PMax, set_SMax, set_MaxMrk,
        ///                         set_TPass, set_PPass, set_SPass, set_Pass
        /// </summary>
        public async Task<int> UpdateMarksAsync(UpdateReAdmissionMarks m)
        {
            // PTYPE, TMAX, TPASS, SMAX, SPASS, PMAX, PPASS, MAXMRK, PASS are in TBL_PAP not TBL_SH
            // REGU (batch number '20') must NOT be updated with Regulation code ('R20')
            // — SPM_ReAdmissionsStudentMarks joins TBL_PAP on SH.REGU = P.REGU, so corrupting REGU breaks the SP
            var sql = @"UPDATE TBL_SH SET
                PCODE   = @PCode,
                PNAME   = @PName,
                SEM     = @Sem,
                TMARKS  = @TMarks,
                MMARKS  = @MMarks,
                RVMARKS = @RVMarks,
                SMARKS  = @SMarks,
                PMARKS  = @PMarks,
                CR      = @Credits,
                SUB_CR  = @SgpaCr,
                ELEC    = CASE WHEN (PCODE != TEMPCODE AND ELEC IS NULL) THEN 'R' ELSE ELEC END
            WHERE aSHID = @ASHID";

            var parameters = new[]
            {
                new SqlParameter("@ASHID",      SqlDbType.VarChar, 20)  { Value = m.AshId      ?? string.Empty },
                new SqlParameter("@PCode",      SqlDbType.VarChar, 20)  { Value = m.PCode      ?? string.Empty },
                new SqlParameter("@PName",      SqlDbType.VarChar, 100) { Value = m.PName      ?? string.Empty },
                new SqlParameter("@Sem",        SqlDbType.VarChar, 5)   { Value = m.Sem        ?? string.Empty },
                new SqlParameter("@TMarks",     SqlDbType.VarChar, 10)  { Value = m.TMarks     ?? string.Empty },
                new SqlParameter("@MMarks",     SqlDbType.VarChar, 10)  { Value = m.MMarks     ?? string.Empty },
                new SqlParameter("@RVMarks",    SqlDbType.VarChar, 10)  { Value = m.RVMarks    ?? string.Empty },
                new SqlParameter("@SMarks",     SqlDbType.VarChar, 10)  { Value = m.SMarks     ?? string.Empty },
                new SqlParameter("@PMarks",     SqlDbType.VarChar, 10)  { Value = m.PMarks     ?? string.Empty },
                new SqlParameter("@Credits",    SqlDbType.VarChar, 10)  { Value = m.Credits    ?? string.Empty },
                new SqlParameter("@SgpaCr",     SqlDbType.VarChar, 10)  { Value = m.SgpaCr     ?? string.Empty }
            };

            return await _repo.ExecuteStoredProcAsync(sql, parameters);
        }

        /// <summary>
        /// Delete a TBL_SH record by ASHID
        /// SP: PROC_DEL_ASHID
        /// params: @ASHID (varchar)
        /// Confirmed: DataAccessLayer.dll UTF-16LE "PROC_DEL_ASHID '"
        ///   BAL method: Delete_ashid (btnDelete_Click / btnConfirmOk_Click confirm popup)
        ///   UI: btnDelete visible only when an existing record is loaded in modal
        /// </summary>
        public async Task<int> DeleteByAshIdAsync(string ashId)
        {
            var sql = StoredProcSql.Exec(StoredProcedures.PROC_DEL_ASHID, "@ASHID");

            var parameters = new[]
            {
                new SqlParameter("@ASHID", SqlDbType.VarChar, 20) { Value = ashId ?? string.Empty }
            };

            return await _repo.ExecuteStoredProcAsync(sql, parameters);
        }
    }
}
