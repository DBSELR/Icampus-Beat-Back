using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Interfaces
{
    public interface IOmrNumberUpdateService
    {
        /// <summary>
        /// Load OMR grid for a given exam
        /// SP: PROC_REGNOVSOMR (@REGULATION, @COURSE, @EXAMMY)
        /// Returns: aSHID, REGNO, GRP, SEM, TEMPCODE, PCODE, PNAME, OMRNUMBER, PKTNO, SCANNED_SNO, SNO
        /// BAL: BAL_StudentWiseMasterCreation → Get_loadOmrNumUpdate
        /// </summary>
        Task<IEnumerable<object>> LoadOmrGridAsync(string regulation, string course, string exammy);

        /// <summary>
        /// Update OMR number for a single row using composite key (REGNO + PCODE + EXAMMY)
        /// SQL: UPDATE TBL_SH SET OMRNUMBER = @OmrNo WHERE REGNO=@Regno AND PCODE=@PCode AND EXAMMY=@ExamMY
        /// </summary>
        Task<int> UpdateOmrNumAsync(string regno, string pcode, string exammy, string omrNo);
    }
}
