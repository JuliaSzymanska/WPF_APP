using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;

namespace Kod
{
    class MyType
    {
        public const int basic = 1000000000;
        public const int basic_digits = 9;
        private List<int> list = new List<int>();
        private int number;
        public MyType()
        {
            this.number = 1;
        }
        public MyType(int lo)
        {
            this.number = lo;
        }
        public MyType stringKonst(MyType w, int l)
        {
            w.list.Add(l);
            if (l < 0) w.number = -w.number;
            return w;
        }
        public MyType(string str)
        {
            read(str);
        }
        public int getSize()
        {
            return list.Count;
        }
        public int getNumber() { return number; }
        public static MyType operator +(MyType w, MyType v)
        {
            MyType res = w;
            if (w.number == v.number)
            {

                for (int i = 0, carry = 0; i < (int)(Math.Max(w.list.Count, v.list.Count)) || carry == 0; ++i)
                {
                    if (i == (int)res.list.Count)
                        res.list.Add(0);
                    res.list[i] += carry + (i < (int)w.list.Count ? w.list[i] : 0);
                    carry = Convert.ToInt32((res.list[i] >= basic));
                    if (Convert.ToBoolean(carry))
                        res.list[i] -= basic;
                }
                return res;
            }
            return w - (-v);
        }

        public static MyType operator -(MyType w, MyType v)
        {
            MyType res = w;
            if (w.number == v.number)
            {
                if (w.abs() >= v.abs())
                {

                    for (int i = 0, carry = 0; i < (int)v.list.Count || carry == 0; ++i)
                    {
                        res.list[i] -= carry + (i < (int)v.list.Count ? v.list[i] : 0);
                        carry = Convert.ToInt32(res.list[i] < 0);
                        if (Convert.ToBoolean(carry))
                            res.list[i] += basic;
                    }
                    res.trim();
                    return res;
                }
                return -res;
            }
            return w + (-v);
        }
        public static MyType operator /(MyType w, MyType v)
        {
            return w.divmod(w, v).Item1;
        }

        public static MyType operator %(MyType w, MyType v)
        {
            return w.divmod(w, v).Item2;
        }

        public static int operator %(MyType w, int v)
        {
            if (v < 0)
                v = -v;
            int m = 0;
            for (int i = w.list.Count - 1; i >= 0; --i)
                m = (int)((w.list[i] + m * (long)basic) % v);
            return m * w.number;
        }
        public MyType DzielRow(MyType w, int v)
        {
            if (v < 0) { w.number = -w.number; v = -v; }
            for (int i = w.list.Count - 1, rem = 0; i >= 0; --i)
            {
                long cur = w.list[i] + rem * (long)basic;
                w.list[i] = (int)(cur / v);
                rem = (int)(cur % v);
            }
            w.trim();
            return w;
        }

        public static MyType operator /(int v, MyType w)
        {
            MyType res = w;
            if (v < 0)
            { w.number = -w.number; v = -v; }
            for (int i = w.list.Count - 1, rem = 0; i >= 0; --i)
            {
                long cur = w.list[i] + rem * (long)basic;
                w.list[i] = (int)(cur / v);
                rem = (int)(cur % v);
            }
            w.trim();
            return res;
        }

        public static bool operator >(MyType w, MyType v)
        {
            return v < w;
        }

        public static bool operator <(MyType w, MyType v)
        {
            if (w.number != v.number)
            { return w.number < v.number; }
            if (w.list.Count != v.list.Count)
            { return w.list.Count * w.number < v.list.Count * v.number; }
            for (int i = w.list.Count - 1; i >= 0; i--)
                if (w.list[i] != v.list[i])
                { return w.list[i] * w.number < v.list[i] * w.number; }

            return false;
        }

        public static bool operator <=(MyType w, MyType v)
        {
            return !(v < w);
        }

        public static bool operator >=(MyType w, MyType v)
        {
            return !(w < v);
        }

        public static bool operator !=(MyType w, MyType v)
        {
            return w < v || v < w;
        }

        public static bool operator ==(MyType w, MyType v)
        {
            return (!(w < v) && !(v < w));
        }

