using ICampus_BusinessLogic.Interfaces;
using ICampus_DataAccessLayer.Helpers;
using ICampus_DataAccessLayer.Interfaces;
using ICampus_Models.Requests;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Services
{
    public class MidHallTicketService : IMidHallTicketService
    {
        private readonly IGenericRepository<object> _repo;

        public MidHallTicketService(IGenericRepository<object> repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Prepare/generate mid hall ticket data
        /// Stored Procedure: SPM_HallTicket_Mid
        /// Parameters: @ExamMY, @Course, @regulation, @Sem, @Regu, @Branch, @Regno, @examtype
        /// Note: Called via InsertData (INSERT/UPDATE operation), not GetData
        /// All parameters are passed as strings (even if empty)
        /// SP handles empty strings: if @Branch ='' set @Branch=null, if @Regno='' set @Regno = null
        /// ExamType expects: 'MID-I' or 'MID-II' (text, not numeric)
        /// </summary>
        public async Task<int> PrepareMidHallTicketsAsync(MidHallTicketRequest request)
        {
            // Convert examType from numeric to text if needed (1=MID-I, 2=MID-II)
            string examTypeText = ConvertExamTypeToText(request.ExamType);

            var parameters = new[]
            {
                new SqlParameter("@ExamMY", SqlDbType.VarChar, 20) { Value = request.ExamMY ?? string.Empty },
                new SqlParameter("@Course", SqlDbType.VarChar, 30) { Value = request.Course ?? string.Empty },
                new SqlParameter("@regulation", SqlDbType.VarChar, 30) { Value = request.Regulation ?? string.Empty },
                new SqlParameter("@Sem", SqlDbType.VarChar, 25) { Value = request.Sem ?? string.Empty },
                new SqlParameter("@Regu", SqlDbType.VarChar, 25) { Value = request.Batch ?? string.Empty },
                new SqlParameter("@Branch", SqlDbType.VarChar, 150) { Value = request.Branch ?? string.Empty },
                new SqlParameter("@Regno", SqlDbType.VarChar, 25) { Value = request.Regno ?? string.Empty },
                new SqlParameter("@examtype", SqlDbType.VarChar, 20) { Value = examTypeText }
            };

            var sql = StoredProcSql.Exec(StoredProcedures.SPM_HallTicket_Mid, "@ExamMY", "@Course", "@regulation", "@Sem", "@Regu", "@Branch", "@Regno", "@examtype");
            var rowsAffected = await _repo.ExecuteStoredProcAsync(sql, parameters);
            return rowsAffected;
        }

        /// <summary>
        /// Get mid hall ticket data after preparation
        /// Queries the tbl_hallticket table populated by SPM_HallTicket_Mid
        /// </summary>
        public async Task<IEnumerable<object>> GetMidHallTicketDataAsync(MidHallTicketRequest request)
        {
            // Query the tbl_hallticket table (populated by SPM_HallTicket_Mid)
            var sql = "SELECT * FROM tbl_hallticket WHERE 1=1";
            var parameters = new List<SqlParameter>();

            if (!string.IsNullOrWhiteSpace(request.ExamMY))
            {
                sql += " AND ExamMY = @ExamMY";
                parameters.Add(new SqlParameter("@ExamMY", SqlDbType.VarChar) { Value = request.ExamMY });
            }

            if (!string.IsNullOrWhiteSpace(request.Course))
            {
                sql += " AND Course = @Course";
                parameters.Add(new SqlParameter("@Course", SqlDbType.VarChar) { Value = request.Course });
            }

            if (!string.IsNullOrWhiteSpace(request.Regulation))
            {
                sql += " AND Regulation = @Regulation";
                parameters.Add(new SqlParameter("@Regulation", SqlDbType.VarChar) { Value = request.Regulation });
            }

            if (!string.IsNullOrWhiteSpace(request.Batch))
            {
                sql += " AND REGU = @Batch";
                parameters.Add(new SqlParameter("@Batch", SqlDbType.VarChar) { Value = request.Batch });
            }

            if (!string.IsNullOrWhiteSpace(request.Branch))
            {
                sql += " AND GRP = @Branch";
                parameters.Add(new SqlParameter("@Branch", SqlDbType.VarChar) { Value = request.Branch });
            }

            if (!string.IsNullOrWhiteSpace(request.Sem))
            {
                sql += " AND Sem = @Sem";
                parameters.Add(new SqlParameter("@Sem", SqlDbType.VarChar) { Value = request.Sem });
            }

            if (!string.IsNullOrWhiteSpace(request.Regno))
            {
                sql += " AND Regno = @Regno";
                parameters.Add(new SqlParameter("@Regno", SqlDbType.VarChar) { Value = request.Regno });
            }

            // Filter by ETYPE if ExamType is provided (SP sets ETYPE=@examtype in tbl_hallticket)
            if (!string.IsNullOrWhiteSpace(request.ExamType))
            {
                string examTypeText = ConvertExamTypeToText(request.ExamType);
                sql += " AND ETYPE = @ETYPE";
                parameters.Add(new SqlParameter("@ETYPE", SqlDbType.VarChar) { Value = examTypeText });
            }

            var raw = await _repo.QueryFromStoredProcAsync(sql, parameters.ToArray());
            return raw ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Convert exam type from numeric to text format
        /// Input: "1" or "MID-I" -> Output: "MID-I"
        /// Input: "2" or "MID-II" -> Output: "MID-II"
        /// </summary>
        private string ConvertExamTypeToText(string examType)
        {
            if (string.IsNullOrWhiteSpace(examType))
                return string.Empty;

            // If already in text format, return as-is
            if (examType.Equals("MID-I", StringComparison.OrdinalIgnoreCase) ||
                examType.Equals("MID-II", StringComparison.OrdinalIgnoreCase))
            {
                return examType;
            }

            // Convert numeric to text
            switch (examType)
            {
                case "1":
                    return "MID-I";
                case "2":
                    return "MID-II";
                default:
                    return examType; // Return as-is if not recognized
            }
        }
    }
}

