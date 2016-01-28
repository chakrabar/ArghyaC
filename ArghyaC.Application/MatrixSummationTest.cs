using System;
using System.Linq;

namespace ArghyaC.Application
{
    public class MatrixSummationTest
    {
        //square matrix. size 1 - 500. 
        //All matrix values are positive integer (or 0).
        //If data supplied is insufficient, assume rest are 0.
        //For any invalid scenario/exception, return -1. 
        public int SumMatrix(int size, params int[] data)
        {
            int result = -1;
            try
            {
                if (size < 1 || size > 500 || data == null)
                    return -1;
                if (data.Any(d => d < 0))
                    return -1;
                if (data.Length > size * size)
                    return -1;

                result = data.Sum();
            }
            catch (Exception ex)
            { }
            return result;
        }
    }
}
