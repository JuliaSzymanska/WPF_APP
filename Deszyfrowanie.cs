using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;

namespace Kod
{
    public class Deszyfrowanie
    {
        private int size = Convert.ToInt32(System.IO.File.ReadAllText(Path.GetFullPath(@".\ZaszyfrowanyRozmiar.txt")));
        public void main(string text, int key)
        {
            string help = "";
            char sign;
            int i = 0;
            bool h = false;
            List<bool> BytesText = new List<bool>();
            while (i < size * 8)
            {
                sign = text[i];
                if (sign == '1') h = true;
                else if (sign == '0') h = false;
                BytesText.Add(h);
                i++;
            }
            help = "";
            for (int u = 0; u < size * 8; u++)
            {
                help += Convert.ToString(Convert.ToInt32(BytesText[u]));
                if ((u + 1) % 8 == 0) help += '\t';
            }
            MessageBox.Show(help, "Podany zaszyfrowany tekst to: ");

            // Generate key
            List<bool> keyBytes = new List<bool>();
            keyBytes = BlumMicali(size, key);
            help = "";
            for (int s = 0; s < size * 8; s++)
            {
                help += Convert.ToInt32(keyBytes[s]);
                if ((s + 1) % 8 == 0) help += '\t';
            }
            MessageBox.Show(help, " KLUCZ: ");

            //Decryption
            help = "";
            List<bool> bytesDescription = new List<bool>();
            for (int s = 0; s < size * 8; s++)
            {
                bytesDescription.Add(keyBytes[s] ^ BytesText[s]);
            }
            for (int s = 0; s < bytesDescription.Count; s++)
            {
                help += Convert.ToInt32(bytesDescription[s]);
                if ((s + 1) % 8 == 0) help += '\t';
            }
            MessageBox.Show(help, "ODSZYFROWANY CIAG BITOW:");

            // Descryption to ASCII
            string ASCII = "";
            bool[] bity = new bool[8];
            for (int s = 0; s < size; s++)
            {
                for (int m = 0; m < 8; m++)
                {
                    bity[m] = bytesDescription[s * 8 + m];
                }
                ASCII += BytesToChars(bity);
            }
            MessageBox.Show(ASCII, "ODSZYFROWANA WIADOMOSC: ");
            return;
        }

        public void main()
        {
            // To bytes
            string text = System.IO.File.ReadAllText(@"ZaszyfrowanyTekst.txt");
            text1 = text;
            string help = "";
            
            char sign;
            int i = 0;
            bool h= false;
            List<bool> BytesText = new List<bool>();
            while (i < size * 8)
            {
                sign = text1[i];
                if (sign == '1') h = true; 
                else if (sign == '0') h = false;
                    BytesText.Add(h);
                i++;
            }
            help = "";
            for (int u = 0; u < size * 8; u++)
            {
                help += Convert.ToString(Convert.ToInt32(BytesText[u]));
                if ((u + 1) % 8 == 0) help += '\t';
            }
            MessageBox.Show(help, "Podany zaszyfrowany tekst to: ");

            // Generate key
            List<bool> keyBytes = new List<bool>();
            keyBytes = BlumMicali(size);
            help = "";
            for (int s = 0; s < size * 8; s++)
            {
                help += Convert.ToInt32(keyBytes[s]);
                if ((s + 1) % 8 == 0) help += '\t';
            }
            MessageBox.Show(help, " KLUCZ: ");

            //Decryption
            help = "";
            List<bool> bytesDescription = new List<bool>();
            for (int s = 0; s < size * 8; s++)
            {
                bytesDescription.Add(keyBytes[s] ^ BytesText[s]);
            }
            for (int s = 0; s < bytesDescription.Count; s++)
            {
                help += Convert.ToInt32(bytesDescription[s]);
                if ((s + 1) % 8 == 0) help += '\t';
            }
            MessageBox.Show(help, "ODSZYFROWANY CIAG BITOW:");

            // Descryption to ASCII
            string ASCII = "";
            bool[] bity = new bool[8];
            for (int s = 0; s < size; s++)
            {
                for (int m = 0; m < 8; m++)
                {
                    bity[m] = bytesDescription[s * 8 + m];
                }
                ASCII += BytesToChars(bity);
            }
            MessageBox.Show(ASCII, "ODSZYFROWANA WIADOMOSC: ");
            return;
        }

        public int j = 7;
        private string text1;

        MyBigType powerTo(MyBigType x, MyBigType y, MyBigType p)
        {
            MyBigType res = new MyBigType(1);
            x = x % p;
            MyBigType zero = new MyBigType(0);
            while (y > zero)
            {
                if ((y % 2) == 1)
                {
                    res *= x;
                    res = res % p;
                }
                x *= x;
                x = x % p;
                y /= 2;
            }
            return res;
        }

        List<bool> BlumMicali(int size, int key)
        {
            MyBigType a = new MyBigType(509);
            MyBigType p = new MyBigType(521);
            int x0i = Convert.ToInt32(System.IO.File.ReadAllText(@"ZaszyfrowanyKlucz.txt"));
            MyBigType x0 = new MyBigType(key);
            MyBigType x = new MyBigType(1);
            string ccout = "x0 = " + Convert.ToString(x0);
            MessageBox.Show(ccout);
            List<bool> klucz = new List<bool>();

            for (int s = 0; s < size * 8; s++)
            {
                x = powerTo(a, x0, p);
                if (x > (p - 1) / 2) klucz.Add(false);
                else klucz.Add(true);
                x0 = x;
            }
            return klucz;
        }

        List<bool> BlumMicali(int size)
        {
            MyBigType a = new MyBigType(509);
            MyBigType p = new MyBigType(521);
            int x0i = Convert.ToInt32(System.IO.File.ReadAllText(@"ZaszyfrowanyKlucz.txt"));
            MyBigType x0 = new MyBigType(x0i);
            MyBigType x = new MyBigType(1);
            string ccout = "x0 = " + Convert.ToString(x0);
            MessageBox.Show(ccout);
            List<bool> klucz = new List<bool>();

            for (int s = 0; s < size * 8; s++)
            {
                x = powerTo(a, x0, p);
                if (x > (p - 1) / 2) klucz.Add(false);
                else klucz.Add(true);
                x0 = x;
            }
            return klucz;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBox.Show(text1);
        }

        public bool[] CharsToBytes(char chars)
        {
            bool[] bytes = new bool[8];
            int j = 7;
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
            for (int j = 7; j >= 0; j--)
            {
                ch += Convert.ToInt32(Convert.ToInt64(bytes[j]) * (int)(Math.Pow(2, 7 - j)));
            }
            chars += (char)ch;
            return chars;
        }

    }

}
