using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Light.Unity.AspNetCore.Phantom
{
    public interface IRetryPolicy
    {
        TimeSpan? NextRetryDelay(RetryContext retryContext);
    }
}
