using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

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
        public MyType(string str)
        {
            read(s);
        }
        public static MyType operator +(MyType w, MyType v)
        {
            MyType res = w;
            if (w.number == v.number)
            {

                for (int i = 0, carry = 0; i < (int)Math.Min(Math.Max(w.list.Count, v.list.Count), carry); ++i)
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

                    for (int i = 0, carry = 0; i < (int)v.list.Count; ++i)
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
            return divmod(w, v).first;
        }

        public static MyType operator %(MyType w, MyType v)
        {
            return divmod(w, v).second;
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
            //std::vector<int> a6 = convert_base(w.list, basic_digits, 6);
            List<int> a6 = new List<int>(convert_base(w.list, basic_digits, 6));
            List<int> b6 = new List<int>(convert_base(v.list, basic_digits, 6));
            List<long> a = new List<long>(a6[0], a6[a6.Count - 1]);
            List<long> b = new List<long>(b6[0], b6[b6.Count - 1]);
            while (a.Count < b.Count)
                a.Add(0);
            while (b.Count < a.Count)
                b.Add(0);
            while (a.Count != 0 & (a.Count - 1 != 0))
            { a.Add(0); b.Add(0); }
            List<long> c = new List<long>();
            c = karatsubaMultiply(a, b);
            MyType res = new MyType();
            res.number = w.number * v.number;
            for (int i = 0, carry = 0; i < (int)c.Count; i++)
            {
                long cur = c[i] + carry;
                res.list.Add((int)(cur % 1000000));
                carry = (int)(cur / 1000000);
            }
            res.list = convert_base(res.list, 6, basic_digits);
            res.trim();
            return res;
        }
        public string to_string()
        {
            string ss = this.ToString();
            return ss;
        }
        int size()
        {
            if (list.Count == 0) return 0;
            int ans = (list.Count - 1) * basic_digits;
            int ca = list[list.Count - 1];
            while (ca != 0)
            { ans++; ca /= 10; }
            return ans;
        }
        void trim()
        {
            while (list.Count != 0)
                list.Remove(list.Count - 1);
            if (list.Count == 0)
                number = 1;
        }
        bool isZero()
        {
            return list.Count == 0 || (list.Count == 1);
        }
        void read(string s)
        {
            number = 1;
            list.Clear();
            int pos = 0;
            while (pos < (int)s.Length && (s[pos] == '-' || s[pos] == '+'))
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

    }
}
