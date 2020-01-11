using System;
using System.Collections.Generic;
using System.Text;

namespace Kod
{
    using unsignIn = System.UInt32;
    public class MyBigType
    {
        private ArrayForDigits m_digits;

        public MyBigType()
        {
            m_digits = new ArrayForDigits(1, 1);
        }

        public MyBigType(long number)
        {
            m_digits = new ArrayForDigits((8 / ArrayForDigits.DataSizeOf) + 1, 0);
            while (number != 0 && m_digits.DataUsed < m_digits.Count)
            {
                m_digits[m_digits.DataUsed] = (unsignIn)(number & ArrayForDigits.AllBits);
                number >>= ArrayForDigits.DataSizeBits;
                m_digits.DataUsed++;
            }
            m_digits.ResetDataUsed();
        }

        public MyBigType(MyBigType m)
        {
            m_digits = m.m_digits;
        }

        public MyBigType(ulong number)
        {
            m_digits = new ArrayForDigits((8 / ArrayForDigits.DataSizeOf) + 1, 0);
            while (number != 0 && m_digits.DataUsed < m_digits.Count)
            {
                m_digits[m_digits.DataUsed] = (unsignIn)(number & ArrayForDigits.AllBits);
                number >>= ArrayForDigits.DataSizeBits;
                m_digits.DataUsed++;
            }
            m_digits.ResetDataUsed();
        }

        public MyBigType(byte[] array)
        {
            ConstructFrom(array, 0, array.Length);
        }

        // Creates a BigInteger initialized from the byte array ending at <paramref name="length" />.
        // <param name="array">A byte array.</param>
        // <param name="length">Int number of bytes to use.</param>
        public MyBigType(byte[] array, int length)
        {
            ConstructFrom(array, 0, length);
        }

        // Creates a BigInteger initialized from <paramref name="length" /> bytes starting at <paramref name="offset" />.
        // <param name="array">A byte array.</param>
        // <param name="offset">Int offset into the <paramref name="array" />.</param>
        // <param name="length">Int number of bytes.</param>
        public MyBigType(byte[] array, int offset, int length)
        {
            ConstructFrom(array, offset, length);
        }

        private void ConstructFrom(byte[] array, int offset, int length)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }
            if (offset > array.Length || length > array.Length)
            {
                throw new ArgumentOutOfRangeException("offset");
            }
            if (length > array.Length || (offset + length) > array.Length)
            {
                throw new ArgumentOutOfRangeException("length");
            }

            int isSize = length / 4;
            int leftOver = length & 3;
            if (leftOver != 0)
            {
                ++isSize;
            }

            m_digits = new ArrayForDigits(isSize + 1, 0); // alloc one extra since we can't init -'s from here.

            for (int i = offset + length - 1, j = 0; (i - offset) >= 3; i -= 4, j++)
            {
                m_digits[j] = (unsignIn)((array[i - 3] << 24) + (array[i - 2] << 16) + (array[i - 1] << 8) + array[i]);
                m_digits.DataUsed++;
            }

            unsignIn accumulator = 0;
            for (int i = leftOver; i > 0; i--)
            {
                unsignIn digit = array[offset + leftOver - i];
                digit = (digit << ((i - 1) * 8));
                accumulator |= digit;
            }
            m_digits[m_digits.DataUsed] = accumulator;

