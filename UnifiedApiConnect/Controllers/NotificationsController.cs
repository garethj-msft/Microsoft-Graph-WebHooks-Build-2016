using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using UnifiedApiConnect.Models;

namespace UnifiedApiConnect.Controllers
{
    public class NotificationsController : ApiController
    {
        // GET: api/Notification
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        /// <summary>
        /// POST: api/Notification
        /// Handle validation
        /// </summary>
        /// <param name="validationToken"></param>
        public HttpResponseMessage Post([FromUri]string validationToken)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
            // Important to set Content as a property initialization to trigger content negotiation.
            response.Content = new StringContent(validationToken, Encoding.UTF8, "text/plain");
            return response;
        }

        /// <summary>
        /// POST: api/Notification
        /// Handle notification messages
        /// </summary>
        /// <param name="value"></param>
        public void Post([FromBody]Notifications value)
        {
            // Get the title of the first event, translate it and write it back.
            // In a real app, this would be offloaded onto a more synchronous process in order to handle load smoothly.
            Notification notification = value.Value.FirstOrDefault();
            if (notification != null)
            {
                notification.Resource=
            }

        }
    }
}
