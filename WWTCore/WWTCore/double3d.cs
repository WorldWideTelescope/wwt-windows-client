using System;
using System.Collections.Generic;
using SharpDX;
using Quaternion = SharpDX.Quaternion;


namespace TerraViewer
{
    // Summary:
    //     Describes a custom vertex format structure that contains position and one
    //     set of texture coordinates.
    public struct PositionTexture
    {
        // Summary:
        //     Retrieves or sets the u component of the texture coordinate.
        public double Tu;
        //
        // Summary:
        //     Retrieves or sets the v component of the texture coordinate.
        public double Tv;
        //
        // Summary:
        //     Retrieves or sets the x component of the position.
        public double X;
        //
        // Summary:
        //     Retrieves or sets the y component of the position.
        public double Y;
        //
        // Summary:
        //     Retrieves or sets the z component of the position.
        public double Z;

     
        public PositionTexture(Vector3d pos, double u, double v)
        {
            Tu = u;
            Tv = v;
            X = pos.X;
            Y = pos.Y;
            Z = pos.Z;
        }
        //
        // Summary:
        //     Initializes a new instance of the .CustomVertex.PositionTextured
        //     class.
        //
        // Parameters:
        //   xvalue:
        //     Floating-point value that represents the x coordinate of the position.
        //
        //   yvalue:
        //     Floating-point value that represents the y coordinate of the position.
        //
        //   zvalue:
        //     Floating-point value that represents the z coordinate of the position.
        //
        //   u:
        //     Floating-point value that represents the CustomVertex.PositionTextured.#ctor()
        //     component of the texture coordinate.
        //
        //   v:
        //     Floating-point value that represents thePositionTextured.#ctor()
        //     component of the texture coordinate.
        public PositionTexture(double xvalue, double yvalue, double zvalue, double u, double v)
        {
            Tu = u;
            Tv = v;
            X = xvalue;
            Y = yvalue;
            Z = zvalue;
        }

        // Summary:
        //     Retrieves or sets the vertex position.
        public Vector3d Position
        {
            get
            {
                return new Vector3d(X, Y, Z);
            }
            set
            {
                X = value.X;
                Y = value.Y;
                Z = value.Z;
            }
        }
        public enum LocationHint { Slash, Backslash, Top };

        public PositionNormalTexturedX2 PositionNormalTextured(Vector3d center, bool backslash)
        {


            var latLng = Coordinates.CartesianToSpherical2(Position);
            //      latLng.Lng += 90;
            if (latLng.Lng < -180)
            {
                latLng.Lng += 360;
            }
            if (latLng.Lng > 180)
            {
                latLng.Lng -= 360;
            }
            if (latLng.Lng == -180 && !backslash)
            {
                latLng.Lng = 180;
            }
            if (latLng.Lng == 180 && backslash)
            {
                latLng.Lng = -180;
            }
            var pnt = new PositionNormalTexturedX2
            {
                X = (float) (X - center.X),
                Y = (float) (Y - center.Y),
                Z = (float) (Z - center.Z),
                Tu = (float) Tu,
                Tv = (float) Tv,
                Lng = latLng.Lng,
                Lat = latLng.Lat,
                Normal = Position
            };

            return pnt;

        }
        
        // Summary:
        //     Obtains a string representation of the current instance.
        //
        // Returns:
        //     String that represents the object.
        public override string ToString()
        {
            return String.Format("{0}, {1}, {2}, {3}, {4}", X, Y, Z, Tu, Tv);
        }



    }
    // Summary:
    //     Describes and manipulates a vector in three-dimensional (3-D) space.
    [Serializable]
    public struct Vector3d
    {
        // Summary:
        //     Retrieves or sets the x component of a 3-D vector.
        public double X;
        //
        // Summary:
        //     Retrieves or sets the y component of a 3-D vector.
        public double Y;
        //
        // Summary:
        //     Retrieves or sets the z component of a 3-D vector.
        public double Z;

        //
        // Summary:
        //     Initializes a new instance of the Vector3d class.
        //
        // Parameters:
        //   valueX:
        //     Initial .Vector3d.X value.
        //
        //   valueY:
        //     Initial .Vector3d.Y value.
        //
        //   valueZ:
        //     Initial Vector3d.Z value.
        public Vector3d(double valueX, double valueY, double valueZ)
        {
            X = valueX;
            Y = valueY;
            Z = valueZ;
        }
        public Vector3d(Vector3d value)
        {
            X = value.X;
            Y = value.Y;
            Z = value.Z;
        }

        public Vector3d(Vector3 value)
        {
            X = value.X;
            Y = value.Y;
            Z = value.Z;
        }

        // Summary:
        //     Negates the vector.
        //
        // Parameters:
        //   vec:
        //     Source Vector3d structure.
        //
        // Returns:
        //     The Vector3d structure that is the result of the operation.
        public static Vector3d operator -(Vector3d vec)
        {
            Vector3d result;
            result.X = -vec.X;
            result.Y = -vec.Y;
            result.Z = -vec.Z;
            return result;
        }

        //
        // Summary:
        //     Adds two 3-D vectors.
        //
        // Parameters:
        //   left:
        //     The Vector3d structure to the left of the subtraction operator.
        //
        //   right:
        //     The Vector3d structure to the right of the subtraction operator.
        //
        // Returns:
        //     Resulting Vector3d structure.
        public static Vector3d operator +(Vector3d left, Vector3d right)
        {
            return new Vector3d(left.X + right.X, left.Y + right.Y, left.Z + right.Z);
        }

        //
        // Summary:
        //     Subtracts two 3-D vectors.
        //
        // Parameters:
        //   left:
        //     The Vector3d structure to the left of the subtraction operator.
        //
        //   right:
        //     The Vector3d structure to the right of the subtraction operator.
        //
        // Returns:
        //     Resulting Vector3d structure.
        public static Vector3d operator -(Vector3d left, Vector3d right)
        {
            return new Vector3d(left.X - right.X, left.Y - right.Y, left.Z - right.Z);
        }
        //
        // Summary:
        //     Compares the current instance of a class to another instance to determine
        //     whether they are different.
        //
        // Parameters:
        //   left:
        //     The Vector3d structure to the left of the inequality operator.
        //
        //   right:
        //     The Vector3d structure to the right of the inequality operator.
        //
        // Returns:
        //     Value that is true if the objects are different, or false if they are the
        //     same.
        public static bool operator !=(Vector3d left, Vector3d right)
        {
            return (left.X != right.X || left.Y != right.Y || left.Z != right.Z);
        }
        //
        // Summary:
        //     Determines the product of a single value and a 3-D vector.
        //
        // Parameters:
        //   right:
        //     Source System.Single structure.
        //
        //   left:
        //     Source Vector3d structure.
        //
        // Returns:
        //     A Vector3d structure that is the product of the Vector3d.op_Multiply()
        //     and Vector3d.op_Multiply() parameters.
        //public static Vector3d operator *(double right, Vector3d left);
        //
        // Summary:
        //     Determines the product of a single value and a 3-D vector.
        //
        // Parameters:
        //   left:
        //     Source Vector3d structure.
        //
        //   right:
        //     Source System.Single structure.
        //
        // Returns:
        //     A Vector3d structure that is the product of the Vector3d.op_Multiply()
        //     and Vector3d.op_Multiply() parameters.
        //public static Vector3d operator *(Vector3d left, double right);
        //
        // Summary:
        //     Adds two vectors.
        //
        // Parameters:
        //   left:
        //     Source Vector3d structure.
        //
        //   right:
        //     Source Vector3d structure.
        //
        // Returns:
        //     A Vector3d structure that contains the sum of the parameters.
        //public static Vector3d operator +(Vector3d left, Vector3d right);
        //
        // Summary:
        //     Compares the current instance of a class to another instance to determine
        //     whether they are the same.
        //
        // Parameters:
        //   left:
        //     The Vector3d structure to the left of the equality operator.
        //
        //   right:
        //     The Vector3d structure to the right of the equality operator.
        //
        // Returns:
        //     Value that is true if the objects are the same, or false if they are different.
        public static bool operator ==(Vector3d left, Vector3d right)
        {
            return (left.X == right.X && left.Y == right.Y && left.Z == right.Z);
        }

        // Multplication by a scalar
        public static Vector3d operator *(Vector3d v, double s)
        {
            return new Vector3d(v.X * s, v.Y * s, v.Z * s);
        }

        // Multiplication by a scalar
        public static Vector3d operator *(double s, Vector3d v)
        {
            return new Vector3d(v.X * s, v.Y * s, v.Z * s);
        }

        // Division by a scalar
        public static Vector3d operator /(Vector3d v, double s)
        {
            return new Vector3d(v.X / s, v.Y / s, v.Z / s);
        }
  
        public static Vector3d MidPoint(Vector3d left, Vector3d right)
        {
            var result = new Vector3d((left.X + right.X) / 2, (left.Y + right.Y) / 2, (left.Z + right.Z) / 2);
            result.Normalize();
            return result;
        }
        public static Vector3d MidPointByLength(Vector3d left, Vector3d right)
        {
            var result = new Vector3d((left.X + right.X) / 2, (left.Y + right.Y) / 2, (left.Z + right.Z) / 2);
            result.Normalize();

            result.Multiply(left.Length());
            return result;
        }
        // Summary:
        //     Retrieves an empty 3-D vector.
        public static Vector3d Empty
        {
            get
            {
                return new Vector3d(0, 0, 0);
            }
        }

        // rounds to factor
        public void Round()
        {
            X = (int)(X*65536)/65536.0;
            Y = (int)(Y*65536)/65536.0;
            Z = (int)(Z*65536)/65536.0;
        }
        // Summary:
        //     Adds two 3-D vectors.
        //
        // Parameters:
        //   source:
        public void Add(Vector3d source)
        {
            X += source.X;
            Y += source.Y;
            Z += source.Z;
        }

        //
        // Summary:
        //     Adds two 3-D vectors.
        //
        // Parameters:
        //   left:
        //     Source Vector3d.
        //
        //   right:
        //     Source Vector3d.
        //
        // Returns:
        //     Sum of the two Vector3d structures.
        public static Vector3d Add(Vector3d left, Vector3d right)
        {
            return new Vector3d(left.X + right.X, left.Y + right.Y, left.Z + right.Z);
        }

