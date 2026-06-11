using ICampus_Models.DTOs;
using ICampus_Models.Requests;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Interfaces
{
    public interface IOmrMarksEntryService
    {
        /// <summary>
        /// Load student record by OMR Number in exam context
        /// Calls PROC_OMRNUM_UPDATE_Get (@Regno, @Course, @Regulation, @ExamMy, @RegSup — confirmed from DLL IL)
        /// Triggered by txtomrNumber AutoPostBack (txtomrNumber_TextChanged1)
        /// Returns: BATCH, GRP, SEM, TEMPCODE, PCODE, PNAME, OMRNUMBER, TMARKS, RVMARKS, V3MARKS
        /// </summary>
        Task<IEnumerable<OmrMarksEntryDto>> LoadByOmrNumberAsync(OmrMarksLoadRequest request);

        /// <summary>
        /// Save marks for the entered OMR number
        /// Calls SP_OMRSAVEMARKSENTRY (@Regulation, @ExamMy, @Course, @OmrNumber[INT], @Marks, @Type, @Sem — confirmed from DLL IL)
        /// OmrNumber is INT (numeric OMRNUMBER from load result — passed without quotes in DLL IL)
        /// Type: "RV" = Revaluation marks, "V3" = V3 marks
        /// Corresponds to btnOmrMarksEntry_Click in reference project
        /// </summary>
        Task<int> SaveMarksAsync(OmrMarksSaveRequest request);
    }
}
