using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Data;

namespace ICampus_Models.Requests
{
    public class FileUploadRequest
    {
        public IFormFile File { get; set; }
        public bool HasHeader { get; set; } = true;
    }

    public class ImportStudentRequest
    {
        public IFormFile File { get; set; }
        public string SheetName { get; set; } = string.Empty;
        public string Regulation { get; set; } = string.Empty;
        public bool HasHeader { get; set; } = true;
        public string UserId { get; set; } = string.Empty;
    }

    public class ImportTvTableRequest
    {
        // Option A: client provides JSON rows as array of objects (preferred for REST)
        public List<Dictionary<string, object>> JsonRows { get; set; } = new();

        // Option B: server-side only (not sent by client) - DataTable produced from uploaded Excel
        public DataTable DataTable { get; set; }

        // Extra metadata fields used by some procs (e.g., REGU, EXAMMY)
        public string ExtraInfo { get; set; } = string.Empty;
    }
}
