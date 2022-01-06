using CsvHelper;
using ENSEK_Web_API.Models.Database;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace ENSEK_Web_API.Utils
{
    public class CsvUtils
    {
        public static (List<int> FailedRows, List<(T, int)> ParsedRows) ParseCSVFile<T>(IFormFile file)
        {
            List<(T rowData, int rowID)> parsedRows = new List<(T rowData, int rowID)>();
            List<int> failedRows = new List<int>();

            //Open the read stream so that we can read the CSV
            using (Stream s = file.OpenReadStream())
            {
                using (TextReader tr = new StreamReader(s))
                {
                    //Create the CSV Reader using the stream of the file
                    CsvReader csv = new CsvReader(tr, CultureInfo.InvariantCulture);

                    //Register the custom mapping that we created so that it can pass the date of the Reading csv
                    csv.Context.RegisterClassMap<ReadingMap>();

                    //Loop over the CSV rows and add the valid records to the list so that we can return
                    while (csv.Read())
                    {

                        try
                        {
                            parsedRows.Add((csv.GetRecord<T>(), csv.Parser.Row));
                        }
                        catch (Exception)
                        {
                            failedRows.Add(csv.Parser.Row);
                        }
                    }
                }
            }

            return (failedRows, parsedRows);
        }

        public static List<Account> GetTestAccounts()
        {
            try
            {
                using (TextReader tr = new StreamReader("Test_Accounts.csv"))
                {
                    using (var csv = new CsvReader(tr, CultureInfo.InvariantCulture))
                    {
                        return csv.GetRecords<Account>().ToList();
                    }
                }
            }
            catch
            {
                //Something went wrong while trying to load the test accounts so lets just return an empty list.
                return new List<Account>();
            }
        }
    }
}