        public static MyType operator -(MyType w)
        {
            MyType res = w;
            res.number = -w.number;
            return res;
        }
        public string operatormniejszy(string chain, MyType v)
        {
            if (v.number == -1)
                chain += '-';
            if (v.list.Count == 0) chain += 0;
            else chain += v.list[v.list.Count - 1];
            for (int i = (int)v.list.Count - 2; i >= 0; --i)
                chain += v.list[i];
            return chain;
        }
        public static MyType operator *(MyType w, MyType v)
        {
            List<int> a6 = new List<int>();
            MessageBox.Show("w ma wielkosc", w.list.Count.ToString());
            a6 = w.convert_base(w.list, basic_digits, 6);
            MessageBox.Show("A6 ma wielkosc", a6.Count.ToString());
            List<int> b6 = new List<int>();
            b6 = w.convert_base(v.list, basic_digits, 6);
            List<long> a = new List<long>();
            for (int i = 0; i <= a6[0]; i++)
            {
                a.Add(a6[a6.Count - 1]);
            }
            List<long> b = new List<long>();
            for (int i = 0; i <= b6[0]; i++)
            {
                a.Add(b6[b6.Count - 1]);
            }
            while (a.Count < b.Count)
                a.Add(0);
            while (b.Count < a.Count)
                b.Add(0);
            while (a.Count != 0)
            { a.Add(0); b.Add(0); }
            List<long> c = new List<long>();
            c = w.karatsubaMultiply(a, b);
            MyType res = new MyType(w.number * v.number);
            for (int i = 0, carry = 0; i < (int)c.Count; i++)
            {
                long cur = c[i] + carry;
                res.list.Add((int)(cur % 1000000));
                carry = (int)(cur / 1000000);
            }
            res.list = w.convert_base(res.list, 6, basic_digits);
            res.trim();
            return res;
        }
        public string to_string()
        {
            string ss = this.ToString();
            return ss;
        }
        public int size()
        {
            if (list.Count == 0) return 0;
            int ans = (list.Count - 1) * basic_digits;
            int ca = list[list.Count - 1];
            while (ca != 0)
            { ans++; ca /= 10; }
            return ans;
        }
        public void trim()
        {

            while (list.Count != 0)
                list.Remove(list[list.Count-1]);
            if (list.Count == 0)
                number = 1;
        }
        public bool isZero()
        {
            return list.Count == 0 || (list.Count == 1);
        }
        public void read(string s)
        {
            number = 1;
            list.Clear();
            int pos = 0;
            while (pos < s.Length && (s[pos] == '-' || s[pos] == '+'))
            {
                if (s[pos] == '-')
                    number = -number;
                ++pos;
            }
            for (int i = s.Length - 1; i >= pos; i -= basic_digits)
            {
                int x = 0;
                for (int j = Math.Max(pos, i - basic_digits + 1); j <= i; j++)
                    x = x * 10 + s[j] - '0';
                list.Add(x);
            }
            trim();
        }
        public List<int> convert_base(List<int> l, int old_digits, int new_digits)
        {
            List<long> p = new List<long>();//(Math.Max(old_digits, new_digits) + 1);
            for(int k=0; k< Math.Max(old_digits, new_digits) + 1; k++)
            {
                p.Add(1);
            }
            for (int i = 1; i < p.Count; i++)
                p[i] = p[i - 1] * 10;

            List<int> res = new List<int>();
            long cur = 0;
            int cur_digits = 0;
                MessageBox.Show("l count", l.Count.ToString());

            for (int i = 0; i < l.Count; i++)
            {
                cur += l[i] * p[cur_digits];
                cur_digits += old_digits;
                MessageBox.Show("cur dif", cur_digits.ToString());
                while (cur_digits >= new_digits)
                {
                    res.Add((int)(cur % p[new_digits]));
                    cur /= p[new_digits];
                    cur_digits -= new_digits;
                }
            }
            res.Add((int)cur);
            MessageBox.Show("rozmiar res", res.Count.ToString()); 
            //while (res.Count != 0 && res[res.Count-1]==0)
            //    res.Remove(res[res.Count - 1]);
MessageBox.Show("rozmiar res ost", res.Count.ToString());
            return res;
        }
        MyType abs()
        {
            MyType res = new MyType();
            res = this;
            res.number = res.number * res.number;
            return res;
        }
        public int getBasic()
        {
            return basic;
        }
        public MyType mnozenierowneMyType (MyType w, MyType v)
        {
            w = w * v;
            return w;
        }
        public MyType mnozenieirowne(MyType w, int v)
        {
            if (v < 0)
            { w.number = -w.number; v = -v; }
            for (int i = 0, carry = 0; (i < (w.list.Count) || carry != 0); ++i)
            {
                if (i == w.list.Count)
                   { w.list.Add(0); MessageBox.Show("jestem w mnoz"); }

                long cur = w.list[i] * (long)v + carry;
                carry = (int)(cur / w.getBasic());
                w.list[i] = (int)(cur % w.getBasic());
            }

            w.trim();
            return w;
        }

