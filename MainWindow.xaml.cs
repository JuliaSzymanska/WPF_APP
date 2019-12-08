using System;
using System.Collections.Generic;
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

namespace Kod
{
    public partial class MainWindow : Window
    {
        private object Service;
        public MainWindow(){
            InitializeComponent();
        }
        string chain;
        private void Button_Click1(object sender, RoutedEventArgs e){
            chain = Text.Text;
            if (String.IsNullOrEmpty(Text.Text) && String.IsNullOrEmpty(Path.Text)) { MessageBox.Show("Nie podano tekstu ani sciezki!"); return; }
            else if (String.IsNullOrEmpty(Text.Text) && !String.IsNullOrEmpty(Path.Text)) chain = Path.Text;
            else if (!String.IsNullOrEmpty(Text.Text) && String.IsNullOrEmpty(Path.Text)) chain = Text.Text;
            else return;
            Szyfrowanie szyfrowanie = new Szyfrowanie(chain);
            this.Content = szyfrowanie;
        }
        private void Button_Click2(object sender, RoutedEventArgs e){
            chain = Text.Text;
            if (String.IsNullOrEmpty(Text.Text) && String.IsNullOrEmpty(Path.Text)) { MessageBox.Show("Nie podano tekstu ani sciezki!"); return; }
            else if (String.IsNullOrEmpty(Text.Text) && !String.IsNullOrEmpty(Path.Text)) chain = Path.Text;
            else if (!String.IsNullOrEmpty(Text.Text) && String.IsNullOrEmpty(Path.Text)) chain = Text.Text;
            else return;
            Deszyfrowanie deszyfrowanie = new Deszyfrowanie();
            this.Content = deszyfrowanie;
        }
    }
}
