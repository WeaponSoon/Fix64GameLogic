using System;
using System.Collections;
using System.Collections.Generic;
//using UnityEngine;

namespace SWPLogicLayer
{

    public struct SQuaternion
    {
        public float x;
        public float y;
        public float z;
        public float w;

        public float sqrtMagnitude
        {
            get
            {
                return x * x + y * y + z * z + w * w;
            }
        }
        public float magnitude
        {
            get
            {
                return (float)Math.Sqrt(sqrtMagnitude);
            }
        }

        public SQuaternion conjugated
        {
            get
            {
                return new SQuaternion(-x, -y, -z, w);
            }
        }

        public SQuaternion conversed
        {
            get
            {
                float l = sqrtMagnitude;
                return new SQuaternion(-x / l, -y / l, -z / l, w / l);
            }
        }

        public SQuaternion normalized
        {
            get
            {
                float l = magnitude;
                return new SQuaternion(x / l, y / l, z / l, w / l);
            }
        }


        public override string ToString()
        {
            return x + " " + y + " " + z + " " + w;
        }
        public SQuaternion(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }
        public static SQuaternion AxisAngle(SVector3 axis, float angle)
        {
            axis.Normalize();
            float x = (float)(axis.x * Math.Sin(angle / 2));
            float y = (float)(axis.y * Math.Sin(angle / 2));
            float z = (float)(axis.z * Math.Sin(angle / 2));
            float w = (float)Math.Cos(angle / 2);
            return new SQuaternion(x, y, z, w);
        }
        public static SQuaternion AxisAngleInDeg(SVector3 axis, float angle)
        {
            float temAngle = angle / 180 * (float)Math.PI;
            return AxisAngle(axis, temAngle);
        }


        public void Normalize()
        {
            float l = magnitude;
            x /= l;
            y /= l;
            z /= l;
            w /= l;
        }
        public void Conjugate()
        {
            x = -x;
            y = -y;
            z = -z;
        }
        public void Converse()
        {
            float l = magnitude;
            x = -x / l;
            y = -y / l;
            z = -z / l;
            w = w / l;
        }
        public static SQuaternion operator -(SQuaternion a)
        {
            return new SQuaternion(-a.x, -a.y, -a.z, -a.w);
        }
        public static SQuaternion operator +(SQuaternion a, SQuaternion b)
        {
            return new SQuaternion(a.x + b.x, a.y + b.y, a.z + b.z, a.w + b.w);
        }
        public static SQuaternion operator *(float num, SQuaternion q)
        {
            return new SQuaternion(num * q.x, num * q.y, num * q.z, num * q.w);
        }
        public static SQuaternion operator *(SQuaternion q, float num)
        {
            return num * q;
        }
        public static SQuaternion operator /(SQuaternion q, float num)
        {
            return (1 / num) * q;
        }

        public static SQuaternion operator *(SQuaternion a, SQuaternion b)
        {
            float tw = (a.w * b.w - a.x * b.x - a.y * b.y - a.z * b.z);
            float tx = (a.w * b.x + a.x * b.w + a.y * b.z - a.z * b.y);
            float ty = (a.w * b.y - a.x * b.z + a.y * b.w + a.z * b.x);
            float tz = (a.w * b.z + a.x * b.y - a.y * b.x + a.z * b.w);
            return new SQuaternion(tx, ty, tz, tw);
        }

        public static bool operator ==(SQuaternion a, SQuaternion b)
        {
            return (a.x == b.x) && (a.y == b.y) && (a.z == b.z) && (a.w == b.w);
        }
        public static bool operator !=(SQuaternion a, SQuaternion b)
        {
            return !(a == b);
        }
        public static bool DirectionEquals(SQuaternion a, SQuaternion b)
        {
            return a == b || a == -b;
        }

        public override bool Equals(object obj)
        {

            //
            // See the full list of guidelines at
            //   http://go.microsoft.com/fwlink/?LinkID=85237
            // and also the guidance for operator== at
            //   http://go.microsoft.com/fwlink/?LinkId=85238
            //

            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            // TODO: write your implementation of Equals() here

            return this == (SQuaternion)obj;

        }
        public override int GetHashCode()
        {
            return x.GetHashCode() + y.GetHashCode() + z.GetHashCode() + w.GetHashCode();
        }

        public static float Dot(SQuaternion a, SQuaternion b)
        {
            return a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
        }
        public static SVector3 GetRotVectorFromOrigin(SVector3 origin, SQuaternion q)
        {
            SQuaternion temq = new SQuaternion(origin.x, origin.y, origin.z, 0);
            SQuaternion finalq = q * temq * q.conjugated;
            return new SVector3(finalq.x, finalq.y, finalq.z);
        }

