using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace JoshuaaM.PrimeSieve
{
    //Only to be used by this library, and only when T is a built in type
    internal class PrimeSieveList<T> : List<T>
    {
        public new virtual int Capacity
        {
            get { return base.Capacity; }
            set
            {
                int maxLength = int.MaxValue / Marshal.SizeOf(typeof(T)); //Max number of items possible in array

                if(value < maxLength)
                {
                    base.Capacity = value;
                }
                else if(value > maxLength && base.Capacity == maxLength)
                {
                    throw new Exception("Cannot make the array any larger or it will hit the 2GB max object size limitation of .NET");
                }
                else //Impliented value <= maxLength && base.Capacity < maxLength
                {
                    base.Capacity = maxLength;
                }
            }
        }
    }
}
