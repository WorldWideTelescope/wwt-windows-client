using System;
using System.Diagnostics;
using System.Linq;

namespace Solver
{
    public class Derivatives
    {
        // _coefficients is the array of differential coefficients matrices.
        // The index corresponds to the position from the left edge
        // of the points.
        // I.e _coefficients[0] is for a matrix with three points in it corresponds to
        // the left most point.
        // The coefficients of the derivatives go down by row.  I.e. the first row
        // is the functional value, the second row is for the first derivative of the functional
        // value, the third row is the second derivative of the functional value.
        // The columns correspond to the points themselves.
        private Matrix[] _coefficients;

        private Derivatives()
        {
        }

        public Derivatives(int numberOfPoints)
            : this()
        {
            SolveCoefs(numberOfPoints);
        }

        public void SolveCoefs(int numberOfPoints)
        {
            _coefficients = new Matrix[numberOfPoints];
            for (var i = 0; i < numberOfPoints; i++)
            {
                var deltsMatrix = new Matrix(numberOfPoints, numberOfPoints);
                for (var j = 0; j < numberOfPoints; j++)
                {
                    var delt = (double)(j - i);
                    var HTerm = 1.0;
                    for (var k = 0; k < numberOfPoints; k++)
                    {
                        deltsMatrix[j, k] = HTerm / Factorial(k);
                        HTerm *= delt;
                    }
                }
                _coefficients[i] = deltsMatrix.Invert();
                var numPointsFactorial = Factorial(numberOfPoints);
                for (var j = 0; j < numberOfPoints; j++)
                {
                    for (var k = 0; k < numberOfPoints; k++)
                    {
                        _coefficients[i][j, k] = (Math.Round(_coefficients[i][j, k] * numPointsFactorial)) / numPointsFactorial;
                    }
                }
            }
        }

        private static double Factorial(int value)
        {
            var result = 1.0;
            for (var i = 1; i <= value; i++)
            {
                result *= i;
            }
            return result;
        }

        /// <summary>
        /// Computes the derivative of a function.
        /// </summary>
        /// <param name="points">Equally spaced function value points</param>
        /// <param name="order">The order of the derivative to take</param>
        /// <param name="variablePosition">The position in the array of function values to take the derivative at.</param>
        /// <param name="step">The x axis step size.</param>
        /// <returns></returns>
        public double ComputeDerivative(double[] points, int order, int variablePosition, double step)
        {
            Debug.Assert(points.Length == _coefficients.Length);
            Debug.Assert(order < _coefficients.Length);
            double result = _coefficients.Select((t, i) => _coefficients[variablePosition][order, i]*points[i]).Sum();
            result /= Math.Pow(step, order);
            return result;
        }

        public double ComputePartialDerivative(functionDelegate function, Parameter parameter, int order)
        {
            var numberOfPoints = _coefficients.Length;
            double originalValue = parameter;
            var points = new double[numberOfPoints];
            var derivativeStepSize = parameter.DerivativeStepSize;
            var centerPoint = (numberOfPoints - 1) / 2;

            for (var i = 0; i < numberOfPoints; i++)
            {
                parameter.Value = originalValue + (i - centerPoint) * derivativeStepSize;
                points[i] = function();
            }
            var result = ComputeDerivative(points, order, centerPoint, derivativeStepSize);
            parameter.Value = originalValue;
            return result;
        }

        public double ComputePartialDerivative(functionDelegate function, Parameter parameter, int order, double currentFunctionValue)
        {
            var numberOfPoints = _coefficients.Length;
            var originalValue = parameter;
            var points = new double[numberOfPoints];
            var derivativeStepSize = parameter.DerivativeStepSize;
            var centerPoint = (numberOfPoints - 1) / 2;

            for (var i = 0; i < numberOfPoints; i++)
            {
                if (i != centerPoint)
                {
                    parameter.Value = originalValue + (i - centerPoint) * derivativeStepSize;
                    points[i] = function();
                }
                else
                {
                    points[i] = currentFunctionValue;
                }
            }
            double result = ComputeDerivative(points, order, centerPoint, derivativeStepSize);
            parameter.Value = originalValue;
            return result;
        }

        public double[] ComputePartialDerivatives(functionDelegate function, Parameter parameter, int[] derivativeOrders)
        {
            int numberOfPoints = _coefficients.Length;
            var result = new double[derivativeOrders.Length];
            double originalValue = parameter;
            var points = new double[numberOfPoints];
            double derivativeStepSize = parameter.DerivativeStepSize;
            int centerPoint = (numberOfPoints - 1) / 2;

            for (int i = 0; i < numberOfPoints; i++)
            {
                parameter.Value = originalValue + (i - centerPoint) * derivativeStepSize;
                points[i] = function();
            }
            for (int i = 0; i < derivativeOrders.Length; i++)
            {
                result[i] = ComputeDerivative(points, derivativeOrders[i], centerPoint, derivativeStepSize);
            }
            parameter.Value = originalValue;
            return result;
        }

        public double[] ComputePartialDerivatives(functionDelegate function, Parameter parameter, int[] derivativeOrders, double currentFunctionValue)
        {
            int numberOfPoints = _coefficients.Length;
            var result = new double[derivativeOrders.Length];
            double originalValue = parameter;
            var points = new double[numberOfPoints];
            double derivativeStepSize = parameter.DerivativeStepSize;
            int centerPoint = (numberOfPoints - 1) / 2;

            for (int i = 0; i < numberOfPoints; i++)
            {
                if (i != centerPoint)
                {
                    parameter.Value = originalValue + (i - centerPoint) * derivativeStepSize;
                    points[i] = function();
                }
                else
                {
                    points[i] = currentFunctionValue;
                }
            }
            for (int i = 0; i < derivativeOrders.Length; i++)
            {
                result[i] = ComputeDerivative(points, derivativeOrders[i], centerPoint, derivativeStepSize);
            }
            parameter.Value = originalValue;
            return result;
        }

    }
}
