using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace epubReader4._0_Dino
{
    /// <summary>
    /// pageWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class DinoPageWindow : Window
    {
        public string[] pageName = new string[2048];
        string subjectNamePW = null;
        string pageDirectory = null;
        string unitNamePage = null;
        string epubFileName1;
        int epubCurrentPageLeft1;

        public DinoPageWindow()
        {
            InitializeComponent();
        }

        //UnitWindow & mainWindow からくるとき
        public void pageShow(string subjectName, string unitName, string epubFileName, int epubCurrentPageLeft)
        {
            epubFileName1 = epubFileName;
            epubCurrentPageLeft1 = epubCurrentPageLeft;

            unitNamePage = unitName;
            int d;
            for (d = 0; d < 1024; d++)
            {
                pageName[d] = null;
            }
            subjectNamePW = subjectName;
            pageDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\ContentsData\\note\\" + subjectName + "\\" + unitName;

            //epubファイルを、大文字小文字を区別して探す
            System.Collections.ObjectModel.ReadOnlyCollection<string> files =
                Microsoft.VisualBasic.FileIO.FileSystem.FindInFiles(
                pageDirectory,
                "",
                false,
                Microsoft.VisualBasic.FileIO.SearchOption.SearchAllSubDirectories,
                new string[] { "*.isf" });

            d = 1;
            int number = 0;
            foreach (string f in files)
            {
                number++;
                //pageName[d] = f;
                //pageName[d] = pageName[d].Replace(pageDirectory + "\\", "");
                //確認用
                // MessageBox.Show(number.ToString());
                //d++;
            }
            for (d = 1; d < number; d++)
            {
                pageName[d] = d + ".isf";
                //MessageBox.Show(pageName[d]);
            }
            /*string l = null;
            if (d < 100)
            {
                l = "0" + d;
                if (d < 10)
                {
                    l = "0" + l;
                }
            }*/

            //ボタンを生成
            int i = 1;//column
            int j = 1;//Row
            int k = 0;
            /*int num = 0;
            for (num = 0; pageName[num] != null; num++)
            {
            }*/
            Button[] btn = new Button[number];
            for (k = 1; k < number; k++)
            {
                if (1 <= i && i <= 6)
                {
                    btn[k] = new Button() { Content = pageName[k] };

                    //RowDefinition rd1 = new RowDefinition() { Height = new GridLength(50) };
                    //grid1.RowDefinitions.Add(rd1);

                    btn[k].Content = string.Format(pageName[k], k);
                    Grid.SetColumn(btn[k], i);
                    Grid.SetRow(btn[k], j);
                    grid1.Children.Add(btn[k]);
                    btn[k].VerticalAlignment = VerticalAlignment.Stretch;
                    btn[k].HorizontalAlignment = HorizontalAlignment.Stretch;
                    btn[k].Width = double.NaN;
                    btn[k].Height = double.NaN;
                    //MessageBox.Show(pageDirectory + "\\" + pageName[k]);
                    if (System.IO.File.Exists(pageDirectory + "\\" + pageName[k].Replace(".isf", ".png")))
                    {
                        BitmapImage bmp = new BitmapImage();
                        System.IO.FileStream stream = System.IO.File.OpenRead(pageDirectory + "\\" + pageName[k].Replace(".isf", ".png"));
                        bmp.BeginInit();
                        bmp.CacheOption = BitmapCacheOption.OnLoad;
                        bmp.StreamSource = stream;
                        bmp.EndInit();
                        stream.Close();
                        btn[k].Background = new ImageBrush(bmp);

                    }
                    i++;

                    btn[k].Click += new RoutedEventHandler(btn_Click);

                    if (i == 6)
                    {
                        i = 1;
                        j++;
                    }
                }

            }
            //grid1.ShowGridLines = true;
        }

        public void btn_Click(object sender, RoutedEventArgs e)
        {
            String pageName = sender.ToString();
            pageName = pageName.Replace("System.Windows.Controls.Button: ", "");
            int x = pageName.IndexOf(".");
            //MessageBox.Show(pageName);

            //pageName = pageName.Remove(0, x+1);

            DinoMainWindow mainDialog = new DinoMainWindow();
            mainDialog.Show();
            this.Owner = mainDialog;
            mainDialog.isfShow(subjectNamePW, pageName, pageDirectory, unitNamePage, epubFileName1, epubCurrentPageLeft1);

            this.Close();
        }

        private void close2_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DinoUnitWindow mainDialog = new DinoUnitWindow();
            mainDialog.Show();
            this.Owner = mainDialog;
            mainDialog.newButton(subjectNamePW, epubFileName1, epubCurrentPageLeft1);

            this.Close();
        }

        private void createButton_Click(object sender, RoutedEventArgs e)
        {
            int newNum = 0;
            int newPageNum = 0;
            for (newNum = 0; pageName[newNum] != null; newNum++)
            {
            }
            newPageNum = newNum + 1;

            DinoMainWindow mainDialog = new DinoMainWindow();
            mainDialog.Show();
            this.Owner = mainDialog;
            mainDialog.isfShow(subjectNamePW, newPageNum.ToString(), pageDirectory, unitNamePage, epubFileName1, epubCurrentPageLeft1);

            this.Close();

        }
    }
}
