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
            string txt="";
            List<bool> bitCipher = new List<bool>();

            for (int s = 0; s < text1.Length * 8; s++)
            {
                bitCipher.Add(BytesText[s] ^ keyBytes[s]);
            }
            for (int s = 0; s < text1.Length * 8; s++)
            {
                help += Convert.ToInt32(bitCipher[s]);
                txt += Convert.ToInt32(bitCipher[s]);
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

            string texts = Convert.ToString(txt);
            System.IO.File.WriteAllText(@".\ZaszyfrowanyTekst.txt", texts);
            return;
        }

        public int j = 7;
        private string text1;
        private int x0i;

        //MyBigType powerTo(MyBigType x, MyBigType y, MyBigType p)
        //{
        //    MyBigType res = new MyBigType(1);
        //    x = x % p;
        //    MyBigType zero = new MyBigType(0);
        //    while (y > zero)
        //    {
        //        if ((y % 2) == 1)
        //        {
        //            res *= x;
        //            res = res % p;
        //        }
        //        x *= x;
        //        x = x % p;
        //        y /= 2;
        //    }
        //    return res;
        //}

        static MyBigType powermodulo(MyBigType x, MyBigType y, MyBigType p)
        {
            MyBigType res = 1;
            x = x % p;
            while (y > 0)
            {
                if ((y & 1) == 1)
                    res = (res * x) % p;
                y = y >> 1;
                x = (x * x) % p;
            }
            return res;
        }

        List<bool> BlumMicali(int size)
        {
            var random = new Random();
            MyBigType g = new MyBigType("1347981406723692103108327603051596927581014069867");
            MyBigType p = new MyBigType("1347981406723692103108327603051596927581014069863");
            x0i = random.Next(10, 500);
            MyBigType x0 = new MyBigType(x0i);
            MyBigType x = new MyBigType(1);
            string ccout = "x0 = " + Convert.ToString(x0);
            System.IO.File.WriteAllText(@".\ZaszyfrowanyKlucz.txt", Convert.ToString(x0));
            MessageBox.Show(ccout);
            List<bool> klucz = new List<bool>();
            for (int s = 0; s < size * 8; s++)
            {
                x = powermodulo(g, x0, p);
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





        // Returns true if n is prime 
        public static bool isPrime(MyBigType n)
        {
            // Corner cases 
            if (n <= 1)
            {
                return false;
            }
            if (n <= 3)
            {
                return true;
            }

            // This is checked so that we can skip 
            // middle five numbers in below loop 
            if (n % 2 == 0 || n % 3 == 0)
            {
                return false;
            }

            for (MyBigType i = 5; i * i <= n; i = i + 6)
            {
                if (n % i == 0 || n % (i + 2) == 0)
                {
                    return false;
                }
            }

            return true;
        }

        /* Iterative Function to calculate (x^n)%p in 
        O(logy) */
        public static MyBigType power(MyBigType x, MyBigType y, MyBigType p)
        {
            MyBigType res = 1;     // Initialize result 

            x = x % p; // Update x if it is more than or 
                       // equal to p 

            while (y > 0)
            {
                // If y is odd, multiply x with result 
                if (y % 2 == 1)
                {
                    res = (res * x) % p;
                }

                // y must be even now 
                y = y >> 1; // y = y/2 
                x = (x * x) % p;
            }
            return res;
        }

        // Utility function to store prime factors of a number 
        public static void findPrimefactors(HashSet<MyBigType> s, MyBigType n)
        {
            // Print the number of 2s that divide n 
            while (n % 2 == 0)
            {
                s.Add(2);
                n = n / 2;
            }

            // n must be odd at this point. So we can skip 
            // one element (Note i = i +2) 
            for (int i = 3; i <= n.Pow(2); i = i + 2)
            {
                // While i divides n, print i and divide n 
                while (n % i == 0)
                {
                    s.Add(i);
                    n = n / i;
                }
            }

            // This condition is to handle the case when 
            // n is a prime number greater than 2 
            if (n > 2)
            {
                s.Add(n);
            }
        }

        // Function to find smallest primitive root of n 
        public static MyBigType findPrimitive(MyBigType n)
        {
            HashSet<MyBigType> s = new HashSet<MyBigType>();

            // Check if n is prime or not 
            if (isPrime(n) == false)
            {
                return -1;
            }

            // Find value of Euler Totient function of n 
            // Since n is a prime number, the value of Euler 
            // Totient function is n-1 as there are n-1 
            // relatively prime numbers. 
            MyBigType phi = n - 1;

            // Find prime factors of phi and store in a set 
            findPrimefactors(s, phi);

            // Check for every number from 2 to phi 
            for (int r = 2; r <= phi; r++)
            {
                // Iterate through all prime factors of phi. 
                // and check if we found a power with value 1 
                bool flag = false;
                foreach (MyBigType a in s)
                {

                    // Check if r^((phi)/primefactors) mod n 
                    // is 1 or not 
                    if (power(r, phi / (a), n) == 1)
                    {
                        flag = true;
                        break;
                    }
                }

                // If there was no power with value 1. 
                if (flag == false)
                {
                    return r;
                }
            }

            // If no primitive root found 
            return -1;
        }





    }
}
