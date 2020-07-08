using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Windows.Media;
using AutoOpen;

namespace AutoOpenWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static List<AutoOpen.File> fileList = new List<AutoOpen.File>();
        private static string fileInfosFile = @".\Files.txt";

        public MainWindow()
        {
            InitializeComponent();

            // 从文件中获取列表
            fileList = FileHandler.GetFiles(fileInfosFile);

            listView.ItemsSource = fileList;

            //程序结束，保存信息至文件
            this.Closing += MainWindow_Closing;
        }

        /// <summary>
        /// 程序关闭，将数据写入到文件
        /// </summary>
        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e) =>
            FileHandler.SaveFileList(fileInfosFile, fileList);

        /// <summary>
        /// ListView Selected Item Change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            AutoOpen.File file = listView.SelectedItem as AutoOpen.File;
            if (file != null)
            {
                foreach (AutoOpen.File f in fileList)
                    f.isSelected = false;
                file.isSelected = true;
                listView.Items.Refresh();
            }

        }

        /// <summary>
        /// 向ListView中添加文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addBtn_Click(object sender, RoutedEventArgs e)
        {
            //打开文件文件夹选择框，选取并获得路径名称https://blog.csdn.net/qq_38261174/article/details/85051812
            //选取文件获得路径
            var files = new OpenFileDialog();
            //多选的情况
            files.Multiselect = true;
            if (files.ShowDialog() == true)
            {
                //openFileDialog.FileNames获取对话框中所有选定文件的文件名（String 类型数组），为绝对路径，类似"E:\\code\\123.xml"
                foreach (string filePath in files.FileNames)
                {
                    // fileList中已经存在当前要添加的file；直接跳过
                    if (fileList.Where(f => f.filePath == filePath).ToList().Count != 0)
                        continue;
                    string fileName = Path.GetFileName(filePath);
                    fileList.Add(new AutoOpen.File(fileName, filePath));
                }
                listView.Items.Refresh();
            }
        }

        //右键移除文件
        private void removeFile_Click(object sender, RoutedEventArgs e)
        {
            foreach (AutoOpen.File file in fileList.Where(f => f.isSelected).ToList())
                fileList.Remove(file);
            listView.Items.Refresh();
        }

        //右键打开文件
        private void openFile_Click(object sender, RoutedEventArgs e)
        {
            foreach (AutoOpen.File item in fileList.Where(f => f.isSelected).ToList())
            {
                //logTextBlock.Text = "";
                string filePath = item.filePath;
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                System.Diagnostics.ProcessStartInfo processStartInfo = new System.Diagnostics.ProcessStartInfo(filePath);
                process.StartInfo = processStartInfo;
                process.StartInfo.Arguments = "";
                process.StartInfo.UseShellExecute = true;
                try
                {
                    process.Start();
                }
                catch
                {
                    MessageBox.Show($"{Path.GetFileName(filePath)} open Filed!");
                }
            }
        }
        
        //右键在资源管理器中打开
        private void openInExplorer_Click(object sender, RoutedEventArgs e)
        {
            foreach (AutoOpen.File file in fileList.Where(f => f.isSelected).ToList())
            {
                string fileName = file.fileName;
                string filePath = file.filePath.Substring(0, file.filePath.LastIndexOf(@"\") + 1);
                try
                {
                    System.Diagnostics.Process.Start("explorer.exe", filePath);
                }
                catch (Exception)
                {
                    MessageBox.Show($"{fileName} open in file explore Failed!");
                }
            }
        }

        //右键添加文件
        private void addFile_Click(object sender, RoutedEventArgs e)
        {
            //打开文件文件夹选择框，选取并获得路径名称https://blog.csdn.net/qq_38261174/article/details/85051812
            //选取文件获得路径
            var files = new OpenFileDialog();
            //多选的情况
            files.Multiselect = true;
            if (files.ShowDialog() == true)
            {
                //openFileDialog.FileNames获取对话框中所有选定文件的文件名（String 类型数组），为绝对路径，类似"E:\\code\\123.xml"
                foreach (string filePath in files.FileNames)
                {
                    // fileList中已经存在当前要添加的file；直接跳过
                    if (fileList.Where(f => f.filePath == filePath).ToList().Count != 0)
                        continue;
                    string fileName = Path.GetFileName(filePath);
                    fileList.Add(new AutoOpen.File(fileName, filePath));
                }
                listView.Items.Refresh();
            }
        }

        //右键移除所有文件
        private void removeAllFile_Click(object sender, RoutedEventArgs e)
        {
            fileList.Clear();
            listView.Items.Refresh();
        }

        //右键打开所有文件
        private void openAllFile_Click(object sender, RoutedEventArgs e)
        {
            

            foreach (AutoOpen.File item in listView.Items)
            {
                string fileName = item.fileName;
                string filePath = item.filePath;
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                System.Diagnostics.ProcessStartInfo processStartInfo = new System.Diagnostics.ProcessStartInfo(filePath);
                process.StartInfo = processStartInfo;
                //打开文件的默认程序在开启时定义的参数
                process.StartInfo.Arguments = "";
                process.StartInfo.UseShellExecute = true;
                try
                {
                    process.Start();
                }
                catch (Exception)
                {
                    MessageBox.Show($"{fileName} open Failed!");
                }
            }
        }

        //右键选中所有
        private void selecteAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (AutoOpen.File file in fileList)
                file.isSelected = true;
            listView.Items.Refresh();
        }

        //右键反选
        private void reverseSelect_Click(object sender, RoutedEventArgs e)
        {
            foreach (AutoOpen.File file in fileList)
                file.isSelected = !file.isSelected;
            listView.Items.Refresh();
        }

        //右键不选
        private void notSelect_Click(object sender, RoutedEventArgs e)
        {
            foreach (AutoOpen.File file in fileList)
                file.isSelected = false;
            listView.Items.Refresh();
        }

    }
}
