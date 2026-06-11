using ICampus_Models.Requests;
using Microsoft.AspNetCore.Http;

namespace ICampus_Api.Requests
{
    public class UploadFileFormRequest : UploadFileRequest
    {
        public IFormFile File { get; set; }
    }
}
