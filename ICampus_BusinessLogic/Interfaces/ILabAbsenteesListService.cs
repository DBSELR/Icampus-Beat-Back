using ICampus_Models.Requests;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Interfaces
{
    public interface ILabAbsenteesListService
    {
        /// <summary>
        /// Load semester list for LabAbsenteesList dropdown (populated on Page_Load)
        /// Inline SQL on tbl_sh: select distinct sem from tbl_sh where course=@Course order by sem
        /// Same pattern as AbsenteesList — 1 param: Course
        /// </summary>
        Task<IEnumerable<object>> GetSemestersAsync(string course);

        /// <summary>
        /// Load branch list — cascade triggered by ddlSemester_SelectedIndexChanged
        /// Inline SQL on TBL_COURSE:
        ///   SELECT DISTINCT GSUB GRP, GRP ID FROM TBL_COURSE WHERE COURSE=@Course AND REGU=@Regulation ORDER BY ID
        /// 2 params: Course, Regulation
        /// </summary>
        Task<IEnumerable<object>> GetBranchesAsync(string course, string regulation);

        /// <summary>
        /// Load subject list — cascade triggered by ddlBranch_SelectedIndexChanged
        /// Inline SQL confirmed from DLL IL (DataAccessLayer.dll offset 0x25AF7):
        ///   select distinct pno, pcode, pcode+'-'+pname pname
        ///   from tbl_sh where Course=@Course and Regu=@Regulation and grp=@GRP and sem=@SEM order by pno
        /// 4 params: Course, Regulation, GRP, SEM (SEM is numeric/unquoted in original)
        /// Returns: pcode (value), pname (display: "CS301-Data Structures")
        /// </summary>
        Task<IEnumerable<object>> GetSubjectsAsync(string course, string regulation, string grp, string sem);

        /// <summary>
        /// Generate Lab Absentees List report
        /// No stored procedure — inline SQL confirmed from DLL IL Crystal Reports selection formula:
        ///   SELECT * FROM tbl_SH WHERE Course=@Course AND ExamMY=@ExamMY
        ///   AND PMARKS IN ('ab','sm') AND PCode=@PCode AND grp=@GRP
        /// ('ab' = Absent, 'sm' = Supplementary-Malpractice)
        /// </summary>
        Task<IEnumerable<object>> GetReportDataAsync(LabAbsenteesListRequest request);
    }
}
