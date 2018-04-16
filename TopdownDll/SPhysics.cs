using System;
using System.Collections;
using System.Collections.Generic;
using FixMath.NET;

namespace SWPLogicLayerF
{
    public struct SCollisionResult
    {
        public SICollider collider;
        public SVector3 position;
        public SVector3 normal;

    }
    public class SPhysics
    {
        private List<SICollider> _physicsObjectList = new List<SICollider>();
        internal List<SICollider> physicsObjectList
        {
            get
            {
                return _physicsObjectList;
            }
        }
        internal void AddColliderBody(SICollider collider)
        {
            _physicsObjectList.Add(collider);
        }
        internal void DeleteColliderBody(SICollider collider)
        {
            _physicsObjectList.Remove(collider);
        }

        internal SPhysics()
        {

        }

        internal void Update()
        {
            foreach (var i in _physicsObjectList)
            {
                foreach (var j in _physicsObjectList)
                {
                    if(i != j)
                    {
                        i.DetectCollision(j);
                    }
                }
            }
        }

    }

    public abstract class SICollider : SComponent
    {
        
        private SVector3 _localPosition = SVector3.zero;
        private SQuaternion _localRotation = new SQuaternion((Fix64)0,(Fix64)0,(Fix64)0,(Fix64)1);
        private Fix64 _radius;
        
        public SVector3 localPosition
        {
            get {
                return _localPosition;
            }
            set {
                _localPosition = value;
            }
        }
        public SQuaternion localRotation
        {
            get
            {
                return _localRotation;
            }            
            set 
            {
                _localRotation = value;
            }
        }
        public SVector3 position
        {
            get
            {
                SVector3 t1 = gameObject.transform.right * localPosition.x;
                SVector3 t2 = gameObject.transform.up * localPosition.y;
                SVector3 t3 = gameObject.transform.forward * localPosition.z;
                return t1 + t2 + t3 + gameObject.transform.position;
            }
            set
            {
                var pPos = gameObject.transform.position;
                var cpq = gameObject.transform.rotation.conjugated;
                _localPosition = SQuaternion.GetRotVectorFromOrigin(value - pPos, cpq);
            }
        }
        public SQuaternion rotation
        {
            get
            {
                return gameObject.transform.rotation * _localRotation;
            }
            set
            {
                _localRotation = gameObject.transform.rotation.conjugated * value;
            }
        }

        public SVector3 up
        {
            get
            {
                return SQuaternion.GetRotVectorFromOrigin(SVector3.up, rotation);
            }
        }
        public SVector3 right
        {
            get
            {
                return SQuaternion.GetRotVectorFromOrigin(SVector3.right, rotation);
            }
        }
        public SVector3 forward
        {
            get
            {
                return SQuaternion.GetRotVectorFromOrigin(SVector3.forward, rotation);
            }
        }

        public Fix64 radius
        {
            get
            {
                return _radius;
            }
            set
            {
                _radius = value;
            }
        }
        public SICollider()
        {
            if(SFrameWork.frameWork != null)
            {
                SFrameWork.frameWork.physics.AddColliderBody(this);
            }
            else
            {
                throw new NoSFrameworkException();
            }
        }
        public override void OnDestroy()
        {
            if(SFrameWork.frameWork != null)
            {
                SFrameWork.frameWork.physics.DeleteColliderBody(this);
            }
        }

        internal abstract void DetectCollision(SICollider other);
    }

