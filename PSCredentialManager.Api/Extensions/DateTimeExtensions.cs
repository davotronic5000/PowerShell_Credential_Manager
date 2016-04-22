using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace PSCredentialManager.Api.Extensions
{
    public static class DateTimeExtensions
    {
        public static FILETIME ToComFileTime(this DateTime dateTime)
        {
            return new FILETIME()
            {
                dwLowDateTime = (int)(dateTime.ToFileTime() & 0xFFFFFFFF),
                dwHighDateTime = (int)(dateTime.ToFileTime() >> 32)
            };
        }
    }
}
