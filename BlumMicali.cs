using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace Kod
{
    class BlumMicali
    {
        MyType power(MyType x, MyType y, MyType p)
        {
            MyType res = new MyType("1"); /* wynik */

            x = x % p;
            while (y.getNumber() > 0)
            {
                if (y % 2 == 1)
                {
                    res *= x;
                    res = res % p;
                }

                x *= x;
                x = x % p;
                y = y.DzielRow(y, 2);
            }
            return res;
        }

        List<bool> Blum(int size)
        {
            MyType a = new MyType("509");
            MyType p = new MyType("521");

            var random = new Random();

            MyType x0 = new MyType(random.Next(10, 500));
            MyType x = new MyType(1);
            string ccout = "x0 = " + x0;
            MessageBox.Show(ccout);
            List<bool> klucz = new List<bool>();
            int k=0;
            int l=0;
            for (int i = 0; i < size * 8; i++)
            {
                x = power(a, x0, p);
                k = x.getNumber();
                l = (p.getNumber() - 1) / 2;
                if (k > l) klucz.Add(true);
                else klucz.Add(false);
                x0 = x;
            }
            return klucz;
        }





    }
}
