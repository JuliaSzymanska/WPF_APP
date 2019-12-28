using System;
using System.Collections.Generic;

namespace Kod
{
	using DType = System.UInt32;

	#region DigitsArray
	internal class DigitsArray
	{
		internal DigitsArray(int size)
		{
			Allocate(size, 0);
		}

		internal DigitsArray(int size, int used)
		{
			Allocate(size, used);
		}

		internal DigitsArray(DType[] copyFrom)
		{
			Allocate(copyFrom.Length);
			CopyFrom(copyFrom, 0, 0, copyFrom.Length);
			ResetDataUsed();
		}

		internal DigitsArray(DigitsArray copyFrom)
		{
			Allocate(copyFrom.Count - 1, copyFrom.DataUsed);
			Array.Copy(copyFrom.m_data, 0, m_data, 0, copyFrom.Count);
		}

		private DType[] m_data;

		internal static readonly DType AllBits;		
		internal static readonly DType HiBitSet;	
		internal static int DataSizeOf
		{
			get { return sizeof(DType); }
		}

		internal static int DataSizeBits
		{
			get { return sizeof(DType) * 8; }
		}

		static DigitsArray()
		{
			unchecked
			{
				AllBits = (DType)~((DType)0);
				HiBitSet = (DType)(((DType)1) << (DataSizeBits) - 1);
			}
		}

		public void Allocate(int size)
		{
			Allocate(size, 0);
		}

		public void Allocate(int size, int used)
		{
			m_data = new DType[size + 1];
			m_dataUsed = used;
		}

		internal void CopyFrom(DType[] source, int sourceOffset, int offset, int length)
		{
			Array.Copy(source, sourceOffset, m_data, 0, length);
		}

		internal void CopyTo(DType[] array, int offset, int length)
		{
			Array.Copy(m_data, 0, array, offset, length);
		}

		internal DType this[int index]
		{
			get
			{
				if (index < m_dataUsed) return m_data[index];
				return (IsNegative ? (DType)AllBits : (DType)0);
			}
			set { m_data[index] = value; }
		}

		private int m_dataUsed;
		internal int DataUsed
		{
			get { return m_dataUsed; }
			set { m_dataUsed = value; }
		}

		internal int Count
		{
			get { return m_data.Length; }
		}

		internal bool IsZero
		{
			get { return m_dataUsed == 0 || (m_dataUsed == 1 && m_data[0] == 0); }
		}

		internal bool IsNegative
		{
			get { return (m_data[m_data.Length - 1] & HiBitSet) == HiBitSet; }
		}

		internal string GetDataAsString()
		{
			string result = "";
			foreach (DType data in m_data) {
				result += data + " ";
			}
			return result;
		}

		internal void ResetDataUsed()
		{
			m_dataUsed = m_data.Length;
			if (IsNegative)
			{
				while (m_dataUsed > 1 && m_data[m_dataUsed - 1] == AllBits)
				{
					--m_dataUsed;
				}
				m_dataUsed++;
			}
			else
			{
				while (m_dataUsed > 1 && m_data[m_dataUsed - 1] == 0)
				{
					--m_dataUsed;
				}
				if (m_dataUsed == 0)
				{
					m_dataUsed = 1;
				}
			}
		}

		internal int ShiftRight(int shiftCount)
		{
			return ShiftRight(m_data, shiftCount);
		}

		internal static int ShiftRight(DType[] buffer, int shiftCount)
		{
			int shiftAmount = DigitsArray.DataSizeBits;
			int invShift = 0;
			int bufLen = buffer.Length;

			while (bufLen > 1 && buffer[bufLen - 1] == 0)
			{
				bufLen--;
			}

			for (int count = shiftCount; count > 0; count -= shiftAmount)
			{
				if (count < shiftAmount)
				{
					shiftAmount = count;
					invShift = DigitsArray.DataSizeBits - shiftAmount;
				}

				ulong carry = 0;
				for (int i = bufLen - 1; i >= 0; i--)
				{
					ulong val = ((ulong)buffer[i]) >> shiftAmount;
					val |= carry;

					carry = ((ulong)buffer[i]) << invShift;
					buffer[i] = (DType)(val);
				}
			}

			while (bufLen > 1 && buffer[bufLen - 1] == 0)
			{
				bufLen--;
			}

			return bufLen;
		}

