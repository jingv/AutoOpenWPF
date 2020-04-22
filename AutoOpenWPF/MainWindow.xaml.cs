using Microsoft.Win32;
using System;
using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
//using System.Windows.Data;
//using System.Windows.Documents;
//using System.Windows.Input;
//using System.Windows.Media;
//using System.Windows.Media.Imaging;
//using System.Windows.Navigation;
//using System.Windows.Shapes;
//using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Documents;
using System.Linq;
//using ServiceStack.Redis;

namespace AutoOpenWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        private static List<AutoOpen.File> fileList = new List<AutoOpen.File>();
        public MainWindow()
        {
            InitializeComponent();

            //if (redisClient.Exists("Files") != 0)
            //{
            //    iniApp();
            //}
            //else
            //{
            //    redisClient.Set<List>("Files", null);
            //}

            iniApp();

            listView.ItemsSource = fileList;

            this.Closing += MainWindow_Closing;
        }

        //主窗口打开，初始化ListView
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

        //主窗口关闭
        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            FileInfo fileInfo = new FileInfo(@".\Files.txt");
            if (fileInfo.Exists == true)
            {
                fileInfo.Delete();
                //fileInfo.Create();
            }        
            //new StreamWriter(@".\Files.txt")
            StreamWriter streamWriter = fileInfo.AppendText();
            foreach (AutoOpen.File file in listView.Items)
            {
                //redisClient.Set<string>(file.filePath, file.fileName);

                string line = file.fileName + "~" + file.filePath;
                streamWriter.WriteLine(line);
            }
            streamWriter.Close();
        }

        private void listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        //添加文件按钮被点击
        private void addBtn_Click(object sender, RoutedEventArgs e)
        {
            //打开文件文件夹选择框，选取并获得路径名称https://blog.csdn.net/qq_38261174/article/details/85051812
            //选取文件获得路径
            var file = new OpenFileDialog();
            //多选的情况
            file.Multiselect = true;
            if (file.ShowDialog() == true)
            {
                string filePath = file.FileName;
                string fileName = file.SafeFileName;
                //MessageBox.Show(filePath + " - " + fileName);
                listView.Items.Add(new AutoOpen.File(fileName, filePath));
                //MessageBox.Show($"{filePath} - {fileName}");

            }

        }

        //删除文件按钮被点击
        private void removeBtn_Click(object sender, RoutedEventArgs e)
        {
            //Button btn = sender as Button;

            //MessageBox.Show(btn.DataContext.GetType().ToString());

        }

        //打开所有文件按钮被点击
        private void openBtn_Click(object sender, RoutedEventArgs e)
        {
            logTextBlock.Text = "";
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
            if (listView.SelectedValue != null)
            {
                AutoOpen.File file = listView.SelectedItem as AutoOpen.File;
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

        private void removeAllBtn_Click(object sender, RoutedEventArgs e)
        {
            //listView.SelectAll();
            listView.Items.Clear();
        }

        private void cbx_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void SelectClick_Click(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            string filePath = Convert.ToString(checkBox.ToolTip);
            if(checkBox.IsChecked == true)
            {
                if(this.listView.Items != null)
                {
                    List<AutoOpen.File> files = this.listView.Items.Cast<AutoOpen.File>().ToList();
                    AutoOpen.File findFile = files.Where(file => file.filePath == filePath).SingleOrDefault();
                    if(findFile == null)
                    {
                        AutoOpen.File file = fileList.Where(file => file.filePath == filePath).FirstOrDefault();
                        this.listView.Items.Add(file);
                    }
                }
                else
                {
                    AutoOpen.File file = fileList.Where(file => file.filePath == filePath).FirstOrDefault();
                    this.listView.Items.Add(file);
                }
            }
            else
            {
                AutoOpen.File file = fileList.Where(file => file.filePath == filePath).FirstOrDefault();
                this.listView.Items.Remove(file);
            }
        }

        //private void tag_Click(object sender, RoutedEventArgs e)
        //{
        //    MessageBox.Show(this.GetType().Name);
        //}
    }
}
