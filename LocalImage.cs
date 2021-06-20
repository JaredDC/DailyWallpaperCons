using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

public class LocalImage
{
	private List<string> files = null;
	private string path;
	public enum Update : int
	{
		YES,
		NO,
		FORCE
	}
	private Update update = LocalImage.Update.NO;
	public LocalImage(string path)
	{
		this.path = path;
	}
	public void ScanLocalPath(string path="", bool print=true)
    {
		if (path.Length < 1) {
			path = this.path;
		}
		List<string> files = new List<string>();
		foreach (string file in System.IO.Directory.GetFiles(path, "*", SearchOption.AllDirectories))
		{
			long length = new System.IO.FileInfo(file).Length/1024;
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
				// update config.ini modified time.
			}
		} catch (ArgumentNullException e)
        {
			Console.WriteLine("Exception caught: {0}", e);
		}
    }
}
