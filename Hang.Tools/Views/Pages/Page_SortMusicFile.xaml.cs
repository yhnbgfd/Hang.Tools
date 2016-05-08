using Hang.Net4.Base.Attributes;
using Hang.Net4.Base.Enums;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using F = System.Windows.Forms;

namespace Hang.Tools.Views.Pages
{
    [Plugin(Name = "SortMusicFile", Type = PluginType.Page)]
    public partial class Page_SortMusicFile : Page
    {
        /// <summary>
        /// 整理音乐文件
        /// </summary>
        public Page_SortMusicFile()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 检查路径是否存在 OK
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private bool CheckPath(string path)
        {
            if (Directory.Exists(path))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 功能完成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_整理到演唱者_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("该操作会删除文件，近乎不可逆，确定要整理吗？", "警告", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                string path = textBox_音乐文件夹路径.Text.Trim();
                if (CheckPath(path))
                {
                    int count = 0;
                    DirectoryInfo folder = new DirectoryInfo(path);
                    foreach (DirectoryInfo folder2 in folder.GetDirectories())//遍历歌手文件夹
                    {
                        foreach (DirectoryInfo folder3 in folder2.GetDirectories())//遍历专辑文件夹
                        {
                            foreach (FileInfo music in folder3.GetFiles())//遍历歌曲
                            {
                                if (!File.Exists(folder2.FullName + "\\" + music.Name))//目标文件不存在
                                {
                                    File.Move(music.FullName, folder2.FullName + "\\" + music.Name);//移动文件夹
                                    count++;
                                }
                            }
                            Directory.Delete(folder3.FullName, true);//整理完一个专辑文件夹，就删掉
                        }
                    }
                    Logger("整理到演唱者 完成，共整理文件：" + count);
                }
                else
                {
                    Logger("失败，找不到该路径");
                }
            }
        }

        /// <summary>
        /// 功能完成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_选择文件夹_Click(object sender, RoutedEventArgs e)
        {
            F.FolderBrowserDialog fb = new F.FolderBrowserDialog();
            if (fb.ShowDialog() == F.DialogResult.OK)
            {
                textBox_音乐文件夹路径.Text = fb.SelectedPath;
            }
        }

        /// <summary>
        /// 功能完成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_清理重复歌曲_Click(object sender, RoutedEventArgs e)
        {
            string path = textBox_音乐文件夹路径.Text.Trim();
            if (CheckPath(path))
            {
                DirectoryInfo folder = new DirectoryInfo(path);
                foreach (DirectoryInfo folder2 in folder.GetDirectories())//遍历歌手文件夹
                {
                    foreach (DirectoryInfo folder3 in folder2.GetDirectories())//遍历专辑文件夹
                    {
                        foreach (FileInfo music in folder3.GetFiles())//遍历专辑文件夹下的歌曲
                        {
                            for (int i = 1; i < 4; i++)
                            {
                                string repeatMusic = music.DirectoryName + "\\" + music.Name.Substring(0, music.Name.LastIndexOf('.')) + " (" + i + ")" + music.Extension;
                                if (File.Exists(repeatMusic))//目标文件存在
                                {
                                    Logger("删除重复文件：" + repeatMusic);
                                    File.Delete(repeatMusic);
                                }
                            }
                        }
                    }
                    foreach (FileInfo music in folder2.GetFiles())//遍历歌手文件夹下的歌曲
                    {
                        for (int i = 1; i < 4; i++)
                        {
                            string repeatMusic = music.DirectoryName + "\\" + music.Name.Substring(0, music.Name.LastIndexOf('.')) + " (" + i + ")" + music.Extension;
                            if (File.Exists(repeatMusic))//目标文件存在
                            {
                                Logger("删除重复文件：" + repeatMusic);
                                File.Delete(repeatMusic);
                            }
                        }
                    }
                }
                Logger("清理重复歌曲 完成");
            }
        }

        /// <summary>
        /// 功能完成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_筛选高质量歌曲_Click(object sender, RoutedEventArgs e)
        {
            Logger("目前只支持 flac -> mp3");
            string path = textBox_音乐文件夹路径.Text.Trim();
            if (CheckPath(path))
            {
                DirectoryInfo folder = new DirectoryInfo(path);
                foreach (DirectoryInfo folder2 in folder.GetDirectories())//遍历歌手文件夹
                {
                    foreach (DirectoryInfo folder3 in folder2.GetDirectories())//遍历专辑文件夹
                    {
                        foreach (FileInfo music in folder3.GetFiles())//遍历专辑文件夹下的歌曲
                        {
                            if (music.Extension == ".flac")//如果找到高音质的，则寻找有没有低音质的
                            {
                                string repeatMusic = music.DirectoryName + "\\" + music.Name.Substring(0, music.Name.LastIndexOf('.')) + ".mp3";
                                if (File.Exists(repeatMusic))//mp3文件存在
                                {
                                    Logger("删除mp3文件：" + repeatMusic);
                                    File.Delete(repeatMusic);
                                }
                            }
                        }
                    }
                    foreach (FileInfo music in folder2.GetFiles())//遍历歌手文件夹下的歌曲
                    {
                        if (music.Extension == ".flac")
                        {
                            string repeatMusic = music.DirectoryName + "\\" + music.Name.Substring(0, music.Name.LastIndexOf('.')) + ".mp3";
                            if (File.Exists(repeatMusic))//mp3文件存在
                            {
                                Logger("删除mp3文件：" + repeatMusic);
                                File.Delete(repeatMusic);
                            }
                        }
                    }
                }
                Logger("筛选高质量歌曲 完成");
            }
        }

        /// <summary>
        /// 本身日志
        /// </summary>
        /// <param name="log"></param>
        private void Logger(string log)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                textBox_Log.AppendText("\n" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss # ") + log);
                textBox_Log.ScrollToEnd();
            }));
        }

    }
}
