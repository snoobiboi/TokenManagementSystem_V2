using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TokenManagementSystem.Models
{
    public class Customer
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "firstName")]
        [Required, StringLength(25, ErrorMessage = "FirstName should be min 2 character and max of 25 characters", MinimumLength = 2)]
        public string FirstName { get; set; }

        [JsonProperty(PropertyName = "surName")]
        [Required, StringLength(25, ErrorMessage = "Surname should be min 2 character and max of 25 characters", MinimumLength = 2)]
        public string Surname { get; set; }

        [JsonProperty(PropertyName = "age")]
        [Required, RegularExpression("^((100)|(1)|([1-9][0-9]?))$", ErrorMessage = "Age should be between 1 and 100")]
        public int Age { get; set; }

        [JsonProperty(PropertyName = "accountNumber")]
        public int AccountNumber { get; set; }

        [JsonProperty(PropertyName = "sociaNumber")]
        public int SocialNumber { get; set; }


        [JsonProperty(PropertyName = "customerType")]
        [Required, RegularExpression(Constants.CustomerType.AccountHolder + "|" + Constants.CustomerType.Guest, ErrorMessage = "Customer should be either 'Account Holder' or 'Guest'")]
        public string CustomerType { get; set; }

        [JsonProperty(PropertyName = "token")]
        public Token Token { get; set; }
    }
}