		internal int ShiftLeft(int shiftCount)
		{
			return ShiftLeft(m_data, shiftCount);
		}

		internal static int ShiftLeft(DType[] buffer, int shiftCount)
		{
			int shiftAmount = DigitsArray.DataSizeBits;
			int bufLen = buffer.Length;

			while (bufLen > 1 && buffer[bufLen - 1] == 0)
			{
				bufLen--;
			}

			for (int count = shiftCount; count > 0; count -= shiftAmount)
			{
				if (count < shiftAmount)
				{
					shiftAmount = count;
				}

				ulong carry = 0;
				for (int i = 0; i < bufLen; i++)
				{
					ulong val = ((ulong)buffer[i]) << shiftAmount;
					val |= carry;

					buffer[i] = (DType)(val & DigitsArray.AllBits);
					carry = (val >> DigitsArray.DataSizeBits);
				}

				if (carry != 0)
				{
					if (bufLen + 1 <= buffer.Length)
					{
						buffer[bufLen] = (DType)carry;
						bufLen++;
						carry = 0;
					}
					else
					{
						throw new OverflowException();
					}
				}
			}
			return bufLen;
		}

		internal int ShiftLeftWithoutOverflow(int shiftCount)
		{
			if (shiftCount == 0) return m_data.Length;

			List<DType> temporary = new List<DType>(m_data);
			int shiftAmount = DigitsArray.DataSizeBits;

			for (int count = shiftCount; count > 0; count -= shiftAmount)
			{
				if (count < shiftAmount)
				{
					shiftAmount = count;
				}

				ulong carry = 0;
				for (int i = 0; i < temporary.Count; i++)
				{
					ulong val = ((ulong)temporary[i]) << shiftAmount;
					val |= carry;

					temporary[i] = (DType)(val & DigitsArray.AllBits);
					carry = (val >> DigitsArray.DataSizeBits);
				}

				if (carry != 0) 
				{
					DType lastNum = (DType)carry;
					if (IsNegative) 
					{
						int byteCount = (int)Math.Floor(Math.Log(carry, 2));
						lastNum = (0xffffffff << byteCount) | (DType)carry;
					}

					temporary.Add(lastNum);
				}
			}
			m_data = new DType[temporary.Count];
			temporary.CopyTo(m_data);
			return m_data.Length;
		}
	}
	#endregion

	/// <summary>
	/// Represents a integer of abitrary length.
	/// </summary>
	/// <remarks>
	/// <para>
	/// A BigInteger object is immutable like System.String. The object can not be modifed, and new BigInteger objects are
	/// created by using the operations of existing BigInteger objects.
	/// </para>
	/// <para>
	/// Internally a BigInteger object is an array of ? that is represents the digits of the n-place integer. Negative BigIntegers
	/// are stored internally as 1's complements, thus every BigInteger object contains 1 or more padding elements to hold the sign.
	/// </para>
	/// </remarks>
	public class BigInteger
	{
		private DigitsArray m_digits;

		public BigInteger()
		{
			m_digits = new DigitsArray(1, 1);
		}

		public BigInteger(long number)
		{
			m_digits = new DigitsArray((8 / DigitsArray.DataSizeOf) + 1, 0);
			while (number != 0 && m_digits.DataUsed < m_digits.Count)
			{
				m_digits[m_digits.DataUsed] = (DType)(number & DigitsArray.AllBits);
				number >>= DigitsArray.DataSizeBits;
				m_digits.DataUsed++;
			}
			m_digits.ResetDataUsed();
		}

		public BigInteger(ulong number)
		{
			m_digits = new DigitsArray((8 / DigitsArray.DataSizeOf) + 1, 0);
			while (number != 0 && m_digits.DataUsed < m_digits.Count)
			{
				m_digits[m_digits.DataUsed] = (DType)(number & DigitsArray.AllBits);
				number >>= DigitsArray.DataSizeBits;
				m_digits.DataUsed++;
			}
			m_digits.ResetDataUsed();
		}

        public BigInteger(byte[] array)
        {
            ConstructFrom(array, 0, array.Length);
        }