        //
        // Summary:
        //     Returns a point in barycentric coordinates, using specified 3-D vectors.
        //
        // Parameters:
        //   v1:
        //     Source Vector3d structure.
        //
        //   v2:
        //     Source Vector3d structure.
        //
        //   v3:
        //     Source Vector3d structure.
        //
        //   f:
        //     Weighting factor. See Remarks.
        //
        //   g:
        //     Weighting factor. See Remarks.
        //
        // Returns:
        //     A Vector3d structure in barycentric coordinates.
        //public static Vector3d BaryCentric(Vector3d v1, Vector3d v2, Vector3d v3, double f, double g);
        //
        // Summary:
        //     Performs a Catmull-Rom interpolation using specified 3-D vectors.
        //
        // Parameters:
        //   position1:
        //     Source Vector3d structure that is a position vector.
        //
        //   position2:
        //     Source Vector3d structure that is a position vector.
        //
        //   position3:
        //     Source Vector3d structure that is a position vector.
        //
        //   position4:
        //     Source Vector3d structure that is a position vector.
        //
        //   weightingFactor:
        //     Weighting factor. See Remarks.
        //
        // Returns:
        //     A Vector3d structure that is the result of the Catmull-Rom
        //     interpolation.
        //public static Vector3d CatmullRom(Vector3d position1, Vector3d position2, Vector3d position3, Vector3d position4, double weightingFactor)
        //{
        //}
        //
        // Summary:
        //     Determines the cross product of two 3-D vectors.
        //
        // Parameters:
        //   left:
        //     Source Vector3d structure.
        //
        //   right:
        //     Source Vector3d structure.
        //
        // Returns:
        //     A Vector3d structure that is the cross product of two 3-D
        //     vectors.
        public static Vector3d Cross(Vector3d left, Vector3d right)
        {
            return new Vector3d(
                  left.Y * right.Z - left.Z * right.Y,
                  left.Z * right.X - left.X * right.Z,
                  left.X * right.Y - left.Y * right.X);

        }
        //
        // Summary:
        //     Determines the dot product of two 3-D vectors.
        //
        // Parameters:
        //   left:
        //     Source Vector3d structure.
        //
        //   right:
        //     Source Vector3d structure.
        //
        // Returns:
        //     A System.Single value that is the dot product.
        public static double Dot(Vector3d left, Vector3d right)
        {
            return left.X * right.X + left.Y * right.Y + left.Z * right.Z;
        }
        //
        // Summary:
        //     Returns a value that indicates whether the current instance is equal to a
        //     specified object.
        //
        // Parameters:
        //   compare:
        //     Object with which to make the comparison.
        //
        // Returns:
        //     Value that is true if the current instance is equal to the specified object,
        //     or false if it is not.
        public override bool Equals(object compare)
        {
            var comp = (Vector3d)compare;
            return X == comp.X && Y == comp.Y && Z == comp.Z;
        }
        //
        // Summary:
        //     Returns the hash code for the current instance.
        //
        // Returns:
        //     Hash code for the instance.
        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode();
        }
        //
        // Summary:
        //     Performs a Hermite spline interpolation using the specified 3-D vectors.
        //
        // Parameters:
        //   position:
        //     Source Vector3d structure that is a position vector.
        //
        //   tangent:
        //     Source Vector3d structure that is a tangent vector.
        //
        //   position2:
        //     Source Vector3d structure that is a position vector.
        //
        //   tangent2:
        //     Source Vector3d structure that is a tangent vector.
        //
        //   weightingFactor:
        //     Weighting factor. See Remarks.
        //
        // Returns:
        //     A Vector3d structure that is the result of the Hermite spline
        //     interpolation.
        //public static Vector3d Hermite(Vector3d position, Vector3d tangent, Vector3d position2, Vector3d tangent2, double weightingFactor);
        //
        // Summary:
        //     Returns the length of a 3-D vector.
        //
        // Returns:
        //     A System.Single value that contains the vector's length.
        public double Length()
        {
            return Math.Sqrt(X * X + Y * Y + Z * Z);
        }
        //
        // Summary:
        //     Returns the length of a 3-D vector.
        //
        // Parameters:
        //   source:
        //     Source Vector3d structure.
        //
        // Returns:
        //     A System.Single value that contains the vector's length.
        public static double Length(Vector3d source)
        {
            return Math.Sqrt(source.X * source.X + source.Y * source.Y + source.Z * source.Z);

        }
        //
        // Summary:
        //     Returns the square of the length of a 3-D vector.
        //
        // Returns:
        //     A System.Single value that contains the vector's squared length.
        public double LengthSq()
        {
            return X * X + Y * Y + Z * Z;
        }
        //
        // Summary:
        //     Returns the square of the length of a 3-D vector.
        //
        // Parameters:
        //   source:
        //     Source Vector3d structure.
        //
        // Returns:
        //     A System.Single value that contains the vector's squared length.
        public static double LengthSq(Vector3d source)
        {
            return source.X * source.X + source.Y * source.Y + source.Z * source.Z;
        }

        //
        // Summary:
        //     Performs a linear interpolation between two 3-D vectors.
        //
        // Parameters:
        //   left:
        //     Source Vector3d structure.
        //
        //   right:
        //     Source Vector3d structure.
        //
        //   interpolater:
        //     Parameter that linearly interpolates between the vectors.
        //
        // Returns:
        //     A Vector3d structure that is the result of the linear interpolation.
        public static Vector3d Lerp(Vector3d left, Vector3d right, double interpolater)
        {
            return new Vector3d(
                left.X * (1.0 - interpolater) + right.X * interpolater,
                left.Y * (1.0 - interpolater) + right.Y * interpolater,
                left.Z * (1.0 - interpolater) + right.Z * interpolater);

        }

        public static Vector3d Midpoint(Vector3d left, Vector3d right)
        {
            var tmp = new Vector3d(
                left.X * (.5) + right.X * .5,
                left.Y * (.5) + right.Y * .5,
                left.Z * (.5) + right.Z * .5);
            tmp.Normalize();
            return tmp;
        }


        public static Vector3d Slerp(Vector3d left, Vector3d right, double interpolater)
        {
            var dot = Dot(left, right);
            while (dot < .98)
            {
                var middle = Midpoint(left, right);
                if (interpolater > .5)
                {
                    left = middle;
                    interpolater -= .5;
                    interpolater *= 2;
                }
                else
                {
                    right = middle;
                    interpolater *= 2;
                }
                dot = Dot(left, right);
            }

            var tmp = Lerp(left, right, interpolater);
            tmp.Normalize();
            return tmp;
        }



        //
        // Summary:
        //     Returns a 3-D vector that is made up of the largest components of two 3-D
        //     vectors.
        //
        // Parameters:
        //   source:
        //     Source Vector3d structure.
        //public void Maximize(Vector3d source);
        //
        // Summary:
        //     Returns a 3-D vector that is made up of the largest components of two 3-D
        //     vectors.
        //
        // Parameters:
        //   left:
        //     Source Vector3d structure.
        //
        //   right:
        //     Source Vector3d structure.
        //
        // Returns:
        //     A Vector3d structure that is made up of the largest components
        //     of the two vectors.
        //public static Vector3d Maximize(Vector3d left, Vector3d right);
        //
        // Summary:
        //     Returns a 3-D vector that is made up of the smallest components of two 3-D
        //     vectors.
        //
        // Parameters:
        //   source:
        //     Source Vector3d structure.
        //public void Minimize(Vector3d source);
        //
        // Summary:
        //     Returns a 3-D vector that is made up of the smallest components of two 3-D
        //     vectors.
        //
        // Parameters:
        //   left:
        //     Source Vector3d structure.
        //
        //   right:
        //     Source Vector3d structure.
        //
        // Returns:
        //     A Vector3d structure that is made up of the smallest components
        //     of the two vectors.
        //public static Vector3d Minimize(Vector3d left, Vector3d right);
        //
        // Summary:
        //     Multiplies a 3-D vector by a System.Single value.
        //
        // Parameters:
        //   s:
        //     Source System.Single value used as a multiplier.
        public void Multiply(double s)
        {
            X *= s;
            Y *= s;
            Z *= s;
        }
        //
        // Summary:
        //     Multiplies a 3-D vector by a System.Single value.
        //
        // Parameters:
        //   source:
        //     Source Vector3d structure.
        //
        //   f:
        //     Source System.Single value used as a multiplier.
        //
        // Returns:
        //     A Vector3d structure that is multiplied by the System.Single
        //     value.
        public static Vector3d Multiply(Vector3d source, double f)
        {
            var result = new Vector3d(source);
            result.Multiply(f);
            return result;
        }
        //
        // Summary:
        //     Returns the normalized version of a 3-D vector.
        public void Normalize()
        {
            // Vector3.Length property is under length section
            var length = Length();
            if (length != 0)
            {
                X /= length;
                Y /= length;
                Z /= length;
            }
        }

        //
        // Summary:
        //     Scales a 3-D vector.
        //
        // Parameters:
        //   source:
        //     Source Vector3d structure.
        //
        //   scalingFactor:
        //     Scaling value.
        //
        // Returns:
        //     A Vector3d structure that is the scaled vector.
        public static Vector3d Scale(Vector3d source, double scalingFactor)
        {
            var result = source;
            result.Multiply(scalingFactor);
            return result;
        }

        public void RotateX(double radians)
        {
            //radians = -radians;
            double yTemp = Y * Math.Cos(radians) - Z * Math.Sin(radians);
            double zTemp = Y * Math.Sin(radians) + Z * Math.Cos(radians);
            Z = zTemp;
            Y = yTemp;
        }

        public void RotateZ(double radians)
        {
            //radians = -radians;
            double xTemp = X * Math.Cos(radians) - Y * Math.Sin(radians);
            double yTemp = X * Math.Sin(radians) + Y * Math.Cos(radians);
            Y = yTemp;
            X = xTemp;
        }

        public void RotateY(double radians)
        {
            //radians = -radians;
            double zTemp = Z * Math.Cos(radians) - X * Math.Sin(radians);
            double xTemp = Z * Math.Sin(radians) + X * Math.Cos(radians);
            X = xTemp;
            Z = zTemp;
        }   
        //
        // Summary:
        //     Subtracts two 3-D vectors.
        //
        // Parameters:
        //   source:
        //     Source Vector3d structure to subtract from the current instance.
        public void Subtract(Vector3d source)
        {
            X -= source.X;
            Y -= source.Y;
            Z -= source.Z;

        }
        //
        // Summary:
        //     Subtracts two 3-D vectors.
        //
        // Parameters:
        //   left:
        //     Source Vector3d structure to the left of the subtraction
        //     operator.
        //
        //   right:
        //     Source Vector3d structure to the right of the subtraction
        //     operator.
        //
        // Returns:
        //     A Vector3d structure that is the result of the operation.
        public static Vector3d Subtract(Vector3d left, Vector3d right)
        {
            var result = left;
            result.Subtract(right);
            return result;
        }
        //
        // Summary:
        //     Obtains a string representation of the current instance.
        //
        // Returns:
        //     String that represents the object.
        public override string ToString()
        {
            return String.Format("{0}, {1}, {2}", X, Y, Z);
        }

        public static Vector3d Parse(string data)
        {
            var newVector = new Vector3d();

            var list = data.Split(new[]{','});
            if (list.Length == 3)
            {
                newVector.X = double.Parse(list[0]);
                newVector.Y = double.Parse(list[1]);
                newVector.Z = double.Parse(list[2]);
            }
            return newVector;
        }

        public Vector2d ToSpherical()
        {

            double ascention;
            double declination;

            var radius = Math.Sqrt(X * X + Y * Y + Z * Z);
            var XZ = Math.Sqrt(X * X + Z * Z);
            declination = Math.Asin(Y / radius);
            if (XZ == 0)
            {
                ascention = 0;
            }
            else if (0 <= X)
            {
                ascention = Math.Asin(Z / XZ);
            }
            else
            {
                ascention = Math.PI - Math.Asin(Z / XZ);
            }

            //if (vector.Z < 0)
            //{
            //    ascention = ascention - Math.PI;
            //}
            // 0 -1.0         return new Vector2d((((ascention + Math.PI) / (2.0 * Math.PI)) % 1.0f), ((declination + (Math.PI / 2.0)) / (Math.PI)));
            return new Vector2d((((ascention + Math.PI) % (2.0 * Math.PI))), ((declination + (Math.PI / 2.0))));

        }
        public Vector2d ToRaDec()
        {
            var point = ToSpherical();
            point.X = point.X / Math.PI * 12;
            point.Y = (point.Y / Math.PI * 180) - 90;

            if (double.IsNaN(point.X) || double.IsNaN(point.Y))
            {
                point.X = point.Y = 0;
            }
            return point;
        }



