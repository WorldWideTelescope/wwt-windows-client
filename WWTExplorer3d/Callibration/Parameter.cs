//Created by Trent F Guidry
//5/14/2010 5:34:20 AM #
// http://www.trentfguidry.net/post/2010/01/17/Latest-version.aspx
//Hi

//What's your license for your source?

//Thanks, Jim
//Jim | Reply
//7/3/2010 3:17:58 AM #

//I really don't know very much about licensing and it’s really not something that I am very interested in reading up on either.
 
//I guess I will just use that one that Microsoft uses for MEF, Prism, and the Silverlight Toolkit. That’s the Ms-PL one which can be found at http://silverlight.codeplex.com/license
 
//Trent | Reply
//9/30/2010 4:27:30 AM #

//Thanks very much! The Ms-PL is a really useful license.

//Best Regards, Jim
//Jim | Reply


using System;
using System.Collections.ObjectModel;

namespace Solver
{
    public class Parameter
    {
        private bool _isSolvedFor = true;
        private double _value;
        private double _derivativeStep = 1e-2;
        private DerivativeStepType _derivativeStepType = DerivativeStepType.Relative;

        public Parameter()
        {
        }

        public Parameter(double value)
            : this()
        {
            _value = value;
        }

        public Parameter(double value, double derivativeStep)
            : this(value)
        {
            _derivativeStep = derivativeStep;
        }

        public Parameter(double value, double derivativeStep, DerivativeStepType stepSizeType)
            : this(value, derivativeStep)
        {
            _derivativeStepType = stepSizeType;
        }

        public Parameter(double value, double derivativeStep, DerivativeStepType stepSizeType, bool isSolvedFor)
            : this(value, derivativeStep, stepSizeType)
        {
            _isSolvedFor = isSolvedFor;
        }

        public Parameter(bool isSolvedFor)
            : this()
        {
            _isSolvedFor = isSolvedFor;
        }

        public Parameter(Parameter clone)
        {
            _isSolvedFor = clone.IsSolvedFor;
            _value = clone.Value;
            _derivativeStep = clone.DerivativeStep;
            _derivativeStepType = clone.DerivativeStepType;
        }

        public bool IsSolvedFor
        {
            get { return _isSolvedFor; }
            set { _isSolvedFor = value; }
        }

        public double Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public double DerivativeStep
        {
            get { return _derivativeStep; }
            set { _derivativeStep = value; }
        }

        public double DerivativeStepSize
        {
            get
            {
                double derivativeStepSize;
                if (_derivativeStepType == DerivativeStepType.Absolute)
                {
                    derivativeStepSize = _derivativeStep;
                }
                else
                {
                    if (!_value.Equals(0.0))
                    {
                        derivativeStepSize = _derivativeStep * Math.Abs(_value);
                    }
                    else
                    {
                        derivativeStepSize = _derivativeStep;
                    }
                }
                return derivativeStepSize;
            }
        }

        public DerivativeStepType DerivativeStepType
        {
            get { return _derivativeStepType; }
            set { _derivativeStepType = value; }
        }

        public static implicit operator double(Parameter p)
        {
            return p.Value;
        }

        public override string ToString()
        {
            return "Parameter: Value:" + Value + " IsSolvedFor:" + _isSolvedFor;
        }
    }

    public enum DerivativeStepType
    {
        Relative,
        Absolute
    }

    public class ParameterCollection : Collection<Parameter>
    {
    }

}
