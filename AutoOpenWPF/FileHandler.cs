using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using AutoOpen;
using System.Security.AccessControl;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace AutoOpen
{
    class FileHandler
    {
        /// <summary>
        /// Get File Info From the File @ FilePath
        /// </summary>
        /// <param name="filePath"> The Path Of the File</param>
        /// <returns>File List</returns>
        public static List<AutoOpen.File> GetFiles(string filePath)
        {
            List<AutoOpen.File> files = new List<File> ();
            FileInfo fileInfo = new FileInfo(filePath);
            if (fileInfo.Exists == true)
            {
                StreamReader streamReader = new StreamReader(filePath);
                string line;
                while((line = streamReader.ReadLine()) != null)
                {
                    string[] items = line.Split("~");
                    try
                    {
                        //可能读到空的字符串
                        string name = items[0];
                        string path = items[1];
                        files.Add(new File(name, path));
                    }
                    catch
                    {
                        continue;
                    }
                }
                streamReader.Close();
            }
            return files;
        }

        /// <summary>
        /// Write Files' Info to the File @ FilePath
        /// </summary>
        /// <param name="filePath">Path of the File</param>
        /// <param name="files">List of Files' Info</param>
        public static void SaveFileList(string filePath, List<File> files)
        {
            FileInfo fileInfo = new FileInfo(filePath);
            if (fileInfo.Exists == true)
            {
                fileInfo.Delete();
            }
            StreamWriter streamWriter = fileInfo.AppendText();
            foreach (var file in files)
            {
                string line = file.fileName + "~" + file.filePath;
                streamWriter.WriteLine(line);
            }
            streamWriter.Close();
        }
    }
}
