using System;
using Xamarin.Forms;
using knock.Droid;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using knock;

[assembly: Dependency (typeof (SaveAndLoad_Android))]

namespace knock.Droid
{
    public class SaveAndLoad_Android : ISaveAndLoad
    {
        #region ISaveAndLoad implementation
		private string stringBeingUsed { get; set; }

		public string GetStoragePath(string filename = "")
        {            
			return CreatePathToFile(filename);
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

        public async Task SaveTextAsync(string filename, string text)
        {
            var path = CreatePathToFile(filename);
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
			catch(Exception ex) {
				Xamarin.Insights.Report (ex,new Dictionary<string, string>()
				{
					{"context", "user"},
					{"detail", "exception writing text on file."},
					{"exception_message",ex.Message}
				});
				Console.WriteLine ("ERROR in writing text file.");

				stringBeingUsed = "";
			}
			stringBeingUsed = "";
        }

        public async Task<string> LoadTextAsync(string filename)
        {
            var path = CreatePathToFile(filename);
			if (stringBeingUsed == path)
			{ await Task.Delay(TimeSpan.FromMilliseconds(100)); }

			try{
			using (StreamReader sr = File.OpenText (path)) {
					stringBeingUsed = path;
				var output =  await sr.ReadToEndAsync ();
				sr.Close ();
					stringBeingUsed = "";
				return output;
			}
			}
			catch (Exception ex) {
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

        string CreatePathToFile(string filename)
        {
            var docsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            return Path.Combine(docsPath, filename);
        }
    }
}