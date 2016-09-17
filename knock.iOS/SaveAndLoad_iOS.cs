using System;
using Xamarin.Forms;
using knock.iOS;
using System.IO;
using System.Threading.Tasks;
using knock;
using Foundation;
using System.Linq;

[assembly: Dependency (typeof(SaveAndLoad_iOS))]

namespace knock.iOS
{
	public class SaveAndLoad_iOS : ISaveAndLoad
	{
		private string stringBeingUsed{get;set;}
        public static string DocumentsPath
        {
            get
            {
                var documentsDirUrl = NSFileManager.DefaultManager.GetUrls(NSSearchPathDirectory.DocumentDirectory, NSSearchPathDomain.User).Last();
                return documentsDirUrl.Path;
            }
        }

        #region ISaveAndLoad implementation

		public string GetStoragePath(string fileName = "")
        {
			return CreatePathToFile(fileName);
        }

        public async Task SaveTextAsync(string filename, string text)
        {
            string path = CreatePathToFile(filename);
			if (stringBeingUsed == path)
			{ await Task.Delay(TimeSpan.FromMilliseconds(100)); }
			try{
			using (StreamWriter sw = File.CreateText (path)) {
					stringBeingUsed = path;
				await sw.WriteAsync (text);
				sw.Close ();
					stringBeingUsed = "";
			}
			}
			catch(Exception ex)
			{
				Xamarin.Insights.Report (ex);
				stringBeingUsed = "";
			}
			stringBeingUsed = "";
        }

		public bool SaveText(string filename, string text)
		{
			string path = CreatePathToFile(filename);
			if (stringBeingUsed == path)
			{ System.Threading.Thread.Sleep(100); }
			try
			{
				using (StreamWriter sw = File.CreateText(path))
				{
					stringBeingUsed = path;
					sw.Write(text);
					sw.Close();
					stringBeingUsed = "";
				}
				return true;
			}
			catch (Exception ex)
			{
				Xamarin.Insights.Report(ex);
				stringBeingUsed = "";
				return false;
			}
			stringBeingUsed = "";
		}

        public async Task<string> LoadTextAsync(string filename)
        {
            string path = CreatePathToFile(filename);
			if (stringBeingUsed == path)
			{ await Task.Delay(TimeSpan.FromMilliseconds(100)); }
			try{
			using (StreamReader sr = File.OpenText (path)) {
					stringBeingUsed = path;
				var output = await sr.ReadToEndAsync ();
				sr.Close ();
					stringBeingUsed = "";
				return output;
			}
			}
			catch(Exception ex) {
				Xamarin.Insights.Report (ex);
				stringBeingUsed = "";
				return "";
			}
			stringBeingUsed = "";
        }

		public string LoadText(string filename)
		{
			var path = CreatePathToFile(filename);
			if (stringBeingUsed == path)
			{ System.Threading.Thread.Sleep(100); }
			try{
				using (StreamReader sr = File.OpenText (path)) {
					stringBeingUsed = path;
					var output =  sr.ReadToEnd();
					sr.Close ();
					stringBeingUsed = "";
					return output;
				}
			}
			catch (Exception ex) {
				Xamarin.Insights.Report (ex);
				stringBeingUsed = "";
				return "";
			}
			stringBeingUsed = "";
		}

        public bool FileExists(string filename)
        {
            return File.Exists(CreatePathToFile(filename));
        }

        public async Task<int> FileDelete(string filename)
        {
            if (FileExists(filename))
            {
                File.Delete(CreatePathToFile(filename));
                return 1;
            }
            else { return -1; }
        }

        #endregion

        static string CreatePathToFile(string fileName)
        {
            return Path.Combine(DocumentsPath, fileName);
        }
    }
}