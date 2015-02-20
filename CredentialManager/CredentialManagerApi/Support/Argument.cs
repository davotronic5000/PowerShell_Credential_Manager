using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace PSCredentialManager.CredentialManagerApi.Support
{
    class Argument
    {
        public static bool IsValid(string argument, string pattern)
        {
            //Initiate variables
            bool isValid = false;

            if (!String.IsNullOrEmpty(argument) && !String.IsNullOrEmpty(pattern))
            {
                if (Regex.IsMatch(argument, pattern))
                {
                    isValid = true;
                }
            }

            return isValid;
        }
    }
}
