using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Visual1993.Core;
using System.Threading.Tasks;

namespace knock
{
    public class HotelForConsole
    {
        public int id { get; set; }
        //public bool isFavourite { get; set; }

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
        
        public PersonalizedData CampiAggiuntivi { get; set; } = new PersonalizedData { };

        public class PersonalizedData
        {
            public string img { get; set; } //url containing hotel thumbnail
            [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
            public object imgObject { get; set; }
            public string nome { get; set; } //name
            [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
            public object posizione { get; set; } //position, see class below
            [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
            public object pictures { get; set; } //url containing hotel pictures
            [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
            public object servicesPictures { get; set; } //url containing pictures
            [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
            public bool isVisible { get; set; } //address
            public string indirizzo { get; set; } //address
            public string descrizione { get; set; } //description
            public string mail { get; set; } //DO NOT USE
            public string nazione { get; set; } //nation
            public string cap { get; set; } //DO NOT USE
            public string citta { get; set; } //DO NOT USE
            public string provincia { get; set; } //DO NOT USE
            public string quartiere { get; set; } //DO NOT USE
            public string telefono { get; set; } //telephone
            public string sitoWeb { get; set; } //website URL
            public double stelle { get; set; } //quality stars (can be 1, 1.5, 2, 2.5, etc up to 5.5)
            public int numeroCamere { get; set; } //DO NOT USE
            public string topEvent { get; set; }
            public bool isPro { get; set; } //TODO: da usare
            public decimal PriceSingle { get; set; }
            public decimal PriceDouble { get; set; }
            public string Currency { get; set; }
        }
        

        public class SingleHotelWeb : WebServiceV2.DefaultResponse
        {
            public HotelForConsole hotel { get; set; }
        }
        public enum HotelIDs
        {
            Generico = 1,
            OM2Rome=4,
        }
    }
}
