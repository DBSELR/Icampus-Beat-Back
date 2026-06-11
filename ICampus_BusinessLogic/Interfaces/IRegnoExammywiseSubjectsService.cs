using ICampus_Models.DTOs;
using ICampus_Models.Requests;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Interfaces
{
    public interface IRegnoExammywiseSubjectsService
    {
        /// <summary>
        /// Load all subject rows for a student by RegNo, Course, Regulation, ExamMY
        /// Calls PROC_OMRNUM_UPDATE (@Regno, @Course, @Regulation, @ExamMy — confirmed from DLL IL)
        /// 4 positional params, all varchar quoted in IL template
        /// Triggered by txtregno AutoPostBack in Regno_Exammywise_Subjets.aspx
        /// Returns: aSHID, PTYPE, Exammy, sem, PCode, TempCode, PName, smarks, TMARKS, RVMARKS, V3, MRK_FIN, pmarks
        /// </summary>
        Task<IEnumerable<RegnoExammywiseSubjectRowDto>> LoadSubjectsAsync(RegnoExammywiseLoadRequest request);

        /// <summary>
        /// Load all subject rows filtered by Reg/Sup type (btnGetData_Click with ddlSemType)
        /// Calls PROC_OMRNUM_UPDATE_Get (@Regno, @Course, @Regulation, @ExamMy, @RegSup — confirmed from DLL IL)
        /// 5 positional params, all varchar quoted in IL template
        /// Corresponds to Get_LoadOmrGrid → Get_loadOmrNumUpdate in the compiled code-behind
        /// RegSup: "Reg" = Regular, "Sup" = Supplementary
        /// Returns same columns as LoadSubjectsAsync
        /// </summary>
        Task<IEnumerable<RegnoExammywiseSubjectRowDto>> LoadSubjectsByRegSupAsync(RegnoExammywiseGetLoadRequest request);

        /// <summary>
        /// Save marks for all grid rows — one SP call per paper row
        /// Calls PROC_marksupdate_regnowise (@AshId, @Smarks, @Pmarks, @Tmarks, @Rvmarks, @v3marks, @Tmrk_final — confirmed from DLL IL)
        /// 7 positional params in that exact order
        /// Corresponds to btnOmrNo "UPDATE MARKS" click in Regno_Exammywise_Subjets.aspx
        /// </summary>
        Task<int> SaveMarksAsync(RegnoExammywiseSaveRequest request);
    }
}
