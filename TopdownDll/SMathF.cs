using System;
using System.Collections;
using System.Collections.Generic;
using FixMath.NET;
//using UnityEngine;

namespace SWPLogicLayerF
{

    public struct SQuaternion
    {
        //Fix64 a;
        public Fix64 x;
        public Fix64 y;
        public Fix64 z;
        public Fix64 w;

        public Fix64 sqrtMagnitude
        {
            get
            {
                
                return x * x + y * y + z * z + w * w;
            }
        }
        public Fix64 magnitude
        {
            get
            {
                return (Fix64)Fix64.Sqrt(sqrtMagnitude);
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
                Fix64 l = sqrtMagnitude;
                return new SQuaternion(-x / l, -y / l, -z / l, w / l);
            }
        }

        public SQuaternion normalized
        {
            get
            {
                Fix64 l = magnitude;
                return new SQuaternion(x / l, y / l, z / l, w / l);
            }
        }


        public override string ToString()
        {
            return x + " " + y + " " + z + " " + w;
        }
        public SQuaternion(Fix64 x, Fix64 y, Fix64 z, Fix64 w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }
        public static SQuaternion AxisAngle(SVector3 axis, Fix64 angle)
        {
            axis.Normalize();
            Fix64 x = (Fix64)(axis.x * Fix64.Sin(angle / (Fix64)2));
            Fix64 y = (Fix64)(axis.y * Fix64.Sin(angle / (Fix64)2));
            Fix64 z = (Fix64)(axis.z * Fix64.Sin(angle / (Fix64)2));
            Fix64 w = (Fix64)Fix64.Cos(angle / (Fix64)2);
            return new SQuaternion(x, y, z, w);
        }
        public static SQuaternion AxisAngleInDeg(SVector3 axis, Fix64 angle)
        {
            Fix64 temAngle = angle / (Fix64)180 * (Fix64)Fix64.Pi;
            return AxisAngle(axis, temAngle);
        }


