using System;
using System.Windows;

namespace Kod
{
    public partial class MainWindow : Window
    {
        public MainWindow(){
            InitializeComponent();
        }
        string chain;
        private void Button_Click1(object sender, RoutedEventArgs e){
            chain = Text.Text;
            if (String.IsNullOrEmpty(Text.Text)) { MessageBox.Show("Nie podano tekstu!"); return; }
            else chain = Text.Text;
            Szyfr szyfr = new Szyfr();
            szyfr.main(chain);
        }
        private void Button_Click2(object sender, RoutedEventArgs e){
            string text = System.IO.File.ReadAllText(@"D:\Dokumenty\Studia\3_semestr\Kryptografia\WPF_APP\Text.txt");
            if (String.IsNullOrEmpty(text)) { MessageBox.Show("Plik jest pusty"); return;}
            Szyfr szyfr = new Szyfr();
            szyfr.main(text);
        }
    }
}