        /// <summary>
        /// Creates a BigInteger initialized from the byte array ending at <paramref name="length" />.
        /// </summary>
        /// <param name="array">A byte array.</param>
        /// <param name="length">Int number of bytes to use.</param>
        public BigInteger(byte[] array, int length)
		{
			ConstructFrom(array, 0, length);
		}

		/// <summary>
		/// Creates a BigInteger initialized from <paramref name="length" /> bytes starting at <paramref name="offset" />.
		/// </summary>
		/// <param name="array">A byte array.</param>
		/// <param name="offset">Int offset into the <paramref name="array" />.</param>
		/// <param name="length">Int number of bytes.</param>
		public BigInteger(byte[] array, int offset, int length)
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

			int estSize = length / 4;
			int leftOver = length & 3;
			if (leftOver != 0)
			{
				++estSize;
			}

			m_digits = new DigitsArray(estSize + 1, 0); // alloc one extra since we can't init -'s from here.

			for (int i = offset + length - 1, j = 0; (i - offset) >= 3; i -= 4, j++)
			{
				m_digits[j] = (DType)((array[i - 3] << 24) + (array[i - 2] << 16) + (array[i - 1] <<  8) + array[i]);
				m_digits.DataUsed++;
			}

			DType accumulator = 0;
			for (int i = leftOver; i > 0; i--)
			{
				DType digit = array[offset + leftOver - i];
				digit = (digit << ((i - 1) * 8));
				accumulator |= digit;
			}
			m_digits[m_digits.DataUsed] = accumulator;

			m_digits.ResetDataUsed();
		}

		public BigInteger(string digits)
		{
			Construct(digits, 10);
		}

		
		public BigInteger(string digits, int radix)
		{
			Construct(digits, radix);
		}