    public class SSphereCollider : SICollider
    {
        internal override void DetectCollision(SICollider other)
        {
            if(other.GetType().Equals(typeof(SSphereCollider)))
            {

            }
            else if(other.GetType().Equals(typeof(SCapsuleCollider)))
            {

            }
            
        }
    }
    public class SCapsuleCollider : SICollider
    {
        private Fix64 _length = Fix64.One;
        public Fix64 length
        {
            get{
                return _length;
            }
            set{
                _length = value;
            }
        }
        public SVector3 positionA
        {
            get
            {
                return position - (length/(Fix64)2) * up;
            }
        }
        public SVector3 positionB
        {
            get
            {
                return position + (length/(Fix64)2) * up;
            }
        }
        
        
        internal override void DetectCollision(SICollider other)
        {
            if(other.GetType().Equals(typeof(SSphereCollider)))
            {

            }
            else if(other.GetType().Equals(typeof(SCapsuleCollider)))
            {

                Fix64 deltaRange = (Fix64)1 / (Fix64)1000;
                //SDebug.Log(deltaRange);
                
                SCapsuleCollider oth = other as SCapsuleCollider;
                if (Fix64.Abs(up.Dot(oth.up)) > deltaRange)
                {
                    Fix64 tIn1 = (oth.positionA.Dot(oth.up) - positionA.Dot(oth.up)) / up.Dot(oth.up);
                    Fix64 tIn2 = (oth.positionB.Dot(oth.up) - positionA.Dot(oth.up)) / up.Dot(oth.up);
                    Fix64 tInMin = ((tIn1 < tIn2) ? tIn1 : tIn2);
                    Fix64 tInMax = ((tIn1 < tIn2) ? tIn2 : tIn1);

                    //SDebug.Log(tInMin + "  " + tInMax);

                    var w = positionA - oth.positionA;
                    var wf = positionA - oth.positionB;
                    var u12 = up.Dot(oth.up);
                    var u1w = up.Dot(w);
                    var u2w = oth.up.Dot(w);
                    var u1wf = up.Dot(wf);


                    var minDis = Fix64.MaxValue;
                    var minMiddle = Fix64.MaxValue;
                    var minLeft = Fix64.MaxValue;
                    var minRight = Fix64.MaxValue;

                    Fix64 et = Fix64.Zero;
                    Fix64 etLeft = Fix64.Zero;
                    Fix64 etMiddle = Fix64.Zero;
                    Fix64 etRight = Fix64.Zero;

                    if(tInMin <= length && tInMax >= Fix64.Zero)
                    {
                        minMiddle = minValueFunc((Fix64)1 - u12 * u12, (Fix64)2 * (u1w - u12 * u2w), w.sqrtMagnitude - u2w * u2w,
                        MathF.max((Fix64)0, tInMin), MathF.min(length, tInMax), out etMiddle);
                        //SDebug.Log(gameObject.name+"  middle: " + minMiddle);

                    }
                    if(tInMin >= Fix64.Zero)
                    {
                        Fix64 temEtLeft1 = Fix64.Zero;
                        Fix64 temEtLeft2 = Fix64.Zero;
                        
                        var temMin1 = 
                        minValueFunc(Fix64.One,(Fix64)2 * u1w, w.sqrtMagnitude, 
                        Fix64.Zero, MathF.min(length, tInMin), out temEtLeft1);
                        
                        var temMin2 = 
                        minValueFunc(Fix64.One,(Fix64)2 * u1wf, wf.sqrtMagnitude, 
                        Fix64.Zero, MathF.min(length, tInMin), out temEtLeft2);
                        
                        etLeft = (temMin1 < temMin2) ? temEtLeft1 : temEtLeft2;
                        minLeft = MathF.min(temMin1, temMin2);
                        //SDebug.Log(gameObject.name+"  left: " + minLeft + " " + etLeft);
                    }
                    if(tInMax <= length)
                    {
                        Fix64 temEtRight1 = Fix64.Zero;
                        Fix64 temEtRight2 = Fix64.Zero;
                        
                        var temMin1 = 
                        minValueFunc(Fix64.One,(Fix64)2 * u1w, w.sqrtMagnitude, 
                        MathF.max(Fix64.Zero,tInMax), length, out temEtRight1);
                        
                        var temMin2 = 
                        minValueFunc(Fix64.One,(Fix64)2 * u1wf, wf.sqrtMagnitude, 
                        MathF.max(Fix64.Zero,tInMax), length, out temEtRight2);
                        
                        etRight = (temMin1 < temMin2) ? temEtRight1 : temEtRight2;
                        minRight = MathF.min(temMin1, temMin2);
                        //SDebug.Log(gameObject.name+"  right: " + minRight + " " + etRight);
                    }
                    if(minLeft <= minRight && minLeft <= minMiddle)
                    {
                        et = etLeft;
                        minDis = minLeft;
                    }
                    else if(minMiddle <= minLeft && minMiddle <= minRight)
                    {
                        et = etMiddle;
                        minDis = minMiddle;
                    }
                    else
                    {
                        et = etRight;
                        minDis = minRight;
                    }
                    if(minDis < radius + oth.radius)
                    {
                        SDebug.LogWarning(gameObject.name + " BAAAAAANG!!!  " + minDis);
                    }
                }
                else
                {
                    Fix64 minDis = Fix64.MaxValue;
                    Fix64 et = Fix64.Zero;
                    SDebug.LogWarning("Vertical");
                    
                    
                    var p1tp2 = positionA - oth.positionA;
                    var p1tp2f = positionA - oth.positionB;
                    var ti = p1tp2.Dot(oth.up);
                    var tj = p1tp2f.Dot(oth.up);

                    if(ti*tj <= Fix64.Zero)
                    {
                        var a = Fix64.One;
                        var w = positionA - oth.positionA;
                        var b = (Fix64)2 * up.Dot(w);
                        var c = w.sqrtMagnitude - oth.up.Dot(w)* oth.up.Dot(w);

                        
                        minDis = 
                        minValueFunc(a,b,c,Fix64.Zero, length, out et );
                        //minDis = temMin;
                    }
                    else if(ti <= Fix64.Zero)
                    {
                        var w = positionA - oth.positionA;
                        var u1w = up.Dot(w);
                        minDis = 
                        minValueFunc(Fix64.One, (Fix64)2 * u1w, w.sqrtMagnitude, Fix64.Zero, length, out et);
                    }
                    else if(tj >= Fix64.Zero)
                    {
                       
                        var wf = positionA - oth.positionB;
                        var u1wf = up.Dot(wf);
                        minDis = 
                        minValueFunc(Fix64.One, (Fix64)2 * u1wf, wf.sqrtMagnitude, Fix64.Zero, length, out et);
                    }

                    if(minDis < radius + oth.radius)
                    {
                        SDebug.LogWarning(gameObject.name + " BAAAAAANG!!!  " + minDis);
                    }
                }




            }
        }
        private Fix64 minValueFunc(Fix64 a, Fix64 b, Fix64 c, Fix64 l1, Fix64 l2, out Fix64 et)
        {
            return minValueFunc(a,b,c,l1,l2,(Fix64)1/(Fix64)1000,out et);
        }
        private Fix64 minValueFunc(Fix64 a, Fix64 b, Fix64 c, Fix64 l1, Fix64 l2, Fix64 aDelta, out Fix64 et)
        {
            if(Fix64.Abs(a) <= aDelta)
            {
                var r1 = b*l1 + c;
                var r2 = b*l2 + c;
                et = (r1 < r2) ? l1 : l2;
                return (r1 < r2) ? r1 : r2;
            }
            if(a > Fix64.Zero)
            {
                var ext = -b/((Fix64)2 * a);
                if(ext < l1)
                {
                    et = l1;
                    return a * l1 * l1 + b * l1 + c;
                }
                if(ext > l2)
                {
                    et = l2;
                    return a * l2 * l2 + b * l2 + c;
                }
                et = ext;
                return a * ext * ext + b * ext + c;
            }
            if(a < Fix64.Zero)
            {
                var ext = -b/((Fix64)2 * a);
                if(ext < l1)
                {
                    et = l2;
                    return a * l2 * l2 + b * l2 + c;
                }
                if(ext > l2)
                {
                    et = l1;
                    return a * l1 * l1 + b * l1 + c;
                }
                var r1 = a * l2 * l2 + b * l2 + c;
                var r2 = a * l1 * l1 + b * l1 + c;
                et = (r1 < r2) ? l1 : l2;
                return (r1 < r2) ? r1 : r2;
            }
            et = Fix64.Zero;
            return (Fix64)0;
        }
       
    }
}