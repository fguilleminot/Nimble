using UnityEngine;
using System.Collections;
using System.IO;

public class LogFile {

	string nameFile = "Assets/Log/";
	StreamWriter writer;

	public void SetName(string sName)
	{
		if (sName.Length > 0)
			nameFile += sName + ".txt";
	}

	public string GetName()
	{
		return nameFile;
	}
	
	public long GetLength()
	{
        if (!File.Exists(nameFile))
            return -1;
        return new FileInfo(nameFile).Length;
	}

	public void WriteLine(string line)
	{
		try 
		{
			if (line.Length == 0)
				return;

            if (!File.Exists(nameFile))
            {
                writer = File.CreateText(nameFile);
            }
            else
            {
                writer = File.AppendText(nameFile);
            }

            if (writer != null)
            {
                writer.WriteLine(line);
                writer.Close();
                writer = null;
            }

		}
		catch(System.Exception e) 
		{
			Debug.LogWarning(e.Message); // Ex : Directory not found ;)
		}
	}
	
	public void ClearFile()
	{
		if (!File.Exists(nameFile))
			return;
		
		File.WriteAllText(nameFile, "");
	}
}
