using ClosedXML.Excel;
using ICampus_BusinessLogic.Interfaces;
using ICampus_DataAccessLayer.Helpers;
using ICampus_DataAccessLayer.Interfaces;
using ICampus_Models.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Services
{
    public class ImportService : IImportService
    {
        private readonly IGenericRepository<object> _repo;
        public ImportService(IGenericRepository<object> repo) => _repo = repo;

        public async Task<IEnumerable<string>> GetExcelSheetsAsync(IFormFile file)
        {
            var sheets = new List<string>();
            using (var ms = new MemoryStream())
            {
                await file.CopyToAsync(ms);
                using (var wb = new XLWorkbook(ms))
                {
                    foreach (var ws in wb.Worksheets) sheets.Add(ws.Name);
                }
            }
            return sheets;
        }

        public async Task<DataTable> BuildDataTableFromIFormFileAsync(IFormFile file, string sheetName, bool hasHeader = true)
        {
            using (var ms = new MemoryStream())
            {
                await file.CopyToAsync(ms);
                using (var wb = new XLWorkbook(ms))
                {
                    var ws = string.IsNullOrWhiteSpace(sheetName) ? wb.Worksheets.First() : wb.Worksheets.FirstOrDefault(s => s.Name == sheetName) ?? wb.Worksheets.First();
                    var dt = new DataTable();

                    var rows = ws.RowsUsed();
                    bool firstRow = true;
                    foreach (var row in rows)
                    {
                        var cells = row.CellsUsed().ToList();
                        if (firstRow)
                        {
                            if (hasHeader)
                            {
                                foreach (var cell in cells)
                                {
                                    var colName = string.IsNullOrWhiteSpace(cell.GetString()) ? $"Column{cell.Address.ColumnNumber}" : cell.GetString();
                                    dt.Columns.Add(colName);
                                }
                                firstRow = false;
                                continue;
                            }
                            else
                            {
                                foreach (var cell in cells)
                                {
                                    dt.Columns.Add($"Column{cell.Address.ColumnNumber}");
                                }
                                firstRow = false;
                            }
                        }

                        var dr = dt.NewRow();
                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            dr[i] = i < cells.Count ? (cells[i].IsEmpty() ? string.Empty : cells[i].GetString()) : string.Empty;
                        }
                        dt.Rows.Add(dr);
                    }
                    return dt;
                }
            }
        }

        public async Task<int> ImportStudentDataAsync(ImportStudentRequest request)
        {
            if (request.File == null) return 0;
            var dt = await BuildDataTableFromIFormFileAsync(request.File, request.SheetName, request.HasHeader);

            var saved = 0;
            foreach (DataRow r in dt.Rows)
            {
                var ps = new[]
                {
                    new SqlParameter("@REGU", SqlDbType.VarChar) { Value = r.Table.Columns.Contains("Regu") ? Convert.ToString(r["Regu"]) ?? string.Empty : (object)DBNull.Value },
                    new SqlParameter("@COURSE", SqlDbType.VarChar) { Value = r.Table.Columns.Contains("Course") ? Convert.ToString(r["Course"]) ?? string.Empty : (object)DBNull.Value },
                    new SqlParameter("@GRP", SqlDbType.VarChar) { Value = r.Table.Columns.Contains("grp") ? Convert.ToString(r["grp"]) ?? string.Empty : (object)DBNull.Value },
                    new SqlParameter("@STREAM", SqlDbType.VarChar) { Value = r.Table.Columns.Contains("STREAM") ? Convert.ToString(r["STREAM"]) ?? string.Empty : (object)DBNull.Value },
                    new SqlParameter("@REGNO", SqlDbType.VarChar) { Value = r.Table.Columns.Contains("RegNo") ? Convert.ToString(r["RegNo"]) ?? string.Empty : (object)DBNull.Value },
                    new SqlParameter("@ROLLNUM", SqlDbType.VarChar) { Value = "001" },
                    new SqlParameter("@MEDIUM", SqlDbType.VarChar) { Value = "ENGLISH" },
                    new SqlParameter("@SECTION", SqlDbType.VarChar) { Value = r.Table.Columns.Contains("Section") ? Convert.ToString(r["Section"]) ?? string.Empty : (object)DBNull.Value },
                    new SqlParameter("@SNAME", SqlDbType.VarChar) { Value = r.Table.Columns.Contains("SName") ? Convert.ToString(r["SName"]) ?? string.Empty : (object)DBNull.Value },
                    new SqlParameter("@FNAME", SqlDbType.VarChar) { Value = r.Table.Columns.Contains("FName") ? Convert.ToString(r["FName"]) ?? string.Empty : (object)DBNull.Value },
                    new SqlParameter("@MNAME", SqlDbType.VarChar) { Value = r.Table.Columns.Contains("MName") ? Convert.ToString(r["MName"]) ?? string.Empty : (object)DBNull.Value },
                    new SqlParameter("@DOB", SqlDbType.VarChar) { Value = ParseDateOrDbNull(r, "DOB") },
                    new SqlParameter("@CASTE", SqlDbType.VarChar) { Value = r.Table.Columns.Contains("Caste") ? Convert.ToString(r["Caste"]) ?? string.Empty : (object)DBNull.Value },
                    new SqlParameter("@GENDER", SqlDbType.VarChar) { Value = r.Table.Columns.Contains("Gender") ? Convert.ToString(r["Gender"]) ?? string.Empty : (object)DBNull.Value },
                    new SqlParameter("@ADDRES", SqlDbType.VarChar) { Value = r.Table.Columns.Contains("Addres") ? Convert.ToString(r["Addres"]) ?? string.Empty : (object)DBNull.Value },
                    new SqlParameter("@MOBILE", SqlDbType.VarChar) { Value = r.Table.Columns.Contains("Mobile") ? Convert.ToString(r["Mobile"]) ?? string.Empty : (object)DBNull.Value },
                    new SqlParameter("@EMAIL", SqlDbType.VarChar) { Value = r.Table.Columns.Contains("Email") ? Convert.ToString(r["Email"]) ?? string.Empty : (object)DBNull.Value },
                    new SqlParameter("@PINCODE", SqlDbType.VarChar) { Value = r.Table.Columns.Contains("Pincode") ? Convert.ToString(r["Pincode"]) ?? string.Empty : (object)DBNull.Value },
                    new SqlParameter("@REGULATION", SqlDbType.VarChar) { Value = !string.IsNullOrWhiteSpace(request.Regulation) ? request.Regulation : (r.Table.Columns.Contains("REGULATION") ? Convert.ToString(r["REGULATION"]) ?? string.Empty : (object)DBNull.Value) },
                    new SqlParameter("@PARENTMOBILE", SqlDbType.VarChar) { Value = r.Table.Columns.Contains("PARENTMOBILE") ? Convert.ToString(r["PARENTMOBILE"]) ?? string.Empty : (object)DBNull.Value },
                    new SqlParameter("@AADHARNO", SqlDbType.VarChar) { Value = r.Table.Columns.Contains("AADHARNO") ? Convert.ToString(r["AADHARNO"]) ?? string.Empty : (object)DBNull.Value }
                };

                // make sure the enum has sp_stdDATA_Importing exactly
                var sql = StoredProcSql.Exec(StoredProcedures.sp_stdDATA_Importing,
                    "@REGU", "@COURSE", "@GRP", "@STREAM", "@REGNO", "@ROLLNUM", "@MEDIUM", "@SECTION", "@SNAME", "@FNAME", "@MNAME", "@DOB", "@CASTE", "@GENDER", "@ADDRES", "@MOBILE", "@EMAIL", "@PINCODE", "@REGULATION", "@PARENTMOBILE", "@AADHARNO");

                saved += await _repo.ExecuteStoredProcAsync(sql, ps);
            }
            return saved;
        }

        private object ParseDateOrDbNull(DataRow r, string colName)
        {
            if (!r.Table.Columns.Contains(colName)) return DBNull.Value;
            var s = Convert.ToString(r[colName]);
            if (string.IsNullOrWhiteSpace(s)) return DBNull.Value;
            if (DateTime.TryParse(s, out var dt)) return dt.ToString("yyyy-MM-dd");
            return DBNull.Value;
        }

        private DataTable ConvertJsonRowsToDataTable(List<Dictionary<string, object>> rows)
        {
            var dt = new DataTable();
            if (rows == null || rows.Count == 0) return dt;
            foreach (var k in rows[0].Keys) dt.Columns.Add(k);
            foreach (var row in rows)
            {
                var dr = dt.NewRow();
                foreach (var kv in row) dr[kv.Key] = kv.Value ?? string.Empty;
                dt.Rows.Add(dr);
            }
            return dt;
        }

        private DataTable ConvertEnumerableToDataTable(IEnumerable<object> raw)
        {
            var dt = new DataTable();
            if (raw == null) return dt;

            var json = JsonConvert.SerializeObject(raw);
            var list = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(json);
            if (list == null || list.Count == 0) return dt;

            var columns = new HashSet<string>(list.SelectMany(d => d.Keys));
            foreach (var c in columns) dt.Columns.Add(c);

            foreach (var dict in list)
            {
                var dr = dt.NewRow();
                foreach (var kv in dict) dr[kv.Key] = kv.Value ?? string.Empty;
                dt.Rows.Add(dr);
            }
            return dt;
        }

        public async Task<int> ImportElectiveDataAsync(ImportTvTableRequest request)
        {
            var dt = request.DataTable ?? ConvertJsonRowsToDataTable(request.JsonRows);
            var param = new SqlParameter("@elecdata", SqlDbType.Structured)
            {
                TypeName = "Elecdata_import",
                Value = dt
            };

            var sql = StoredProcSql.Exec(StoredProcedures.SPM_ELECTIVEDATA_IMPORTING, "@elecdata");
            return await _repo.ExecuteStoredProcAsync(sql, param);
        }

        public async Task<int> ImportExamSessionDatesAsync(ImportTvTableRequest request)
        {
            var dt = request.DataTable ?? ConvertJsonRowsToDataTable(request.JsonRows);
            var param = new SqlParameter("@ExamSessionDate", SqlDbType.Structured)
            {
                TypeName = "Import_Exam_Session_Date",
                Value = dt
            };

            var sql = StoredProcSql.Exec(StoredProcedures.SP_Import_Exam_Session_Date, "@ExamSessionDate");
            return await _repo.ExecuteStoredProcAsync(sql, param);
        }

        public async Task<int> ImportCondonationFeeAsync(ImportTvTableRequest request)
        {
            var dt = request.DataTable ?? ConvertJsonRowsToDataTable(request.JsonRows);
            var param = new SqlParameter("@CONDONATION_FEE", SqlDbType.Structured)
            {
                TypeName = "IMPORT_CONDONATION_FEE",
                Value = dt
            };

            var sql = StoredProcSql.Exec(StoredProcedures.SP_IMPORT_CONDONATION_FEE, "@CONDONATION_FEE");
            return await _repo.ExecuteStoredProcAsync(sql, param);
        }

        public async Task<int> ImportRegistrationBlockAsync(ImportTvTableRequest request)
        {
            var dt = request.DataTable ?? ConvertJsonRowsToDataTable(request.JsonRows);
            var param = new SqlParameter("@Reg_block", SqlDbType.Structured)
            {
                TypeName = "Import_Registration_block",
                Value = dt
            };

            var sql = StoredProcSql.Exec(StoredProcedures.Proc_Import_Registration_block, "@Reg_block");
            return await _repo.ExecuteStoredProcAsync(sql, param);
        }

        public async Task<int> ImportHallticketsBlockAsync(ImportTvTableRequest request)
        {
            var dt = request.DataTable ?? ConvertJsonRowsToDataTable(request.JsonRows);
            var param = new SqlParameter("@Hall_block", SqlDbType.Structured)
            {
                TypeName = "Import_Halltickets_block",
                Value = dt
            };

            var sql = StoredProcSql.Exec(StoredProcedures.Proc_Import_Halltickets_block, "@Hall_block");
            return await _repo.ExecuteStoredProcAsync(sql, param);
        }

        public async Task<int> ImportResultBlockAsync(ImportTvTableRequest request)
        {
            var dt = request.DataTable ?? ConvertJsonRowsToDataTable(request.JsonRows);
            var param = new SqlParameter("@Result_block", SqlDbType.Structured)
            {
                TypeName = "Import_Result_block",
                Value = dt
            };

            var sql = StoredProcSql.Exec(StoredProcedures.Proc_Import_Result_block, "@Result_block");
            return await _repo.ExecuteStoredProcAsync(sql, param);
        }

        public async Task<int> ImportInternalMarksAsync(ImportTvTableRequest request)
        {
            var dt = request.DataTable ?? ConvertJsonRowsToDataTable(request.JsonRows);
            var param = new SqlParameter("@Internal_Improve", SqlDbType.Structured)
            {
                TypeName = "Import_Internal_ImproveMarks",
                Value = dt
            };

            var sql = StoredProcSql.Exec(StoredProcedures.Proc_Import_Internal_ImproveMarks, "@Internal_Improve");
            return await _repo.ExecuteStoredProcAsync(sql, param);
        }

        public async Task<int> ImportAuditCourseDataAsync(ImportTvTableRequest request)
        {
            var dt = request.DataTable ?? ConvertJsonRowsToDataTable(request.JsonRows);
            var param = new SqlParameter("@audit_course", SqlDbType.Structured)
            {
                TypeName = "Import_AuditCourse_Data",
                Value = dt
            };

            var sql = StoredProcSql.Exec(StoredProcedures.Proc_Import_AuditCourse_Data, "@audit_course");
            return await _repo.ExecuteStoredProcAsync(sql, param);
        }

        public async Task<int> ImportRCRVMarksAsync(ImportTvTableRequest request)
        {
            var dt = request.DataTable ?? ConvertJsonRowsToDataTable(request.JsonRows);
            var param = new SqlParameter("@RCRV_Marks", SqlDbType.Structured)
            {
                TypeName = "Import_RCRV_Marks",
                Value = dt
            };

            var sql = StoredProcSql.Exec(StoredProcedures.Proc_Import_RCRV_Marks, "@RCRV_Marks");
            return await _repo.ExecuteStoredProcAsync(sql, param);
        }

        public async Task<DataTable> LoadExcelFormatExtraColumnsAsync(string tableName, string userId)
        {
            var q = $"SP_EXCEL_FORMAT_ExtraColumns '{tableName}', '{userId}'";
            var raw = await _repo.QueryFromStoredProcAsync(q);
            var dt = ConvertEnumerableToDataTable(raw);
            return dt;
        }

        public async Task<IEnumerable<object>> LoadCourseListAsync(string regulation)
        {
            var sql = StoredProcSql.Exec(StoredProcedures.SPM_COURSE_LIST, "@REGULATION");
            var p = new SqlParameter("@REGULATION", SqlDbType.VarChar) { Value = regulation ?? string.Empty };
            var raw = await _repo.QueryFromStoredProcAsync(sql, p);
            return raw ?? Enumerable.Empty<object>();
        }

        // Replaced previous (incorrect) proc call with the actual inline SELECT used by DAL
        public async Task<IEnumerable<object>> LoadReguAsync(string course, string regulation, string examMy)
        {
            var sql = @"
                SELECT DISTINCT REGU
                FROM tbl_sh
                WHERE course = @COURSE
                  AND regulation = @REGULATION
                  AND exammy = @EXAMMY
                ORDER BY REGU DESC
            ";

            var p1 = new SqlParameter("@COURSE", SqlDbType.VarChar) { Value = course ?? string.Empty };
            var p2 = new SqlParameter("@REGULATION", SqlDbType.VarChar) { Value = regulation ?? string.Empty };
            var p3 = new SqlParameter("@EXAMMY", SqlDbType.VarChar) { Value = examMy ?? string.Empty };

            var raw = await _repo.QueryFromStoredProcAsync(sql, p1, p2, p3);
            return raw ?? Enumerable.Empty<object>();
        }
    }
}
