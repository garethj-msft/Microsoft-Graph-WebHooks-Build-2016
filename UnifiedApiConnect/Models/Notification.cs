using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace UnifiedApiConnect.Models
{
   
    [DataContract]
    public class Notification
    {

        [DataMember(Name = "subscriptionId")]
        public string SubscriptionId { get; set; }

        [DataMember(Name = "clientState")]
        public string ClientState { get; set; }

         [DataMember(Name = "changeType")]
        [JsonConverter(typeof(StringEnumConverter))]
        public ChangeTypes ChangeType { get; set; }

        [DataMember(Name = "resource")]
        public string Resource { get; set; }

        [DataMember(Name = "resourceData")]
        public JObject ResourceData { get; set; }

    }
}

