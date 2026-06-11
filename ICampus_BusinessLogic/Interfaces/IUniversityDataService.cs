using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Interfaces
{
    public interface IUniversityDataService
    {
        // ── Shared Dropdowns ──────────────────────────────────────────────────────
        // Batch: SELECT DISTINCT REGU, '20'+REGU+'-'+CAST(REGU+MAXSEM/2 AS VARCHAR) BATCH
        //        FROM TBL_COURSE WHERE COURSE=@Course ORDER BY REGU
        // Used by: University_SubjectData.aspx, University_CD_Data.aspx,
        //          University_PC_Formate.aspx (ddlBatch)
        Task<IEnumerable<object>> LoadBatchAsync(string course);

        // Branch: SELECT DISTINCT GRP, GRP+' - '+GSUB BRANCH FROM TBL_COURSE
        //         WHERE COURSE=@Course AND REGU=@Regu ORDER BY GRP
        // Used by: University_PC_Formate.aspx (ddlBatch AutoPostBack → ddlBranch)
        Task<IEnumerable<object>> LoadBranchAsync(string course, string regu);

        // Semester: SELECT DISTINCT cast(SEM as varchar(250)) SEM FROM tbl_sh
        //           WHERE COURSE=@Course and exammy=@ExamMY ORDER BY SEM
        // Used by: UniversityData.aspx (ddlSemester), University_Formate_Data.aspx,
        //          University_SubjectData.aspx, University_CD_Data.aspx
        Task<IEnumerable<object>> LoadSemesterAsync(string course, string exammy);

        // ── UniversityData.aspx — 3 format buttons ────────────────────────────────
        // btnview  → "Result Format"     → Get_Univeristy_Formate  (BAL_Reports_Results)
        // btnView2 → "Registred Format"  → Get_Univeristy_Formate_R18
        // btnview3 → "Registred Format2" → UniversityData_format2
        // All three call PROC_EXPORT_MARKSDATA; RegSup discriminates output type.
        // Course + REGU + ExamMY come from master page session (client passes them).

        /// <summary>Result Format — all students (RegSup='')</summary>
        Task<IEnumerable<object>> GetResultFormatAsync(string course, string regu, string sem, string exammy);

        /// <summary>Registred Format — regular students (RegSup='REG')</summary>
        Task<IEnumerable<object>> GetRegisteredFormatAsync(string course, string regu, string sem, string exammy);

        /// <summary>Registred Format2 — supplementary students (RegSup='SUP')</summary>
        Task<IEnumerable<object>> GetRegisteredFormat2Async(string course, string regu, string sem, string exammy);

        // ── University_Formate_Data.aspx — btnDownLoad ────────────────────────────
        // BAL: Univeristy_Formate_Data (App_Web_m2jhophz → iCampus_Results_University_Formate_Data)
        // Controls: ddlSemester, ddlRegsup (SELECT/REG/SUP)
        // SP: PROC_EXPORT_MARKSDATA(@Course,@REGU,@Sem,@RegSup,@ExamMY,@IsRv=0)
        Task<IEnumerable<object>> GetFormateDataAsync(
            string course, string regu, string sem, string regSup, string exammy);

        // ── University_SubjectData.aspx ───────────────────────────────────────────
        // Controls: ddlbatch, ddlregsup (REG/SUP), ddlSemester, ChkIsrv (hidden)
        // SP: PROC_EXPORT_MARKSDATA(@Course,@REGU,@Sem,@RegSup,@ExamMY,@IsRv)

        /// <summary>btnSubjectList → "Subjects Data"</summary>
        Task<IEnumerable<object>> GetSubjectListAsync(
            string course, string regu, string sem, string regSup, string exammy, bool isRv);

        /// <summary>btnSubjectData → "Students Data"</summary>
        Task<IEnumerable<object>> GetSubjectDataAsync(
            string course, string regu, string sem, string regSup, string exammy, bool isRv);

        // ── University_CD_Data.aspx ───────────────────────────────────────────────
        // Same controls as University_SubjectData.aspx
        // BAL: iCampus_Reports_University_CD_Data_aspx (App_Web_gp3pforx)
        // SP: PROC_EXPORT_MARKSDATA(@Course,@REGU,@Sem,@RegSup,@ExamMY,@IsRv)

        /// <summary>btnSubjectList → "Subjects Data"</summary>
        Task<IEnumerable<object>> GetCdSubjectListAsync(
            string course, string regu, string sem, string regSup, string exammy, bool isRv);

        /// <summary>btnStudentData → "Students Data"</summary>
        Task<IEnumerable<object>> GetCdStudentDataAsync(
            string course, string regu, string sem, string regSup, string exammy, bool isRv);

        // ── University_PC_Formate.aspx — btnDownLoad ──────────────────────────────
        // SP: PROC_AWARD_DEGREE_UNIVERSITY(@regulation,@course,@exammy,@regu,@sem,@type)
        // @type values: 'CourseComplete' | 'All' | 'Regnowise' | 'JNTUK CE'
        Task<IEnumerable<object>> GetPcFormatAsync(
            string course, string regu, string exammy, string sem, string type);
    }
}
