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
    /// SelectWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class DinoSelectSubjectWindow : Window
    {
        public DinoSelectSubjectWindow()
        {
            InitializeComponent();
        }

        string subjectName = null;
        string epubFileName1;
        int epubCurrentPageLeft1;

        private void kokugo_Click(object sender, RoutedEventArgs e)
        {
            subjectName = "国語";

            DinoUnitWindow dialog = new DinoUnitWindow();
            dialog.Show();
            this.Owner = dialog;

            dialog.newButton(subjectName, epubFileName1, epubCurrentPageLeft1);

            this.Close();
        }

        private void sansu_Click(object sender, RoutedEventArgs e)
        {
            subjectName = "算数";

            DinoUnitWindow dialog = new DinoUnitWindow();
            dialog.Show();
            this.Owner = dialog;

            dialog.newButton(subjectName, epubFileName1, epubCurrentPageLeft1);

            this.Close();
        }

        private void rika_Click(object sender, RoutedEventArgs e)
        {
            subjectName = "理科";

            DinoUnitWindow dialog = new DinoUnitWindow();
            dialog.Show();
            this.Owner = dialog;

            dialog.newButton(subjectName, epubFileName1, epubCurrentPageLeft1);

            this.Close();
        }

        private void syakai_Click(object sender, RoutedEventArgs e)
        {
            subjectName = "社会";

            DinoUnitWindow dialog = new DinoUnitWindow();
            dialog.Show();
            this.Owner = dialog;

            dialog.newButton(subjectName, epubFileName1, epubCurrentPageLeft1);

            this.Close();
        }

        private void sonota_Click(object sender, RoutedEventArgs e)
        {
            subjectName = "その他";

            DinoUnitWindow dialog = new DinoUnitWindow();
            dialog.Show();
            this.Owner = dialog;

            dialog.newButton(subjectName, epubFileName1, epubCurrentPageLeft1);

            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            DinoUnitWindow mainDialog = new DinoUnitWindow();
            mainDialog.Show();
            this.Owner = mainDialog;
            this.Close();
        }

        public void setEpubInfo(string epubFileName, int epubCurrentPageLeft)
        {
            epubFileName1 = epubFileName;
            epubCurrentPageLeft1 = epubCurrentPageLeft;
        }
    }
}
