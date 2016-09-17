using System;
using System.Threading.Tasks;
namespace knock.CSV
{
	public interface ISaveAndLoad
	{
		Task SaveTextAsync(string filename, string text);
		bool SaveText(string filename, string text);
		Task<string> LoadTextAsync(string filename);
		string LoadText(string filename);
		bool FileExists(string filename);
		Task<int> FileDelete(string filename);
		string GetStoragePath(string fileName = "");
	}
}

