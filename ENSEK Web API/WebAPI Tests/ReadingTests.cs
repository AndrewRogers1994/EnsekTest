using ENSEK_Web_API.Models;
using ENSEK_Web_API.Models.Database;
using ENSEK_Web_API.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Xunit;

namespace WebAPI_Tests
{
    public class ReadingTests : IDisposable
    {
        public readonly ReadingRepository _ReadingRepository;
        public readonly DatabaseContext _DatabaseContext;

        public ReadingTests()
        {
            //Setup the tests to run on a seperate SQLite database
            DbContextOptionsBuilder<DatabaseContext> builder = new DbContextOptionsBuilder<DatabaseContext>().UseSqlite("Data source=unittest.db");
            _DatabaseContext = new DatabaseContext(null, builder.Options);

            //Create the reading repository
            _ReadingRepository = new ReadingRepository(_DatabaseContext);
        }

        public void Dispose()
        {
            //Clean up the database between tests
            _DatabaseContext.Dispose();
            File.Delete("unittest.db");
        }

        [Theory]
        [MemberData(nameof(ValidReadings))]
        public async void InsertValidReading(Reading readingToinsert)
        {
            (bool result, string message) = await _ReadingRepository.CreateReading(readingToinsert);
            Assert.True(result, message);
        }

        [Theory]
        [MemberData(nameof(ValidReadings))]
        public async void InsertNewerReading(Reading readingToinsert)
        {
            //Insert the inital meter reading
            (bool result, string message) = await _ReadingRepository.CreateReading(readingToinsert);
            Assert.True(result, message);

            //Create a new reading that is identical but with a newer DateTime
            Reading newerReading = new Reading()
            {
                AccountId = readingToinsert.AccountId,
                MeterReadingDateTime = DateTime.Now,
                MeterReadValue = readingToinsert.MeterReadValue
            };

            (bool resultSecond, string messageSecond) = await _ReadingRepository.CreateReading(newerReading);
            Assert.True(resultSecond, messageSecond);
        }

        [Theory]
        [MemberData(nameof(InvalidReadings_InvalidAccountId))]
        public async void InsertInvalidReading_InvalidAccountID(Reading readingToinsert)
        {
            (bool result, string message) = await _ReadingRepository.CreateReading(readingToinsert);
            Assert.False(result, message);
        }

        [Theory]
        [MemberData(nameof(InvalidReadings_DuplicateEntry))]
        public async void InsertInvalidReading_DuplicateReading(Reading readingToinsert)
        {
            //Insert the meter reading
            (bool result, string message) = await _ReadingRepository.CreateReading(readingToinsert);
            Assert.True(result, message);

            //Try to add the same record again
            (bool resultSecond, string messageSecond) = await _ReadingRepository.CreateReading(readingToinsert);
            Assert.False(resultSecond, messageSecond);
        }

        [Theory]
        [MemberData(nameof(ValidReadings))]
        public async void InsertInvalidReading_OlderReading(Reading readingToinsert)
        {
            //Insert the meter reading
            (bool result, string message) = await _ReadingRepository.CreateReading(readingToinsert);
            Assert.True(result, message);

            //Create a new reading that is identical but with an older DateTime
            Reading olderReading = new Reading()
            {
                AccountId = readingToinsert.AccountId,
                MeterReadingDateTime = readingToinsert.MeterReadingDateTime.AddDays(-1),
                MeterReadValue = readingToinsert.MeterReadValue
            };

            (bool resultSecond, string messageSecond) = await _ReadingRepository.CreateReading(olderReading);
            Assert.False(resultSecond, messageSecond);
        }


        public static IEnumerable<object[]> ValidReadings()
        {
            yield return new object[]
            {
                new Reading()
                {
                    AccountId = "2345",
                    MeterReadingDateTime = DateTime.ParseExact("22/04/2019 09:24", "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture),
                    MeterReadValue = "45522"
                }
            };

            yield return new object[]
            {
                new Reading()
                {
                    AccountId = "2345",
                    MeterReadingDateTime = DateTime.ParseExact("22/04/2019 09:25", "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture),
                    MeterReadValue = "45522"
                }
            };
        }

        public static IEnumerable<object[]> InvalidReadings_InvalidAccountId()
        {
            yield return new object[]
            {
                new Reading()
                {
                    AccountId = "1",
                    MeterReadingDateTime = DateTime.ParseExact("22/04/2019 09:24", "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture),
                    MeterReadValue = "45522"
                }
            };

            yield return new object[]
            {
                new Reading()
                {
                    AccountId = "a",
                    MeterReadingDateTime = DateTime.ParseExact("22/04/2019 09:25", "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture),
                    MeterReadValue = "45522"
                }
            };
        }

        public static IEnumerable<object[]> InvalidReadings_DuplicateEntry()
        {
            yield return new object[]
            {
                new Reading()
                {
                    AccountId = "2344",
                    MeterReadingDateTime = DateTime.ParseExact("22/04/2019 09:24", "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture),
                    MeterReadValue = "45522"
                }
            };

            yield return new object[]
            {
                new Reading()
                {
                    AccountId = "2345",
                    MeterReadingDateTime = DateTime.ParseExact("22/04/2019 09:25", "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture),
                    MeterReadValue = "45522"
                }
            };
        }
    }
}