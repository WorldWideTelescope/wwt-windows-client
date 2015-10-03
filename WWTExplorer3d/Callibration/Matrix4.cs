using System.Diagnostics;
using System;

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
        private readonly double[,] _values;

        private readonly int _rowCount = 3;
        public int RowCount
        {
            get { return _rowCount; }
        }

        private readonly int _columnCount = 3;
        public int ColumnCount
        {
            get { return _columnCount; }
        }
        #endregion

        #region basic single matrix stuff
        public static Matrix Identity(int size)
        {
            var resultMatrix = new Matrix(size, size);
            for(var i = 0; i<size; i++)
            {
                for (var j = 0; j < size; j++)
                {
                    resultMatrix[i, j] = (i == j) ? 1.0 : 0.0;
                }
            }
            return resultMatrix;
        }

        public Matrix Clone()
        {
            var resultMatrix = new Matrix(_rowCount, _columnCount);
            for(var i = 0 ; i < _rowCount; i++)
            {
                for (var j = 0; j < _columnCount; j++)
                {
                    resultMatrix[i, j] = this[i, j];
                }
            }
            return resultMatrix;
        }

        public Matrix Transpose()
        {
            var resultMatrix = new Matrix(_columnCount, _rowCount);
            for(var i =0; i < _rowCount; i++)
            {
                for (var j = 0; j < _columnCount; j++)
                {
                    resultMatrix[j, i] = this[i, j];
                }
            }
            return resultMatrix;
        }

        #endregion

        #region Binary Math
        public static Matrix Add(Matrix leftMatrix, Matrix rightMatrix)
        {
            Debug.Assert(leftMatrix.ColumnCount == rightMatrix.ColumnCount);
            Debug.Assert(leftMatrix.RowCount == rightMatrix.RowCount);

            var resultMatrix = new Matrix(leftMatrix.RowCount, rightMatrix.ColumnCount);
            for(var i=0; i< leftMatrix.RowCount; i++)
            {
                for (var j = 0; j < leftMatrix.ColumnCount; j++)
                {
                    resultMatrix[i, j] = leftMatrix[i, j] + rightMatrix[i, j];
                }
            }
            return resultMatrix;
        }

        public static Matrix operator +(Matrix leftMatrix, Matrix rightMatrix)
        {
            return Add(leftMatrix, rightMatrix);
        }

        public static Matrix Subtract(Matrix leftMatrix, Matrix rightMatrix)
        {
            Debug.Assert(leftMatrix.ColumnCount == rightMatrix.ColumnCount);
            Debug.Assert(leftMatrix.RowCount == rightMatrix.RowCount);
            var resultMatrix = new Matrix(leftMatrix.RowCount, rightMatrix.ColumnCount);
            for(var i=0; i < leftMatrix.RowCount; i++)
            {
                for (var j = 0; j < leftMatrix.ColumnCount; j++)
                {
                    resultMatrix[i, j] = leftMatrix[i, j] - rightMatrix[i, j];
                }
            }
            return resultMatrix;
        }

        public static Matrix operator -(Matrix leftMatrix, Matrix rightMatrix)
        {
            return Subtract(leftMatrix, rightMatrix);
        }

        public static Matrix Multiply(Matrix leftMatrix, Matrix rightMatrix)
        {
            Debug.Assert(leftMatrix.ColumnCount == rightMatrix.RowCount);
            var resultMatrix = new Matrix(leftMatrix.RowCount, rightMatrix.ColumnCount);
            for(var i=0; i< resultMatrix.ColumnCount; i++)
            {
                for (var j = 0; j < leftMatrix.RowCount; j++)
                {
                    var value = 0.0;
                    for (var k = 0; k < rightMatrix.RowCount; k++)
                    {
                        value += leftMatrix[j, k] * rightMatrix[k, i];
                    }
                    resultMatrix[j, i] = value;
                }
            }
            return resultMatrix;
        }

        public static Matrix operator *(Matrix leftMatrix, Matrix rightMatrix)
        {
            return Multiply(leftMatrix, rightMatrix);
        }

        public static Matrix Multiply(double left, Matrix rightMatrix)
        {
            var resultMatrix = new Matrix(rightMatrix.RowCount, rightMatrix.ColumnCount);
            for(var i=0; i< resultMatrix.RowCount; i++)
            {
                for (var j = 0; j < rightMatrix.ColumnCount; j++)
                {
                    resultMatrix[i, j] = left * rightMatrix[i, j];
                }
            }
            return resultMatrix;
        }

        public static Matrix operator *(double left, Matrix rightMatrix)
        {
            return Multiply(left, rightMatrix);
        }

        public static Matrix Multiply(Matrix leftMatrix, double right)
        {
            var resultMatrix = new Matrix(leftMatrix.RowCount, leftMatrix.ColumnCount);
            for(var i=0;i < leftMatrix.RowCount; i++)
            {
                for (var j = 0; j < leftMatrix.ColumnCount; j++)
                {
                    resultMatrix[i, j] = leftMatrix[i, j] * right;
                }
            }
            return resultMatrix;
        }

        public static Matrix operator *(Matrix leftMatrix, double right)
        {
            return Multiply(leftMatrix, right);
        }

        public static Matrix Divide(Matrix leftMatrix, double right)
        {
            var resultMatrix = new Matrix(leftMatrix.RowCount, leftMatrix.ColumnCount);
            for (var i = 0; i < leftMatrix.RowCount; i++)
            {
                for (var j = 0; j < leftMatrix.ColumnCount; j++)
                {
                    resultMatrix[i, j] = leftMatrix[i, j]/right;
                }
            }
            return resultMatrix;
        }

        public static Matrix operator /(Matrix leftMatrix, double right)
        {
            return Divide(leftMatrix, right);
        }
        #endregion

        #region Assorted Casts
        public static Matrix FromArray(double[] left)
        {
            var length = left.Length;
            var resultMatrix = new Matrix(length, 1);
            for (var i = 0; i < length; i++)
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

            double[] result;
            if (leftMatrix.ColumnCount > 1)
            {
                var numElements = leftMatrix.ColumnCount;
                result = new double[numElements];
                for (var i = 0; i < numElements; i++)
                {
                    result[i] = leftMatrix[0, i];
                }
            }
            else
            {
                var numElements = leftMatrix.RowCount;
                result = new double[numElements];
                for (var i = 0; i < numElements; i++)
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
            var length0 = left.GetLength(0);
            var length1 = left.GetLength(1);
            var resultMatrix = new Matrix(length0, length1);
            for (var i = 0; i < length0; i++)
            {
                for (var j = 0; j < length1; j++)
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
            var result = new double[leftMatrix.RowCount, leftMatrix.ColumnCount];
            for (var i = 0; i < leftMatrix.RowCount; i++)
            {
                for (var j = 0; j < leftMatrix.ColumnCount; j++)
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

            var resultMatrix = new Matrix(_columnCount, rightMatrix.ColumnCount);
            var resDecomp = LUDecompose();
            var nP = resDecomp.PivotArray;
            var lMatrix = resDecomp.L;
            var uMatrix = resDecomp.U;
            for(var k=0; k<rightMatrix.ColumnCount; k++)
            {
                //Solve for the corresponding d Matrix from Ld=Pb
                double sum;
                var dMatrix = new Matrix(_rowCount, 1);
                dMatrix[0, 0] = rightMatrix[nP[0], k] / lMatrix[0, 0];
                for (var i = 1; i < _rowCount; i++)
                {
                    sum = 0.0;
                    for (var j = 0; j < i; j++)
                    {
                        sum += lMatrix[i, j] * dMatrix[j, 0];
                    }
                    dMatrix[i, 0] = (rightMatrix[nP[i], k] - sum) / lMatrix[i, i];
                }
                //Solve for x using Ux = d
                resultMatrix[_rowCount - 1, k] = dMatrix[_rowCount - 1, 0];
                for (var i = _rowCount - 2; i >= 0; i--)
                {
                    sum = 0.0;
                    for (var j = i + 1; j < _rowCount; j++)
                    {
                        sum += uMatrix[i, j] * resultMatrix[j, k];
                    }
                    resultMatrix[i, k] = dMatrix[i, 0] - sum;
                }
            }

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
            var result = new LUDecompositionResults();
            try
            {
                var pivotArray = new int[_rowCount]; //Pivot matrix.
                var uMatrix = new Matrix(_rowCount, _columnCount);
                var lMatrix = new Matrix(_rowCount, _columnCount);
                var workingUMatrix = Clone();
                var workingLMatrix = new Matrix(_rowCount, _columnCount);
                for(var i=0; i< _rowCount; i++)
                {
                    pivotArray[i] = i;
                }

                //Iterate down the number of rows in the U matrix.
                for (var i = 0; i < _rowCount; i++)
                {
                    //Do pivots first.
                    //I want to make the matrix diagnolaly dominate.
                    //Initialize the variables used to determine the pivot row.
                    var maxRowRatio = double.NegativeInfinity;
                    var maxRow = -1;
                    var maxPosition = -1;
                    //Check all of the rows below and including the current row
                    //to determine which row should be pivoted to the working row position.
                    //The pivot row will be set to the row with the maximum ratio
                    //of the absolute value of the first column element divided by the
                    //sum of the absolute values of the elements in that row.
                    for (var j = i; j < _rowCount; j++)
                    {
                        //Store the sum of the absolute values of the row elements in
                        //dRowSum. Clear it out now because I am checking a new row.
                        var rowSum = 0.0;
                        //Go across the columns, add the absolute values of the elements in
                        //that column to dRowSum.
                        for (var k = i; k < _columnCount; k++)
                        {
                            rowSum += Math.Abs(workingUMatrix[pivotArray[j], k]);
                        }
                        //Check to see if the absolute value of the ratio of the lead
                        //element over the sum of the absolute values of the elements is larger
                        //that the ratio for preceding rows. If it is, then the current row
                        //becomes the new pivot candidate.
                        if (rowSum.Equals(0.0))
                        {
                            throw new SingularMatrixException();
                        }
                        var dCurrentRatio = Math.Abs(workingUMatrix[pivotArray[j], i]) / rowSum;
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
                        var hold = pivotArray[i];
                        pivotArray[i] = maxRow;
                        pivotArray[maxPosition] = hold;
                    }
                    //Store the value of the left most element in the working U
                    //matrix in dRowFirstElementValue.
                    var rowFirstElementValue = workingUMatrix[pivotArray[i], i];
                    //Update the columns of the working row. j is the column index.
                    for (var j = 0; j < _columnCount; j++)
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

                    //For the working U matrix, subtract the ratioed active row from the rows below it.
                    //Update the columns of the rows below the working row. k is the row index.
                    for (var k = i + 1; k < _rowCount; k++)
                    {
                        //Store the value of the first element in the working row
                        //of the U matrix.
                        rowFirstElementValue = workingUMatrix[pivotArray[k], i];
                        //Go accross the columns of row k.
                        for (var j = 0; j < _columnCount; j++)
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

                    }
                }

                for (var i = 0; i < _rowCount; i++)
                {
                    for (var j = 0; j < _rowCount; j++)
                    {
                        uMatrix[i, j] = workingUMatrix[pivotArray[i], j];
                        lMatrix[i, j] = workingLMatrix[pivotArray[i], j];
                    }
                }
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
            SolveFor(Identity(_rowCount));

            return SolveFor(Identity(_rowCount));
        }
    }
    public class LUDecompositionResults
    {
        public LUDecompositionResults()
        {
        }

        public LUDecompositionResults(Matrix matL, Matrix matU, int[] nPivotArray)
        {
            L = matL;
            U = matU;
            PivotArray = nPivotArray;
        }

        public Matrix L { get; set; }

        public Matrix U { get; set; }

        public int[] PivotArray { get; set; }
    }

    public class SingularMatrixException : ArithmeticException
    {
        public SingularMatrixException()
            : base("Invalid operation on a singular matrix.")
        {
        }
    }
}
