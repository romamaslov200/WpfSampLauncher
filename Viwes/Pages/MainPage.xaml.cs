﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
using YandexDisk.Client.Http;
using YandexDisk.Client;
using System.ComponentModel;
using System.IO.Compression;
using Microsoft.Win32;
using System.Diagnostics;
using System.Security.Principal;
using WpfSampLauncher.Models;
using Newtonsoft.Json;
using System.Drawing;
using System.Security.Policy;
using System.Runtime.InteropServices;


namespace WpfSampLauncher.Viwes.Pages
{
    /// <summary>
    /// Логика взаимодействия для MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        
        [DllImport("gdi32.dll", EntryPoint = "RemoveFontResourceW", SetLastError = true)]
        public static extern int RemoveFontResource([In][MarshalAs(UnmanagedType.LPWStr)]
                                            string lpFileName);


        public MainPage()
        {
            InitializeComponent();
            try
            {
                var obj = JsonConvert.DeserializeObject<JsonSave>(File.ReadAllText("jsoninfo.json"));
                gunaTextBox1.Text = obj.NickGame;
            }
            catch (Exception ex) { }
        }

        private void Btn_Play_Click(object sender, RoutedEventArgs e)
        {
            Btn_Play.Dispatcher.BeginInvoke(new Action(() => Btn_Play.IsEnabled = false));
            checkversion();
        }


        public void TextBox_OnLostFocus(object sender, RoutedEventArgs e)
        {
            var jsoninfo = File.Exists("jsoninfo.json") ? JsonConvert.DeserializeObject<JsonSave>(File.ReadAllText("jsoninfo.json")) : new JsonSave
            {
                NickGame = gunaTextBox1.Text
            };

            string jsonData = JsonConvert.SerializeObject(jsoninfo);

            var jsoninfo2 = JsonConvert.DeserializeObject<JsonSave>(jsonData);

            jsoninfo.NickGame = gunaTextBox1.Text;
            File.WriteAllText("jsoninfo.json", JsonConvert.SerializeObject(jsoninfo));
        }






        async void checkversion()
        {
            await Task.Run(() =>
            {
                var task = Task.Run(Getlinks);
                task.Wait();
                compare_ver();

                /*
                try
                {
                    var task = Task.Run((Func<Task>)Form1.Getlinks);
                    task.Wait();
                    compare_ver();
                }
                catch
                {
                    MessageBox.Show("Плохое соединение с интернетом!");
                }
                */
            });


            static async Task Getlinks()
            {
                string oauthToken = "http://f0927201.xsph.ru/";
                using (IDiskApi diskApi = new DiskHttpApi(oauthToken))
                {
                    //var rc = await diskApi.Files.GetDownloadLinkAsync(path: "readme.txt");
                    //var uc = await diskApi.Files.GetDownloadLinkAsync(path: "Zip.zip");
                    //File.WriteAllText("ucl.txt", uc.Href);
                    using (var client = new WebClient())
                    {
                        client.DownloadFile($"{oauthToken}readme.txt", "readme.txt");
                    }
                }
            }
        }

