using System.Diagnostics;

namespace Solver
{
    public delegate double functionDelegate();

    public class LevenbergMarquardt
    {
        private readonly Matrix _jacobian;
        private readonly Matrix _residuals;
        private readonly Matrix _regressionParameters0;
        private readonly Derivatives _derivatives;
        private readonly Parameter[] _regressionParameters;
        private readonly Parameter[] _observedParameters;

        private readonly functionDelegate _regressionFunction;
        private readonly double[,] _data;
        private double _l0 = 100.0;
        private const double _v = 10.0;


        public LevenbergMarquardt(functionDelegate regressionFunction, Parameter[] regressionParameters, Parameter[] observedParameters, double[,] data, int numberOfDerivativePoints)
        {
            Debug.Assert(data.GetLength(0) == observedParameters.Length + 1);
            _data = data;
            _observedParameters = observedParameters;
            _regressionParameters = regressionParameters;
            _regressionFunction = regressionFunction;
            int numberOfParameters = _regressionParameters.Length;
            int numberOfPoints = data.GetLength(1);

            _derivatives = new Derivatives(numberOfDerivativePoints);

            _jacobian = new Matrix(numberOfPoints, numberOfParameters);
            _residuals = new Matrix(numberOfPoints, 1);
            _regressionParameters0 = new Matrix(numberOfParameters, 1);
        }

        public LevenbergMarquardt(functionDelegate function, Parameter[] regressionParameters, Parameter[] observedParameters, double[,] data) :
            this(function, regressionParameters, observedParameters, data, 3)
        {
        }

        public double AverageError = 0;
        public void Iterate()
        {
            var numberOfPoints = _data.GetLength(1);
            var numberOfParameters = _regressionParameters.Length;

            AverageError = 0;

            var currentResidual = 0.0;
            for (int i = 0; i < numberOfPoints; i++)
            {
                for (int j = 0; j < _observedParameters.Length; j++)
                {
                    _observedParameters[j].Value = _data[j, i];
                }
                var functionValue = _regressionFunction();
                var residual = _data[_observedParameters.Length, i] - functionValue;
                _residuals[i, 0] = residual;
                currentResidual += residual * residual;
                AverageError += residual;
                for (var j = 0; j < numberOfParameters; j++)
                {
                    _jacobian[i, j] = _derivatives.ComputePartialDerivative(_regressionFunction, _regressionParameters[j], 1, functionValue);
                }
            }
            AverageError /= numberOfPoints;
            AverageError /= 100;
            for (int i = 0; i < numberOfParameters; i++)
            {
                _regressionParameters0[i, 0] = _regressionParameters[i];
            }

            var jacobianTranspose = _jacobian.Transpose();
            var jacobianTransposeResiduals = jacobianTranspose * _residuals;
            var jacobianTransposeJacobian = jacobianTranspose * _jacobian;
            var jacobianTransposeJacobianDiagnol = new Matrix(jacobianTransposeJacobian.RowCount, jacobianTransposeJacobian.RowCount);
            for (int i = 0; i < jacobianTransposeJacobian.RowCount; i++)
            {
                jacobianTransposeJacobianDiagnol[i, i] = jacobianTransposeJacobian[i, i];
            }

            double newResidual = currentResidual + 1.0;
            _l0 /= _v;
            while (newResidual > currentResidual)
            {
                newResidual = 0.0;
                _l0 *= _v;
                var matLHS = jacobianTransposeJacobian + _l0 * jacobianTransposeJacobianDiagnol;
                var delta = matLHS.SolveFor(jacobianTransposeResiduals);
                var newRegressionParameters = _regressionParameters0 + delta;

                for (var i = 0; i < numberOfParameters; i++)
                {
                    _regressionParameters[i].Value = newRegressionParameters[i, 0];
                }

                for (var i = 0; i < numberOfPoints; i++)
                {
                    for (int j = 0; j < _observedParameters.Length; j++)
                    {
                        _observedParameters[j].Value = _data[j, i];
                    }
                    var functionValue = _regressionFunction();
                    var residual = _data[_observedParameters.Length, i] - functionValue;
                    newResidual += residual * residual;
                }
            }
            _l0 /= _v;
        }
    }

