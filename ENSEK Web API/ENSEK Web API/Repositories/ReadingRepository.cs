using ENSEK_Web_API.Models;
using ENSEK_Web_API.Models.Database;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ENSEK_Web_API.Repositories
{
    public class ReadingRepository : IReadingRepository
    {
        private readonly DatabaseContext _DatabaseContext;

        public ReadingRepository(DatabaseContext context)
        {
            _DatabaseContext = context;
        }

        public async Task<(bool result, string message)> CreateReading(Reading reading)
        {
            string message;

            //Check that the Account ID is not empty
            if (!string.IsNullOrWhiteSpace(reading.AccountId))
            {
                //Check that there is an account id in the database that matches this record.
                Account linkedAccount = await _DatabaseContext.Accounts.FindAsync(reading.AccountId);
                if(linkedAccount != null)
                {
                    //Check that the meter reading is in the correct format
                    MatchCollection matches = Regex.Matches(reading.MeterReadValue, "^[0-9]*$");

                    //Check that there is exactly 1 match
                    if(matches.Count == 1)
                    {
                        //Check to see if there is already an entry in the database
                        Reading currentDatabaseReading = await _DatabaseContext.Readings.FindAsync(reading.AccountId);

                        //if there is already an existing reading then lets check that the new reading can replace it
                        if (currentDatabaseReading != null)
                        {
                            //Check that it's not an exact match
                            if(!reading.Equals(currentDatabaseReading))
                            {
                                //Check if the submitted entry is newer than the current entry
                                if(reading.MeterReadingDateTime > currentDatabaseReading.MeterReadingDateTime)
                                {
                                    //The update has passed all checks so lets update the stored values
                                    currentDatabaseReading.MeterReadingDateTime = reading.MeterReadingDateTime;
                                    currentDatabaseReading.MeterReadValue = reading.MeterReadValue;
                                    await _DatabaseContext.SaveChangesAsync();
                                    return (true,null);
                                }
                                else
                                {
                                    message = "This reading is older than the current reading in the system.";
                                }
                            }
                            else
                            {
                                message = "This reading has already been entered into the system.";
                            }
                        }
                        else
                        {
                            //The reading has passed all checks so lets create it in the database
                            _DatabaseContext.Readings.Add(reading);
                            await _DatabaseContext.SaveChangesAsync();
                            return (true,null);
                        }
                    }
                    else
                    {
                        message = "Invalid Meter Reading Provided.";
                    }
                }
                else
                {
                    message = "Invalid AccountId Provided";
                }
            }
            else
            {
                message = "AccountId Not Provided.";
            }
            return (false,message);
        }
    }
}