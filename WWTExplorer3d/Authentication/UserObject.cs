using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraViewer
{
    internal class UserObject
    {
        [JsonProperty("id")]
        public string Id { get; set; }


        [JsonProperty("name")]
        public string Name { get; set; }


        [JsonProperty("emails")]
        public EMails Emails { get; set; }

    }
    internal class EMails
    {
        [JsonProperty("preferred")]
        public string Preferred { get; set; }

        [JsonProperty("account")]
        public string Account { get; set; }

        [JsonProperty("personal")]
        public string Personal { get; set; }

        [JsonProperty("business")]
        public string Business { get; set; }
    }
}