            m_digits.ResetDataUsed();
        }

        public MyBigType(string digits)
        {
            Construct(digits, 10);
        }


        public MyBigType(string digits, int radix)
        {
            Construct(digits, radix);
        }

        private void Construct(string digits, int radix)
        {
            if (digits == null)
            {
                throw new ArgumentNullException("digits");
            }

            MyBigType multiplier = new MyBigType(1);
            MyBigType result = new MyBigType();
            digits = digits.ToUpper(System.Globalization.CultureInfo.CurrentCulture).Trim();

            int nDigits = (digits[0] == '-' ? 1 : 0);

            for (int idx = digits.Length - 1; idx >= nDigits; idx--)
            {
                int d = (int)digits[idx];
                if (d >= '0' && d <= '9')
                {
                    d -= '0';
                }
                else if (d >= 'A' && d <= 'Z')
                {
                    d = (d - 'A') + 10;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("digits");
                }

                if (d >= radix)
                {
                    throw new ArgumentOutOfRangeException("digits");
                }
                result += (multiplier * d);
                multiplier *= radix;
            }

            if (digits[0] == '-')
            {
                result = -result;
            }

            this.m_digits = result.m_digits;
        }

        private MyBigType(ArrayForDigits digits)
        {
            digits.ResetDataUsed();
            this.m_digits = digits;
        }

        public bool IsNegative { get { return m_digits.IsNegative; } }
        public bool IsZero { get { return m_digits.IsZero; } }

        // Create BigInteger from int, uint, long, ulong;

        public static implicit operator MyBigType(long value)
        {
            return (new MyBigType(value));
        }

        public static implicit operator MyBigType(ulong value)
        {
            return (new MyBigType(value));
        }

        public static implicit operator MyBigType(int value)
        {
            return (new MyBigType((long)value));
        }

        public static implicit operator MyBigType(uint value)
        {
            return (new MyBigType((ulong)value));
        }

        // Overload operand +, add two BigIntegers

        public static MyBigType operator +(MyBigType leftSide, MyBigType rightSide)
        {
            int size = System.Math.Max(leftSide.m_digits.DataUsed, rightSide.m_digits.DataUsed);
            ArrayForDigits da = new ArrayForDigits(size + 1);

            long carry = 0;
            for (int i = 0; i < da.Count; i++)
            {
                long sum = (long)leftSide.m_digits[i] + (long)rightSide.m_digits[i] + carry;
                carry = (long)(sum >> ArrayForDigits.DataSizeBits);
                da[i] = (unsignIn)(sum & ArrayForDigits.AllBits);
            }

            return new MyBigType(da);
        }

        // <param name="leftSide">A BigInteger</param>
        // <param name="rightSide">A BigInteger</param>
        // <returns>The BigInteger result of adding <paramref name="leftSide" /> and <paramref name="rightSide" />.</returns>

        // Overload ++ operand, increments the BigInteger operand by 1
        public static MyBigType operator ++(MyBigType leftSide)
        {
            return (leftSide + 1);
        }

        public static MyBigType operator -(MyBigType leftSide, MyBigType rightSide)
        {
            int size = System.Math.Max(leftSide.m_digits.DataUsed, rightSide.m_digits.DataUsed) + 1;
            ArrayForDigits da = new ArrayForDigits(size);

            long carry = 0;
            for (int i = 0; i < da.Count; i++)
            {
                long diff = (long)leftSide.m_digits[i] - (long)rightSide.m_digits[i] - carry;
                da[i] = (unsignIn)(diff & ArrayForDigits.AllBits);
                da.DataUsed++;
                carry = ((diff < 0) ? 1 : 0);
            }
            return new MyBigType(da);
        }

        public static MyBigType Subtract(MyBigType leftSide, MyBigType rightSide)
        {
            return leftSide - rightSide;
        }

        public static MyBigType operator --(MyBigType leftSide)
        {
            return (leftSide - 1);
        }

        public static MyBigType operator -(MyBigType leftSide)
        {
            if (object.ReferenceEquals(leftSide, null))
            {
                throw new ArgumentNullException("leftSide");
            }

            if (leftSide.IsZero)
            {
                return new MyBigType(0);
            }

            ArrayForDigits da = new ArrayForDigits(leftSide.m_digits.DataUsed + 1, leftSide.m_digits.DataUsed + 1);

            for (int i = 0; i < da.Count; i++)
            {
                da[i] = (unsignIn)(~(leftSide.m_digits[i]));
            }

            bool carry = true;
            int index = 0;
            while (carry && index < da.Count)
            {
                long val = (long)da[index] + 1;
                da[index] = (unsignIn)(val & ArrayForDigits.AllBits);
                carry = ((val >> ArrayForDigits.DataSizeBits) > 0);
                index++;
            }

            return new MyBigType(da);
        }

        public MyBigType Negate()
        {
            return -this;
        }

        public static MyBigType Abs(MyBigType leftSide)
        {
            if (object.ReferenceEquals(leftSide, null))
            {
                throw new ArgumentNullException("leftSide");
            }
            if (leftSide.IsNegative)
            {
                return -leftSide;
            }
            return leftSide;
        }

        public static MyBigType operator *(MyBigType leftSide, MyBigType rightSide)
        {
            if (object.ReferenceEquals(leftSide, null))
            {
                throw new ArgumentNullException("leftSide");
            }
            if (object.ReferenceEquals(rightSide, null))
            {
                throw new ArgumentNullException("rightSide");
            }

            bool leftSideNeg = leftSide.IsNegative;
            bool rightSideNeg = rightSide.IsNegative;

            leftSide = Abs(leftSide);
            rightSide = Abs(rightSide);

            ArrayForDigits da = new ArrayForDigits(leftSide.m_digits.DataUsed + rightSide.m_digits.DataUsed);
            da.DataUsed = da.Count;

            for (int i = 0; i < leftSide.m_digits.DataUsed; i++)
            {
                ulong carry = 0;
                for (int j = 0, k = i; j < rightSide.m_digits.DataUsed; j++, k++)
                {
                    ulong val = ((ulong)leftSide.m_digits[i] * (ulong)rightSide.m_digits[j]) + (ulong)da[k] + carry;

                    da[k] = (unsignIn)(val & ArrayForDigits.AllBits);
                    carry = (val >> ArrayForDigits.DataSizeBits);
                }

                if (carry != 0)
                {
                    da[i + rightSide.m_digits.DataUsed] = (unsignIn)carry;
                }
            }

            MyBigType result = new MyBigType(da);
            return (leftSideNeg != rightSideNeg ? -result : result);
        }

        public static MyBigType operator /(MyBigType leftSide, MyBigType rightSide)
        {
            if (leftSide == null)
            {
                throw new ArgumentNullException("leftSide");
            }
            if (rightSide == null)
            {
                throw new ArgumentNullException("rightSide");
            }

            if (rightSide.IsZero)
            {
                throw new DivideByZeroException();
            }

            bool divisorNeg = rightSide.IsNegative;
            bool dividendNeg = leftSide.IsNegative;

            leftSide = Abs(leftSide);
            rightSide = Abs(rightSide);

            if (leftSide < rightSide)
            {
                return new MyBigType(0);
            }

            MyBigType quotient;
            MyBigType remainder;
            Divide(leftSide, rightSide, out quotient, out remainder);

            return (dividendNeg != divisorNeg ? -quotient : quotient);
        }

        private static void Divide(MyBigType leftSide, MyBigType rightSide, out MyBigType quotient, out MyBigType remainder)
        {
            if (leftSide.IsZero)
            {
                quotient = new MyBigType();
                remainder = new MyBigType();
                return;
            }

            if (rightSide.m_digits.DataUsed == 1)
            {
                SingleDivide(leftSide, rightSide, out quotient, out remainder);
            }
            else
            {
                MultiDivide(leftSide, rightSide, out quotient, out remainder);
            }
        }

        private static void MultiDivide(MyBigType leftSide, MyBigType rightSide, out MyBigType quotient, out MyBigType remainder)
        {
            if (rightSide.IsZero)
            {
                throw new DivideByZeroException();
            }

            unsignIn val = rightSide.m_digits[rightSide.m_digits.DataUsed - 1];
            int d = 0;
            for (uint mask = ArrayForDigits.HiBitSet; mask != 0 && (val & mask) == 0; mask >>= 1)
            {
                d++;
            }

            int remainderLen = leftSide.m_digits.DataUsed + 1;
            unsignIn[] remainderDat = new unsignIn[remainderLen];
            leftSide.m_digits.CopyTo(remainderDat, 0, leftSide.m_digits.DataUsed);

            ArrayForDigits.ShiftLeft(remainderDat, d);
            rightSide = rightSide << d;

            ulong firstDivisor = rightSide.m_digits[rightSide.m_digits.DataUsed - 1];
            ulong secondDivisor = (rightSide.m_digits.DataUsed < 2 ? (unsignIn)0 : rightSide.m_digits[rightSide.m_digits.DataUsed - 2]);

            int divisorLen = rightSide.m_digits.DataUsed + 1;
            ArrayForDigits dividendPart = new ArrayForDigits(divisorLen, divisorLen);
            unsignIn[] result = new unsignIn[leftSide.m_digits.Count + 1];
            int resultPos = 0;

            ulong carryBit = (ulong)0x1 << ArrayForDigits.DataSizeBits; // 0x100000000
            for (int j = remainderLen - rightSide.m_digits.DataUsed, pos = remainderLen - 1; j > 0; j--, pos--)
            {
                ulong dividend = ((ulong)remainderDat[pos] << ArrayForDigits.DataSizeBits) + (ulong)remainderDat[pos - 1];
                ulong qHat = (dividend / firstDivisor);
                ulong rHat = (dividend % firstDivisor);

                while (pos >= 2)
                {
                    if (qHat == carryBit || (qHat * secondDivisor) > ((rHat << ArrayForDigits.DataSizeBits) + remainderDat[pos - 2]))
                    {
                        qHat--;
                        rHat += firstDivisor;
                        if (rHat < carryBit)
                        {
                            continue;
                        }
                    }
                    break;
                }

                for (int h = 0; h < divisorLen; h++)
                {
                    dividendPart[divisorLen - h - 1] = remainderDat[pos - h];
                }

                MyBigType dTemp = new MyBigType(dividendPart);
                MyBigType rTemp = rightSide * (long)qHat;
                while (rTemp > dTemp)
                {
                    qHat--;
                    rTemp -= rightSide;
                }

                rTemp = dTemp - rTemp;
                for (int h = 0; h < divisorLen; h++)
                {
                    remainderDat[pos - h] = rTemp.m_digits[rightSide.m_digits.DataUsed - h];
                }

                result[resultPos++] = (unsignIn)qHat;
            }

            Array.Reverse(result, 0, resultPos);
            quotient = new MyBigType(new ArrayForDigits(result));

            int n = ArrayForDigits.ShiftRight(remainderDat, d);
            ArrayForDigits rDA = new ArrayForDigits(n, n);
            rDA.CopyFrom(remainderDat, 0, 0, rDA.DataUsed);
            remainder = new MyBigType(rDA);
        }

        private static void SingleDivide(MyBigType leftSide, MyBigType rightSide, out MyBigType quotient, out MyBigType remainder)
        {
            if (rightSide.IsZero)
            {
                throw new DivideByZeroException();
            }

            ArrayForDigits remainderDigits = new ArrayForDigits(leftSide.m_digits);
            remainderDigits.ResetDataUsed();

            int pos = remainderDigits.DataUsed - 1;
            ulong divisor = (ulong)rightSide.m_digits[0];
            ulong dividend = (ulong)remainderDigits[pos];

            unsignIn[] result = new unsignIn[leftSide.m_digits.Count];
            leftSide.m_digits.CopyTo(result, 0, result.Length);
            int resultPos = 0;

            if (dividend >= divisor)
            {
                result[resultPos++] = (unsignIn)(dividend / divisor);
                remainderDigits[pos] = (unsignIn)(dividend % divisor);
            }
            pos--;

            while (pos >= 0)
            {
                dividend = ((ulong)(remainderDigits[pos + 1]) << ArrayForDigits.DataSizeBits) + (ulong)remainderDigits[pos];
                result[resultPos++] = (unsignIn)(dividend / divisor);
                remainderDigits[pos + 1] = 0;
                remainderDigits[pos--] = (unsignIn)(dividend % divisor);
            }
            remainder = new MyBigType(remainderDigits);

            ArrayForDigits quotientDigits = new ArrayForDigits(resultPos + 1, resultPos);
            int j = 0;
            for (int i = quotientDigits.DataUsed - 1; i >= 0; i--, j++)
            {
                quotientDigits[j] = result[i];
            }
            quotient = new MyBigType(quotientDigits);
        }

        public static MyBigType operator %(MyBigType leftSide, MyBigType rightSide)
        {
            if (leftSide == null)
            {
                throw new ArgumentNullException("leftSide");
            }

            if (rightSide == null)
            {
                throw new ArgumentNullException("rightSide");
            }

            if (rightSide.IsZero)
            {
                throw new DivideByZeroException();
            }

            MyBigType quotient;
            MyBigType remainder;

            bool dividendNeg = leftSide.IsNegative;
            leftSide = Abs(leftSide);
            rightSide = Abs(rightSide);

            if (leftSide < rightSide)
            {
                return leftSide;
            }

            Divide(leftSide, rightSide, out quotient, out remainder);

            return (dividendNeg ? -remainder : remainder);
        }

        public MyBigType findPrime(MyBigType a)
        {
            bool isPrime = true;
            MyBigType i = new MyBigType(2);
            for (; ; a++)
            {
                while (i < a && isPrime == true)
                {
                    if (a % i == 0) { isPrime = false; }
                    i++;
                }
                i = 2;
                isPrime = true;
            }
        }

        public MyBigType Pow(MyBigType power)
        {
            return Pow(this, power);
        }

        public static MyBigType Pow(MyBigType b, MyBigType power)
        {

            if (b == null)
            {
                throw new ArgumentNullException("b");
            }

            if (power == null)
            {
                throw new ArgumentNullException("power");
            }

            if (power < 0)
            {
                throw new ArgumentOutOfRangeException("power", "Currently negative exponents are not supported");
            }


            MyBigType result = 1;
            while (power != 0)
            {

                if ((power & 1) != 0)
                    result *= b;
                power >>= 1;
                b *= b;
            }

            return result;
        }


        public static MyBigType operator &(MyBigType leftSide, MyBigType rightSide)
        {
            int len = System.Math.Max(leftSide.m_digits.DataUsed, rightSide.m_digits.DataUsed);
            ArrayForDigits da = new ArrayForDigits(len, len);
            for (int idx = 0; idx < len; idx++)
            {
                da[idx] = (unsignIn)(leftSide.m_digits[idx] & rightSide.m_digits[idx]);
            }
            return new MyBigType(da);
        }

        public static MyBigType operator |(MyBigType leftSide, MyBigType rightSide)
        {
            int len = System.Math.Max(leftSide.m_digits.DataUsed, rightSide.m_digits.DataUsed);
            ArrayForDigits da = new ArrayForDigits(len, len);
            for (int idx = 0; idx < len; idx++)
            {
                da[idx] = (unsignIn)(leftSide.m_digits[idx] | rightSide.m_digits[idx]);
            }
            return new MyBigType(da);
        }

        public static MyBigType operator ^(MyBigType leftSide, MyBigType rightSide)
        {
            int len = System.Math.Max(leftSide.m_digits.DataUsed, rightSide.m_digits.DataUsed);
            ArrayForDigits da = new ArrayForDigits(len, len);
            for (int idx = 0; idx < len; idx++)
            {
                da[idx] = (unsignIn)(leftSide.m_digits[idx] ^ rightSide.m_digits[idx]);
            }
            return new MyBigType(da);
        }

        public static MyBigType operator ~(MyBigType leftSide)
        {
            ArrayForDigits da = new ArrayForDigits(leftSide.m_digits.Count);
            for (int idx = 0; idx < da.Count; idx++)
            {
                da[idx] = (unsignIn)(~(leftSide.m_digits[idx]));
            }

            return new MyBigType(da);
        }

        public static MyBigType operator <<(MyBigType leftSide, int shiftCount)
        {
            if (leftSide == null)
            {
                throw new ArgumentNullException("leftSide");
            }

            ArrayForDigits da = new ArrayForDigits(leftSide.m_digits);
            da.DataUsed = da.ShiftLeftWithoutOverflow(shiftCount);

            return new MyBigType(da);
        }

        public static MyBigType operator >>(MyBigType leftSide, int shiftCount)
        {
            if (leftSide == null)
            {
                throw new ArgumentNullException("leftSide");
            }

            ArrayForDigits da = new ArrayForDigits(leftSide.m_digits);
            da.DataUsed = da.ShiftRight(shiftCount);

            if (leftSide.IsNegative)
            {
                for (int i = da.Count - 1; i >= da.DataUsed; i--)
                {
                    da[i] = ArrayForDigits.AllBits;
                }

                unsignIn mask = ArrayForDigits.HiBitSet;
                for (int i = 0; i < ArrayForDigits.DataSizeBits; i++)
                {
                    if ((da[da.DataUsed - 1] & mask) == ArrayForDigits.HiBitSet)
                    {
                        break;
                    }
                    da[da.DataUsed - 1] |= mask;
                    mask >>= 1;
                }
                da.DataUsed = da.Count;
            }

            return new MyBigType(da);
        }




        //// Returns true if n is prime 
        //public static bool isPrime(MyBigType n)
        //{
        //    // Corner cases 
        //    if (n <= 1)
        //    {
        //        return false;
        //    }
        //    if (n <= 3)
        //    {
        //        return true;
        //    }

        //    // This is checked so that we can skip 
        //    // middle five numbers in below loop 
        //    if (n % 2 == 0 || n % 3 == 0)
        //    {
        //        return false;
        //    }

        //    for (MyBigType i = 5; i * i <= n; i = i + 6)
        //    {
        //        if (n % i == 0 || n % (i + 2) == 0)
        //        {
        //            return false;
        //        }
        //    }

        //    return true;
        //}

        ///* Iterative Function to calculate (x^n)%p in 
        //O(logy) */
        //public static MyBigType power(MyBigType x, MyBigType y, MyBigType p)
        //{
        //    MyBigType res = 1;     // Initialize result 

        //    x = x % p; // Update x if it is more than or 
        //               // equal to p 

        //    while (y > 0)
        //    {
        //        // If y is odd, multiply x with result 
        //        if (y % 2 == 1)
        //        {
        //            res = (res * x) % p;
        //        }

        //        // y must be even now 
        //        y = y >> 1; // y = y/2 
        //        x = (x * x) % p;
        //    }
        //    return res;
        //}

        //// Utility function to store prime factors of a number 
        //public static void findPrimefactors(HashSet<MyBigType> s, MyBigType n)
        //{
        //    // Print the number of 2s that divide n 
        //    while (n % 2 == 0)
        //    {
        //        s.Add(2);
        //        n = n / 2;
        //    }

        //    // n must be odd at this point. So we can skip 
        //    // one element (Note i = i +2) 
        //    for (int i = 3; i <= n.Pow(2); i = i + 2)
        //    {
        //        // While i divides n, print i and divide n 
        //        while (n % i == 0)
        //        {
        //            s.Add(i);
        //            n = n / i;
        //        }
        //    }

        //    // This condition is to handle the case when 
        //    // n is a prime number greater than 2 
        //    if (n > 2)
        //    {
        //        s.Add(n);
        //    }
        //}

        //// Function to find smallest primitive root of n 
        //public static MyBigType findPrimitive(MyBigType n)
        //{
        //    HashSet<MyBigType> s = new HashSet<MyBigType>();

        //    // Check if n is prime or not 
        //    if (isPrime(n) == false)
        //    {
        //        return -1;
        //    }

        //    // Find value of Euler Totient function of n 
        //    // Since n is a prime number, the value of Euler 
        //    // Totient function is n-1 as there are n-1 
        //    // relatively prime numbers. 
        //    MyBigType phi = n - 1;

        //    // Find prime factors of phi and store in a set 
        //    findPrimefactors(s, phi);

        //    // Check for every number from 2 to phi 
        //    for (int r = 2; r <= phi; r++)
        //    {
        //        // Iterate through all prime factors of phi. 
        //        // and check if we found a power with value 1 
        //        bool flag = false;
        //        foreach (MyBigType a in s)
        //        {

        //            // Check if r^((phi)/primefactors) mod n 
        //            // is 1 or not 
        //            if (power(r, phi / (a), n) == 1)
        //            {
        //                flag = true;
        //                break;
        //            }
        //        }

        //        // If there was no power with value 1. 
        //        if (flag == false)
        //        {
        //            return r;
        //        }
        //    }

        //    // If no primitive root found 
        //    return -1;
        //}






        public static MyBigType RightShift(MyBigType leftSide, int shiftCount)
        {
            if (leftSide == null)
            {
                throw new ArgumentNullException("leftSide");
            }

            return leftSide >> shiftCount;
        }

        public int CompareTo(MyBigType value)
        {
            return Compare(this, value);
        }


        public static int Compare(MyBigType leftSide, MyBigType rightSide)
        {
            if (object.ReferenceEquals(leftSide, rightSide))
            {
                return 0;
            }

            if (object.ReferenceEquals(leftSide, null))
            {
                throw new ArgumentNullException("leftSide");
            }

            if (object.ReferenceEquals(rightSide, null))
            {
                throw new ArgumentNullException("rightSide");
            }

            if (leftSide > rightSide) return 1;
            if (leftSide == rightSide) return 0;
            return -1;
        }

        public static bool operator ==(MyBigType leftSide, MyBigType rightSide)
        {
            if (object.ReferenceEquals(leftSide, rightSide))
            {
                return true;
            }

            if (object.ReferenceEquals(leftSide, null) || object.ReferenceEquals(rightSide, null))
            {
                return false;
            }

            if (leftSide.IsNegative != rightSide.IsNegative)
            {
                return false;
            }

            return leftSide.Equals(rightSide);
        }

        public static bool operator !=(MyBigType leftSide, MyBigType rightSide)
        {
            return !(leftSide == rightSide);
        }

        public static bool operator >(MyBigType leftSide, MyBigType rightSide)
        {
            if (object.ReferenceEquals(leftSide, null))
            {
                throw new ArgumentNullException("leftSide");
            }

            if (object.ReferenceEquals(rightSide, null))
            {
                throw new ArgumentNullException("rightSide");
            }

            if (leftSide.IsNegative != rightSide.IsNegative)
            {
                return rightSide.IsNegative;
            }

            if (leftSide.m_digits.DataUsed != rightSide.m_digits.DataUsed)
            {
                return leftSide.m_digits.DataUsed > rightSide.m_digits.DataUsed;
            }

            for (int idx = leftSide.m_digits.DataUsed - 1; idx >= 0; idx--)
            {
                if (leftSide.m_digits[idx] != rightSide.m_digits[idx])
                {
                    return (leftSide.m_digits[idx] > rightSide.m_digits[idx]);
                }
            }
            return false;
        }

        public static bool operator <(MyBigType leftSide, MyBigType rightSide)
        {
            if (object.ReferenceEquals(leftSide, null))
            {
                throw new ArgumentNullException("leftSide");
            }

            if (object.ReferenceEquals(rightSide, null))
            {
                throw new ArgumentNullException("rightSide");
            }

            if (leftSide.IsNegative != rightSide.IsNegative)
            {
                return leftSide.IsNegative;
            }

            if (leftSide.m_digits.DataUsed != rightSide.m_digits.DataUsed)
            {
                return leftSide.m_digits.DataUsed < rightSide.m_digits.DataUsed;
            }

            for (int idx = leftSide.m_digits.DataUsed - 1; idx >= 0; idx--)
            {
                if (leftSide.m_digits[idx] != rightSide.m_digits[idx])
                {
                    return (leftSide.m_digits[idx] < rightSide.m_digits[idx]);
                }
            }
            return false;
        }

        public static bool operator >=(MyBigType leftSide, MyBigType rightSide)
        {
            return Compare(leftSide, rightSide) >= 0;
        }

        public static bool operator <=(MyBigType leftSide, MyBigType rightSide)
        {
            return Compare(leftSide, rightSide) <= 0;
        }

        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals(obj, null))
            {
                return false;
            }

            if (object.ReferenceEquals(this, obj))
            {
                return true;
            }

            MyBigType c = (MyBigType)obj;
            if (this.m_digits.DataUsed != c.m_digits.DataUsed)
            {
                return false;
            }

            for (int idx = 0; idx < this.m_digits.DataUsed; idx++)
            {
                if (this.m_digits[idx] != c.m_digits[idx])
                {
                    return false;
                }
            }
            return true;
        }

        public override int GetHashCode()
        {
            return this.m_digits.GetHashCode();
        }

        public string GetDataAsString()
        {
            return this.m_digits.GetDataAsString();
        }
        public override string ToString()
        {
            return ToString(10);
        }

        public string ToString(int radix)
        {
            if (radix < 2 || radix > 36)
            {
                throw new ArgumentOutOfRangeException("radix");
            }

            if (IsZero)
            {
                return "0";
            }

            MyBigType a = this;
            bool negative = a.IsNegative;
            a = Abs(this);

            MyBigType quotient;
            MyBigType remainder;
            MyBigType biRadix = new MyBigType(radix);

            const string charSet = "0123456789abcdefghijklmnopqrstuvwxyz";
            System.Collections.ArrayList al = new System.Collections.ArrayList();
            while (a.m_digits.DataUsed > 1 || (a.m_digits.DataUsed == 1 && a.m_digits[0] != 0))
            {
                Divide(a, biRadix, out quotient, out remainder);
                al.Insert(0, charSet[(int)remainder.m_digits[0]]);
                a = quotient;
            }

            string result = new String((char[])al.ToArray(typeof(char)));
            if (radix == 10 && negative)
            {
                return "-" + result;
            }
            return result;
        }
    }
}