        public static SVector3 GetRotVectorFromX(SVector3 origin, SQuaternion q)
        {
            float length = origin.magnitude;
            return GetRotVectorFromOrigin(SVector3.right, q) * length;
        }




        public static SQuaternion Lerp(SQuaternion a, SQuaternion b, float t)
        {
            if (a == b)
            {
                return a;
            }



            float cosa = Dot(a, b);
            if (cosa < 0)
            {
                b = -b;
            }


            float sina = (float)Math.Sqrt(1.0f - cosa * cosa);
            float angle = (float)Math.Atan2(sina, cosa);
            float k0 = (float)Math.Sin((1.0f - t) * angle) / sina;
            float k1 = (float)Math.Sin(t * angle) / sina;

            return k0 * a + k1 * b;
        }
        public static SQuaternion LerpLargeAngle(SQuaternion a, SQuaternion b, float t)
        {
            if (a == b)
            {
                return a;
            }



            float cosa = Dot(a, b);
            if (cosa >= 0)
            {
                b = -b;
            }


            float sina = (float)Math.Sqrt(1.0f - cosa * cosa);
            float angle = (float)Math.Atan2(sina, cosa);
            float k0 = (float)Math.Sin((1.0f - t) * angle) / sina;
            float k1 = (float)Math.Sin(t * angle) / sina;

            return k0 * a + k1 * b;
        }


    }


    public struct SVector2
    {

        public float x;

        public float y;

        public override string ToString()
        {
            return "(" + x + ", " + y + ")";
        }

        public SVector2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public SVector2(float angleFromX)
        {
            this.x = (float)Math.Cos(angleFromX);
            this.y = (float)Math.Sin(angleFromX);
        }



        #region operators

        public float GetAngleFromX()
        {
            if (sqrtMagnitude <= 0)
            {
                return 0;
            }
            float cosA = x / magnitude;
            float sinA = y / magnitude;
            if (sinA == 0 && cosA < -0.5)
            {
                return (float)Math.PI;
            }
            float temAngle = Math.Sign(sinA) * (float)Math.Acos(cosA);

            return temAngle;
        }

        public static bool operator ==(SVector2 a, SVector2 b)
        {
            return (a.x == b.x) && (a.y == b.y);
        }
        public static bool operator !=(SVector2 a, SVector2 b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            //
            // See the full list of guidelines at
            //   http://go.microsoft.com/fwlink/?LinkID=85237
            // and also the guidance for operator== at
            //   http://go.microsoft.com/fwlink/?LinkId=85238
            //

            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            // TODO: write your implementation of Equals() here

            return this == (SVector2)obj;
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            // TODO: write your implementation of GetHashCode() here

            return x.GetHashCode() + y.GetHashCode();
        }

        public static SVector2 operator +(SVector2 a, SVector2 b)
        {
            return new SVector2(a.x + b.x, a.y + b.y);
        }
        public static SVector2 operator -(SVector2 a, SVector2 b)
        {
            return new SVector2(a.x - b.x, a.y - b.y);
        }
        public static SVector2 operator *(SVector2 a, SVector2 b)
        {
            return new SVector2(a.x * b.x, a.y * b.y);
        }
        public static SVector2 operator *(float a, SVector2 b)
        {
            return new SVector2(a * b.x, a * b.y);
        }
        public static SVector2 operator *(SVector2 b, float a)
        {
            return new SVector2(a * b.x, a * b.y);
        }

        public void Add(SVector2 b)
        {
            this.x += b.x;
            this.y += b.y;
        }
        public void Add(float x, float y)
        {
            this.x += x;
            this.y += y;
        }
        public void RotFromThis(float angle)
        {
            float dAngle = GetAngleFromX() + angle;
            float length = magnitude;
            x = (float)Math.Cos(dAngle) * length;
            y = (float)Math.Sin(dAngle) * length;
        }
        public void RotFromX(float angle)
        {
            float length = magnitude;
            x = (float)Math.Cos(angle) * length;
            y = (float)Math.Sin(angle) * length;
        }
        public SVector2 GetRotVectorFromThis(float angle)
        {
            float dAngle = GetAngleFromX() + angle;
            return magnitude * new SVector2(dAngle);
        }
        public SVector2 GetRotVectorFromX(float angle)
        {
            return magnitude * new SVector2(angle);
        }
        public float Dot(SVector2 other)
        {
            return other.x * x + other.y * y;
        }
        public static float Dot(SVector2 a, SVector2 b)
        {

            return a.Dot(b);
        }
        public SVector3 Cross(SVector2 b)
        {
            return new SVector3(0, 0, x * b.y - y * b.x);
        }
        public static SVector3 Cross(SVector2 a, SVector2 b)
        {
            return a.Cross(b);
        }
        public float magnitude
        {
            get
            {
                return (float)Math.Sqrt(x * x + y * y);
            }
        }
        public float sqrtMagnitude
        {
            get
            {
                return x * x + y * y;
            }
        }
        public void Normalize()
        {
            float length = magnitude;
            x /= length;
            y /= length;

        }
        public SVector2 normalized
        {
            get
            {
                float length = magnitude;
                return new SVector2(x / length, y / length);
            }
        }
        #endregion

