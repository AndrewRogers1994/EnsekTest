﻿using ENSEK_Web_API.Models;
using System.Threading.Tasks;

namespace ENSEK_Web_API.Repositories
{
    public interface IReadingRepository
    {
        Task<(bool result, string message)> CreateReading(Reading reading);
    }
}