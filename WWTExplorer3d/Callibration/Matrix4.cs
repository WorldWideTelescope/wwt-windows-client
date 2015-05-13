using System.Diagnostics;
using System;
//using System.Threading.Tasks;

namespace Solver
{
    public class Matrix
    {
        #region ctor
        public Matrix()
        {
            _values = new double[_rowCount, _columnCount];
        }

        public Matrix(int rowCount, int columnCount)
        {
            _rowCount = rowCount;
            _columnCount = columnCount;
            _values = new double[_rowCount, _columnCount];
        }
        #endregion

        #region Row Column values
        public double this[int row, int column]
        {
            get { return _values[row, column]; }
            set { _values[row, column] = value; }
        }
        #endregion

        #region F&P
        private double[,] _values;

        private int _rowCount = 3;
        public int RowCount
        {
            get { return _rowCount; }
        }

        private int _columnCount = 3;
        public int ColumnCount
        {
            get { return _columnCount; }
        }
        #endregion

        #region basic single matrix stuff
        public static Matrix Identity(int size)
        {
            Matrix resultMatrix = new Matrix(size, size);
  //          Parallel.For(0, size, (i) =>
            for(int i = 0; i<size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    resultMatrix[i, j] = (i == j) ? 1.0 : 0.0;
                }
            }
      //      );
            return resultMatrix;
        }

        public Matrix Clone()
        {
            Matrix resultMatrix = new Matrix(_rowCount, _columnCount);
  //          Parallel.For(0, _rowCount, (i) =>
            for(int i = 0 ; i < _rowCount; i++)
            {
                for (int j = 0; j < _columnCount; j++)
                {
                    resultMatrix[i, j] = this[i, j];
                }
            }
  //          );
            return resultMatrix;
        }

        public Matrix Transpose()
        {
            Matrix resultMatrix = new Matrix(_columnCount, _rowCount);
      //      Parallel.For(0, _rowCount, (i) =>
            for(int i =0; i < _rowCount; i++)
            {
                for (int j = 0; j < _columnCount; j++)
                {
                    resultMatrix[j, i] = this[i, j];
                }
            }
  //          );
            return resultMatrix;
        }

        #endregion

        #region Binary Math
        public static Matrix Add(Matrix leftMatrix, Matrix rightMatrix)
        {
            Debug.Assert(leftMatrix.ColumnCount == rightMatrix.ColumnCount);
            Debug.Assert(leftMatrix.RowCount == rightMatrix.RowCount);

            Matrix resultMatrix = new Matrix(leftMatrix.RowCount, rightMatrix.ColumnCount);
  //          Parallel.For(0, leftMatrix.RowCount, (i) =>
            for(int i=0; i< leftMatrix.RowCount; i++)
            {
                for (int j = 0; j < leftMatrix.ColumnCount; j++)
                {
                    resultMatrix[i, j] = leftMatrix[i, j] + rightMatrix[i, j];
                }
            }
     //       );
            return resultMatrix;
        }

        public static Matrix operator +(Matrix leftMatrix, Matrix rightMatrix)
        {
            return Matrix.Add(leftMatrix, rightMatrix);
        }

        public static Matrix Subtract(Matrix leftMatrix, Matrix rightMatrix)
        {
            Debug.Assert(leftMatrix.ColumnCount == rightMatrix.ColumnCount);
            Debug.Assert(leftMatrix.RowCount == rightMatrix.RowCount);
            Matrix resultMatrix = new Matrix(leftMatrix.RowCount, rightMatrix.ColumnCount);
     //      Parallel.For(0, leftMatrix.RowCount, (i) =>
            for(int i=0; i < leftMatrix.RowCount; i++)
            {
                for (int j = 0; j < leftMatrix.ColumnCount; j++)
                {
                    resultMatrix[i, j] = leftMatrix[i, j] - rightMatrix[i, j];
                }
            }
    //        );
            return resultMatrix;
        }

        public static Matrix operator -(Matrix leftMatrix, Matrix rightMatrix)
        {
            return Matrix.Subtract(leftMatrix, rightMatrix);
        }

        public static Matrix Multiply(Matrix leftMatrix, Matrix rightMatrix)
        {
            Debug.Assert(leftMatrix.ColumnCount == rightMatrix.RowCount);
            Matrix resultMatrix = new Matrix(leftMatrix.RowCount, rightMatrix.ColumnCount);
       //     Parallel.For(0, resultMatrix.ColumnCount, (i) =>
            for(int i=0; i< resultMatrix.ColumnCount; i++)
            {
                for (int j = 0; j < leftMatrix.RowCount; j++)
                {
                    double value = 0.0;
                    for (int k = 0; k < rightMatrix.RowCount; k++)
                    {
                        value += leftMatrix[j, k] * rightMatrix[k, i];
                    }
                    resultMatrix[j, i] = value;
                }
            }
   //         );
            return resultMatrix;
        }

