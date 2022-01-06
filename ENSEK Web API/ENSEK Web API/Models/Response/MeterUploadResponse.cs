using System.Collections.Generic;

namespace ENSEK_Web_API.Models.Response
{
    public class MeterUploadResponse
    {
        public string Message { get; set; }
        public int SuccessCount { get; set; }
        public int ErrorCount { get; set; }
        public List<MeterReadingError> ErrorMessages { get; set; } = new List<MeterReadingError>();
    }
}