using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GraphWebhooksTranslator.Models
{
    [Flags]
    public enum ChangeTypes
    {
        Created = 1,
        Updated=2,
        Deleted=4,
        Acknowledgement=8,
        Missed=16
    }
    [DataContract]
    public class Subscription
    {

        [DataMember(Name = "changeType")]
        [JsonConverter(typeof(StringEnumConverter))]
        public ChangeTypes ChangeType { get; set; }

        [DataMember(Name = "subscriptionId")]
        public string Id { get; set; }

        [DataMember(Name = "clientState")]
        public string ClientState { get; set; }

        [DataMember(Name = "notificationUrl")]
        public string NotificationUrl { get; set; }

        [DataMember(Name = "resource")]
        public string Resource { get; set; }

        [DataMember(Name = "subscriptionExpirationDateTime")]
        public DateTimeOffset? ExpirationDateTime { get; set; }

    }
}