        private void compare_ver()
        {
            string fileName = "ver.txt";
            string fileNameSer = "readme.txt";
            if (File.Exists(fileName) == true)
            {
                string vernow = File.ReadAllText(fileName);
                string verser = File.ReadAllText(fileNameSer);
                if (verser != vernow)
                {
                    l1.Dispatcher.BeginInvoke(new Action(() => l1.Content = "Обновление клиента..."));
                    Btn_Play.Dispatcher.BeginInvoke(new Action(() => Btn_Play.Content = "Обновление"));

                    dwn();
                    File.WriteAllText(fileName, File.ReadAllText(fileNameSer));
                }
                else
                {
                    if (Directory.Exists("data/") & File.Exists("data/gta_sa.exe") == true)
                    {
                        try
                        {
                            l1.Dispatcher.BeginInvoke(new Action(() => l1.Content = "Клиент обновлен"));
                            gunaProgressBar1.Dispatcher.Invoke(new Action(() => gunaProgressBar1.Value = 120));

                            string nick = null;
                            Dispatcher.Invoke(() => nick = gunaTextBox1.Text);

                            string sid = WindowsIdentity.GetCurrent().User.Value;
                            string nickgame = sid + "\\Software\\SAMP";
                            string putg = AppDomain.CurrentDomain.BaseDirectory + "\\data\\gta_sa.exe";
                            RegistryKey saveKey = Registry.Users.CreateSubKey(nickgame);
                            saveKey.SetValue("PlayerName", nick);
                            saveKey.SetValue("gta_sa_exe", putg);

                            saveKey.Close();
                            string vernoww = "46.174.50.42:7787";
                            Process.Start("data\\samp.exe", vernoww);
                        }
                        catch (Exception e)
                        {
                            MessageBox.Show(e.Message);
                        }
                    }
                    else
                    {
                        Btn_Play.Dispatcher.BeginInvoke(new Action(() => Btn_Play.Content = "Обновление"));
                        l1.Dispatcher.BeginInvoke(new Action(() => l1.Content = "Обновление клиента..."));
                        dwn();
                        File.WriteAllText(fileName, File.ReadAllText(fileNameSer));
                    }
                }
            }
            else
            {
                File.WriteAllText("ver.txt", "000");
                //l1.Content = "Обновление клиента...!";
                l1.Dispatcher.BeginInvoke(new Action(() => l1.Content = "Обновление клиента..."));

                dwn();
                File.WriteAllText(fileName, File.ReadAllText(fileNameSer));
            }
        }

        private void dwn()
        {
            string fileName = "Zip.zip";
            string linkx = "http://f0927201.xsph.ru/Zip.zip";

            if (File.Exists("data/sampaux3.ttf"))
            {
                RemoveFontResource("data/sampaux3.ttf");
            }
            
            if (File.Exists("data/Deleted/GTAWEAP3.TTF"))
            {
                RemoveFontResource("data/Deleted/GTAWEAP3.TTF");
                Btn_Play.Dispatcher.BeginInvoke(new Action(() => Btn_Play.Content = "Обновление клиента..."));
            }

            try
            {
                Directory.Delete("data/", true);
                File.Delete(fileName);
            }
            catch
            {

            }
            File.Delete(fileName);
            WebClient client = new WebClient();
            //client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
            client.DownloadFileCompleted += new AsyncCompletedEventHandler(client_DownloadFileCompleted);

            client.OpenRead(linkx);
            string size = client.ResponseHeaders["Content-Length"];

            client.DownloadFileAsync(new Uri(linkx), "Zip.zip");

            client.DownloadProgressChanged += (s, b) =>
            {
                l1.Dispatcher.BeginInvoke(new Action(() => l1.Content = $"{b.ProgressPercentage.ToString()}% ({((double)b.BytesReceived / 1048576).ToString("#.#")}MB/{Convert.ToUInt32(size) / 1048576}MB)"));
                gunaProgressBar1.Dispatcher.Invoke(new Action(() => gunaProgressBar1.Value = b.ProgressPercentage));
                };
            }


            void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
            {
                ext();
            }



            async void ext()
            {
                await Task.Run(() =>
                {
                    l1.Dispatcher.BeginInvoke(new Action(() => l1.Content = "Распаковка архива (3-5 минут)"));

                    var task = Task.Run((Func<Task>)WaitExt);
                    task.Wait();
                    gunaProgressBar1.Dispatcher.Invoke(new Action(() => gunaProgressBar1.Value = gunaProgressBar1.Maximum));

                });

            }

            async Task WaitExt()
            {
                await Task.Run(() =>
                {
                    try
                    {
                        DirectoryInfo directoryinfo = new DirectoryInfo("data/");
                        if (directoryinfo.Exists) directoryinfo.Delete(true);
                        ZipFile.ExtractToDirectory("Zip.zip", "data/");
                        l1.Dispatcher.BeginInvoke(new Action(() => l1.Content = "Готово"));
                        Btn_Play.Dispatcher.BeginInvoke(new Action(() => l1.Content = "Готово"));
                        Btn_Play.Dispatcher.BeginInvoke(new Action(() => Btn_Play.IsEnabled = true));
                        Btn_Play.Dispatcher.BeginInvoke(new Action(() => Btn_Play.Content = "Play"));
                    }
                    catch
                    {

                    }
                });
            }




        }




    }
