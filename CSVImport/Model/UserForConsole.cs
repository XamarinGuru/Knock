using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace knock
{
    public class UserForConsole
    {
        public int id { get; set; }
        public string nome { get; set; }
        public string cognome { get; set; }
        public int sesso { get; set; }
        public int relazione { get; set; }
        public string mail { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string mail2 { get; set; }
        public int eta { get; set; }
        public string accessToken { get; set; }
        public DateTime ultimoAccesso { get; set; }
        public string notificationUri { get; set; }
        public string cellulare { get; set; }
        public string provider { get; set; } //SocialProvider
        public string providerID { get; set; }
        public string img { get; set; }
        public int status { get; set; } //vedi UtenteStatus
        public HotelForConsole hotelAppartenenza { get; set; } //rinomina in PHP
        public string frase { get; set; }
        public int doNotDisturb { get; set; }
        public int online { get; set; }
        public string nazionalita { get; set; }
        public DateTime lastUnreadSMSUpdate { get; set; }
        public List<int> idHotelPreferiti { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string data
        {
            get
            {
                return JsonConvert.SerializeObject(this.CampiAggiuntivi);
            }
            set
            {
                CampiAggiuntivi = JsonConvert.DeserializeObject<PersonalizedData>(value);
            }
        }
        //data, non posso metterlo private perchè viene usato dal codice in runtime
        public PersonalizedData CampiAggiuntivi { get; set; } = new PersonalizedData { };

        public class PersonalizedData
        {
            public Nazione nazione { get; set; }
            //public string test { get; set; }
            public List<Valutazione> valutazioni { get; set; }
        }
        public class Valutazione
        {
            public int userID { get; set; }
            public decimal value { get; set; }
        }
    }
}
