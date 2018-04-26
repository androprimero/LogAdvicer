using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
namespace LogAdvicer
{
    public class FileReader
    {
        StreamReader reader;
        FileStream fileStream;
        public FileReader(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    fileStream = new FileStream(path, FileMode.Open);
                    reader = new StreamReader(fileStream);
                }
            }
            catch(Exception e)
            {
                Console.WriteLine("Cannot open File " + path + ": " + e.ToString());
            }
            
        }
        public string readLine()
        {
            string line = null;
            try
            {
                if (reader != null)
                {
                    line = reader.ReadLine();
                }
            }catch(Exception e)
            {
                Console.WriteLine("Cannot Read : " + e.ToString());
            }
            return line;
        }
        public string[] readlAll()
        {
            List<String> lines = new List<string>();
            string line;
            try
            {
                while ((line = reader.ReadLine()) != null)
                {
                    lines.Add(line);
                }
            }
            catch(Exception e)
            {
                Console.WriteLine("Cannot Read All : " + e.ToString());
            }
            return lines.ToArray();
        }
        public void Close()
        {
            reader.Close();
        }
    }
}
