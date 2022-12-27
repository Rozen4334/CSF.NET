using System;
using System.Collections.Generic;
using System.Text;

namespace CSF
{
    public sealed class MissingValueException : ArgumentException
    {
        public MissingValueException(string message, string paramName, Exception exception)
            : base(message, paramName, exception)
        {

        }

        public MissingValueException(string message, string paramName) 
            : base(paramName, message)
        {

        }
    }
}
