using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Interfaces
{
    public interface IRvClosingDatesService
    {
        /// <summary>
        /// Load RV Closing Dates grid (Page_Load → gvRvCloseDate)
        /// SP: SP_RV_CLOSINGDATES_LIST (@REGULATION, @COURSE, @EXAMMY)
        /// BAL method: rvClosingDates_List (BAL_RVRegistrations)
        /// Returns: REGULATION, COURSE, EXAMMY, SEM, RV_CLOSEDATE, RV_CDATE_SUP
        /// </summary>
        Task<IEnumerable<object>> GetClosingDatesAsync(string regulation, string course, string examMY);

        /// <summary>
        /// Save RV Closing Date for one row (btnSave_Click per row)
        /// SP: SP_RV_CLOSINGDATES_Update (6 params)
        /// BAL method: rvClosingDates_Update (BAL_RVRegistrations)
        /// Params: @Regulation, @Course, @ExamMY, @Sem, @RV_CLOSEDATE, @RV_CDATE_SUP
        /// BOL: RvRegCloseDt (txtReg_DATE), RvSupCloseDt (txtSup_DATE)
        /// </summary>
        Task<int> UpdateClosingDateAsync(
            string regulation, string course, string examMY, string sem,
            string rvCloseDate, string rvCDateSup);
    }
}
