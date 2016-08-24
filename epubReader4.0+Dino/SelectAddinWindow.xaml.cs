using System;
using System.Collections.Generic;
using System.IO;
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
    /// SelectAddinWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class SelectAddinWindow : Window
    {
        public SelectAddinWindow()
        {
            InitializeComponent();
        }

        //初期処理
        public void init()
        {
            //ボタンを生成
            Button[] btn = new Button[1024];

            int j = 0; //グリッドの列要素の位置
            int k = 0; //グリッドの行要素の位置

            //追加教材ディレクトリに画像が何枚保存されているか調べる
            string[] files = System.IO.Directory.GetFiles(System.Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), ".png", System.IO.SearchOption.TopDirectoryOnly);

            int i = 0;
            foreach (string f in files)
            {

                btn[i] = new Button() { Content = f };
                //if (System.IO.File.Exists(f)
                //{
                //    btn[i].Background = new ImageBrush(new BitmapImage(new Uri(epubCover[i], UriKind.Relative)));
                //}
                //else
                //{
                //    btn[i].Background = new SolidColorBrush(Color.FromArgb(255, 200, 240, 190));
                //}
                //if (j < 5)
                //{
                //    ColumnDefinition cd1 = new ColumnDefinition() { Width = new GridLength(200) };
                //    grid1.ColumnDefinitions.Add(cd1);
                //    j++;
                //}
                //else
                //{
                //    RowDefinition rd1 = new RowDefinition() { Height = new GridLength(200) };
                //    grid1.RowDefinitions.Add(rd1);
                //    j = 1;
                //    k++;
                //}
                //btn[i].Content = string.Format("{0}." + epubName[i], i + 1);
                //Grid.SetColumn(btn[i], j);
                //Grid.SetRow(btn[i], k);
                //grid1.Children.Add(btn[i]);
                //btn[i].VerticalAlignment = VerticalAlignment.Stretch;
                //btn[i].HorizontalAlignment = HorizontalAlignment.Stretch;
                //btn[i].Width = double.NaN;  //Autoという意味
                //btn[i].Height = double.NaN; //Autoという意味

                //btn[i].Click += new RoutedEventHandler(btn_Click);
            }
        }

        public void btn_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
