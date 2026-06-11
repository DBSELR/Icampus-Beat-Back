using ICampus_Models.Requests;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Interfaces
{
    public interface IAbsenteesListService
    {
        /// <summary>
        /// Load semester list for AbsenteesList dropdown (populated on Page_Load)
        /// Inline SQL on tbl_sh: select distinct sem from tbl_sh where course=@Course order by sem
        /// Confirmed from DLL IL: DataAccessLayer.dll — 1 param: Course
        /// </summary>
        Task<IEnumerable<object>> GetSemestersAsync(string course);

        /// <summary>
        /// Load exam date list — critical cascade triggered by ddlSemester_SelectedIndexChanged
        /// Inline SQL on tbl_sh confirmed from DLL IL (method Get_Exams_Edates, 3 params):
        ///   select distinct convert(nVarchar,EDate,105) edate1, convert(nVarchar,EDate,105) EDATE
        ///   from tbl_sh WHERE EDATE IS NOT NULL AND COURSE=@Course AND sem=@Sem AND ExamMY=@ExamMY
        /// Returns edate1 (dd-MM-yyyy display) and EDATE (raw)
        /// Pass the raw EDATE formatted as yyyy-MM-dd when calling GetReportDataAsync
        /// </summary>
        Task<IEnumerable<object>> GetEdatesAsync(string course, string examMY, string sem);

        /// <summary>
        /// Generate Theory Absentees List report
        /// Calls SP_REP_ABLIST
        /// 5 positional params confirmed from DLL IL (App_Web_gp3pforx.dll offset 0x000128e7):
        ///   Regulation(1,varchar), Course(2,varchar), ExamMY(3,varchar),
        ///   SEM(4,INT unquoted optional), EDate(5,varchar yyyy-MM-dd optional)
        /// </summary>
        Task<IEnumerable<object>> GetReportDataAsync(AbsenteesListRequest request);
    }
}
