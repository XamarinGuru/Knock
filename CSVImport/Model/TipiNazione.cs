using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace knock
{
	public class TipiNazione
	{
		private IList<Nazione> _nations;
		public IList<Nazione> Nations
		{
			get
			{
				return this._nations;
			}
			set
			{
				if (this._nations == value)
					return;
				this._nations = value;
				//this.RaisePropertyChanged();
			}
		}

		private Nazione _nation;
		public Nazione Nation
		{
			get
			{
				return this._nation;
			}
			set
			{
				if (this._nation == value)
					return;
				this._nation = value;
				//this.RaisePropertyChanged();
			}
		}

	}
	public class Nazione
	{
		public string label {get;set;}
		public string ISOinfo {get;set;}
		public override string ToString ()
		{
			return string.Format ("{0}", label);
		}
	}
}