        public static Matrix operator *(Matrix leftMatrix, Matrix rightMatrix)
        {
            return Matrix.Multiply(leftMatrix, rightMatrix);
        }

        public static Matrix Multiply(double left, Matrix rightMatrix)
        {
            Matrix resultMatrix = new Matrix(rightMatrix.RowCount, rightMatrix.ColumnCount);
    //        Parallel.For(0, resultMatrix.RowCount, (i) =>
            for(int i=0; i< resultMatrix.RowCount; i++)
            {
                for (int j = 0; j < rightMatrix.ColumnCount; j++)
                {
                    resultMatrix[i, j] = left * rightMatrix[i, j];
                }
            }
    //        );
            return resultMatrix;
        }

        public static Matrix operator *(double left, Matrix rightMatrix)
        {
            return Matrix.Multiply(left, rightMatrix);
        }

        public static Matrix Multiply(Matrix leftMatrix, double right)
        {
            Matrix resultMatrix = new Matrix(leftMatrix.RowCount, leftMatrix.ColumnCount);
      //      Parallel.For(0, leftMatrix.RowCount, (i) =>
            for(int i=0;i < leftMatrix.RowCount; i++)
            {
                for (int j = 0; j < leftMatrix.ColumnCount; j++)
                {
                    resultMatrix[i, j] = leftMatrix[i, j] * right;
                }
            }
      //      );
            return resultMatrix;
        }

        public static Matrix operator *(Matrix leftMatrix, double right)
        {
            return Matrix.Multiply(leftMatrix, right);
        }

        public static Matrix Divide(Matrix leftMatrix, double right)
        {
            Matrix resultMatrix = new Matrix(leftMatrix.RowCount, leftMatrix.ColumnCount);
   //         Parallel.For(0, leftMatrix.RowCount, (i) =>
            for(int i=0; i < leftMatrix.RowCount; i++)
            {
                for (int j = 0; j < leftMatrix.ColumnCount; j++)
                {
                    resultMatrix[i, j] = leftMatrix[i, j] / right;
                }
            }
 //           );
            return resultMatrix;
        }

        public static Matrix operator /(Matrix leftMatrix, double right)
        {
            return Matrix.Divide(leftMatrix, right);
        }
        #endregion

        #region Assorted Casts
        public static Matrix FromArray(double[] left)
        {
            int length = left.Length;
            Matrix resultMatrix = new Matrix(length, 1);
            for (int i = 0; i < length; i++)
            {
                resultMatrix[i, 0] = left[i];
            }
            return resultMatrix;
        }

        public static implicit operator Matrix(double[] left)
        {
            return FromArray(left);
        }

        public static double[] ToArray(Matrix leftMatrix)
        {
            Debug.Assert((leftMatrix.ColumnCount == 1 && leftMatrix.RowCount >= 1) || (leftMatrix.RowCount == 1 && leftMatrix.ColumnCount >= 1));

            double[] result = null;
            if (leftMatrix.ColumnCount > 1)
            {
                int numElements = leftMatrix.ColumnCount;
                result = new double[numElements];
                for (int i = 0; i < numElements; i++)
                {
                    result[i] = leftMatrix[0, i];
                }
            }
            else
            {
                int numElements = leftMatrix.RowCount;
                result = new double[numElements];
                for (int i = 0; i < numElements; i++)
                {
                    result[i] = leftMatrix[i, 0];
                }
            }
            return result;
        }

        public static implicit operator double[](Matrix leftMatrix)
        {
            return ToArray(leftMatrix);
        }

        public static Matrix FromDoubleArray(double[,] left)
        {
            int length0 = left.GetLength(0);
            int length1 = left.GetLength(1);
            Matrix resultMatrix = new Matrix(length0, length1);
            for (int i = 0; i < length0; i++)
            {
                for (int j = 0; j < length1; j++)
                {
                    resultMatrix[i, j] = left[i, j];
                }
            }
            return resultMatrix;
        }

        public static implicit operator Matrix(double[,] left)
        {
            return FromDoubleArray(left);
        }

        public static double[,] ToDoubleArray(Matrix leftMatrix)
        {
            double[,] result = new double[leftMatrix.RowCount, leftMatrix.ColumnCount];
            for (int i = 0; i < leftMatrix.RowCount; i++)
            {
                for (int j = 0; j < leftMatrix.ColumnCount; j++)
                {
                    result[i, j] = leftMatrix[i, j];
                }
            }
            return result;
        }

