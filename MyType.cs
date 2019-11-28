using System;
using System.Collections.Generic;
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
            //nie wiadomo
        }

        public static MyType operator +(MyType w, MyType v) {
        if (w.number == v.number) {
            MyType res = v;
            for (int i = 0, carry = 0; i<(int) Math.Min(Math.Max(w.list.Count, v.list.Count),carry); ++i) {
                if (i == (int) res.list.Count)
                    res.list.Add(0);
                res.list[i] += carry + (i<(int) w.list.Count ? w.list[i] : 0);
                carry = Convert.ToInt32((res.list[i] >= basic));
                if (Convert.ToBoolean(carry))
                    res.list[i] -= basic;
    }
            return res;
        }
        return w - (-v);
}
    }
}
