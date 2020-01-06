using System;
using System.Collections.Generic;
using System.Text;

namespace Kod
{
    using unsignIn = System.UInt32; //unsigned int with values from 0 to 4 294 967 295
    public class ArrayForDigits
    {
        internal ArrayForDigits(int size)
        {
            Allocate(size, 0);
        }

        internal ArrayForDigits(int size, int used)
        {
            Allocate(size, used);
        }

        internal ArrayForDigits(unsignIn[] copyFrom)
        {
            Allocate(copyFrom.Length);
            CopyFrom(copyFrom, 0, 0, copyFrom.Length);
            ResetDataUsed();
        }

        internal ArrayForDigits(ArrayForDigits copyFrom)
        {
            Allocate(copyFrom.Count - 1, copyFrom.DataUsed);
            Array.Copy(copyFrom.m_data, 0, m_data, 0, copyFrom.Count);
        }

        private unsignIn[] m_data;

        internal static readonly unsignIn AllBits;
        internal static readonly unsignIn HiBitSet;
        internal static int DataSizeOf
        {
            get { return sizeof(unsignIn); }
        }

        internal static int DataSizeBits
        {
            get { return sizeof(unsignIn) * 8; }
        }

        static ArrayForDigits()
        {
            unchecked
            {
                AllBits = (unsignIn)~((unsignIn)0);
                HiBitSet = (unsignIn)(((unsignIn)1) << (DataSizeBits) - 1);
            }
        }

        public void Allocate(int size)
        {
            Allocate(size, 0);
        }

        public void Allocate(int size, int used)
        {
            m_data = new unsignIn[size + 1];
            m_dataUsed = used;
        }

        internal void CopyFrom(unsignIn[] source, int sourceOffset, int offset, int length)
        {
            Array.Copy(source, sourceOffset, m_data, 0, length);
        }

        internal void CopyTo(unsignIn[] array, int offset, int length)
        {
            Array.Copy(m_data, 0, array, offset, length);
        }

        internal unsignIn this[int index]
        {
            get
            {
                if (index < m_dataUsed) return m_data[index];
                return (IsNegative ? (unsignIn)AllBits : (unsignIn)0);
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
            foreach (unsignIn data in m_data)
            {
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

        internal static int ShiftRight(unsignIn[] buffer, int shiftCount)
        {
            int shiftAmount = ArrayForDigits.DataSizeBits;
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
                    invShift = ArrayForDigits.DataSizeBits - shiftAmount;
                }

                ulong carry = 0;
                for (int i = bufLen - 1; i >= 0; i--)
                {
                    ulong val = ((ulong)buffer[i]) >> shiftAmount;
                    val |= carry;

                    carry = ((ulong)buffer[i]) << invShift;
                    buffer[i] = (unsignIn)(val);
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

        internal static int ShiftLeft(unsignIn[] buffer, int shiftCount)
        {
            int shiftAmount = ArrayForDigits.DataSizeBits;
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

                    buffer[i] = (unsignIn)(val & ArrayForDigits.AllBits);
                    carry = (val >> ArrayForDigits.DataSizeBits);
                }

                if (carry != 0)
                {
                    if (bufLen + 1 <= buffer.Length)
                    {
                        buffer[bufLen] = (unsignIn)carry;
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

            List<unsignIn> temporary = new List<unsignIn>(m_data);
            int shiftAmount = ArrayForDigits.DataSizeBits;

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

                    temporary[i] = (unsignIn)(val & ArrayForDigits.AllBits);
                    carry = (val >> ArrayForDigits.DataSizeBits);
                }

                if (carry != 0)
                {
                    unsignIn lastNum = (unsignIn)carry;
                    if (IsNegative)
                    {
                        int byteCount = (int)Math.Floor(Math.Log(carry, 2));
                        lastNum = (0xffffffff << byteCount) | (unsignIn)carry;
                    }

                    temporary.Add(lastNum);
                }
            }
            m_data = new unsignIn[temporary.Count];
            temporary.CopyTo(m_data);
            return m_data.Length;
        }
    }
}
