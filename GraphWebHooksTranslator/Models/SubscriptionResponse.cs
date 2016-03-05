using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace UnifiedApiConnect.Models
{
    /// <summary>
    /// Union to hold either a successful subscription or an error response.
    /// </summary>
    public class SubscriptionResponse
    {
        public Subscription Subscription { get; set; }

        public string Error { get; set; }
    }
}