        public static implicit operator double[,](Matrix leftMatrix)
        {
            return ToDoubleArray(leftMatrix);
        }
        #endregion
        public Matrix SolveFor(Matrix rightMatrix)
        {
            Debug.Assert(rightMatrix.RowCount == _columnCount);
            Debug.Assert(_columnCount == _rowCount);

            Matrix resultMatrix = new Matrix(_columnCount, rightMatrix.ColumnCount);
            LUDecompositionResults resDecomp = LUDecompose();
            int[] nP = resDecomp.PivotArray;
            Matrix lMatrix = resDecomp.L;
            Matrix uMatrix = resDecomp.U;
   //         Parallel.For(0, rightMatrix.ColumnCount, k =>
            for(int k=0; k<rightMatrix.ColumnCount; k++)
            {
                //Solve for the corresponding d Matrix from Ld=Pb
                double sum = 0.0;
                Matrix dMatrix = new Matrix(_rowCount, 1);
                dMatrix[0, 0] = rightMatrix[nP[0], k] / lMatrix[0, 0];
                for (int i = 1; i < _rowCount; i++)
                {
                    sum = 0.0;
                    for (int j = 0; j < i; j++)
                    {
                        sum += lMatrix[i, j] * dMatrix[j, 0];
                    }
                    dMatrix[i, 0] = (rightMatrix[nP[i], k] - sum) / lMatrix[i, i];
                }
                //Solve for x using Ux = d
                resultMatrix[_rowCount - 1, k] = dMatrix[_rowCount - 1, 0];
                for (int i = _rowCount - 2; i >= 0; i--)
                {
                    sum = 0.0;
                    for (int j = i + 1; j < _rowCount; j++)
                    {
                        sum += uMatrix[i, j] * resultMatrix[j, k];
                    }
                    resultMatrix[i, k] = dMatrix[i, 0] - sum;
                }
            }
   //         );
            return resultMatrix;
        }

