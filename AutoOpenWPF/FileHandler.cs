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
using System.Runtime.InteropServices;
using System.Diagnostics.SymbolStore;
using ServiceStack.Messaging;
using System.Windows;

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

        /// <summary>
        /// Determine if the File @ filePath is avaliable
        /// </summary>
        /// <param name="filePath">The path of the file</param>
        /// <returns>true means the file is avaliable; else not.</returns>
        public static Boolean IsFileAvaliable(string filePath)
        {
            Boolean isFileAvaliable = false;
            // 文件不存在
            if (!System.IO.File.Exists(filePath))
            {
                isFileAvaliable = false;
            }
            // 文件存在， 判断文件是否被其他程序使用
            else
            {
                //逻辑：尝试执行打开文件的操作，如果文件已经被其他程序使用，则打开失败，抛出异常，根据异常可以判断文件是否已经被其他程序使用
                System.IO.FileStream fileStream = null;
                try
                {
                    fileStream = System.IO.File.Open(filePath, System.IO.FileMode.Open, System.IO.FileAccess.ReadWrite, System.IO.FileShare.None);
                    isFileAvaliable = true;
                }
                catch (System.IO.IOException ioEx)
                {
                    isFileAvaliable = false;
                    MessageBox.Show(ioEx.Message);
                    // throw;
                }
                catch(System.Exception ex)
                {
                    isFileAvaliable = false;
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    if(fileStream != null)
                    {
                        fileStream.Close();
                    }
                }
            }
            return isFileAvaliable;
        }
    }
}