        public void Normalize()
        {
            Fix64 l = magnitude;
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
            Fix64 l = magnitude;
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
        public static SQuaternion operator *(Fix64 num, SQuaternion q)
        {
            return new SQuaternion(num * q.x, num * q.y, num * q.z, num * q.w);
        }
        public static SQuaternion operator *(SQuaternion q, Fix64 num)
        {
            return num * q;
        }
        public static SQuaternion operator /(SQuaternion q, Fix64 num)
        {
            return ((Fix64)1 / num) * q;
        }

        public static SQuaternion operator *(SQuaternion a, SQuaternion b)
        {
            Fix64 tw = (a.w * b.w - a.x * b.x - a.y * b.y - a.z * b.z);
            Fix64 tx = (a.w * b.x + a.x * b.w + a.y * b.z - a.z * b.y);
            Fix64 ty = (a.w * b.y - a.x * b.z + a.y * b.w + a.z * b.x);
            Fix64 tz = (a.w * b.z + a.x * b.y - a.y * b.x + a.z * b.w);
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

        public static Fix64 Dot(SQuaternion a, SQuaternion b)
        {
            return a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
        }
        public static SVector3 GetRotVectorFromOrigin(SVector3 origin, SQuaternion q)
        {
            SQuaternion temq = new SQuaternion(origin.x, origin.y, origin.z, (Fix64)0);
            SQuaternion finalq = q * temq * q.conjugated;
            return new SVector3(finalq.x, finalq.y, finalq.z);
        }

        public static SVector3 GetRotVectorFromX(SVector3 origin, SQuaternion q)
        {
            Fix64 length = origin.magnitude;
            return GetRotVectorFromOrigin(SVector3.right, q) * length;
        }




        public static SQuaternion Lerp(SQuaternion a, SQuaternion b, Fix64 t)
        {
            if (a == b)
            {
                return a;
            }



            Fix64 cosa = Dot(a, b);
            if (cosa < (Fix64)0)
            {
                b = -b;
            }


            Fix64 sina = (Fix64)Fix64.Sqrt((Fix64)1 - cosa * cosa);
            Fix64 angle = (Fix64)Fix64.Atan2(sina, cosa);
            Fix64 k0 = (Fix64)Fix64.Sin(((Fix64)1 - t) * angle) / sina;
            Fix64 k1 = (Fix64)Fix64.Sin(t * angle) / sina;

            return k0 * a + k1 * b;
        }
        public static SQuaternion LerpLargeAngle(SQuaternion a, SQuaternion b, Fix64 t)
        {
            if (a == b)
            {
                return a;
            }



            Fix64 cosa = Dot(a, b);
            if (cosa >= (Fix64)0)
            {
                b = -b;
            }


            Fix64 sina = (Fix64)Fix64.Sqrt((Fix64)1 - cosa * cosa);
            Fix64 angle = (Fix64)Fix64.Atan2(sina, cosa);
            Fix64 k0 = (Fix64)Fix64.Sin(((Fix64)1 - t) * angle) / sina;
            Fix64 k1 = (Fix64)Fix64.Sin(t * angle) / sina;

            return k0 * a + k1 * b;
        }


    }


    public struct SVector2
    {

        public Fix64 x;

        public Fix64 y;

        public override string ToString()
        {
            return "(" + x + ", " + y + ")";
        }

        public SVector2(Fix64 x, Fix64 y)
        {
            this.x = x;
            this.y = y;
        }

        public SVector2(Fix64 angleFromX)
        {
            this.x = (Fix64)Fix64.Cos(angleFromX);
            this.y = (Fix64)Fix64.Sin(angleFromX);
        }



        #region operators

        public Fix64 GetAngleFromX()
        {
            if (sqrtMagnitude <= (Fix64)0)
            {
                return (Fix64)0;
            }
            Fix64 cosA = x / magnitude;
            Fix64 sinA = y / magnitude;
            if (sinA == (Fix64)0 && cosA < -Fix64.One / (Fix64)2)
            {
                return (Fix64)Fix64.Pi;
            }
            Fix64 temAngle = (Fix64)Fix64.Sign(sinA) * (Fix64)Fix64.Acos(cosA);

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
        public static SVector2 operator *(Fix64 a, SVector2 b)
        {
            return new SVector2(a * b.x, a * b.y);
        }
        public static SVector2 operator *(SVector2 b, Fix64 a)
        {
            return new SVector2(a * b.x, a * b.y);
        }

        public void Add(SVector2 b)
        {
            this.x += b.x;
            this.y += b.y;
        }
        public void Add(Fix64 x, Fix64 y)
        {
            this.x += x;
            this.y += y;
        }
        public void RotFromThis(Fix64 angle)
        {
            Fix64 dAngle = GetAngleFromX() + angle;
            Fix64 length = magnitude;
            x = (Fix64)Fix64.Cos(dAngle) * length;
            y = (Fix64)Fix64.Sin(dAngle) * length;
        }
        public void RotFromX(Fix64 angle)
        {
            Fix64 length = magnitude;
            x = (Fix64)Fix64.Cos(angle) * length;
            y = (Fix64)Fix64.Sin(angle) * length;
        }
        public SVector2 GetRotVectorFromThis(Fix64 angle)
        {
            Fix64 dAngle = GetAngleFromX() + angle;
            return magnitude * new SVector2(dAngle);
        }
        public SVector2 GetRotVectorFromX(Fix64 angle)
        {
            return magnitude * new SVector2(angle);
        }
        public Fix64 Dot(SVector2 other)
        {
            return other.x * x + other.y * y;
        }
        public static Fix64 Dot(SVector2 a, SVector2 b)
        {

            return a.Dot(b);
        }
        public SVector3 Cross(SVector2 b)
        {
            return new SVector3((Fix64)0, (Fix64)0, x * b.y - y * b.x);
        }
        public static SVector3 Cross(SVector2 a, SVector2 b)
        {
            return a.Cross(b);
        }
        public Fix64 magnitude
        {
            get
            {
                return (Fix64)Fix64.Sqrt(x * x + y * y);
            }
        }
        public Fix64 sqrtMagnitude
        {
            get
            {
                return x * x + y * y;
            }
        }
        public void Normalize()
        {
            Fix64 length = magnitude;
            x /= length;
            y /= length;

        }
        public SVector2 normalized
        {
            get
            {
                Fix64 length = magnitude;
                return new SVector2(x / length, y / length);
            }
        }
        #endregion

        #region constant vector
        public static SVector2 zero
        {
            get
            {
                return new SVector2((Fix64)0, (Fix64)0);
            }
        }
        public static SVector2 up
        {
            get
            {
                return new SVector2((Fix64)0, (Fix64)1);
            }
        }
        public static SVector2 right
        {
            get
            {
                return new SVector2((Fix64)1, (Fix64)0);
            }
        }
        #endregion


    }

    public struct SVector3
    {
        public Fix64 x;
        public Fix64 y;
        public Fix64 z;
        public override string ToString()
        {
            return "(" + x + "," + y + "," + z + ")";
        }
        public SVector3(Fix64 x, Fix64 y, Fix64 z)
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
        public static SVector3 operator *(Fix64 a, SVector3 b)
        {
            return new SVector3(a * b.x, a * b.y, a * b.z);
        }
        public static SVector3 operator *(SVector3 b, Fix64 a)
        {
            return new SVector3(a * b.x, a * b.y, a * b.z);
        }
        public void Add(SVector3 b)
        {
            this.x += b.x;
            this.y += b.y;
            this.z += b.z;
        }
        public void Add(Fix64 x, Fix64 y, Fix64 z)
        {
            this.x += x;
            this.y += y;
            this.z += z;
        }

        public Fix64 Dot(SVector3 other)
        {
            return other.x * x + other.y * y + other.z * z;
        }
        public static Fix64 Dot(SVector3 a, SVector3 b)
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
        public static Fix64 AngleFromTo(SVector3 a, SVector3 b)
        {
            return (Fix64)Fix64.Acos(Dot(a.normalized, b.normalized));

        }

        public static SVector3 Project(SVector3 a, SVector3 normal)
        {

            Fix64 pl = Dot(a, normal);
            return pl * normal.normalized;
        }
        public static SVector3 ProjectOnPlane(SVector3 a, SVector3 normal)
        {
            return a - Project(a, normal);
        }
        public Fix64 magnitude
        {
            get
            {
                return (Fix64)Fix64.Sqrt(x * x + y * y + z * z);
            }
        }
        public Fix64 sqrtMagnitude
        {
            get
            {
                return x * x + y * y + z * z;
            }
        }
        public void Normalize()
        {

            Fix64 length = magnitude;
            if(length == (Fix64)0)
                return;
            x /= length;
            y /= length;
            z /= length;
        }
        public SVector3 normalized
        {
            get
            {
                Fix64 length = magnitude;
                if (length == (Fix64)0)
                    return new SVector3((Fix64)0, (Fix64)0, (Fix64)0);
                return new SVector3(x / length, y / length, z / length);
            }
        }
        #endregion

        #region constant vector
        public static SVector3 zero
        {
            get
            {
                return new SVector3((Fix64)0, (Fix64)0, (Fix64)0);
            }
        }
        public static SVector3 up
        {
            get
            {
                return new SVector3((Fix64)0, (Fix64)1, (Fix64)0);
            }
        }
        public static SVector3 right
        {
            get
            {
                return new SVector3((Fix64)1, (Fix64)0, (Fix64)0);
            }
        }
        public static SVector3 forward
        {
            get
            {
                return new SVector3((Fix64)0, (Fix64)0, (Fix64)1);
            }
        }
        #endregion
    }
}