    //public class LevenbergMarquardtXY
    //{
    //    protected Matrix _jacobian;
    //    protected Matrix _residuals;
    //    protected Matrix _regressionParameters0;
    //    protected Derivatives _derivatives;
    //    protected Parameter[] _regressionParameters;
    //    protected Func<double, double> _function;
    //    protected XYDataCollection _data;
    //    protected double _l0 = 100.0;
    //    protected double _v = 10.0;

    //    public LevenbergMarquardtXY(Func<double, double> function, ModelBase model, XYDataCollection data, int numberOfDerivativePoints)
    //    {
    //        _data = data;
    //        model.ResetParameters();
    //        _regressionParameters = model.GetSolvedForParameters();
    //        _function = function;
    //        int numberOfParameters = _regressionParameters.Length;
    //        int numberOfPoints = data.Count;

    //        _derivatives = new Derivatives(numberOfDerivativePoints);

    //        _jacobian = new Matrix(numberOfPoints, numberOfParameters);
    //        _residuals = new Matrix(numberOfPoints, 1);
    //        _regressionParameters0 = new Matrix(numberOfParameters, 1);
    //    }

    //    public LevenbergMarquardtXY(Func<double, double> function, ModelBase model, XYDataCollection data)
    //        : this(function, model, data, 3)
    //    {
    //    }

    //    public void Iterate()
    //    {
    //        int numberOfPoints = _data.Count;
    //        int numberOfParameters = _regressionParameters.Length;

    //        double currentResidual = 0.0;
    //        for (int i = 0; i < numberOfPoints; i++)
    //        {
    //            double x = _data[i].X;
    //            double y = _data[i].Y;
    //            double functionValue = _function(x);
    //            double residual = y - functionValue;
    //            _residuals[i, 0] = residual;
    //            currentResidual += residual * residual;
    //            for (int j = 0; j < numberOfParameters; j++)
    //            {
    //                _jacobian[i, j] = _derivatives.ComputePartialDerivative(() => _function(x), _regressionParameters[j], 1, functionValue);
    //            }
    //        }

    //        for (int i = 0; i < numberOfParameters; i++)
    //        {
    //            _regressionParameters0[i, 0] = _regressionParameters[i];
    //        }

    //        Matrix matJacobianTrans = _jacobian.Transpose();
    //        Matrix matJTranResid = matJacobianTrans * _residuals;
    //        Matrix matJTranJ = matJacobianTrans * _jacobian;
    //        Matrix matJTranJDiag = new Matrix(matJTranJ.RowCount, matJTranJ.RowCount);
    //        for (int i = 0; i < matJTranJ.RowCount; i++)
    //        {
    //            matJTranJDiag[i, i] = matJTranJ[i, i];
    //        }

    //        Matrix matDelta = null;
    //        Matrix matNewRegPars = null;

    //        double newResidual = currentResidual + 1.0;
    //        _l0 /= _v;
    //        while (newResidual > currentResidual)
    //        {
    //            newResidual = 0.0;
    //            _l0 *= _v;
    //            Matrix matLHS = matJTranJ + _l0 * matJTranJDiag;
    //            matDelta = matLHS.SolveFor(matJTranResid); ;
    //            matNewRegPars = _regressionParameters0 + matDelta;

    //            for (int i = 0; i < numberOfParameters; i++)
    //            {
    //                _regressionParameters[i].Value = matNewRegPars[i, 0];
    //            }

    //            for (int i = 0; i < numberOfPoints; i++)
    //            {
    //                double x = _data[i].X;
    //                double y = _data[i].Y;
    //                double functionValue = _function(x);
    //                double residual = y - functionValue;
    //                newResidual += residual * residual;
    //            }
    //        }
    //        _l0 /= _v;
    //    }
    //}
}
