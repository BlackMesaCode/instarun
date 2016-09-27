using System;

namespace InstaRun.GlobalExceptionHandling
{
    internal interface IExceptionLogger
    {
        void Log(Exception ex);
    }
}