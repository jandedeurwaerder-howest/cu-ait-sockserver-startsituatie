using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using SockServerLib;
using System.Windows.Forms;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Windows.Threading;

namespace SockServerWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Socket listener;
        int numberOfClients = 1;
        bool verderdoen = true;
        string activeFolder;
        string baseFolder;

        public MainWindow()
        {

        }
        public static void DoEvents()
        {
            System.Windows.Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));
        }
        private void DoStartup()
        {
        }

        private void BtnSaveConfig_Click(object sender, RoutedEventArgs e)
        {
        }

        private void BtnSelectWorkingFolder_Click(object sender, RoutedEventArgs e)
        {
        }

        private void FillDictionary()
        {
            tbkDictionary.Text = "";
            tbkDictionary.Text += "DIRALL" + Environment.NewLine;
            tbkDictionary.Text += "DIRFILES" + Environment.NewLine;
            tbkDictionary.Text += "DIRFOLDERS" + Environment.NewLine;
            tbkDictionary.Text += "CURRENTDIR" + Environment.NewLine;
            tbkDictionary.Text += "CHANGEDIR <existing foldername>" + Environment.NewLine;
            tbkDictionary.Text += "CHANGEDIR UP" + Environment.NewLine;
            tbkDictionary.Text += "CHANGEDIR ROOT" + Environment.NewLine;
            tbkDictionary.Text += "MAKEDIR <non existing foldername>" + Environment.NewLine;
            tbkDictionary.Text += "REMOVEDIR <remove existing empty folder>" + Environment.NewLine;
            tbkDictionary.Text += "REMOVEDIRALL <remove existing folder>" + Environment.NewLine;
            tbkDictionary.Text += "RENAMEDIR <existing folder>,<new name>" + Environment.NewLine;
            tbkDictionary.Text += "CONTENTFILE <existing file>" + Environment.NewLine;
            tbkDictionary.Text += "REMOVEFILE <remove existing file>" + Environment.NewLine;
            tbkDictionary.Text += "RENAMEFILE <rename existing file>,<new name>" + Environment.NewLine;

        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
        }

        private void BtnStop_Click(object sender, RoutedEventArgs e)
        {
        }
        public void ExecuteServer()
        {
        }

        //private string ExecuteCommand(string command)
        //{
 
        //}

        private string DIRALL()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"Subfolders & files in current folder : \n");
            DirectoryInfo basedir = new DirectoryInfo(activeFolder);
            foreach (DirectoryInfo di in basedir.GetDirectories())
            {
                sb.AppendLine($"<dir>\t{di.Name}");
            }
            foreach (FileInfo fi in basedir.GetFiles())
            {
                sb.AppendLine($"\t{fi.Name}");
            }
            return sb.ToString();
        }
        private string DIRFILES()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"Files in current folder : \n");
            DirectoryInfo basedir = new DirectoryInfo(activeFolder);
            foreach (FileInfo fi in basedir.GetFiles())
            {
                sb.AppendLine($"\t{fi.Name}");
            }
            return sb.ToString();
        }
        private string DIRFOLDERS()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"Subfolders in current folder : \n");

            DirectoryInfo basedir = new DirectoryInfo(activeFolder);
            foreach (DirectoryInfo di in basedir.GetDirectories())
            {
                sb.AppendLine($"<dir>\t{di.Name}");
            }
            return sb.ToString();
        }
        private string CURRENTDIR()
        {
            return activeFolder;
        }
        private string CHANGEDIR(string subfolder)
        {
            if (Directory.Exists(activeFolder + "\\" + subfolder))
            {
                activeFolder = activeFolder + "\\" + subfolder;
                return "<cf>" + activeFolder;
            }
            else
                return $"Error : folder unchanged\nCurrent folder is still : {activeFolder}";
        }
        private string CHANGEDIR_UP()
        {
            if (activeFolder == baseFolder)
            {
                return "<cf>" + activeFolder;
            }
            DirectoryInfo di = new DirectoryInfo(activeFolder);
            activeFolder = di.Parent.FullName;
            return "<cf>" + activeFolder;

        }
        private string CHANGEDIR_ROOT()
        {
            activeFolder = baseFolder;
            return "<cf>" + activeFolder;

        }
        private string MAKEDIR(string subfolder)
        {
            if (!Directory.Exists(activeFolder + "\\" + subfolder))
            {
                try
                {
                    Directory.CreateDirectory(activeFolder + "\\" + subfolder);
                }
                catch (Exception fout)
                {
                    return $"Error : {fout.Message}\nCurrent folder is still : {activeFolder}";

                }
                activeFolder = activeFolder + "\\" + subfolder;
                return "<cf>" + activeFolder;
            }
            else
                return $"Error : folder allready exists\nCurrent folder is still : {activeFolder}";
        }
        private string REMOVEDIR(string folder)
        {
            if (Directory.Exists(activeFolder + "\\" + folder))
            {
                try
                {
                    DirectoryInfo di = new DirectoryInfo(activeFolder + "\\" + folder);
                    int aantalMappen = di.GetDirectories().Count();
                    int aantalBestanden = di.GetFiles().Count();
                    if (aantalBestanden > 0 || aantalMappen > 0)
                    {
                        return $"Error : folder NOT empty";

                    }
                    Directory.Delete(activeFolder + "\\" + folder);
                }
                catch (Exception fout)
                {
                    return $"Error : {fout.Message}";

                }
                return "<cf>" + activeFolder;
            }
            else
                return $"Error : folder does not exists";
        }
        private string REMOVEDIRALL(string folder)
        {
            if (Directory.Exists(activeFolder + "\\" + folder))
            {
                try
                {
                    Directory.Delete(activeFolder + "\\" + folder, true);
                }
                catch (Exception fout)
                {
                    return $"Error : {fout.Message}";

                }
                return "<cf>" + activeFolder;
            }
            else
                return $"Error : folder does not exists";
        }
        private string RENAMEDIR(string oldfoldername, string newfoldername)
        {
            if (Directory.Exists(activeFolder + "\\" + oldfoldername))
            {
                try
                {
                    Directory.Move(activeFolder + "\\" + oldfoldername, activeFolder + "\\" + newfoldername);
                    return "<cf>" + activeFolder;
                }
                catch (Exception fout)
                {
                    return $"Error : {fout.Message}";
                }
            }
            else
                return $"Error : folder does not exists";
        }
        private string RENAMEFILE(string oldfilename, string newfilename)
        {
            if (File.Exists(activeFolder + "\\" + oldfilename))
            {
                try
                {
                    File.Move(activeFolder + "\\" + oldfilename, activeFolder + "\\" + newfilename);
                    return "<cf>" + activeFolder;
                }
                catch (Exception fout)
                {
                    return $"Error : {fout.Message}";
                }
            }
            else
                return $"Error : file does not exists";
        }
        private string REMOVEFILE(string filename)
        {
            if (File.Exists(activeFolder + "\\" + filename))
            {
                try
                {
                    File.Delete(activeFolder + "\\" + filename);
                }
                catch (Exception fout)
                {
                    return $"Error : {fout.Message}";

                }
                return "<cf>" + activeFolder;
            }
            else
                return $"Error : file does not exists";
        }
        private string CONTENTFILE(string filename)
        {
            if (File.Exists(activeFolder + "\\" + filename))
            {
                try
                {
                    byte[] filebytes = File.ReadAllBytes(activeFolder + "\\" + filename);
                    return Encoding.ASCII.GetString(filebytes);
                }
                catch (Exception fout)
                {
                    return $"Error : {fout.Message}";

                }
            }
            else
                return $"Error : file does not exists";
        }
    }
}
