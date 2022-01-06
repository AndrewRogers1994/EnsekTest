using CsvHelper.Configuration;
using System;
using System.ComponentModel.DataAnnotations;

namespace ENSEK_Web_API.Models
{
    public class Reading
    {
        [Key]
        public string AccountId { get; set; }
        public DateTime MeterReadingDateTime { get; set; }
        public string MeterReadValue { get; set; }

        public override bool Equals(object obj)
        {
            Reading other = obj as Reading;

            return obj != null && AccountId.Equals(other.AccountId) && MeterReadingDateTime.Equals(other.MeterReadingDateTime) && MeterReadValue.Equals(other.MeterReadValue);
        }
    }

    public class ReadingMap: ClassMap<Reading>
    {
        public ReadingMap()
        {
            Map(m => m.AccountId);
            Map(m => m.MeterReadingDateTime).TypeConverterOption.Format("dd/MM/yyyy HH:mm");
            Map(m => m.MeterReadValue);
        }
    }
}