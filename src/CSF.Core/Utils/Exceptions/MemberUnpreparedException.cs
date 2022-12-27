using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace CSF
{
    public sealed class MemberUnpreparedException : Exception
    {
        public MemberUnpreparedException()
            : base("Called an unprepared operation in that was not ready to execute.")
        {

        }

        public MemberUnpreparedException(Exception innerException)
            : base("Called an unprepared operation in that was not ready to execute.", innerException)
        {

        }
    }
}
