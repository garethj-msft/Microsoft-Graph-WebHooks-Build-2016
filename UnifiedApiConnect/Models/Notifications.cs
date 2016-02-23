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
    public class Notifications
    {

        [DataMember(Name = "value")]
        public Notification[] Value { get; set; }
    }
}