        #region constant vector
        public static SVector2 zero
        {
            get
            {
                return new SVector2(0, 0);
            }
        }
        public static SVector2 up
        {
            get
            {
                return new SVector2(0, 1);
            }
        }
        public static SVector2 right
        {
            get
            {
                return new SVector2(1, 0);
            }
        }
        #endregion


    }

    public struct SVector3
    {
        public float x;
        public float y;
        public float z;
        public override string ToString()
        {
            return "(" + x + "," + y + "," + z + ")";
        }
        public SVector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        #region operators

        public static SVector3 operator -(SVector3 a)
        {
            return new SVector3(-a.x, -a.y, -a.z);
        }
        public static bool operator ==(SVector3 a, SVector3 b)
        {
            return (a.x == b.x) && (a.y == b.y) && (a.z == b.z);
        }
        public static bool operator !=(SVector3 a, SVector3 b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            //
            // See the full list of guidelines at
            //   http://go.microsoft.com/fwlink/?LinkID=85237
            // and also the guidance for operator== at
            //   http://go.microsoft.com/fwlink/?LinkId=85238
            //

            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            // TODO: write your implementation of Equals() here

            return this == (SVector3)obj;
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            // TODO: write your implementation of GetHashCode() here

            return x.GetHashCode() + y.GetHashCode() + z.GetHashCode();
        }

        public static SVector3 operator +(SVector3 a, SVector3 b)
        {
            return new SVector3(a.x + b.x, a.y + b.y, a.z + b.z);
        }
        public static SVector3 operator -(SVector3 a, SVector3 b)
        {
            return new SVector3(a.x - b.x, a.y - b.y, a.z - b.z);
        }
        public static SVector3 operator *(SVector3 a, SVector3 b)
        {
            return new SVector3(a.x * b.x, a.y * b.y, a.z * b.z);
        }
        public static SVector3 operator *(float a, SVector3 b)
        {
            return new SVector3(a * b.x, a * b.y, a * b.z);
        }
        public static SVector3 operator *(SVector3 b, float a)
        {
            return new SVector3(a * b.x, a * b.y, a * b.z);
        }
        public void Add(SVector3 b)
        {
            this.x += b.x;
            this.y += b.y;
            this.z += b.z;
        }
        public void Add(float x, float y, float z)
        {
            this.x += x;
            this.y += y;
            this.z += z;
        }

        public float Dot(SVector3 other)
        {
            return other.x * x + other.y * y + other.z * z;
        }
        public static float Dot(SVector3 a, SVector3 b)
        {

            return a.Dot(b);
        }
        public SVector3 Cross(SVector3 b)
        {
            return new SVector3(y * b.z - z * b.y, z * b.x - x * b.z, x * b.y - y * b.x);
        }
        public static SVector3 Cross(SVector3 a, SVector3 b)
        {
            return a.Cross(b);
        }
        public static float AngleFromTo(SVector3 a, SVector3 b)
        {
            return (float)Math.Acos(Dot(a.normalized, b.normalized));

        }

        public static SVector3 Project(SVector3 a, SVector3 normal)
        {

            float pl = Dot(a, normal);
            return pl * normal.normalized;
        }
        public static SVector3 ProjectOnPlane(SVector3 a, SVector3 normal)
        {
            return a - Project(a, normal);
        }
        public float magnitude
        {
            get
            {
                return (float)Math.Sqrt(x * x + y * y + z * z);
            }
        }
        public float sqrtMagnitude
        {
            get
            {
                return x * x + y * y + z * z;
            }
        }
        public void Normalize()
        {

            float length = magnitude;
            if(length == 0)
                return;
            x /= length;
            y /= length;
            z /= length;
        }
        public SVector3 normalized
        {
            get
            {
                float length = magnitude;
                if (length == 0)
                    return new SVector3(0, 0, 0);
                return new SVector3(x / length, y / length, z / length);
            }
        }
        #endregion

        #region constant vector
        public static SVector3 zero
        {
            get
            {
                return new SVector3(0, 0, 0);
            }
        }
        public static SVector3 up
        {
            get
            {
                return new SVector3(0, 1, 0);
            }
        }
        public static SVector3 right
        {
            get
            {
                return new SVector3(1, 0, 0);
            }
        }
        public static SVector3 forward
        {
            get
            {
                return new SVector3(0, 0, 1);
            }
        }
        #endregion
    }
}