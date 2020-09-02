using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TokenManagementSystem.Models
{
    public class Token
    {
        [JsonProperty(PropertyName = "tokenNumber")]
        public int TokenNumber { get; set; }

        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }

        [JsonProperty(PropertyName = "counter")]
        public int Counter { get; set; }

        [JsonProperty(PropertyName = "serviceType")]
        [Required, RegularExpression(Constants.ServiceType.Service + "|" + Constants.ServiceType.Transaction, ErrorMessage = "ServiceType should be either 'Bank Transaction' or 'Service'")]
        public string ServiceType { get; set; }
    }
}
