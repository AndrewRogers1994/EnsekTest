using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.TypeConversion;
using ENSEK_Web_API.Models;
using ENSEK_Web_API.Models.Response;
using ENSEK_Web_API.Repositories;
using ENSEK_Web_API.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ENSEK_Web_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReadingController : ControllerBase
    {
        private readonly IReadingRepository _ReadingRepository;

        public ReadingController(IReadingRepository readingRepository)
        {
            _ReadingRepository = readingRepository;
        }

        [Route("meter-reading-upload")]
        [HttpPost]
        public async Task<ActionResult<MeterUploadResponse>> MeterReadingUpload(IFormFile file)
        {
            //Check if a CSV file was sent as part of the request
            if(file != null && file.Length > 0 && file.ContentType.Equals("text/csv"))
            {
                MeterUploadResponse uploadResponse = new MeterUploadResponse();

                //Extract the CSV Data from the 
                var (failedRows, successRows) = CsvUtils.ParseCSVFile<Reading>(file);

                //Update the failed count of the response with the number of rows that failed to read from the CSV file
                foreach(int row in failedRows)
                {
                    uploadResponse.ErrorCount++;
                    uploadResponse.ErrorMessages.Add("Row[" + row + "] Contains Invalid Information.");
                }

                //Attempt to add the rows that succesfully parsed to the database
                foreach((Reading reading, int row) in successRows)
                {
                    var (outcome, message) = await _ReadingRepository.CreateReading(reading);

                    //Check the outcome and increment the count based on result
                    if(outcome)
                    {
                        uploadResponse.SuccessCount++;
                    }
                    else
                    {
                        //if the insert failed lets also return the reason why
                        uploadResponse.ErrorCount++;
                        uploadResponse.ErrorMessages.Add("Row[" + row + "] " +  message);
                    }
                }

                //Return the response to the client
                return Ok(uploadResponse);
            }
            else
            {
                return BadRequest("Please upload a valid CSV File.");
            }
        }
    }
}