using System;
using System.Collections.Generic;
using System.Text;
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
    public partial class Szyfrowanie : Page
    {
        public Szyfrowanie(string text)
        {
            // To bytes
            text1 = text;
            InitializeComponent();
            MessageBox.Show(text1, "Podany tekst to: ");
            char sign;
            int i = 0;
            bool[] charByte;
            List<bool> BytesText = new List<bool>();
            while (i < text1.Length)
            {
                sign = text1[i];
                charByte = CharsToBytes(sign);
                for (int k = 0; k < 8; k++)
                {
                    BytesText.Add(charByte[k]);
                }
                i++;
            }
            string help = "";
            for (int u = 0; u < text1.Length * 8; u++)
            {
                help += Convert.ToString(Convert.ToInt32(BytesText[u]));
                if ((u + 1) % 8 == 0) help += "        ";
            }
            MessageBox.Show(help, " POSTAC BINARNA: ");
            // Generate key
            List<bool> keyBytes = new List<bool>();
            keyBytes = BlumMicali(text1.Length);
            MessageBox.Show("wyszedl" );
            help = "";
            for (int s = 0; s < text1.Length * 8; s++)
            {
                help += keyBytes[s];
                if ((s + 1) % 8 == 0) help += '\t';
            }
            MessageBox.Show(help, " KLUCZ: ");

            //Encryption
            help = "";
            List<bool> bitCipher = new List<bool>();

            for (int s = 0; s < text1.Length * 8; s++)
            {
                bitCipher.Add(BytesText[s] ^ keyBytes[s]);
            }
            for (int s = 0; s < text1.Length * 8; s++)
            {
                help += bitCipher[s];
                if ((s + 1) % 8 == 0) help += '\t';
            }
            MessageBox.Show(help, "ZASZYFROWANY CIAG BITOW:");

            // CipherToASCII
            string ASCIICipher = "";
            bool[] bits = new bool[8];
            help = "";
            for (int s = 0; s < text1.Length; s++)
            {
                for (int m = 0; m < 8; m++)
                {
                    bits[m] = bitCipher[s * 8 + m];
                }
                ASCIICipher += BytesToChars(bits);
            }
            help = ASCIICipher;
            MessageBox.Show(help, "ZASZYFROWANA WIADOMOSC W ASCII: ");

            //Decryption
            help = "";
            List<bool> bytesDescription = new List<bool> ();
            for (int s = 0; s < text1.Length * 8; s++)
            {
                bytesDescription.Add(keyBytes[s] ^ bitCipher[s]);
            }
            for (int s = 0; i < text1.Length * 8; s++)
            {
                help+= bytesDescription[s];
                if ((s + 1) % 8 == 0)help+= '\t';
            }
            MessageBox.Show(help, "ODSZYFROWANY CIAG BITOW:");


            // Descryption to ASCII
            string ASCII = "";
            bool[] bity = new bool[8];
            for (int s = 0; s < text1.Length; s++)
            {
                for (int m = 0; m < 8; m++)
                {
                    bity[m] = bytesDescription[s * 8 + m];
                }
                ASCII += BytesToChars(bity);
            }
            MessageBox.Show(ASCII, "ODSZYFROWANA WIADOMOSC: ");

        }



        MyType power(MyType x, MyType y, MyType p)
        {
            MyType res = new MyType();
            res = res.stringKonst(res, 1);
            MessageBox.Show("rozmiar x", x.getSize().ToString());
            x = x % p;
            MessageBox.Show("rozmiar x", x.getSize().ToString());
            while (y.getNumber() > 0)
            {
                if (y % 2 == 1)
                {
                    res = res.mnozenierowneMyType(res, x);
                    //res *= x;
                    res = res % p;
                }
                MessageBox.Show("dzielro");
                x = x.mnozenierowneMyType(x, x);
                //x *= x;
                x = x % p;
                y = y.DzielRow(y, 2);
                MessageBox.Show("dzielro");
            }
            return res;
        }

        List<bool> BlumMicali(int size)
        {
            MyType a = new MyType();
            a = a.stringKonst(a,509);
            MyType p = new MyType();
            p = p.stringKonst(p,521); 
            var random = new Random();
            MessageBox.Show("rozmiar a", a.getSize().ToString());
            MyType x0 = new MyType(random.Next(10, 500));
            MyType x = new MyType(1);
            string ccout = "x0 = " + Convert.ToString(x0.getNumber());
            MessageBox.Show(ccout);
            List<bool> klucz = new List<bool>();
            int k = 0;
            int l = 0;
            for (int s = 0; s < size * 8; s++)
            {
                x = power(a, x0, p);
                MessageBox.Show("jest w for");
                k = x.getNumber();
                l = (p.getNumber() - 1) / 2;
                if (k > l) klucz.Add(true);
                else klucz.Add(false);
                x0 = x;
            }
            return klucz;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(text1);
        }


        public int j = 7;


        public bool[] CharsToBytes(char chars)
        {
            bool[] bytes = new bool[8];
            j = 7;
            do
            {
                bytes[j] = Convert.ToBoolean((int)chars % 2);
                chars = (char)((int)chars / 2);
                j--;
            } while (j >= 0);
            return bytes;
        }


        public char BytesToChars(bool[] bytes)
        {
            char chars = (char)0;
            int ch = 0;
            for (j = 7; j >= 0; j--)
            {
                ch += Convert.ToInt32(Convert.ToInt64(bytes[j]) * (int)(Math.Pow(2, 7 - j)));
            }
            chars = (char)ch;
            return chars;
        }
        private string text1;


    }
}
