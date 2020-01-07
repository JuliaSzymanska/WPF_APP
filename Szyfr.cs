using System;
using System.Collections.Generic;
using System.Windows;

namespace Kod
{
    class Szyfr
    {
        public void main(string text)
        {
            // To bytes
            text1 = text;
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
                if ((u + 1) % 8 == 0) help += '\t';
            }
            MessageBox.Show(help, " POSTAC BINARNA: ");

            // Generate key
            List<bool> keyBytes = new List<bool>();
            keyBytes = BlumMicali(text1.Length);
            help = "";
            for (int s = 0; s < text1.Length * 8; s++)
            {
                help += Convert.ToInt32(keyBytes[s]);
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
                help += Convert.ToInt32(bitCipher[s]);
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

            string texts = ASCIICipher;
            System.IO.File.WriteAllText(@".\ZaszyfrowanyTekst.txt", texts);
            return;
        }

        public int j = 7;
        private string text1;
        private int x0i;

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
        
        List<bool> BlumMicali(int size)
        {
            MyBigType a = new MyBigType(509);
            MyBigType p = new MyBigType(521);
            var random = new Random();
            x0i = random.Next(10, 500);
            MyBigType x0 = new MyBigType(x0i);
            MyBigType x = new MyBigType(1);
            string ccout = "x0 = " + Convert.ToString(x0);
            System.IO.File.WriteAllText(@".\ZaszyfrowanyKlucz.txt", Convert.ToString(x0));
            System.IO.File.WriteAllText(@".\ZaszyfrowanyRozmiar.txt", Convert.ToString(size));
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
            MessageBox.Show(text1);
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