        private LUDecompositionResults LUDecompose()
        {
            Debug.Assert(_columnCount == _rowCount);
            // Using Crout Decomp with P
            //
            // Ax = b //By definition of problem variables.
            //
            // LU = PA //By definition of L, U, and P.
            //
            // LUx = Pb //By substition for PA.
            //
            // Ux = d //By definition of d
            //
            // Ld = Pb //By subsitition for d.
            //
            //For 4x4 with P = I
            // [l11 0 0 0 ] [1 u12 u13 u14] [a11 a12 a13 a14]
            // [l21 l22 0 0 ] [0 1 u23 u24] = [a21 a22 a23 a24]
            // [l31 l32 l33 0 ] [0 0 1 u34] [a31 a32 a33 a34]
            // [l41 l42 l43 l44] [0 0 0 1 ] [a41 a42 a43 a44]
            LUDecompositionResults result = new LUDecompositionResults();
            try
            {
                int[] pivotArray = new int[_rowCount]; //Pivot matrix.
                Matrix uMatrix = new Matrix(_rowCount, _columnCount);
                Matrix lMatrix = new Matrix(_rowCount, _columnCount);
                Matrix workingUMatrix = Clone();
                Matrix workingLMatrix = new Matrix(_rowCount, _columnCount);
//               Parallel.For(0, _rowCount, i =>
                for(int i=0; i< _rowCount; i++)
                {
                    pivotArray[i] = i;
                }
  //              );
                //Iterate down the number of rows in the U matrix.
                for (int i = 0; i < _rowCount; i++)
                {
                    //Do pivots first.
                    //I want to make the matrix diagnolaly dominate.
                    //Initialize the variables used to determine the pivot row.
                    double maxRowRatio = double.NegativeInfinity;
                    int maxRow = -1;
                    int maxPosition = -1;
                    //Check all of the rows below and including the current row
                    //to determine which row should be pivoted to the working row position.
                    //The pivot row will be set to the row with the maximum ratio
                    //of the absolute value of the first column element divided by the
                    //sum of the absolute values of the elements in that row.
            //        Parallel.For(i, _rowCount, j =>
                    for(int j =i; j < _rowCount; j++)
                    {
                        //Store the sum of the absolute values of the row elements in
                        //dRowSum. Clear it out now because I am checking a new row.
                        double rowSum = 0.0;
                        //Go across the columns, add the absolute values of the elements in
                        //that column to dRowSum.
                        for (int k = i; k < _columnCount; k++)
                        {
                            rowSum += Math.Abs(workingUMatrix[pivotArray[j], k]);
                        }
                        //Check to see if the absolute value of the ratio of the lead
                        //element over the sum of the absolute values of the elements is larger
                        //that the ratio for preceding rows. If it is, then the current row
                        //becomes the new pivot candidate.
                        if (rowSum == 0.0)
                        {
                            throw new SingularMatrixException();
                        }
                        double dCurrentRatio = Math.Abs(workingUMatrix[pivotArray[j], i]) / rowSum;
                        lock (this)
                        {
                            if (dCurrentRatio > maxRowRatio)
                            {
                                maxRowRatio = Math.Abs(workingUMatrix[pivotArray[j], i] / rowSum);
                                maxRow = pivotArray[j];
                                maxPosition = j;
                            }
                        }
                    }
               //     );

                    //If the pivot candidate isn't the current row, update the
                    //pivot array to swap the current row with the pivot row.
                    if (maxRow != pivotArray[i])
                    {
                        int hold = pivotArray[i];
                        pivotArray[i] = maxRow;
                        pivotArray[maxPosition] = hold;
                    }
                    //Store the value of the left most element in the working U
                    //matrix in dRowFirstElementValue.
                    double rowFirstElementValue = workingUMatrix[pivotArray[i], i];
                    //Update the columns of the working row. j is the column index.
                //    Parallel.For(0, _columnCount, j =>
                    for(int j=0; j< _columnCount; j++)
                    {
                        if (j < i)
                        {
                            //If j<1, then the U matrix element value is 0.
                            workingUMatrix[pivotArray[i], j] = 0.0;
                        }
                        else if (j == i)
                        {
                            //If i == j, the L matrix value is the value of the
                            //element in the working U matrix.
                            workingLMatrix[pivotArray[i], j] = rowFirstElementValue;
                            //The value of the U matrix for i == j is 1
                            workingUMatrix[pivotArray[i], j] = 1.0;
                        }
                        else // j>i
                        {
                            //Divide each element in the current row of the U matrix by the
                            //value of the first element in the row
                            workingUMatrix[pivotArray[i], j] /= rowFirstElementValue;
                            //The element value of the L matrix for j>i is 0
                            workingLMatrix[pivotArray[i], j] = 0.0;
                        }
                    }
              //      );
                    //For the working U matrix, subtract the ratioed active row from the rows below it.
                    //Update the columns of the rows below the working row. k is the row index.
                    for (int k = i + 1; k < _rowCount; k++)
                    {
                        //Store the value of the first element in the working row
                        //of the U matrix.
                        rowFirstElementValue = workingUMatrix[pivotArray[k], i];
                        //Go accross the columns of row k.
              //          Parallel.For(0, _columnCount, j =>
                        for(int j=0; j< _columnCount; j++)
                        {
                            if (j < i)
                            {
                                //If j<1, then the U matrix element value is 0.
                                workingUMatrix[pivotArray[k], j] = 0.0;
                            }
                            else if (j == i)
                            {
                                //If i == j, the L matrix value is the value of the
                                //element in the working U matrix.
                                workingLMatrix[pivotArray[k], j] = rowFirstElementValue;
                                //The element value of the L matrix for j>i is 0
                                workingUMatrix[pivotArray[k], j] = 0.0;
                            }
                            else //j>i
                            {
                                workingUMatrix[pivotArray[k], j] = workingUMatrix[pivotArray[k], j] - rowFirstElementValue * workingUMatrix[pivotArray[i], j];
                            }
                        }
               //         );
                    }
                }
            //    Parallel.For(0, _rowCount, i =>
                for(int i=0; i<_rowCount; i++)
                {
                    for (int j = 0; j < _rowCount; j++)
                    {
                        uMatrix[i, j] = workingUMatrix[pivotArray[i], j];
                        lMatrix[i, j] = workingLMatrix[pivotArray[i], j];
                    }
                }
           //     );
                result.U = uMatrix;
                result.L = lMatrix;
                result.PivotArray = pivotArray;
            }
            catch 
            {
            
            }
          
            return result;
        }

        public Matrix Invert()
        {
            Debug.Assert(_rowCount == _columnCount);
            Matrix resultMatrix = SolveFor(Identity(_rowCount));
            Matrix matIdent = this * resultMatrix;

            return SolveFor(Identity(_rowCount));
        }
    }
    public class LUDecompositionResults
    {
        private Matrix _lMatrix;
        private Matrix _uMatrix;
        private int[] _pivotArray;

        public LUDecompositionResults()
        {
        }

        public LUDecompositionResults(Matrix matL, Matrix matU, int[] nPivotArray)
        {
            _lMatrix = matL;
            _uMatrix = matU;
            _pivotArray = nPivotArray;
        }

        public Matrix L
        {
            get { return _lMatrix; }
            set { _lMatrix = value; }
        }

        public Matrix U
        {
            get { return _uMatrix; }
            set { _uMatrix = value; }
        }

        public int[] PivotArray
        {
            get { return _pivotArray; }
            set { _pivotArray = value; }
        }

    }

    public class SingularMatrixException : ArithmeticException
    {
        public SingularMatrixException()
            : base("Invalid operation on a singular matrix.")
        {
        }
    }
}