        public static MyType operator *(MyType w, int v)
        {
            MyType res = new MyType();
            res = w;
            res = res.mnozenieirowne(res, v);
            return res;
        }

        public MyType PlusmMnoz(MyType w, MyType v)
        {
            w = w + v;
            return w;
        }
        public MyType PlusmMnoz(MyType w, int i)
        {
            w.number += i;
            return w;
        }
        public MyType rowne(MyType w, MyType v)
        {
            w.number = v.number;
            w.list = v.list;
            return w;
        }

        Tuple<MyType, MyType> divmod(MyType a1, MyType b1)
        {
            int norm = basic / (b1.list[b1.list.Count - 1] + 1);
            MyType a = new MyType();
            a = a1.abs() * norm;
            MyType b = new MyType();
            b = b1.abs() * norm;

            int pom = 1;
            MyType q = new MyType(pom); MyType r = new MyType(pom);
            for(int l = 0; l < a.list.Count; l++)
            {
                q.list.Add(0);
            }
            for (int i = a.list.Count - 1; i >= 0; i--)
            {
                r = r.mnozenieirowne(r, basic);
                r = r.PlusmMnoz(r, a.list[i]);
                int s1 = r.list.Count <= b.list.Count ? 0 : r.list[b.list.Count];
                int s2 = r.list.Count <= b.list.Count - 1 ? 0 : r.list[b.list.Count - 1];
                int d = ((basic * s1 + s2)) / (b.list[b.list.Count]);
                r -= b * d;
                while (r.number < 0)
                { r += b; --d; }
                q.list[i] = d;
            }
            q.number = a1.number * b1.number;
            r.number = a1.number;
            q.trim();
            r.trim();
            r.number = r.number / norm;
            Tuple<MyType, MyType> par = new Tuple<MyType, MyType>(q, r);
            return par;
        }
        List<long> karatsubaMultiply(List<long> a, List<long> b)
        {
            int n = a.Count;
            List<long> res = new List<long>(n + n);
            if (n <= 32)
            {
                for (int i = 0; i < n; i++)
                    for (int j = 0; j < n; j++)
                        res[i + j] += a[i] * b[j];
                return res;
            }

            int k = n >> 1;
            List<long> a1 = new List<long>();
            for (int i = 0; i <= a[0]; i++)
            {
                a.Add(a[0] + k);
            }
            List<long> a2 = new List<long>();
            for (int i = 0; i <= a[0] + k; i++)
            {
                a.Add(a[a.Count - 1]);
            }
            List<long> b1 = new List<long>();
            for (int i = 0; i <= b[0]; i++)
            {
                a.Add(b[0] + k);
            }
            List<long> b2 = new List<long>();
            for (int i = 0; i <= b[0] + k; i++)
            {
                a.Add(b[b.Count + 1]);
            }
            List<long> a1b1 = karatsubaMultiply(a1, b1);
            List<long> a2b2 = karatsubaMultiply(a2, b2);

            for (int i = 0; i < k; i++)
                a2[i] += a1[i];
            for (int i = 0; i < k; i++)
                b2[i] += b1[i];

            List<long> r = karatsubaMultiply(a2, b2);
            for (int i = 0; i < (int)a1b1.Count; i++)
                r[i] -= a1b1[i];
            for (int i = 0; i < (int)a2b2.Count; i++)
                r[i] -= a2b2[i];

            for (int i = 0; i < (int)r.Count; i++)
                res[i + k] += r[i];
            for (int i = 0; i < (int)a1b1.Count; i++)
                res[i] += a1b1[i];
            for (int i = 0; i < (int)a2b2.Count; i++)
                res[i + n] += a2b2[i];
            return res;
        }






    }
}
