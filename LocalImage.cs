using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

public class LocalImage
{
	private List<string> files = null;
	private string path;
	private int invalidCnt = 0;
	public enum Update : int
	{
		YES,
		NO,
		FORCE,
		CleanInvalid
	}
	private Update update = LocalImage.Update.YES;
	public LocalImage(string path)
	{
		this.path = path;
	}
	private bool ShoulditUpdate()
    {
		if (this.update == Update.FORCE)
        {
			return true;
        } else if (this.update == Update.CleanInvalid)
		{
			return true;
		} else if (this.update == Update.NO)
		{
			return false;
		} else
        {
			Console.WriteLine(new FileInfo(this.path).LastWriteTime);
			return true;
		}
    }

	public void ScanLocalPath(string path="", bool print=true)
    {
		if (!ShoulditUpdate()) {
			return;
		}
		if (this.update == Update.CleanInvalid) {
			List2Txt();
			return;
		}

		if (path.Length < 1) {
			path = this.path;
		}
		List<string> files = new List<string>();
		foreach (string file in Directory.GetFiles(path, "*", SearchOption.AllDirectories))
		{
			long length = new FileInfo(file).Length/1024;
			string file_low = file.ToLower();
			if (file_low.EndsWith(".jpg") || file_low.EndsWith(".jpeg") || file_low.EndsWith(".png")) { 
				if (length > 100) {
					Bitmap img = new Bitmap(file);
					if (img.Width > 1900 && (img.Width+0.0/img.Height>1.4)) { 
						files.Add(file);
						if (print) { Console.WriteLine(file + ": " + length + "KB"); }
					}
					img.Dispose();
				}
			}
		}
		if (files.Count < 1)
        {
			if (print) { Console.WriteLine("No suitable image files."); }
		}
		this.files = files;
		List2Txt();
	}
	public void List2Txt(string txtFile="_img_list.txt")
    {
		txtFile = Path.Combine(this.path, txtFile);
		if (this.files == null) {
			this.ScanLocalPath(print:false);
		}
		if (!new FileInfo(txtFile).Exists) {
			this.update = Update.FORCE;
			// update config.ini modified time.
		}
		try {
			if (this.update != Update.NO) {
				File.WriteAllLines(txtFile, this.files);
				if (this.update == Update.FORCE)
                {
					Console.WriteLine("Created: {0}", txtFile);
				} else if (this.update == Update.CleanInvalid)
                {
					Console.WriteLine("Clean Invalid files[{1:D}]: {0}", txtFile, this.invalidCnt);
				} else { 
					Console.WriteLine("Updated: {0}", txtFile);
				}
				// update config.ini modified time.
			}
		} catch (ArgumentNullException e)
        {
			Console.WriteLine("Exception caught: {0}", e);
		}
    }
	public void Txt2List(string txtFile = "_img_list.txt")
	{
		txtFile = Path.Combine(this.path, txtFile);
		if (this.files == null)
		{
			this.ScanLocalPath(print: false);
		}
		if (!new FileInfo(txtFile).Exists)
		{
			ScanLocalPath();
		}
		
		try
		{
			List<string> readfiles = File.ReadLines(txtFile).ToList();
			List<string> effectivefiles = new List<string>();
			foreach (string file in readfiles)
			{
				if (File.Exists(file))
                {
					effectivefiles.Add(file);
				} else
                {
					Console.WriteLine("File does not exist: " + file);
				}
				
			}
			this.invalidCnt = readfiles.Count - effectivefiles.Count;
			if (this.invalidCnt != 0)
            {
				this.update = Update.CleanInvalid;
				ScanLocalPath();
			}
			this.files = effectivefiles;
		}
		catch (Exception e)
		{
			Console.WriteLine("Exception caught: {0}", e);
		} finally
        {
			/*foreach (string s in this.files) {
				Console.WriteLine(s);
			}*/
        }
	}
	private void PrintList()
    {
		foreach (string file in this.files)
        {
			Console.WriteLine(file);
        }
    }
	private string RandomChoiceFromList()
    {
		ScanLocalPath();
		var random = new Random();
		// PrintList();
		int index = random.Next(this.files.Count);
		string file = this.files[index];
		Console.WriteLine(file);
		return file;
	}
	public void RandomSelectOneImgToWallpaper() {
		Wallpaper.SetWallPaper(RandomChoiceFromList());
	}
}
