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
    /// UnitWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class DinoUnitWindow : Window
    {
        public string[] unitName = new string[2048];
        string subjectName2 = null;

        public DinoUnitWindow()
        {
            InitializeComponent();
        }

        string epubFileName1;
        int epubCurrentPageLeft1;

        public void newButton(string subjectName, string epubFileName, int epubCurrentPageLeft)
        {
            epubFileName1 = epubFileName;
            epubCurrentPageLeft1 = epubCurrentPageLeft;

            subjectName2 = subjectName;

            int d = 0;
            for (d = 0; d < 1024; d++)
            {
                unitName[d] = null;
            }

            string unitDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\note";

            //epubファイルを、大文字小文字を区別して探す
            string[] subFolders = System.IO.Directory.GetDirectories(
                System.Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\note\\" + subjectName, "*", System.IO.SearchOption.TopDirectoryOnly);

            d = 0;
            foreach (string f in subFolders)
            {
                //確認用
                //MessageBox.Show(f);
                unitName[d] = f;
                unitName[d] = unitName[d].Replace(unitDirectory + "\\" + subjectName + "\\", "");
                //確認用
                //MessageBox.Show("unitName[" + i + "] = " + unitName[i]);
                d++;
            }

            //ボタンを生成
            int i = 1;
            int j = 1;
            int k = 0;
            int num = 0;
            for (num = 0; unitName[num] != null; num++)
            {
            }
            Button[] btn = new Button[num];
            for (k = 0; k < num; k++)
            {
                if (1 <= i && i <= 5)
                {
                    btn[k] = new Button() { Content = unitName[k] };

                    //RowDefinition rd1 = new RowDefinition() { Height = new GridLength(50)};
                    //grid1.RowDefinitions.Add(rd1);

                    btn[k].Content = string.Format(unitName[k], k);
                    Grid.SetColumn(btn[k], i);
                    Grid.SetRow(btn[k], j);
                    grid1.Children.Add(btn[k]);
                    btn[k].VerticalAlignment = VerticalAlignment.Stretch;
                    btn[k].HorizontalAlignment = HorizontalAlignment.Stretch;
                    btn[k].Width = double.NaN;
                    btn[k].Height = double.NaN;
                    btn[k].Background = new SolidColorBrush(Colors.White);
                    i++;

                    btn[k].Click += new RoutedEventHandler(btn_Click);

                    if (i == 5)
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
            String unitName = sender.ToString();
            unitName = unitName.Replace("System.Windows.Controls.Button: ", "");
            int x = unitName.IndexOf(".");
            unitName = unitName.Remove(0, x + 1);

            DinoPageWindow mainDialog = new DinoPageWindow();
            mainDialog.Show();
            this.Owner = mainDialog;
            mainDialog.pageShow(subjectName2, unitName, epubFileName1, epubCurrentPageLeft1);

            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            newFolder(subjectName2);
        }

        public void newFolder(string subject)
        {
            System.IO.DirectoryInfo di = System.IO.Directory.CreateDirectory(System.Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\Note\\" + subject);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            DinoSelectSubjectWindow mainDialog = new DinoSelectSubjectWindow();
            mainDialog.Show();
            mainDialog.setEpubInfo(epubFileName1, epubCurrentPageLeft1);
            this.Owner = mainDialog;

            this.Close();
        }

        private void newUnitButton_Click(object sender, RoutedEventArgs e)
        {
            DinoUnitNameWindow mainDialog = new DinoUnitNameWindow();
            mainDialog.Show();
            this.Owner = mainDialog;
            mainDialog.unitNameIn(subjectName2, epubFileName1, epubCurrentPageLeft1);
        }
    }
}
