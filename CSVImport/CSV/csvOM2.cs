using System;
using System.Threading.Tasks;
using System.Collections.Generic;
namespace knock.CSV
{
	public class OM2
	{
		public CSVResponseModel getList(string sourceFilename, ISaveAndLoad fileService, int hotelID, string rawInput="")
		{
			CSVResponseModel output = new CSVResponseModel();
			output.activations = new List<Attivazione>();

			string fileText = "";
			if (string.IsNullOrWhiteSpace(rawInput)){
				fileText = fileService.LoadText(sourceFilename);
			}
			else { //use raw input data
				fileText = rawInput;
			}
			var newText = fileText.Replace("\"", ""); //remove quotes " "
			if (fileService.FileExists("normalizedInput.csv"))
			{
				fileService.FileDelete("normalizedInput.csv");
			}
			fileService.SaveText("normalizedInput.csv", newText);
			var path = fileService.GetStoragePath("normalizedInput.csv");

			var csvConfig = new CsvHelper.Configuration.CsvConfiguration
			{
				IgnoreQuotes = false,
				IgnoreHeaderWhiteSpace = true,
				CultureInfo = new System.Globalization.CultureInfo("it-IT"),
				Delimiter = ";",

			};
            try
            {
                using (var csv = new CsvHelper.CsvReader(System.IO.File.OpenText(path), csvConfig))
                {
                    while (csv.Read())
                    {
                        try
                        {
                            Attivazione attivazione = new Attivazione();
                            attivazione.hotelID = hotelID; //may BE SET BY CODE, not runtime
                            attivazione.nome = csv.GetField<string>("Riferimento");
                            attivazione.checkin = csv.GetField<DateTime>("Dal");
                            attivazione.checkout = csv.GetField<DateTime>("Al");
                            attivazione.mail = csv.GetField<string>("Emaildicontatto");
                            if (string.IsNullOrWhiteSpace(attivazione.mail)) { continue; } //no mail? -> skip it
                            attivazione.stanza = csv.GetField<string>("Camere");
                            var adulti = csv.GetField<int>("Adulti");
                            var bambini = csv.GetField<int>("Bambini");
                            var infanti = csv.GetField<int>("Infanti");
                            attivazione.occupanti = adulti + bambini + infanti;
                            //var dal = DateTime.ParseExact(csv.GetField<string>("Dal"), "dd/mm/yyyy",System.Globalization.CultureInfo.InvariantCulture);
                            output.activations.Add(attivazione);
                        }
                        catch (Exception ex)
                        {
                            output.errorMessage += Environment.NewLine + ex.Message;
                            continue; //skip row
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                output.state = Visual1993.Core.WebServiceV2.WebRequestState.GenericError;
                output.errorMessage = ex.Message;
                return output;
            }
            return output;
        }
	}
}

