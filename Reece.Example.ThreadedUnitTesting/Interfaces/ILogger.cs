using System;
using Reece.Example.ThreadedUnitTesting.Objects;

namespace Reece.Example.ThreadedUnitTesting.Interfaces
{
    public interface ILogger
    {
        void Log(LogLevel level, string message);
        void Log(LogLevel level, Exception exception);
    }
}