		private void Construct(string digits, int radix)
		{
			if (digits == null)
			{
				throw new ArgumentNullException("digits");
			}

			BigInteger multiplier = new BigInteger(1);
			BigInteger result = new BigInteger();
			digits = digits.ToUpper(System.Globalization.CultureInfo.CurrentCulture).Trim();

			int nDigits = (digits[0] == '-' ? 1 : 0);

			for (int idx = digits.Length - 1; idx >= nDigits ; idx--)
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

		private BigInteger(DigitsArray digits)
		{
			digits.ResetDataUsed();
			this.m_digits = digits;
		}

		public bool IsNegative { get { return m_digits.IsNegative; } }
		public bool IsZero { get { return m_digits.IsZero; } }

        // Create BigInteger from int, uint, long, ulong;

		public static implicit operator BigInteger(long value)
        {
            return (new BigInteger(value));
        }

        public static implicit operator BigInteger(ulong value)
        {
            return (new BigInteger(value));
        }

        public static implicit operator BigInteger(int value)
		{
			return (new BigInteger((long)value));
		}

		public static implicit operator BigInteger(uint value)
		{
			return (new BigInteger((ulong)value));
		}

		// Overload operand +, add two BigIntegers

        public static BigInteger operator + (BigInteger leftSide, BigInteger rightSide)
		{
			int size = System.Math.Max(leftSide.m_digits.DataUsed, rightSide.m_digits.DataUsed);
			DigitsArray da = new DigitsArray(size + 1);

			long carry = 0;
			for (int i = 0; i < da.Count; i++)
			{
				long sum = (long)leftSide.m_digits[i] + (long)rightSide.m_digits[i] + carry;
				carry  = (long)(sum >> DigitsArray.DataSizeBits);
				da[i] = (DType)(sum & DigitsArray.AllBits);
			}

			return new BigInteger(da);
		}

        /// <summary>
        /// Adds two BigIntegers and returns a new BigInteger that represents the sum.
        /// </summary>
        /// <param name="leftSide">A BigInteger</param>
        /// <param name="rightSide">A BigInteger</param>
        /// <returns>The BigInteger result of adding <paramref name="leftSide" /> and <paramref name="rightSide" />.</returns>
        //public static BigInteger Add(BigInteger leftSide, BigInteger rightSide)
        //{
        //	return leftSide + rightSide;	
        //}

        // Overload ++ operand, increments the BigInteger operand by 1
        public static BigInteger operator ++ (BigInteger leftSide)
		{
			return (leftSide + 1);
		}

		//public static BigInteger Increment(BigInteger leftSide)
		//{
		//	return (leftSide + 1);
		//}

		public static BigInteger operator - (BigInteger leftSide, BigInteger rightSide)
		{
			int size = System.Math.Max(leftSide.m_digits.DataUsed, rightSide.m_digits.DataUsed) + 1;
			DigitsArray da = new DigitsArray(size);

			long carry = 0;
			for (int i = 0; i < da.Count; i++)
			{
				long diff = (long)leftSide.m_digits[i] - (long)rightSide.m_digits[i] - carry;
				da[i] = (DType)(diff & DigitsArray.AllBits);
				da.DataUsed++;
				carry = ((diff < 0) ? 1 : 0);
			}
			return new BigInteger(da);
		}

		public static BigInteger Subtract(BigInteger leftSide, BigInteger rightSide)
		{
			return leftSide - rightSide;
		}

		public static BigInteger operator -- (BigInteger leftSide)
		{
			return (leftSide - 1);
		}

		//public static BigInteger Decrement(BigInteger leftSide)
		//{
		//	return (leftSide - 1);
		//}

		public static BigInteger operator - (BigInteger leftSide)
		{
			if (object.ReferenceEquals(leftSide, null))
			{
				throw new ArgumentNullException("leftSide");
			}

			if (leftSide.IsZero)
			{
				return new BigInteger(0);
			}

			DigitsArray da = new DigitsArray(leftSide.m_digits.DataUsed + 1, leftSide.m_digits.DataUsed + 1);

			for (int i = 0; i < da.Count; i++)
			{
				da[i] = (DType)(~(leftSide.m_digits[i]));
			}

			bool carry = true;
			int index = 0;
			while (carry && index < da.Count)
			{
				long val = (long)da[index] + 1;
				da[index] = (DType)(val & DigitsArray.AllBits);
				carry = ((val >> DigitsArray.DataSizeBits) > 0);
				index++;
			}

			return new BigInteger(da);
		}

		public BigInteger Negate()
		{
			return -this;
		}

		public static BigInteger Abs(BigInteger leftSide)
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

		public static BigInteger operator * (BigInteger leftSide, BigInteger rightSide)
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

			DigitsArray da = new DigitsArray(leftSide.m_digits.DataUsed + rightSide.m_digits.DataUsed);
			da.DataUsed = da.Count;

			for (int i = 0; i < leftSide.m_digits.DataUsed; i++)
			{
				ulong carry = 0;
				for (int j = 0, k = i; j < rightSide.m_digits.DataUsed; j++, k++)
				{
					ulong val = ((ulong)leftSide.m_digits[i] * (ulong)rightSide.m_digits[j]) + (ulong)da[k] + carry;

					da[k] = (DType)(val & DigitsArray.AllBits);
					carry = (val >> DigitsArray.DataSizeBits);
				}

				if (carry != 0)
				{
					da[i + rightSide.m_digits.DataUsed] = (DType)carry;
				}
			}

			BigInteger result = new BigInteger(da);
			return (leftSideNeg != rightSideNeg ? -result : result);
		}

	//public static BigInteger Multiply(BigInteger leftSide, BigInteger rightSide)
	//	{
	//		return leftSide * rightSide;
	//	}