        public double DistanceToLine(Vector3d x1, Vector3d x2)
        {
            var t1 = x2 - x1;
            var t2 = x1 - this;
            var t3 = Cross(t1, t2);
            var d1 = t3.Length();
            var t4 = x2 - x1;
            var d2 = t4.Length();
            return d1 / d2;

        }
        public Vector3 Vector3
        {
            get
            {
                return new Vector3((float)X, (float)Y, (float)Z);
            }
        }
        public Vector4 Vector4
        {
            get
            {
                return new Vector4((float)X, (float)Y, (float)Z, 1f);
            }
        }
        public Vector3 Vector311
        {
            get
            {
                return new Vector3((float)X, (float)Y, (float)Z);
            }
        }

        public void TransformCoordinate(Matrix3d lookAtAdjust)
        {
            this = lookAtAdjust.Transform(this);
        }

        public static Vector3d TransformCoordinate(Vector3d vector3d, Matrix3d mat)
        {
            return mat.Transform(vector3d);
        }


        public static Vector3d GetMinCoordinate(IEnumerable<Vector3d> points)
        {
            var min = new Vector3d(double.MaxValue, double.MaxValue, double.MaxValue);

            foreach (var point in points)
            {
                min.X = Math.Min(min.X, point.X);
                min.Y = Math.Min(min.Y, point.Y);
                min.Z = Math.Min(min.Z, point.Z);
            }

            return min;
        }

