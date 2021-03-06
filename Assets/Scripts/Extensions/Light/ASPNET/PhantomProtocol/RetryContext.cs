using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Light.Unity.AspNetCore.Phantom
{
    public sealed class RetryContext
    {
        public long PreviousRetryCount
        {
            get;
            set;
        }

        public TimeSpan ElapsedTime
        {
            get;
            set;
        }

        public Exception RetryReason
        {
            get;
            set;
        }
    }
}
