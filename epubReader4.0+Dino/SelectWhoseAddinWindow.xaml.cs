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
    /// SelectWhoseAddinWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class SelectWhoseAddinWindow : Window
    {
        public SelectWhoseAddinWindow()
        {
            InitializeComponent();
        }

        //管理者が追加した教材
        private void administratorButton_Click(object sender, RoutedEventArgs e)
        {

        }

        //自分以外の児童が追加した教材
        private void studentButton_Click(object sender, RoutedEventArgs e)
        {

        }

        //自分が追加した教材
        private void myButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
