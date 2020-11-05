using System.Collections.Generic;
using Newtonsoft.Json;

namespace CreateUser
{

    public class PersonaPull
    {
        [JsonProperty("PersName")]
        public string PersName { get; set; }
       
        [JsonProperty("OrgFolder")]
        public string OrgFolder { get; set; }
        
        [JsonProperty("Group")]
        public List<string> Group { get; set; }
       
        [JsonProperty("Access")]
        public string Access { get; set; }
       
        [JsonProperty("PersonaName")]
        public List<string> PersonaName { get; set; }

    }

    public class PersonaDB
    {
        [JsonProperty("PersonaPull")]
        public List<PersonaPull> PersonaPull { get; set; }

    }
}








