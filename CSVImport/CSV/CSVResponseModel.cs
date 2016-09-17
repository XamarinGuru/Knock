using System;
using Visual1993.Core;
using knock;
using System.Collections.Generic;
namespace knock.CSV
{
	public class CSVResponseModel: WebServiceV2.DefaultResponse
	{
		public List<Attivazione> activations { get; set; }
	}
}

