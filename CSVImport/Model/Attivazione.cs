using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Visual1993.Core;
using Newtonsoft.Json;

namespace knock
{
	public class Attivazione
	{
		public int id { get; set; }
		public string nome { get; set; }
		public string telefono { get; set; }
		public string mail { get; set; }
		public string stanza { get; set; }
		public int occupanti { get; set; }
		public string codice { get; set; }
		public int soggiornoID { get; set; }
		public int hotelID { get; set; }
		public DateTime checkin { get; set; }
		public DateTime checkout { get; set; }
		public string data
		{
			get
			{
				return JsonConvert.SerializeObject(this.Extra);
			}
			set
			{
				try
				{
					Extra = JsonConvert.DeserializeObject<PersonalizedData>(value);
				}
				catch (Exception ex)
				{
					Extra = new PersonalizedData();
				}
			}
		}
		//data, non posso metterlo private perchè viene usato dal codice in runtime
		public PersonalizedData Extra { get; set; } = new PersonalizedData { };

		public class PersonalizedData
		{
			public DateTime bookingDate { get; set; }
			public DateTime insertDate { get; set; }
			public DateTime lastChange { get; set; }
		}

		public static async Task<Attivazione.WebReq> getActivations(int hotelID, string accessToken) //to retrieve all the guest management items for the specified hotel
		{
			WebServiceV2 webService = new WebServiceV2();
			var result = await webService.UrlToString(Constants.sitoAPI + "getAttivazioni.php?accessToken="+accessToken+"&hotelID="+hotelID.ToString(), null);
			Attivazione.WebReq obj;
			if (result != null) {
				obj = JsonConvert.DeserializeObject<Attivazione.WebReq> (result);
			} else {obj = null;
			}
			return obj;
		}

		public async Task<DeleteArgs> delete(string accessToken)
		{
			WebServiceV2 webService = new WebServiceV2();
			List<KeyValuePair<string, string>> pairs = new List<KeyValuePair<string, string>> {
				new KeyValuePair<string, string> ("soggiornoID", this.soggiornoID.ToString ()),
			};
			var result = await webService.UrlToString(Constants.sitoAPI + "postAttivazione.php?azione=rimuovi&accessToken="+accessToken+"&attivazioneID="+this.id, pairs);
			Attivazione.DeleteArgs obj = JsonConvert.DeserializeObject<Attivazione.DeleteArgs>(result);
			return obj;
		}

		public class DeleteArgs
		{
			public int status { get; set; }
			public string erroreMessaggio { get; set; }
		}
		public async Task<InsertArgs> insert(string accessToken)
		{
			var url = Constants.sitoAPI + "postAttivazione.php?azione=inserisci";
			if (this.Extra == null) { this.Extra = new PersonalizedData();}
			this.Extra.insertDate = DateTime.UtcNow; this.Extra.lastChange = DateTime.UtcNow;
		List<KeyValuePair<string, string>> pairs = new List<KeyValuePair<string, string>>
		{
				new KeyValuePair<string, string>("nome", this.nome),
				new KeyValuePair<string, string>("telefono", this.telefono),
				new KeyValuePair<string, string>("mail", this.mail),
				new KeyValuePair<string, string>("stanza", this.stanza),
				new KeyValuePair<string, string>("occupanti", this.occupanti.ToString()),
				new KeyValuePair<string, string>("checkin", this.checkin.Date.ToString(Constants.formatoData)),
				new KeyValuePair<string, string>("checkout", this.checkout.Date.ToString(Constants.formatoData)),
				new KeyValuePair<string, string>("soggiornoID", this.soggiornoID.ToString()),
				new KeyValuePair<string, string>("hotelID", this.hotelID.ToString()),
				new KeyValuePair<string, string>("data", this.data),
				new KeyValuePair<string, string>("accessToken", accessToken),
		};

		WebServiceV2 webRequest = new WebServiceV2();
		var result = await webRequest.UrlToString(url,pairs);
            if (string.IsNullOrWhiteSpace(result)) { return null; }
		Attivazione.InsertArgs obj = JsonConvert.DeserializeObject<Attivazione.InsertArgs>(result);
			return obj;
		}


		public async Task<InsertArgs> update(string accessToken)
		{
			var url = Constants.sitoAPI + "postAttivazione.php?azione=aggiorna&attivazioneID="+this.id.ToString();
			if (this.Extra == null) { this.Extra = new PersonalizedData(); }
			this.Extra.lastChange = DateTime.UtcNow;

			List<KeyValuePair<string, string>> pairs = new List<KeyValuePair<string, string>>
			{
				new KeyValuePair<string, string>("nome", this.nome),
				new KeyValuePair<string, string>("telefono", this.telefono),
				new KeyValuePair<string, string>("mail", this.mail),
				new KeyValuePair<string, string>("stanza", this.stanza),
				new KeyValuePair<string, string>("occupanti", this.occupanti.ToString()),
				new KeyValuePair<string, string>("checkin", this.checkin.Date.ToString(Constants.formatoData)),
				new KeyValuePair<string, string>("checkout", this.checkout.Date.ToString(Constants.formatoData)),
				new KeyValuePair<string, string>("soggiornoID", this.soggiornoID.ToString()),
				new KeyValuePair<string, string>("hotelID", this.hotelID.ToString()),
				new KeyValuePair<string, string>("data", this.data),
				new KeyValuePair<string, string>("accessToken", accessToken),
			};

			WebServiceV2 webRequest = new WebServiceV2();
			var result = await webRequest.UrlToString(url,pairs);
			Attivazione.InsertArgs obj = JsonConvert.DeserializeObject<Attivazione.InsertArgs>(result);
			return obj;
		}

		public class InsertArgs
		{
			public Attivazione resultObj { get; set; }
			public int status { get; set; }
			public string erroreMessaggio { get; set; }
		}

		public class AssociazioneRisposta
		{
			public int status { get; set; } //1->ok aggiorna utente
			public string erroreMessaggio { get; set; }
		}

		public class WebReq
		{
			public int status { get; set; }
			public string erroreMessaggio { get; set; }
			public List<Attivazione> attivazioni { get; set; }
		}
	}

}
