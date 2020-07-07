using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using ServiceStack;
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
        public MainWindow()
        {
            InitializeComponent();

            // 从文件中获取列表
            fileList = FileHandler.GetFiles(@".\Files.txt");

            //iniApp();

            listView.ItemsSource = fileList;

            this.Closing += MainWindow_Closing;
        }

        /// <summary>
        /// 软件开启，初始化程序
        /// </summary>
        public void iniApp()
        {
            FileInfo fileInfo = new FileInfo(@".\Files.txt");
            //配置文件存在
            if (fileInfo.Exists == true)
            {
                StreamReader streamReader = new StreamReader(@".\Files.txt");
                string line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    string[] items = line.Split("~");
                    try
                    {
                        //可能读到空的字符串
                        string fileName = items[0];
                        string filePath = items[1];
                        //listView.Items.Add(new File(fileName, filePath));
                        fileList.Add(new AutoOpen.File(fileName, filePath));
                    }
                    catch
                    {
                        continue;
                    }
                }
                streamReader.Close();
            }
        }

        /// <summary>
        /// 程序关闭，将数据写入到文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            FileHandler.SaveFileList( @".\Files.txt", fileList);
            /*
            FileInfo fileInfo = new FileInfo(@".\Files.txt");
            if (fileInfo.Exists == true)
            {
                fileInfo.Delete();
                fileInfo.Create();
            }        
            new StreamWriter(@".\Files.txt")
            StreamWriter streamWriter = fileInfo.AppendText();
            foreach (AutoOpen.File file in listView.Items)
            {
                string line = file.fileName + "~" + file.filePath;
                streamWriter.WriteLine(line);
            }
            streamWriter.Close();
            */
        }

        /// <summary>
        /// ListView Selected Item Change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //MessageBox.Show("Here");
            AutoOpen.File file = listView.SelectedItem as AutoOpen.File;
            if(file != null)
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

        //移除文件
        private void removeBtn_Click(object sender, RoutedEventArgs e)
        {
            foreach (AutoOpen.File file in fileList.Where(f => f.isSelected).ToList())
                fileList.Remove(file);
            listView.Items.Refresh();
        }

        //移除所有文件
        private void removeAllBtn_Click(object sender, RoutedEventArgs e)
        {
            //listView.SelectAll();
            fileList.Clear();
            listView.Items.Refresh();
            //listView.Items.Clear();
        }

        //打开指定文件
        private void openBtn_Click(object sender, RoutedEventArgs e)
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
                    logTextBlock.Text += $"{Path.GetFileName(filePath)} open Success\n";
                }
                catch
                {
                    logTextBlock.Text += $"{Path.GetFileName(filePath)} open Filed\n";
                }
            }
        }

        //打开所有文件按钮被点击
        private void openAllBtn_Click(object sender, RoutedEventArgs e)
        {
            //logTextBlock.Text = "";
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
                    logTextBlock.Text += $"{fileName} open Success\n";
                }
                catch (Exception)
                {
                    logTextBlock.Text += $"{fileName} open Failed\n";
                }
            }
        }

        //在文件夹中打开被点击
        private void openInFEBtn_Click(object sender, RoutedEventArgs e)
        {
            foreach (AutoOpen.File file in fileList.Where(f => f.isSelected).ToList())
            {
                string fileName = file.fileName;
                string filePath = file.filePath.Substring(0, file.filePath.LastIndexOf(@"\") + 1);
                try
                {
                    System.Diagnostics.Process.Start("explorer.exe", filePath);
                    logTextBlock.Text += $"{fileName} open in file explore Success\n";
                }
                catch (Exception)
                {
                    logTextBlock.Text += $"{fileName} open in file explore Failed\n";
                }
            }

        }

        //多选框
        private void cbx_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if((ComboBoxItem)cbx.SelectedItem == null)
            {
                return;
            }
            string selection = ((ComboBoxItem)cbx.SelectedItem).Content.ToString();
            switch (selection)
            {
                case "全选":
                    foreach (AutoOpen.File file in fileList)
                        file.isSelected = true;
                    break;
                case "反选":
                    foreach (AutoOpen.File file in fileList)
                        file.isSelected = !file.isSelected;
                    break;
                case "不选":
                    foreach (AutoOpen.File file in fileList)
                        file.isSelected = false;
                    break;
                default:
                    break;
            }
            cbx.SelectedIndex = -1;
            listView.Items.Refresh();
        }

        //checkbox被点击
        private void SelectClick_Click(object sender, RoutedEventArgs e)
        {
            //CheckBox checkBox = sender as CheckBox;
            //string filePath = Convert.ToString(checkBox.ToolTip);

            /* 获取相应的对象，并将其isSelected属性值改变
            AutoOpen.File file = fileList.Where(file => file.filePath == filePath).FirstOrDefault();
            MessageBox.Show(file.fileName + " " + file.isSelected);
            */

            /*
            if(checkBox.IsChecked == true)
            {
                if(this.listView.Items != null)
                {
                    List<AutoOpen.File> files = this.listView.Items.Cast<AutoOpen.File>().ToList();
                    AutoOpen.File findFile = files.Where(file => file.filePath == filePath).SingleOrDefault();
                    if(findFile == null)
                    {
                        AutoOpen.File file = fileList.Where(file => file.filePath == filePath).FirstOrDefault();
                        //this.listView.Items.Add(file);
                    }
                }
                else
                {
                    AutoOpen.File file = fileList.Where(file => file.filePath == filePath).FirstOrDefault();
                    //this.listView.Items.Add(file);
                }
            }
            else
            {
                AutoOpen.File file = fileList.Where(file => file.filePath == filePath).FirstOrDefault();
                //this.listView.Items.Remove(file);
            }
            */
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
                    logTextBlock.Text += $"{Path.GetFileName(filePath)} open Success\n";
                }
                catch
                {
                    logTextBlock.Text += $"{Path.GetFileName(filePath)} open Filed\n";
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
                    logTextBlock.Text += $"{fileName} open in file explore Success\n";
                }
                catch (Exception)
                {
                    logTextBlock.Text += $"{fileName} open in file explore Failed\n";
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
            //listView.SelectAll();
            fileList.Clear();
            listView.Items.Refresh();
            //listView.Items.Clear();
        }

        //右键打开所有文件
        private void openAllFile_Click(object sender, RoutedEventArgs e)
        {
            //logTextBlock.Text = "";
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
                    logTextBlock.Text += $"{fileName} open Success\n";
                }
                catch (Exception)
                {
                    logTextBlock.Text += $"{fileName} open Failed\n";
                }
            }
        }

        //private void tag_Click(object sender, RoutedEventArgs e)
        //{
        //    MessageBox.Show(this.GetType().Name);
        //}
    }
}