        /// <summary>
        /// Returns the maximum point in X,Y,Z for a collection of Point3D
        /// </summary>
        /// <param name="points"></param>
        /// <returns>Returns double min value if no data is provided</returns>
        public static Vector3d GetMaxCoordinate(IEnumerable<Vector3d> points)
        {
            var max = new Vector3d(double.MinValue, double.MinValue, double.MinValue);

            foreach (var point in points)
            {
                max.X = Math.Max(max.X, point.X);
                max.Y = Math.Max(max.Y, point.Y);
                max.Z = Math.Max(max.Z, point.Z);
            }

            return max;
        }
    }
    public struct Vector2d
    {
        public double X;
        public double Y;
        public Vector2d(double x, double y)
        {
            X = x;
            Y = y;
        }
        public static Vector2d Lerp(Vector2d left, Vector2d right, double interpolater)
        {
            if (Math.Abs(left.X - right.X) > 12)
            {
                if (left.X > right.X)
                {
                    right.X += 24;
                }
                else
                {
                    left.X += 24;
                }
            }
            return new Vector2d(left.X * (1 - interpolater) + right.X * interpolater, left.Y * (1 - interpolater) + right.Y * interpolater);

        }

        static public Vector2d CartesianToSpherical2(Vector3d vector)
        {
            var rho = Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z);
            var longitude = Math.Atan2(vector.Z, vector.X);
            var latitude = Math.Asin(vector.Y / rho);

            return new Vector2d(longitude / Math.PI * 180.0, latitude / Math.PI * 180.0);

        }

        public double Distance3d(Vector2d pointB)
        {
            var pnt1 = Coordinates.GeoTo3dDouble(pointB.Y, pointB.X);
            var pnt2 = Coordinates.GeoTo3dDouble(Y, X);

            var pntDiff = pnt1 - pnt2;

            return pntDiff.Length() /Math.PI * 180;
        }

        public static Vector2d Average3d(Vector2d left, Vector2d right)
        {
            var pntLeft = Coordinates.GeoTo3dDouble(left.Y, left.X);
            var pntRight = Coordinates.GeoTo3dDouble(right.Y, right.X);

            var pntOut = Vector3d.Add(pntLeft, pntRight);
            pntOut.Multiply(.5);
            pntOut.Normalize();

            return CartesianToSpherical2(pntOut);

        }

        public double Length
        {
            get
            {
                return (Math.Sqrt(X * X + Y * Y));
            }
        }

        public static Vector2d operator -(Vector2d vec)
        {
            Vector2d result;
            result.X = -vec.X;
            result.Y = -vec.Y;
            return result;
        }

        public static Vector2d operator -(Vector2d left, Vector2d right)
        {
            return new Vector2d(left.X - right.X, left.Y - right.Y);
        }

        public static Vector2d operator +(Vector2d vec)
        {
            Vector2d result;
            result.X = +vec.X;
            result.Y = +vec.Y;
            return result;
        }

        public static Vector2d operator +(Vector2d left, Vector2d right)
        {
            return new Vector2d(left.X + right.X, left.Y + right.Y);
        }

        public void Normalize()
        {
 
            // Vector3.Length property is under length section
            var length = Length;
            if (length != 0)
            {
                X /= length;
                Y /= length;
            }
        }

        public void Scale(double scaler)
        {
            X *= scaler;
            Y *= scaler;
        }
    }
    public struct Matrix3d : IFormattable
    {
        private double _m11;
        private double _m12;
        private double _m13;
        private double _m14;
        private double _m21;
        private double _m22;
        private double _m23;
        private double _m24;
        private double _m31;
        private double _m32;
        private double _m33;
        private double _m34;
        private double _offsetX;
        private double _offsetY;
        private double _offsetZ;
        private double _m44;
        private bool _isNotKnownToBeIdentity;

        private static readonly Matrix3d s_identity;

        public Matrix3d(double m11, double m12, double m13, double m14, double m21, double m22, double m23, double m24, double m31, double m32, double m33, double m34, double offsetX, double offsetY, double offsetZ, double m44)
        {
            _m11 = m11;
            _m12 = m12;
            _m13 = m13;
            _m14 = m14;
            _m21 = m21;
            _m22 = m22;
            _m23 = m23;
            _m24 = m24;
            _m31 = m31;
            _m32 = m32;
            _m33 = m33;
            _m34 = m34;
            _offsetX = offsetX;
            _offsetY = offsetY;
            _offsetZ = offsetZ;
            _m44 = m44;
            _isNotKnownToBeIdentity = true;
        }

        public Matrix Matrix 
        {
            get
            {
                var mat = new Matrix
                {
                    M11 = (float) _m11,
                    M12 = (float) _m12,
                    M13 = (float) _m13,
                    M14 = (float) _m14,
                    M21 = (float) _m21,
                    M22 = (float) _m22,
                    M23 = (float) _m23,
                    M24 = (float) _m24,
                    M31 = (float) _m31,
                    M32 = (float) _m32,
                    M33 = (float) _m33,
                    M34 = (float) _m34,
                    M41 = (float) _offsetX,
                    M42 = (float) _offsetY,
                    M43 = (float) _offsetZ,
                    M44 = (float) _m44
                };
                return mat;
            }
            set
            {
                this = s_identity;
                IsDistinguishedIdentity = false;
                _m11 = value.M11;
                _m12 = value.M12;
                _m13 = value.M13;
                _m14 = value.M14;
                _m21 = value.M21;
                _m22 = value.M22;
                _m23 = value.M23;
                _m24 = value.M24;
                _m31 = value.M31;
                _m32 = value.M32;
                _m33 = value.M33;
                _m34 = value.M34;
                _offsetX = value.M41;
                _offsetY = value.M42;
                _offsetZ = value.M43;
                _m44 = value.M44;
            }
        }

        public Matrix Matrix11
        {
            get
            {
                var mat = new Matrix
                {
                    M11 = (float) _m11,
                    M12 = (float) _m12,
                    M13 = (float) _m13,
                    M14 = (float) _m14,
                    M21 = (float) _m21,
                    M22 = (float) _m22,
                    M23 = (float) _m23,
                    M24 = (float) _m24,
                    M31 = (float) _m31,
                    M32 = (float) _m32,
                    M33 = (float) _m33,
                    M34 = (float) _m34,
                    M41 = (float) _offsetX,
                    M42 = (float) _offsetY,
                    M43 = (float) _offsetZ,
                    M44 = (float) _m44
                };
                return mat;
            }
            set
            {
                this = s_identity;
                IsDistinguishedIdentity = false;
                _m11 = value.M11;
                _m12 = value.M12;
                _m13 = value.M13;
                _m14 = value.M14;
                _m21 = value.M21;
                _m22 = value.M22;
                _m23 = value.M23;
                _m24 = value.M24;
                _m31 = value.M31;
                _m32 = value.M32;
                _m33 = value.M33;
                _m34 = value.M34;
                _offsetX = value.M41;
                _offsetY = value.M42;
                _offsetZ = value.M43;
                _m44 = value.M44;
            }
        }

        public static Matrix3d Identity
        {
            get
            {
                return s_identity;
            }
        }

        public void SetIdentity()
        {
            this = s_identity;
        }

        public bool IsIdentity
        {
            get
            {
                if (IsDistinguishedIdentity)
                {
                    return true;
                }
                if (((((_m11 == 1) && (_m12 == 0)) && ((_m13 == 0) && (_m14 == 0))) && (((_m21 == 0) && (_m22 == 1)) && ((_m23 == 0) && (_m24 == 0)))) && ((((_m31 == 0) && (_m32 == 0)) && ((_m33 == 1) && (_m34 == 0))) && (((_offsetX == 0) && (_offsetY == 0)) && ((_offsetZ == 0) && (_m44 == 1)))))
                {
                    IsDistinguishedIdentity = true;
                    return true;
                }
                return false;
            }
        }

        public void Prepend(Matrix3d matrix)
        {
            this = matrix * this;
        }

        public void Append(Matrix3d matrix)
        {
            this *= matrix;
        }

        public void Rotate(Quaternion quaternion)
        {
            var center = new Vector3d();
            this *= CreateRotationMatrix(ref quaternion, ref center);
        }

        public void RotatePrepend(Quaternion quaternion)
        {
            var center = new Vector3d();
            this = CreateRotationMatrix(ref quaternion, ref center) * this;
        }

        public void RotateAt(Quaternion quaternion, Vector3d center)
        {
            this *= CreateRotationMatrix(ref quaternion, ref center);
        }

        public void RotateAtPrepend(Quaternion quaternion, Vector3d center)
        {
            this = CreateRotationMatrix(ref quaternion, ref center) * this;
        }

        public void Scale(Vector3d scale)
        {
            if (IsDistinguishedIdentity)
            {
                SetScaleMatrix(ref scale);
            }
            else
            {
                _m11 *= scale.X;
                _m12 *= scale.Y;
                _m13 *= scale.Z;
                _m21 *= scale.X;
                _m22 *= scale.Y;
                _m23 *= scale.Z;
                _m31 *= scale.X;
                _m32 *= scale.Y;
                _m33 *= scale.Z;
                _offsetX *= scale.X;
                _offsetY *= scale.Y;
                _offsetZ *= scale.Z;
            }
        }

        public void ScalePrepend(Vector3d scale)
        {
            if (IsDistinguishedIdentity)
            {
                SetScaleMatrix(ref scale);
            }
            else
            {
                _m11 *= scale.X;
                _m12 *= scale.X;
                _m13 *= scale.X;
                _m14 *= scale.X;
                _m21 *= scale.Y;
                _m22 *= scale.Y;
                _m23 *= scale.Y;
                _m24 *= scale.Y;
                _m31 *= scale.Z;
                _m32 *= scale.Z;
                _m33 *= scale.Z;
                _m34 *= scale.Z;
            }
        }

        public void ScaleAt(Vector3d scale, Vector3d center)
        {
            if (IsDistinguishedIdentity)
            {
                SetScaleMatrix(ref scale, ref center);
            }
            else
            {
                var num = _m14 * center.X;
                _m11 = num + (scale.X * (_m11 - num));
                num = _m14 * center.Y;
                _m12 = num + (scale.Y * (_m12 - num));
                num = _m14 * center.Z;
                _m13 = num + (scale.Z * (_m13 - num));
                num = _m24 * center.X;
                _m21 = num + (scale.X * (_m21 - num));
                num = _m24 * center.Y;
                _m22 = num + (scale.Y * (_m22 - num));
                num = _m24 * center.Z;
                _m23 = num + (scale.Z * (_m23 - num));
                num = _m34 * center.X;
                _m31 = num + (scale.X * (_m31 - num));
                num = _m34 * center.Y;
                _m32 = num + (scale.Y * (_m32 - num));
                num = _m34 * center.Z;
                _m33 = num + (scale.Z * (_m33 - num));
                num = _m44 * center.X;
                _offsetX = num + (scale.X * (_offsetX - num));
                num = _m44 * center.Y;
                _offsetY = num + (scale.Y * (_offsetY - num));
                num = _m44 * center.Z;
                _offsetZ = num + (scale.Z * (_offsetZ - num));
            }
        }

        public void ScaleAtPrepend(Vector3d scale, Vector3d center)
        {
            if (IsDistinguishedIdentity)
            {
                SetScaleMatrix(ref scale, ref center);
            }
            else
            {
                var num3 = center.X - (center.X * scale.X);
                var num2 = center.Y - (center.Y * scale.Y);
                var num = center.Z - (center.Z * scale.Z);
                _offsetX += ((_m11 * num3) + (_m21 * num2)) + (_m31 * num);
                _offsetY += ((_m12 * num3) + (_m22 * num2)) + (_m32 * num);
                _offsetZ += ((_m13 * num3) + (_m23 * num2)) + (_m33 * num);
                _m44 += ((_m14 * num3) + (_m24 * num2)) + (_m34 * num);
                _m11 *= scale.X;
                _m12 *= scale.X;
                _m13 *= scale.X;
                _m14 *= scale.X;
                _m21 *= scale.Y;
                _m22 *= scale.Y;
                _m23 *= scale.Y;
                _m24 *= scale.Y;
                _m31 *= scale.Z;
                _m32 *= scale.Z;
                _m33 *= scale.Z;
                _m34 *= scale.Z;
            }
        }

        public void Translate(Vector3d offset)
        {
            if (IsDistinguishedIdentity)
            {
                SetTranslationMatrix(ref offset);
            }
            else
            {
                _m11 += _m14 * offset.X;
                _m12 += _m14 * offset.Y;
                _m13 += _m14 * offset.Z;
                _m21 += _m24 * offset.X;
                _m22 += _m24 * offset.Y;
                _m23 += _m24 * offset.Z;
                _m31 += _m34 * offset.X;
                _m32 += _m34 * offset.Y;
                _m33 += _m34 * offset.Z;
                _offsetX += _m44 * offset.X;
                _offsetY += _m44 * offset.Y;
                _offsetZ += _m44 * offset.Z;
            }
        }

        public void TranslatePrepend(Vector3d offset)
        {
            if (IsDistinguishedIdentity)
            {
                SetTranslationMatrix(ref offset);
            }
            else
            {
                _offsetX += ((_m11 * offset.X) + (_m21 * offset.Y)) + (_m31 * offset.Z);
                _offsetY += ((_m12 * offset.X) + (_m22 * offset.Y)) + (_m32 * offset.Z);
                _offsetZ += ((_m13 * offset.X) + (_m23 * offset.Y)) + (_m33 * offset.Z);
                _m44 += ((_m14 * offset.X) + (_m24 * offset.Y)) + (_m34 * offset.Z);
            }
        }

        public static Matrix3d operator *(Matrix3d matrix1, Matrix3d matrix2)
        {
            if (matrix1.IsDistinguishedIdentity)
            {
                return matrix2;
            }
            if (matrix2.IsDistinguishedIdentity)
            {
                return matrix1;
            }
            return new Matrix3d((((matrix1._m11 * matrix2._m11) + (matrix1._m12 * matrix2._m21)) + (matrix1._m13 * matrix2._m31)) + (matrix1._m14 * matrix2._offsetX), (((matrix1._m11 * matrix2._m12) + (matrix1._m12 * matrix2._m22)) + (matrix1._m13 * matrix2._m32)) + (matrix1._m14 * matrix2._offsetY), (((matrix1._m11 * matrix2._m13) + (matrix1._m12 * matrix2._m23)) + (matrix1._m13 * matrix2._m33)) + (matrix1._m14 * matrix2._offsetZ), (((matrix1._m11 * matrix2._m14) + (matrix1._m12 * matrix2._m24)) + (matrix1._m13 * matrix2._m34)) + (matrix1._m14 * matrix2._m44), (((matrix1._m21 * matrix2._m11) + (matrix1._m22 * matrix2._m21)) + (matrix1._m23 * matrix2._m31)) + (matrix1._m24 * matrix2._offsetX), (((matrix1._m21 * matrix2._m12) + (matrix1._m22 * matrix2._m22)) + (matrix1._m23 * matrix2._m32)) + (matrix1._m24 * matrix2._offsetY), (((matrix1._m21 * matrix2._m13) + (matrix1._m22 * matrix2._m23)) + (matrix1._m23 * matrix2._m33)) + (matrix1._m24 * matrix2._offsetZ), (((matrix1._m21 * matrix2._m14) + (matrix1._m22 * matrix2._m24)) + (matrix1._m23 * matrix2._m34)) + (matrix1._m24 * matrix2._m44), (((matrix1._m31 * matrix2._m11) + (matrix1._m32 * matrix2._m21)) + (matrix1._m33 * matrix2._m31)) + (matrix1._m34 * matrix2._offsetX), (((matrix1._m31 * matrix2._m12) + (matrix1._m32 * matrix2._m22)) + (matrix1._m33 * matrix2._m32)) + (matrix1._m34 * matrix2._offsetY), (((matrix1._m31 * matrix2._m13) + (matrix1._m32 * matrix2._m23)) + (matrix1._m33 * matrix2._m33)) + (matrix1._m34 * matrix2._offsetZ), (((matrix1._m31 * matrix2._m14) + (matrix1._m32 * matrix2._m24)) + (matrix1._m33 * matrix2._m34)) + (matrix1._m34 * matrix2._m44), (((matrix1._offsetX * matrix2._m11) + (matrix1._offsetY * matrix2._m21)) + (matrix1._offsetZ * matrix2._m31)) + (matrix1._m44 * matrix2._offsetX), (((matrix1._offsetX * matrix2._m12) + (matrix1._offsetY * matrix2._m22)) + (matrix1._offsetZ * matrix2._m32)) + (matrix1._m44 * matrix2._offsetY), (((matrix1._offsetX * matrix2._m13) + (matrix1._offsetY * matrix2._m23)) + (matrix1._offsetZ * matrix2._m33)) + (matrix1._m44 * matrix2._offsetZ), (((matrix1._offsetX * matrix2._m14) + (matrix1._offsetY * matrix2._m24)) + (matrix1._offsetZ * matrix2._m34)) + (matrix1._m44 * matrix2._m44));
        }

        public static Matrix3d Multiply(Matrix3d matrix1, Matrix3d matrix2)
        {
            return (matrix1 * matrix2);
        }

        public Vector3d Transform(Vector3d point)
        {
            MultiplyPoint(ref point);
            return point;
        }

        public void Transform(Vector3d[] points)
        {
            if (points != null)
            {
                for (var i = 0; i < points.Length; i++)
                {
                    MultiplyPoint(ref points[i]);
                }
            }
        }

        //public Vector3d Transform(Vector3d vector)
        //{
        //    this.MultiplyVector(ref vector);
        //    return vector;
        //}

        //public void Transform(Vector3d[] vectors)
        //{
        //    if (vectors != null)
        //    {
        //        for (int i = 0; i < vectors.Length; i++)
        //        {
        //            this.MultiplyVector(ref vectors[i]);
        //        }
        //    }
        //}

        public bool IsAffine
        {
            get
            {
                if (IsDistinguishedIdentity)
                {
                    return true;
                }
                if (((_m14 == 0) && (_m24 == 0)) && (_m34 == 0))
                {
                    return (_m44 == 1);
                }
                return false;
            }
        }

        public double Determinant
        {
            get
            {
                if (IsDistinguishedIdentity)
                {
                    return 1;
                }
                if (IsAffine)
                {
                    return GetNormalizedAffineDeterminant();
                }
                var num6 = (_m13 * _m24) - (_m23 * _m14);
                var num5 = (_m13 * _m34) - (_m33 * _m14);
                var num4 = (_m13 * _m44) - (_offsetZ * _m14);
                var num3 = (_m23 * _m34) - (_m33 * _m24);
                var num2 = (_m23 * _m44) - (_offsetZ * _m24);
                var num = (_m33 * _m44) - (_offsetZ * _m34);
                var num10 = ((_m22 * num5) - (_m32 * num6)) - (_m12 * num3);
                var num9 = ((_m12 * num2) - (_m22 * num4)) + (_offsetY * num6);
                var num8 = ((_m32 * num4) - (_offsetY * num5)) - (_m12 * num);
                var num7 = ((_m22 * num) - (_m32 * num2)) + (_offsetY * num3);
                return ((((_offsetX * num10) + (_m31 * num9)) + (_m21 * num8)) + (_m11 * num7));
            }
        }

        public bool HasInverse
        {
            get
            {
                return !DoubleUtilities.IsZero(Determinant);
            }
        }
        public void Invert()
        {
            if (!InvertCore())
            {
                throw new InvalidOperationException();
            }
        }

        public void Transpose()
        {
            Swap(ref _m12, ref _m21);
            Swap(ref _m13, ref _m31);
            Swap(ref _m14, ref _offsetX);
            Swap(ref _m23, ref _m32);
            Swap(ref _m24, ref _offsetY);
            Swap(ref _m34, ref _offsetZ);
        }

        private static void Swap(ref double a, ref double b)
        {
            var temp = a;
            a = b;
            b = temp;
        }

        public double M11
        {
            get
            {
                if (IsDistinguishedIdentity)
                {
                    return 1;
                }
                return _m11;
            }
            set
            {
                if (IsDistinguishedIdentity)
                {
                    this = s_identity;
                    IsDistinguishedIdentity = false;
                }
                _m11 = value;
            }
        }

        public double M12
        {
            get
            {
                return _m12;
            }
            set
            {
                if (IsDistinguishedIdentity)
                {
                    this = s_identity;
                    IsDistinguishedIdentity = false;
                }
                _m12 = value;
            }
        }

        public double M13
        {
            get
            {
                return _m13;
            }
            set
            {
                if (IsDistinguishedIdentity)
                {
                    this = s_identity;
                    IsDistinguishedIdentity = false;
                }
                _m13 = value;
            }
        }

        public double M14
        {
            get
            {
                return _m14;
            }
            set
            {
                if (IsDistinguishedIdentity)
                {
                    this = s_identity;
                    IsDistinguishedIdentity = false;
                }
                _m14 = value;
            }
        }

        public double M21
        {
            get
            {
                return _m21;
            }
            set
            {
                if (IsDistinguishedIdentity)
                {
                    this = s_identity;
                    IsDistinguishedIdentity = false;
                }
                _m21 = value;
            }
        }
        public double M22
        {
            get
            {
                if (IsDistinguishedIdentity)
                {
                    return 1;
                }
                return _m22;
            }
            set
            {
                if (IsDistinguishedIdentity)
                {
                    this = s_identity;
                    IsDistinguishedIdentity = false;
                }
                _m22 = value;
            }
        }
        public double M23
        {
            get
            {
                return _m23;
            }
            set
            {
                if (IsDistinguishedIdentity)
                {
                    this = s_identity;
                    IsDistinguishedIdentity = false;
                }
                _m23 = value;
            }
        }
        public double M24
        {
            get
            {
                return _m24;
            }
            set
            {
                if (IsDistinguishedIdentity)
                {
                    this = s_identity;
                    IsDistinguishedIdentity = false;
                }
                _m24 = value;
            }
        }
        public double M31
        {
            get
            {
                return _m31;
            }
            set
            {
                if (IsDistinguishedIdentity)
                {
                    this = s_identity;
                    IsDistinguishedIdentity = false;
                }
                _m31 = value;
            }
        }
        public double M32
        {
            get
            {
                return _m32;
            }
            set
            {
                if (IsDistinguishedIdentity)
                {
                    this = s_identity;
                    IsDistinguishedIdentity = false;
                }
                _m32 = value;
            }
        }
        public double M33
        {
            get
            {
                if (IsDistinguishedIdentity)
                {
                    return 1;
                }
                return _m33;
            }
            set
            {
                if (IsDistinguishedIdentity)
                {
                    this = s_identity;
                    IsDistinguishedIdentity = false;
                }
                _m33 = value;
            }
        }
        public double M34
        {
            get
            {
                return _m34;
            }
            set
            {
                if (IsDistinguishedIdentity)
                {
                    this = s_identity;
                    IsDistinguishedIdentity = false;
                }
                _m34 = value;
            }
        }

        public double M41
        {
            get
            {
                return OffsetX;
            }
            set
            {
                OffsetX = value;
            }
        }

        public double M42
        {
            get
            {
                return OffsetY;
            }
            set
            {
                OffsetY = value;
            }
        }

        public double M43
        {
            get
            {
                return OffsetZ;
            }
            set
            {
                OffsetZ = value;
            }
        }    

        public double OffsetX
        {
            get
            {
                return _offsetX;
            }
            set
            {
                if (IsDistinguishedIdentity)
                {
                    this = s_identity;
                    IsDistinguishedIdentity = false;
                }
                _offsetX = value;
            }
        }
        public double OffsetY
        {
            get
            {
                return _offsetY;
            }
            set
            {
                if (IsDistinguishedIdentity)
                {
                    this = s_identity;
                    IsDistinguishedIdentity = false;
                }
                _offsetY = value;
            }
        }
        public double OffsetZ
        {
            get
            {
                return _offsetZ;
            }
            set
            {
                if (IsDistinguishedIdentity)
                {
                    this = s_identity;
                    IsDistinguishedIdentity = false;
                }
                _offsetZ = value;
            }
        }
        public double M44
        {
            get
            {
                if (IsDistinguishedIdentity)
                {
                    return 1;
                }
                return _m44;
            }
            set
            {
                if (IsDistinguishedIdentity)
                {
                    this = s_identity;
                    IsDistinguishedIdentity = false;
                }
                _m44 = value;
            }
        }
        private void SetScaleMatrix(ref Vector3d scale)
        {
            _m11 = scale.X;
            _m22 = scale.Y;
            _m33 = scale.Z;
            _m44 = 1;
            IsDistinguishedIdentity = false;
        }

        private void SetScaleMatrix(ref Vector3d scale, ref Vector3d center)
        {
            _m11 = scale.X;
            _m22 = scale.Y;
            _m33 = scale.Z;
            _m44 = 1;
            _offsetX = center.X - (center.X * scale.X);
            _offsetY = center.Y - (center.Y * scale.Y);
            _offsetZ = center.Z - (center.Z * scale.Z);
            IsDistinguishedIdentity = false;
        }

        private void SetTranslationMatrix(ref Vector3d offset)
        {
            _m11 = _m22 = _m33 = _m44 = 1;
            _offsetX = offset.X;
            _offsetY = offset.Y;
            _offsetZ = offset.Z;
            IsDistinguishedIdentity = false;
        }

        public static Matrix3d CreateRotationMatrix(ref Quaternion quaternion, ref Vector3d center)
        {
            var matrixd = s_identity;
            matrixd.IsDistinguishedIdentity = false;
            double num12 = quaternion.X + quaternion.X;
            double num2 = quaternion.Y + quaternion.Y;
            double num = quaternion.Z + quaternion.Z;
            var num11 = quaternion.X * num12;
            var num10 = quaternion.X * num2;
            var num9 = quaternion.X * num;
            var num8 = quaternion.Y * num2;
            var num7 = quaternion.Y * num;
            var num6 = quaternion.Z * num;
            var num5 = quaternion.W * num12;
            var num4 = quaternion.W * num2;
            var num3 = quaternion.W * num;
            matrixd._m11 = 1 - (num8 + num6);
            matrixd._m12 = num10 + num3;
            matrixd._m13 = num9 - num4;
            matrixd._m21 = num10 - num3;
            matrixd._m22 = 1 - (num11 + num6);
            matrixd._m23 = num7 + num5;
            matrixd._m31 = num9 + num4;
            matrixd._m32 = num7 - num5;
            matrixd._m33 = 1 - (num11 + num8);
            if (((center.X != 0) || (center.Y != 0)) || (center.Z != 0))
            {
                matrixd._offsetX = (((-center.X * matrixd._m11) - (center.Y * matrixd._m21)) - (center.Z * matrixd._m31)) + center.X;
                matrixd._offsetY = (((-center.X * matrixd._m12) - (center.Y * matrixd._m22)) - (center.Z * matrixd._m32)) + center.Y;
                matrixd._offsetZ = (((-center.X * matrixd._m13) - (center.Y * matrixd._m23)) - (center.Z * matrixd._m33)) + center.Z;
            }
            return matrixd;
        }

        private void MultiplyPoint(ref Vector3d point)
        {
            if (!IsDistinguishedIdentity)
            {
                var x = point.X;
                var y = point.Y;
                var z = point.Z;
                point.X = (((x * _m11) + (y * _m21)) + (z * _m31)) + _offsetX;
                point.Y = (((x * _m12) + (y * _m22)) + (z * _m32)) + _offsetY;
                point.Z = (((x * _m13) + (y * _m23)) + (z * _m33)) + _offsetZ;
                if (!IsAffine)
                {
                    var num4 = (((x * _m14) + (y * _m24)) + (z * _m34)) + _m44;
                    point.X /= num4;
                    point.Y /= num4;
                    point.Z /= num4;
                }
            }
        }

        public void MultiplyVector(ref Vector3d vector)
        {
            if (!IsDistinguishedIdentity)
            {
                var x = vector.X;
                var y = vector.Y;
                var z = vector.Z;
                vector.X = ((x * _m11) + (y * _m21)) + (z * _m31);
                vector.Y = ((x * _m12) + (y * _m22)) + (z * _m32);
                vector.Z = ((x * _m13) + (y * _m23)) + (z * _m33);
            }
        }

        private double GetNormalizedAffineDeterminant()
        {
            var num3 = (_m12 * _m23) - (_m22 * _m13);
            var num2 = (_m32 * _m13) - (_m12 * _m33);
            var num = (_m22 * _m33) - (_m32 * _m23);
            return (((_m31 * num3) + (_m21 * num2)) + (_m11 * num));
        }

        private bool NormalizedAffineInvert()
        {
            var num11 = (_m12 * _m23) - (_m22 * _m13);
            var num10 = (_m32 * _m13) - (_m12 * _m33);
            var num9 = (_m22 * _m33) - (_m32 * _m23);
            var num8 = ((_m31 * num11) + (_m21 * num10)) + (_m11 * num9);
            if (DoubleUtilities.IsZero(num8))
            {
                return false;
            }
            var num20 = (_m21 * _m13) - (_m11 * _m23);
            var num19 = (_m11 * _m33) - (_m31 * _m13);
            var num18 = (_m31 * _m23) - (_m21 * _m33);
            var num7 = (_m11 * _m22) - (_m21 * _m12);
            var num6 = (_m11 * _m32) - (_m31 * _m12);
            var num5 = (_m11 * _offsetY) - (_offsetX * _m12);
            var num4 = (_m21 * _m32) - (_m31 * _m22);
            var num3 = (_m21 * _offsetY) - (_offsetX * _m22);
            var num2 = (_m31 * _offsetY) - (_offsetX * _m32);
            var num17 = ((_m23 * num5) - (_offsetZ * num7)) - (_m13 * num3);
            var num16 = ((_m13 * num2) - (_m33 * num5)) + (_offsetZ * num6);
            var num15 = ((_m33 * num3) - (_offsetZ * num4)) - (_m23 * num2);
            var num14 = num7;
            var num13 = -num6;
            var num12 = num4;
            var num = 1 / num8;
            _m11 = num9 * num;
            _m12 = num10 * num;
            _m13 = num11 * num;
            _m21 = num18 * num;
            _m22 = num19 * num;
            _m23 = num20 * num;
            _m31 = num12 * num;
            _m32 = num13 * num;
            _m33 = num14 * num;
            _offsetX = num15 * num;
            _offsetY = num16 * num;
            _offsetZ = num17 * num;
            return true;
        }


        private bool InvertCore()
        {
            if (!IsDistinguishedIdentity)
            {
                if (IsAffine)
                {
                    return NormalizedAffineInvert();
                }
                var num7 = (_m13 * _m24) - (_m23 * _m14);
                var num6 = (_m13 * _m34) - (_m33 * _m14);
                var num5 = (_m13 * _m44) - (_offsetZ * _m14);
                var num4 = (_m23 * _m34) - (_m33 * _m24);
                var num3 = (_m23 * _m44) - (_offsetZ * _m24);
                var num2 = (_m33 * _m44) - (_offsetZ * _m34);
                var num12 = ((_m22 * num6) - (_m32 * num7)) - (_m12 * num4);
                var num11 = ((_m12 * num3) - (_m22 * num5)) + (_offsetY * num7);
                var num10 = ((_m32 * num5) - (_offsetY * num6)) - (_m12 * num2);
                var num9 = ((_m22 * num2) - (_m32 * num3)) + (_offsetY * num4);
                var num8 = (((_offsetX * num12) + (_m31 * num11)) + (_m21 * num10)) + (_m11 * num9);
                if (DoubleUtilities.IsZero(num8))
                {
                    return false;
                }
                var num24 = ((_m11 * num4) - (_m21 * num6)) + (_m31 * num7);
                var num23 = ((_m21 * num5) - (_offsetX * num7)) - (_m11 * num3);
                var num22 = ((_m11 * num2) - (_m31 * num5)) + (_offsetX * num6);
                var num21 = ((_m31 * num3) - (_offsetX * num4)) - (_m21 * num2);
                num7 = (_m11 * _m22) - (_m21 * _m12);
                num6 = (_m11 * _m32) - (_m31 * _m12);
                num5 = (_m11 * _offsetY) - (_offsetX * _m12);
                num4 = (_m21 * _m32) - (_m31 * _m22);
                num3 = (_m21 * _offsetY) - (_offsetX * _m22);
                num2 = (_m31 * _offsetY) - (_offsetX * _m32);
                var num20 = ((_m13 * num4) - (_m23 * num6)) + (_m33 * num7);
                var num19 = ((_m23 * num5) - (_offsetZ * num7)) - (_m13 * num3);
                var num18 = ((_m13 * num2) - (_m33 * num5)) + (_offsetZ * num6);
                var num17 = ((_m33 * num3) - (_offsetZ * num4)) - (_m23 * num2);
                var num16 = ((_m24 * num6) - (_m34 * num7)) - (_m14 * num4);
                var num15 = ((_m14 * num3) - (_m24 * num5)) + (_m44 * num7);
                var num14 = ((_m34 * num5) - (_m44 * num6)) - (_m14 * num2);
                var num13 = ((_m24 * num2) - (_m34 * num3)) + (_m44 * num4);
                var num = 1 / num8;
                _m11 = num9 * num;
                _m12 = num10 * num;
                _m13 = num11 * num;
                _m14 = num12 * num;
                _m21 = num21 * num;
                _m22 = num22 * num;
                _m23 = num23 * num;
                _m24 = num24 * num;
                _m31 = num13 * num;
                _m32 = num14 * num;
                _m33 = num15 * num;
                _m34 = num16 * num;
                _offsetX = num17 * num;
                _offsetY = num18 * num;
                _offsetZ = num19 * num;
                _m44 = num20 * num;
            }
            return true;
        }

        public static Matrix3d LookAtLH(Vector3d cameraPosition, Vector3d cameraTarget, Vector3d cameraUpVector)
        {

            var zaxis = cameraTarget - cameraPosition;
            zaxis.Normalize();
            var xaxis = Vector3d.Cross(cameraUpVector, zaxis);
            xaxis.Normalize();
            var yaxis = Vector3d.Cross(zaxis, xaxis);

            var mat = new Matrix3d(xaxis.X, yaxis.X, zaxis.X, 0, xaxis.Y, yaxis.Y, zaxis.Y, 0, xaxis.Z, yaxis.Z, zaxis.Z, 0, -Vector3d.Dot(xaxis, cameraPosition), -Vector3d.Dot(yaxis, cameraPosition), -Vector3d.Dot(zaxis, cameraPosition), 1);
            return mat;
        }

        private static Matrix3d CreateIdentity()
        {
            var matrixd = new Matrix3d(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1) {IsDistinguishedIdentity = true};
            return matrixd;
        }

        private bool IsDistinguishedIdentity
        {
            get
            {
                return !_isNotKnownToBeIdentity;
            }
            set
            {
                _isNotKnownToBeIdentity = !value;
            }
        }
        public static bool operator ==(Matrix3d matrix1, Matrix3d matrix2)
        {
            if (matrix1.IsDistinguishedIdentity || matrix2.IsDistinguishedIdentity)
            {
                return (matrix1.IsIdentity == matrix2.IsIdentity);
            }
            if (((((matrix1.M11 == matrix2.M11) && (matrix1.M12 == matrix2.M12)) && ((matrix1.M13 == matrix2.M13) && (matrix1.M14 == matrix2.M14))) && (((matrix1.M21 == matrix2.M21) && (matrix1.M22 == matrix2.M22)) && ((matrix1.M23 == matrix2.M23) && (matrix1.M24 == matrix2.M24)))) && ((((matrix1.M31 == matrix2.M31) && (matrix1.M32 == matrix2.M32)) && ((matrix1.M33 == matrix2.M33) && (matrix1.M34 == matrix2.M34))) && (((matrix1.OffsetX == matrix2.OffsetX) && (matrix1.OffsetY == matrix2.OffsetY)) && (matrix1.OffsetZ == matrix2.OffsetZ))))
            {
                return (matrix1.M44 == matrix2.M44);
            }
            return false;
        }

        public static bool operator !=(Matrix3d matrix1, Matrix3d matrix2)
        {
            return !(matrix1 == matrix2);
        }

        public static bool Equals(Matrix3d matrix1, Matrix3d matrix2)
        {
            if (matrix1.IsDistinguishedIdentity || matrix2.IsDistinguishedIdentity)
            {
                return (matrix1.IsIdentity == matrix2.IsIdentity);
            }
            if ((((matrix1.M11.Equals(matrix2.M11) && matrix1.M12.Equals(matrix2.M12)) && (matrix1.M13.Equals(matrix2.M13) && matrix1.M14.Equals(matrix2.M14))) && ((matrix1.M21.Equals(matrix2.M21) && matrix1.M22.Equals(matrix2.M22)) && (matrix1.M23.Equals(matrix2.M23) && matrix1.M24.Equals(matrix2.M24)))) && (((matrix1.M31.Equals(matrix2.M31) && matrix1.M32.Equals(matrix2.M32)) && (matrix1.M33.Equals(matrix2.M33) && matrix1.M34.Equals(matrix2.M34))) && ((matrix1.OffsetX.Equals(matrix2.OffsetX) && matrix1.OffsetY.Equals(matrix2.OffsetY)) && matrix1.OffsetZ.Equals(matrix2.OffsetZ))))
            {
                return matrix1.M44.Equals(matrix2.M44);
            }
            return false;
        }

        public override bool Equals(object o)
        {
            if ((o == null) || !(o is Matrix3d))
            {
                return false;
            }
            var matrixd = (Matrix3d)o;
            return Equals(this, matrixd);
        }

        public bool Equals(Matrix3d value)
        {
            return Equals(this, value);
        }

        public override int GetHashCode()
        {
            if (IsDistinguishedIdentity)
            {
                return 0;
            }
            return (((((((((((((((M11.GetHashCode() ^ M12.GetHashCode()) ^ M13.GetHashCode()) ^ M14.GetHashCode()) ^ M21.GetHashCode()) ^ M22.GetHashCode()) ^ M23.GetHashCode()) ^ M24.GetHashCode()) ^ M31.GetHashCode()) ^ M32.GetHashCode()) ^ M33.GetHashCode()) ^ M34.GetHashCode()) ^ OffsetX.GetHashCode()) ^ OffsetY.GetHashCode()) ^ OffsetZ.GetHashCode()) ^ M44.GetHashCode());
        }

        public override string ToString()
        {
            return ConvertToString(null, null);
        }

        string IFormattable.ToString(string format, IFormatProvider provider)
        {
            return ConvertToString(format, provider);
        }

        private string ConvertToString(string format, IFormatProvider provider)
        {
            if (IsIdentity)
            {
                return "Identity";
            }
            const char numericListSeparator = ',';
            return string.Format(provider, "{1:" + format + "}{0}{2:" + format + "}{0}{3:" + format + "}{0}{4:" + format + "}{0}{5:" + format + "}{0}{6:" + format + "}{0}{7:" + format + "}{0}{8:" + format + "}{0}{9:" + format + "}{0}{10:" + format + "}{0}{11:" + format + "}{0}{12:" + format + "}{0}{13:" + format + "}{0}{14:" + format + "}{0}{15:" + format + "}{0}{16:" + format + "}", new object[] { 
            numericListSeparator, _m11, _m12, _m13, _m14, _m21, _m22, _m23, _m24, _m31, _m32, _m33, _m34, _offsetX, _offsetY, _offsetZ, 
            _m44
         });
        }

        public static Matrix3d FromMatrix2d(Matrix2d mat)
        {
            var mat3d = CreateIdentity();

            mat3d.M11 = mat.M11;
            mat3d.M12 = mat.M12;
            mat3d.M13 = mat.M13;
            mat3d.M21 = mat.M21;
            mat3d.M22 = mat.M22;
            mat3d.M23 = mat.M23;
            mat3d.M31 = mat.M31;
            mat3d.M32 = mat.M32;
            mat3d.M33 = mat.M33;
            mat3d._isNotKnownToBeIdentity = true;

            return mat3d;
        }


        static Matrix3d()
        {
            s_identity = CreateIdentity();
        }

        public double this[int x, int y]
        {
            get
            {
                switch (x)
                {
                    case 0:
                        switch (y)
                        {
                            case 0:
                                return _m11;
                            case 1:
                                return _m12;
                            case 2:
                                return _m13;
                            case 3:
                                return _m14;
                        }
                        break;
                     case 1:
                            switch (y)
                        {
                            case 0:
                                return _m21;
                            case 1:
                                return _m22;
                            case 2:
                                return _m23;
                            case 3:
                                return _m24;
                        }
                            break;
                     case 2:
                            switch (y)
                            {
                                case 0:
                                    return _m31;
                                case 1:
                                    return _m32;
                                case 2:
                                    return _m33;
                                case 3:
                                    return _m34;
                            }
                            break;
                     case 3:
                            switch (y)
                            {
                                case 0:
                                    return _offsetX;
                                case 1:
                                    return _offsetY;
                                case 2:
                                    return _offsetZ;
                                case 3:
                                    return _m44;
                            }
                            break;
                }
                return 0;

               
            }
            set
            {
                switch (x)
                {
                    case 0:
                        switch (y)
                        {
                            case 0:
                                _m11 = value;
                                return;
                            case 1:
                                _m12 = value;
                                return;
                            case 2:
                                _m13 = value;
                                return;
                            case 3:
                                _m14 = value;
                                return;
                        }
                        break;
                    case 1:
                        switch (y)
                        {
                            case 0:
                                _m21 = value;
                                return;
                            case 1:
                                _m22 = value;
                                return;
                            case 2:
                                _m23 = value;
                                return;
                            case 3:
                                _m24 = value;
                                return;
                        }
                        break;
                    case 2:
                        switch (y)
                        {
                            case 0:
                                _m31 = value;
                                return;
                            case 1:
                                _m32 = value;
                                return;
                            case 2:
                                _m33 = value;
                                return;
                            case 3:
                                _m34 = value;
                                return;
                        }
                        break;
                    case 3:
                        switch (y)
                        {
                            case 0:
                                _offsetX = value;
                                return;
                            case 1:
                                _offsetY = value;
                                return;
                            case 2:
                                _offsetZ = value;
                                return;
                            case 3:
                                _m44 = value;
                                return;
                        }
                        break;
                }
            }
        }

        public static Matrix3d RotationY(double p)
        {
            var v = p;

            var matNew = Identity;
            matNew._m11 = Math.Cos(v);
            matNew._m22 = 1;
            matNew._m31 = Math.Sin(v);
            matNew._m13 = -Math.Sin(v);
            matNew._m33 = Math.Cos(v);
            matNew._isNotKnownToBeIdentity = true;
            return matNew;
        }

        public static Matrix3d RotationX(double p)
        {
            var v = p;

            var matNew = Identity;
            matNew._m11 = 1;
            matNew._m22 = Math.Cos(v);
            matNew._m32 = -Math.Sin(v);
            matNew._m23 = Math.Sin(v);
            matNew._m33 = Math.Cos(v);
            matNew._isNotKnownToBeIdentity = true;
            return matNew;
        }
        public static Matrix3d RotationZ(double p)
        {
            var v = p;

            var matNew = Identity;
            matNew._m11 = Math.Cos(v);
            matNew._m21 = -Math.Sin(v);
            matNew._m12 = Math.Sin(v);
            matNew._m22 = Math.Cos(v);
            matNew._m33 = 1;
            matNew._isNotKnownToBeIdentity = true;
            return matNew;
        }
        public static Matrix3d Scaling(double x, double y, double z)
        {
            var matNew = Identity;
            matNew._m11 = x;
            matNew._m22 = y;
            matNew._m33 = z;
            matNew._isNotKnownToBeIdentity = true;
            return matNew;
        }

        public static Matrix3d Translation(double x, double y, double z)
        {
            var matNew = Identity;
            matNew.OffsetX = x;
            matNew.OffsetY = y;
            matNew.OffsetZ = z;
            matNew._isNotKnownToBeIdentity = true;
            return matNew;
        }

        public void Multiply(Matrix3d mat)
        {
            this = Multiply(this, mat);
        }

        public static Matrix3d PerspectiveFovLH(double fieldOfViewY, double aspectRatio, double znearPlane, double zfarPlane)
        {
            var h = 1 / Math.Tan(fieldOfViewY/2);
            var w = h / aspectRatio;

            return new Matrix3d(w, 0, 0, 0, 0, h, 0, 0, 0, 0, zfarPlane / (zfarPlane - znearPlane), 1, 0, 0, -znearPlane * zfarPlane / (zfarPlane - znearPlane), 0);
        }

        public static Matrix3d PerspectiveOffCenterLH(double left, double right, double bottom, double top, double znearPlane, double zfarPlane)
        {
            return new Matrix3d(
                2 * znearPlane / (right - left),    0,                                  0,                                                  0,
                0,                                  2 * znearPlane / (top - bottom),    0,                                                  0,
                (left + right) / (left - right),    (top + bottom) / (bottom - top),    zfarPlane / (zfarPlane - znearPlane),               1,
                0,                                  0,                                  znearPlane * zfarPlane / (znearPlane - zfarPlane),  0
                
                );
        }

        public static Matrix3d OrthoLH(double width, double height, double znear, double zfar)
        {
            return new Matrix3d(
                2 / width,    0,             0,                         0,
                0,            2 / height,    0,                         0,
                0,            0,             1 / (zfar - znear),        0,
                0,            0,             znear / (znear - zfar),    1                
                );
        }

        public static Matrix3d Invert(Matrix3d matrix3d)
        {
            var mat = matrix3d;
            mat.Invert();
            return mat;
        }

        public static Matrix3d Translation(Vector3d vector3d)
        {
            return Translation(vector3d.X, vector3d.Y, vector3d.Z);
        }

        static public Matrix3d GetMapMatrix(Coordinates center, double fieldWidth, double fieldHeight, double rotation)
        {
            double offsetX = 0;
            double offsetY = 0;

            offsetX = -(((center.Lng + 180 - (fieldWidth / 2)) / 360));
            offsetY = -((1 - ((center.Lat + 90 + (fieldHeight / 2)) / 180)));

            var mat = new Matrix2d();

            double scaleX = 0;
            double scaleY = 0;

            scaleX = 360 / fieldWidth;
            scaleY = 180 / fieldHeight;
            mat = mat * Matrix2d.Translation(offsetX, offsetY);
            mat = mat * Matrix2d.Scaling(scaleX, scaleY);
            if (rotation != 0)
            {
                mat = mat * Matrix2d.Translation(-.5, -.5);
                mat = mat * Matrix2d.Rotation(rotation);
                mat = mat * Matrix2d.Translation(.5, .5);
            }



            return FromMatrix2d(mat);
        }
    }

    public class Matrix2d
    {
        public double M11 = 1;
        public double M12;
        public double M13;
        public double M21;
        public double M22 = 1;
        public double M23;
        public double M31;
        public double M32;
        public double M33 = 1;

        public Matrix2d()
        {
        }
        public Matrix2d(double m11, double m12, double m13, double m21, double m22, double m23, double m31, double m32, double m33)
        {
            M11 = m11;
            M12 = m12;
            M13 = m13;
            M21 = m21;
            M22 = m22;
            M23 = m23;
            M31 = m31;
            M32 = m32;
            M33 = m33;
        }

        public static Matrix2d Rotation(double angle)
        {
            var mat = new Matrix2d
            {
                M11 = Math.Cos(angle),
                M21 = -Math.Sin(angle),
                M12 = Math.Sin(angle),
                M22 = Math.Cos(angle)
            };
            return mat;
        }
        public static Matrix2d Translation(double x, double y)
        {
            var mat = new Matrix2d {M31 = x, M32 = y};

            return mat;
        }

        public static Matrix2d Scaling(double x, double y)
        {
            var mat = new Matrix2d {M11 = x, M22 = y};
            return mat;
        }
        public static Matrix2d operator *(Matrix2d matrix1, Matrix2d matrix2)
        {

            return new Matrix2d
            (
                (((matrix1.M11 * matrix2.M11) + (matrix1.M12 * matrix2.M21)) + (matrix1.M13 * matrix2.M31)),
                (((matrix1.M11 * matrix2.M12) + (matrix1.M12 * matrix2.M22)) + (matrix1.M13 * matrix2.M32)),
                (((matrix1.M11 * matrix2.M13) + (matrix1.M12 * matrix2.M23)) + (matrix1.M13 * matrix2.M33)),
                (((matrix1.M21 * matrix2.M11) + (matrix1.M22 * matrix2.M21)) + (matrix1.M23 * matrix2.M31)),
                (((matrix1.M21 * matrix2.M12) + (matrix1.M22 * matrix2.M22)) + (matrix1.M23 * matrix2.M32)),
                (((matrix1.M21 * matrix2.M13) + (matrix1.M22 * matrix2.M23)) + (matrix1.M23 * matrix2.M33)),
                (((matrix1.M31 * matrix2.M11) + (matrix1.M32 * matrix2.M21)) + (matrix1.M33 * matrix2.M31)),
                (((matrix1.M31 * matrix2.M12) + (matrix1.M32 * matrix2.M22)) + (matrix1.M33 * matrix2.M32)),
                (((matrix1.M31 * matrix2.M13) + (matrix1.M32 * matrix2.M23)) + (matrix1.M33 * matrix2.M33))
            );
        }
    }

    public static class DoubleUtilities
    {
        public static bool IsZero(double value)
        {
            return Math.Abs(value) < double.Epsilon;
        }

        public static bool IsOne(double value)
        {
            return (Math.Abs(value - 1) < double.Epsilon);
        }

        public static double RadiansToDegrees(double radians)
        {
            return radians * 180 / Math.PI;
        }

        public static double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }

        public static double Clamp(double x, double min, double max)
        {
            return Math.Max(min, Math.Min(x, max));
        }
    }
    public struct PlaneD
    {
        public double A;
        public double B;
        public double C;
        public double D;

        public PlaneD(double valuePointA, double valuePointB, double valuePointC, double valuePointD)
        {
            A = valuePointA;
            B = valuePointB;
            C = valuePointC;
            D = valuePointD;
        }

        //public override bool Equals(object compare);
        //public static bool operator ==(Plane left, Plane right);
        // public static bool operator !=(Plane left, Plane right);
        // public override int GetHashCode();
        // public Plane();
        //  public static Plane Empty { get; }
        //  public override string ToString();
        //  public static float DotNormal(Plane p, Vector3 v);
        public void Normalize()
        {
            var length = Math.Sqrt(A * A + B * B + C * C);

            A /= length;
            B /= length;
            C /= length;
            D /= length;


            //Vector3d vector = new Vector3d(A, B, C);
            //vector.Normalize();
            //A = vector.X;
            //B = vector.Y;
            //C = vector.Z;
        }

        //   public static Plane Normalize(Plane p);
        //   public static Vector3 IntersectLine(Plane p, Vector3 v1, Vector3 v2);
        //   public static Plane FromPointNormal(Vector3 point, Vector3 normal);
        //   public static Plane FromPoints(Vector3 p1, Vector3 p2, Vector3 p3);
        //    public void Transform(Matrix m);
        //    public static Plane Transform(Plane p, Matrix m);
        //    public void Scale(float s);
        //    public static Plane Scale(Plane p, float s);
        //   public float Dot(Vector3 v);
        public double Dot(Vector4d v)
        {
            //return ((((planeRef[4] * *(((float*) (&v + 4)))) + (planeRef[8] * *(((float*) (&v + 8))))) + (planeRef[12] * *(((float*) (&v + 12))))) + (planeRef[0] * *(((float*) &v))));
            return B * v.Y + C * v.Z + D * v.W + A * v.X;
        }


    }
    public struct Vector4d
    {
        public Vector4d(double valueX, double valueY, double valueZ, double valueW)
        {
            X = valueX;
            Y = valueY;
            Z = valueZ;
            W = valueW;
        }
        public double X;
        public double Y;
        public double Z;
        public double W;
    }
    public class ConvexHull
    {
        public static void FindEnclosingSphere(Vector3d[] list, out Vector3d cen, out double rad)
        {
            var count = list.Length;
            int i;
            var xmin = new Vector3d();
            var xmax = new Vector3d();
            var ymin = new Vector3d();
            var ymax = new Vector3d();
            var zmin = new Vector3d();
            var zmax = new Vector3d();
            var dia1 = new Vector3d();
            var dia2 = new Vector3d();


            // FIRST PASS: find 6 minima/maxima points 
            xmin.X = ymin.Y = zmin.Z = 100000000; // initialize for min/max compare 
            xmax.X = ymax.Y = zmax.Z = -1000000000;
            for (i = 0; i < count; i++)
            {
                var current = list[i];
                // his ith point. 
                if (current.X < xmin.X)
                    xmin = current; // New xminimum point 
                if (current.X > xmax.X)
                    xmax = current;
                if (current.Y < ymin.Y)
                    ymin = current;
                if (current.Y > ymax.Y)
                    ymax = current;
                if (current.Z < zmin.Z)
                    zmin = current;
                if (current.Z > zmax.Z)
                    zmax = current;
            }
            // Set xspan = distance between the 2 points xmin & xmax (squared) 
            double dx = xmax.X - xmin.X;
            double dy = xmax.Y - xmin.Y;
            double dz = xmax.Z - xmin.Z;
            double xspan = dx * dx + dy * dy + dz * dz;

            // Same for y & z spans 
            dx = ymax.X - ymin.X;
            dy = ymax.Y - ymin.Y;
            dz = ymax.Z - ymin.Z;
            double yspan = dx * dx + dy * dy + dz * dz;

            dx = zmax.X - zmin.X;
            dy = zmax.Y - zmin.Y;
            dz = zmax.Z - zmin.Z;
            double zspan = dx * dx + dy * dy + dz * dz;

            dia1 = xmin; // assume xspan biggest 
            dia2 = xmax;
            double maxspan = xspan;
            if (yspan > maxspan)
            {
                maxspan = yspan;
                dia1 = ymin;
                dia2 = ymax;
            }
            if (zspan > maxspan)
            {
                dia1 = zmin;
                dia2 = zmax;
            }


            // dia1,dia2 is a diameter of initial sphere 
            // calc initial center 
            cen.X = (dia1.X + dia2.X) / 2.0;
            cen.Y = (dia1.Y + dia2.Y) / 2.0;
            cen.Z = (dia1.Z + dia2.Z) / 2.0;
            // calculate initial radius**2 and radius 
            dx = dia2.X - cen.X; // x component of radius vector 
            dy = dia2.Y - cen.Y; // y component of radius vector 
            dz = dia2.Z - cen.Z; // z component of radius vector 
            double rad_sq = dx * dx + dy * dy + dz * dz;
            rad = Math.Sqrt(rad_sq);

            // SECOND PASS: increment current sphere 

            for (i = 0; i < count; i++)
            {
                var current = list[i]; // load global struct caller_p 
                // with his ith point. 
                dx = current.X - cen.X;
                dy = current.Y - cen.Y;
                dz = current.Z - cen.Z;
                double old_to_p_sq = dx * dx + dy * dy + dz * dz;
                if (old_to_p_sq > rad_sq) // do r**2 test first 
                { // this point is outside of current sphere 
                    double old_to_p = Math.Sqrt(old_to_p_sq);
                    // calc radius of new sphere 
                    rad = (rad + old_to_p) / 2.0;
                    rad_sq = rad * rad; // for next r**2 compare 
                    double old_to_new = old_to_p - rad;
                    // calc center of new sphere 
                    cen.X = (rad * cen.X + old_to_new * current.X) / old_to_p;
                    cen.Y = (rad * cen.Y + old_to_new * current.Y) / old_to_p;
                    cen.Z = (rad * cen.Z + old_to_new * current.Z) / old_to_p;
                    // Suppress if desired 
                    //Console.Write("\n New sphere: cen,rad = {0:f} {1:f} {2:f}   {3:f}", cen.X, cen.Y, cen.Z, rad);
                }
            }
        }// end of find_bounding_sphere() 

        public static void FindEnclosingCircle(Vector2d[] list, out Vector2d cen, out double rad)
        {
            cen = new Vector2d();
            var count = list.Length;
            int i;
            var xmin = new Vector2d();
            var xmax = new Vector2d();
            var ymin = new Vector2d();
            var ymax = new Vector2d();
            var dia1 = new Vector2d();
            var dia2 = new Vector2d();


            // FIRST PASS: find 6 minima/maxima points 
            xmin.X = ymin.Y = 100000000; // initialize for min/max compare 
            xmax.X = ymax.Y = -1000000000;
            for (i = 0; i < count; i++)
            {
                var current = list[i];
                // his ith point. 
                if (current.X < xmin.X)
                    xmin = current; // New xminimum point 
                if (current.X > xmax.X)
                    xmax = current;
                if (current.Y < ymin.Y)
                    ymin = current;
                if (current.Y > ymax.Y)
                    ymax = current;

            }
            // Set xspan = distance between the 2 points xmin & xmax (squared) 
            double dx = xmax.X - xmin.X;
            double dy = xmax.Y - xmin.Y;
            double xspan = dx * dx + dy * dy;

            // Same for y & z spans 
            dx = ymax.X - ymin.X;
            dy = ymax.Y - ymin.Y;
            double yspan = dx * dx + dy * dy;

            dia1 = xmin; // assume xspan biggest 
            dia2 = xmax;
            double maxspan = xspan;
            if (yspan > maxspan)
            {
                maxspan = yspan;
                dia1 = ymin;
                dia2 = ymax;
            }


            // dia1,dia2 is a diameter of initial sphere 
            // calc initial center 
            cen.X = (dia1.X + dia2.X) / 2.0;
            cen.Y = (dia1.Y + dia2.Y) / 2.0;
            // calculate initial radius**2 and radius 
            dx = dia2.X - cen.X; // x component of radius vector 
            dy = dia2.Y - cen.Y; // y component of radius vector 
            double rad_sq = dx * dx + dy * dy;
            rad = Math.Sqrt(rad_sq);

            // SECOND PASS: increment current sphere 

            for (i = 0; i < count; i++)
            {
                var current = list[i]; // load global struct caller_p 
                // with his ith point. 
                dx = current.X - cen.X;
                dy = current.Y - cen.Y;
                double old_to_p_sq = dx * dx + dy * dy;
                if (old_to_p_sq > rad_sq) // do r**2 test first 
                { // this point is outside of current sphere 
                    double old_to_p = Math.Sqrt(old_to_p_sq);
                    // calc radius of new sphere 
                    rad = (rad + old_to_p) / 2.0;
                    rad_sq = rad * rad; // for next r**2 compare 
                    double old_to_new = old_to_p - rad;
                    // calc center of new sphere 
                    cen.X = (rad * cen.X + old_to_new * current.X) / old_to_p;
                    cen.Y = (rad * cen.Y + old_to_new * current.Y) / old_to_p;
                }
            }
        }// end of find_bounding_circle() 
    }

    // Summary:
    //     Describes a custom vertex format structure that contains position, normal
    //     data, and one set of texture coordinates.
    public struct PositionNormalTexturedX2
    {
        // Summary:
        //     Retrieves the Microsoft.DirectX.Direct3D.VertexFormats for the current custom
        //     vertex.
        public float X;
        //
        // Summary:
        //     Retrieves or sets the y component of the position.
        public float Y;
        //
        // Summary:
        //     Retrieves or sets the z component of the position.
        public float Z;
        // Summary:
        //     Retrieves or sets the nx component of the vertex normal.
        public float Nx;
        //
        // Summary:
        //     Retrieves or sets the ny component of the vertex normal.
        public float Ny;
        //
        // Summary:
        //     Retrieves or sets the nz component of the vertex normal.
        public float Nz;
        //
        // Summary:
        //     Retrieves or sets the u component of the texture coordinate.
        public float Tu;
        //
        // Summary:
        //     Retrieves or sets the v component of the texture coordinate.
        public float Tv;
        //
        // Summary:
        //     Retrieves or sets the u component of the texture coordinate.
        public float Tu1;
        //
        // Summary:
        //     Retrieves or sets the v component of the texture coordinate.
        public float Tv1;
        //
        // Summary:
        //     Retrieves or sets the x component of the position.


        //
        // Summary:
        //     Initializes a new instance of the PositionNormalTexturedX2
        //     class.
        //
        // Parameters:
        //   pos:
        //     A Microsoft.DirectX.Vector3 object that contains the vertex position.
        //
        //   nor:
        //     A Microsoft.DirectX.Vector3 object that contains the vertex normal data.
        //
        //   u:
        //     Floating-point value that represents the PositionNormalTexturedX2.#ctor()
        //     component of the texture coordinate.
        //
        //   v:
        //     Floating-point value that represents the PositionNormalTexturedX2.#ctor()
        //     component of the texture coordinate.
        public PositionNormalTexturedX2(Vector3d pos, Vector3d nor, float u, float v, float u1, float v1)
        {
            X = (float)pos.X;
            Y = (float)pos.Y;
            Z = (float)pos.Z;
            Nx = (float)nor.X;
            Ny = (float)nor.Y;
            Nz = (float)nor.Z;
            Tu = u;
            Tv = v;
            Tu1 = u1;
            Tv1 = v1;
        }

        public PositionNormalTexturedX2(Vector3d pos, Vector3d nor, float u, float v)
        {
            X = (float)pos.X;
            Y = (float)pos.Y;
            Z = (float)pos.Z;
            Nx = (float)nor.X;
            Ny = (float)nor.Y;
            Nz = (float)nor.Z;
            Tu = u;
            Tv = v;
            var result = Coordinates.CartesianToSpherical2(nor);
            Tu1 = (float)((result.Lng + 180.0) / 360.0);
            Tv1 = (float)(1 - ((result.Lat + 90.0) / 180.0));
        }


        public double Lat
        {
            get { return (1 - Tv1) * 180 - 90; }
            set { Tv1 = (float)(1 - ((value + 90.0) / 180.0)); }
        }

        public double Lng
        {
            get { return Tu1 * 360 - 180; }
            set { Tu1 = (float)((value + 180.0) / 360.0); }
        }


        //
        // Summary:
        //     Initializes a new instance of the PositionNormalTexturedX2
        //     class.
        //
        // Parameters:
        //   xvalue:
        //     Floating-point value that represents the x coordinate of the position.
        //
        //   yvalue:
        //     Floating-point value that represents the y coordinate of the position.
        //
        //   zvalue:
        //     Floating-point value that represents the z coordinate of the position.
        //
        //   nxvalue:
        //     Floating-point value that represents the nx coordinate of the vertex normal.
        //
        //   nyvalue:
        //     Floating-point value that represents the ny coordinate of the vertex normal.
        //
        //   nzvalue:
        //     Floating-point value that represents the nz coordinate of the vertex normal.
        //
        //   u:
        //     Floating-point value that represents the PositionNormalTexturedX2.#ctor()
        //     component of the texture coordinate.
        //
        //   v:
        //     Floating-point value that represents the PositionNormalTexturedX2.#ctor()
        //     component of the texture coordinate.
        public PositionNormalTexturedX2(float xvalue, float yvalue, float zvalue, float nxvalue, float nyvalue, float nzvalue, float u, float v, float u1, float v1)
        {
            X = xvalue;
            Y = yvalue;
            Z = zvalue;
            Nx = nxvalue;
            Ny = nyvalue;
            Nz = nzvalue;
            Tu = u;
            Tv = v;
            Tu1 = u1;
            Tv1 = v1;
        }
        public PositionNormalTexturedX2(float xvalue, float yvalue, float zvalue, float nxvalue, float nyvalue, float nzvalue, float u, float v)
        {
            X = xvalue;
            Y = yvalue;
            Z = zvalue;
            Nx = nxvalue;
            Ny = nyvalue;
            Nz = nzvalue;
            Tu = u;
            Tv = v;
            var result = Coordinates.CartesianToSpherical2(new Vector3d(Nx, Ny, Nz));
            Tu1 = (float)((result.Lng + 180.0) / 360.0);
            Tv1 = (float)(1 - ((result.Lat + 90.0) / 180.0));
        }
        // Summary:
        //     Retrieves or sets the vertex normal data.
        public Vector3d Normal
        {
            get
            {
                return new Vector3d(Nx, Ny, Nz);
            }
            set
            {
                Nx = (float)value.X;
                Ny = (float)value.Y;
                Nz = (float)value.Z;
            }
        }
        //
        // Summary:
        //     Retrieves or sets the vertex position.
        public Vector3d Position
        {
            get
            {
                return new Vector3d(X, Y, Z);
            }
            set
            {
                X = (float)value.X;
                Y = (float)value.Y;
                Z = (float)value.Z;
            }
        }

        public Vector3 Pos
        {
            get
            {
                return new Vector3(X, Y, Z);
            }
            set
            {
                X = value.X;
                Y = value.Y;
                Z = value.Z;
            }
        }

        //
        // Summary:
        //     Retrieves the size of the PositionNormalTexturedX2
        //     structure.
        public static int StrideSize
        {
            get
            {
                return 40;
            }

        }

        // Summary:
        //     Obtains a string representation of the current instance.
        //
        // Returns:
        //     String that represents the object.
        public override string ToString()
        {
            return string.Format(
                "X={0}, Y={1}, Z={2}, Nx={3}, Ny={4}, Nz={5}, U={6}, V={7}, U1={8}, U2={9}",
                X, Y, Z, Nx, Ny, Nz, Tu, Tv, Tu1, Tv1
                );
        }
    }
}
