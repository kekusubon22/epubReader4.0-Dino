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
    /// subWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class DinoSubWindow : Window
    {
        public DinoMainWindow MainWindowPointer;

        public DinoSubWindow()
        {
            InitializeComponent();
        }
        string pageNameSub, pageDirectorySub;

        // MainWindow & tbWindow からくるときに
        public void sub(string pageNameMain, string pageDirectory1)
        {
            pageNameSub = pageNameMain;
            pageDirectorySub = pageDirectory1;
        }

        //とる
        private void capture_Click(object sender, RoutedEventArgs e)
        {
            DinoTbWindow mainDialog = new DinoTbWindow();
            mainDialog.Show();
            mainDialog.subToTb(pageNameSub, pageDirectorySub);

            this.Close();
        }

        //やりなおす
        private void cansel_Click(object sender, RoutedEventArgs e)
        {

        }

        //画像をデジタルノートに送る
        private void sendButton_Click(object sender, RoutedEventArgs e)
        {
            //System.Windows.MessageBox.Show(pageNameSub, pageDirectorySub);

            DinoMainWindow mainDialog = new DinoMainWindow();
            mainDialog.Show();
            this.Owner = mainDialog;
            mainDialog.tbShow(pageNameSub, pageDirectorySub);

            this.Close();
        }

        //ノートに戻る
        private void note_Click(object sender, RoutedEventArgs e)
        {
            //System.Windows.MessageBox.Show(pageNameSub + pageDirectorySub);
            DinoMainWindow mainDialog = new DinoMainWindow();
            mainDialog.Show();
            mainDialog.subShow(pageNameSub, pageDirectorySub);

            this.Close();
        }

        //おわる
        private void close_Click(object sender, RoutedEventArgs e)
        {
            MainWindow dialog = new MainWindow();
            dialog.Close();
            this.Close();
        }
    }
}