		public static BigInteger operator / (BigInteger leftSide, BigInteger rightSide)
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
				return new BigInteger(0);
			}

			BigInteger quotient;
			BigInteger remainder;
			Divide(leftSide, rightSide, out quotient, out remainder);

			return (dividendNeg != divisorNeg ? -quotient : quotient);
		}

		//public static BigInteger Divide(BigInteger leftSide, BigInteger rightSide)
		//{
		//	return leftSide / rightSide;
		//}

		private static void Divide(BigInteger leftSide, BigInteger rightSide, out BigInteger quotient, out BigInteger remainder)
		{
			if (leftSide.IsZero)
			{
				quotient = new BigInteger();
				remainder = new BigInteger();
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

		private static void MultiDivide(BigInteger leftSide, BigInteger rightSide, out BigInteger quotient, out BigInteger remainder)
		{
			if (rightSide.IsZero)
			{
				throw new DivideByZeroException();
			}

			DType val = rightSide.m_digits[rightSide.m_digits.DataUsed - 1];
			int d = 0;
			for (uint mask = DigitsArray.HiBitSet; mask != 0 && (val & mask) == 0; mask >>= 1)
			{
				d++;
			}

			int remainderLen = leftSide.m_digits.DataUsed + 1;
			DType[] remainderDat = new DType[remainderLen];
			leftSide.m_digits.CopyTo(remainderDat, 0, leftSide.m_digits.DataUsed);

			DigitsArray.ShiftLeft(remainderDat, d);
			rightSide = rightSide << d;

			ulong firstDivisor = rightSide.m_digits[rightSide.m_digits.DataUsed - 1];
			ulong secondDivisor = (rightSide.m_digits.DataUsed < 2 ? (DType)0 : rightSide.m_digits[rightSide.m_digits.DataUsed - 2]);

			int divisorLen = rightSide.m_digits.DataUsed + 1;
			DigitsArray dividendPart = new DigitsArray(divisorLen, divisorLen);
			DType[] result = new DType[leftSide.m_digits.Count + 1];
			int resultPos = 0;

			ulong carryBit = (ulong)0x1 << DigitsArray.DataSizeBits; // 0x100000000
			for (int j = remainderLen - rightSide.m_digits.DataUsed, pos = remainderLen - 1; j > 0; j--, pos--)
			{
				ulong dividend = ((ulong)remainderDat[pos] << DigitsArray.DataSizeBits) + (ulong)remainderDat[pos - 1];
				ulong qHat = (dividend / firstDivisor);
				ulong rHat = (dividend % firstDivisor);

				while (pos >= 2)
				{
					if (qHat == carryBit || (qHat * secondDivisor) > ((rHat << DigitsArray.DataSizeBits) + remainderDat[pos - 2]))
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

				BigInteger dTemp = new BigInteger(dividendPart);
				BigInteger rTemp = rightSide * (long)qHat;
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

				result[resultPos++] = (DType)qHat;
			}

			Array.Reverse(result, 0, resultPos);
			quotient = new BigInteger(new DigitsArray(result));

			int n = DigitsArray.ShiftRight(remainderDat, d);
			DigitsArray rDA = new DigitsArray(n, n);
			rDA.CopyFrom(remainderDat, 0, 0, rDA.DataUsed);
			remainder = new BigInteger(rDA);
		}

		private static void SingleDivide(BigInteger leftSide, BigInteger rightSide, out BigInteger quotient, out BigInteger remainder)
		{
			if (rightSide.IsZero)
			{
				throw new DivideByZeroException();
			}

			DigitsArray remainderDigits = new DigitsArray(leftSide.m_digits);
			remainderDigits.ResetDataUsed();

			int pos = remainderDigits.DataUsed - 1;
			ulong divisor = (ulong)rightSide.m_digits[0];
			ulong dividend = (ulong)remainderDigits[pos];

			DType[] result = new DType[leftSide.m_digits.Count];
			leftSide.m_digits.CopyTo(result, 0, result.Length);
			int resultPos = 0;

			if (dividend >= divisor)
			{
				result[resultPos++] = (DType)(dividend / divisor);
				remainderDigits[pos] = (DType)(dividend % divisor);
			}
			pos--;

			while (pos >= 0)
			{
				dividend = ((ulong)(remainderDigits[pos + 1]) << DigitsArray.DataSizeBits) + (ulong)remainderDigits[pos];
				result[resultPos++] = (DType)(dividend / divisor);
				remainderDigits[pos + 1] = 0;
				remainderDigits[pos--] = (DType)(dividend % divisor);
			}
			remainder = new BigInteger(remainderDigits);

			DigitsArray quotientDigits = new DigitsArray(resultPos + 1, resultPos);
			int j = 0;
			for (int i = quotientDigits.DataUsed - 1; i >= 0; i--, j++)
			{
				quotientDigits[j] = result[i];
			}
			quotient = new BigInteger(quotientDigits);
		}

		public static BigInteger operator % (BigInteger leftSide, BigInteger rightSide)
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

			BigInteger quotient;
			BigInteger remainder;

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

		//public static BigInteger Modulus(BigInteger leftSide, BigInteger rightSide)
		//{
		//	return leftSide % rightSide;
		//}

        public BigInteger Pow(BigInteger power) {
			return Pow (this, power);
		}

		public static BigInteger Pow(BigInteger b, BigInteger power) {

			if (b == null) {
				throw new ArgumentNullException ("b");
			}

			if (power == null) {
				throw new ArgumentNullException("power");
			}

			if (power < 0) {
				throw new ArgumentOutOfRangeException ("power", "Currently negative exponents are not supported");
			}


			BigInteger result = 1;
			while (power != 0) {

				if ((power & 1) != 0)
					result *= b;
				power >>= 1;
				b *= b;
			}

			return result;
		}


		public static BigInteger operator & (BigInteger leftSide, BigInteger rightSide)
		{
			int len = System.Math.Max(leftSide.m_digits.DataUsed, rightSide.m_digits.DataUsed);
			DigitsArray da = new DigitsArray(len, len);
			for (int idx = 0; idx < len; idx++)
			{
				da[idx] = (DType)(leftSide.m_digits[idx] & rightSide.m_digits[idx]);
			}
			return new BigInteger(da);
		}

		//public static BigInteger BitwiseAnd(BigInteger leftSide, BigInteger rightSide)
		//{
		//	return leftSide & rightSide;
		//}

		public static BigInteger operator | (BigInteger leftSide, BigInteger rightSide)
		{
			int len = System.Math.Max(leftSide.m_digits.DataUsed, rightSide.m_digits.DataUsed);
			DigitsArray da = new DigitsArray(len, len);
			for (int idx = 0; idx < len; idx++)
			{
				da[idx] = (DType)(leftSide.m_digits[idx] | rightSide.m_digits[idx]);
			}
			return new BigInteger(da);
		}

		//public static BigInteger BitwiseOr(BigInteger leftSide, BigInteger rightSide)
		//{
		//	return leftSide | rightSide;
		//}

		public static BigInteger operator ^ (BigInteger leftSide, BigInteger rightSide)
		{
			int len = System.Math.Max(leftSide.m_digits.DataUsed, rightSide.m_digits.DataUsed);
			DigitsArray da = new DigitsArray(len, len);
			for (int idx = 0; idx < len; idx++)
			{
				da[idx] = (DType)(leftSide.m_digits[idx] ^ rightSide.m_digits[idx]);
			}
			return new BigInteger(da);
		}

		//public static BigInteger Xor(BigInteger leftSide, BigInteger rightSide)
		//{
		//	return leftSide ^ rightSide;
		//}

		public static BigInteger operator ~ (BigInteger leftSide)
		{
			DigitsArray da = new DigitsArray(leftSide.m_digits.Count);
			for(int idx = 0; idx < da.Count; idx++)
			{
				da[idx] = (DType)(~(leftSide.m_digits[idx]));
			}

			return new BigInteger(da);
		}

		//public static BigInteger OnesComplement(BigInteger leftSide)
		//{
		//	return ~leftSide;
		//}

		public static BigInteger operator << (BigInteger leftSide, int shiftCount)
		{
			if (leftSide == null)
			{
				throw new ArgumentNullException("leftSide");
			}

			DigitsArray da = new DigitsArray(leftSide.m_digits);
			da.DataUsed = da.ShiftLeftWithoutOverflow(shiftCount);

			return new BigInteger(da);
		}

		//public static BigInteger LeftShift(BigInteger leftSide, int shiftCount)
		//{
		//	return leftSide << shiftCount;
		//}

		public static BigInteger operator >> (BigInteger leftSide, int shiftCount)
		{
			if (leftSide == null)
			{
				throw new ArgumentNullException("leftSide");
			}

			DigitsArray da = new DigitsArray(leftSide.m_digits);
			da.DataUsed = da.ShiftRight(shiftCount);

			if (leftSide.IsNegative)
			{
				for (int i = da.Count - 1; i >= da.DataUsed; i--)
				{
					da[i] = DigitsArray.AllBits;
				}

				DType mask = DigitsArray.HiBitSet;
				for (int i = 0; i < DigitsArray.DataSizeBits; i++)
				{
					if ((da[da.DataUsed - 1] & mask) == DigitsArray.HiBitSet)
					{
						break;
					}
					da[da.DataUsed - 1] |= mask;
					mask >>= 1;
				}
				da.DataUsed = da.Count;
			}

			return new BigInteger(da);
		}

		public static BigInteger RightShift(BigInteger leftSide, int shiftCount)
		{
			if (leftSide == null)
			{
				throw new ArgumentNullException("leftSide");
			}

			return leftSide >> shiftCount;
		}

		public int CompareTo(BigInteger value)
		{
			return Compare(this, value);
		}

		
		public static int Compare(BigInteger leftSide, BigInteger rightSide)
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

		public static bool operator == (BigInteger leftSide, BigInteger rightSide)
		{
			if (object.ReferenceEquals(leftSide, rightSide))
			{
				return true;
			}

			if (object.ReferenceEquals(leftSide, null ) || object.ReferenceEquals(rightSide, null ))
			{
				return false;
			}

			if (leftSide.IsNegative != rightSide.IsNegative)
			{
				return false;
			}

			return leftSide.Equals(rightSide);
		}

		public static bool operator != (BigInteger leftSide, BigInteger rightSide)
		{
			return !(leftSide == rightSide);
		}

		public static bool operator > (BigInteger leftSide, BigInteger rightSide)
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

		public static bool operator < (BigInteger leftSide, BigInteger rightSide)
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

		public static bool operator >= (BigInteger leftSide, BigInteger rightSide)
		{
			return Compare(leftSide, rightSide) >= 0;
		}

		public static bool operator <= (BigInteger leftSide, BigInteger rightSide)
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

            BigInteger c = (BigInteger)obj;
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

            BigInteger a = this;
            bool negative = a.IsNegative;
            a = Abs(this);

            BigInteger quotient;
            BigInteger remainder;
            BigInteger biRadix = new BigInteger(radix);

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

        //public string ToHexString()
        //{
        //    System.Text.StringBuilder sb = new System.Text.StringBuilder();
        //    sb.AppendFormat("{0:X}", m_digits[m_digits.DataUsed - 1]);

        //    string f = "{0:X" + (2 * DigitsArray.DataSizeOf) + "}";
        //    for (int i = m_digits.DataUsed - 2; i >= 0; i--)
        //    {
        //        sb.AppendFormat(f, m_digits[i]);
        //    }

        //    return sb.ToString();
        //}

        //public static int ToInt16(BigInteger value)
        //{
        //    if (object.ReferenceEquals(value, null))
        //    {
        //        throw new ArgumentNullException("value");
        //    }
        //    return System.Int16.Parse(value.ToString(), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.CurrentCulture);
        //}

        //public static uint ToUInt16(BigInteger value)
        //{
        //    if (object.ReferenceEquals(value, null))
        //    {
        //        throw new ArgumentNullException("value");
        //    }
        //    return System.UInt16.Parse(value.ToString(), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.CurrentCulture);
        //}

        //public static int ToInt32(BigInteger value)
        //{
        //    if (object.ReferenceEquals(value, null))
        //    {
        //        throw new ArgumentNullException("value");
        //    }
        //    return System.Int32.Parse(value.ToString(), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.CurrentCulture);
        //}

        //public static uint ToUInt32(BigInteger value)
        //{
        //    if (object.ReferenceEquals(value, null))
        //    {
        //        throw new ArgumentNullException("value");
        //    }
        //    return System.UInt32.Parse(value.ToString(), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.CurrentCulture);
        //}

        //public static long ToInt64(BigInteger value)
        //{
        //    if (object.ReferenceEquals(value, null))
        //    {
        //        throw new ArgumentNullException("value");
        //    }
        //    return System.Int64.Parse(value.ToString(), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.CurrentCulture);
        //}

        //public static ulong ToUInt64(BigInteger value)
        //{
        //    if (object.ReferenceEquals(value, null))
        //    {
        //        throw new ArgumentNullException("value");
        //    }
        //    return System.UInt64.Parse(value.ToString(), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.CurrentCulture);
        //}
    }
}
