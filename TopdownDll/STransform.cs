using FixMath.NET;
using System;
using System.Collections;
using System.Collections.Generic;
//using UnityEngine;

namespace SWPLogicLayerF
{

    public enum FollowTag
    {
        KeepWorld,
        Snap,
        KeepRaletive
    }

    public class STransform : SComponent
    {
        private SVector3 _localPosition;
        private SQuaternion _localRotation;

        public SVector3 localPosition
        {
            get
            {
                return _localPosition;
            }
            set
            {
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

        public SVector3 forward
        {
            get
            {
                return SQuaternion.GetRotVectorFromOrigin(SVector3.forward, rotation);
            }
        }
        public SVector3 right
        {
            get
            {
                return SQuaternion.GetRotVectorFromOrigin(SVector3.right, rotation);
            }
        }
        public SVector3 up
        {
            get
            {
                return SQuaternion.GetRotVectorFromOrigin(SVector3.up, rotation);
            }
        }
        public SVector3 position
        {
            get
            {

                if (gameObject != null && gameObject.parents != null)
                {
                    SVector3 pPos = gameObject.parents.GetComponent<STransform>().position;
                    SVector3 right = gameObject.parents.GetComponent<STransform>().right;
                    SVector3 up = gameObject.parents.GetComponent<STransform>().up;
                    SVector3 forward = gameObject.parents.GetComponent<STransform>().forward;
                    return _localPosition.x * right + _localPosition.y * up + localPosition.z * forward + pPos;

                    // SVector2 pPos = gameObject.parents.GetComponent<STransform2D>().position;
                    // Fix64 pRot = gameObject.parents.GetComponent<STransform2D>().rotation;
                    // return pPos + _localPosition.GetRotVectorFromThis(pRot);

                }
                else
                {

                    return _localPosition;
                }
            }
            set
            {
                if (gameObject != null && gameObject.parents != null)
                {
                    SVector3 pPos = gameObject.parents.GetComponent<STransform>().position;
                    SQuaternion cpq = gameObject.parents.GetComponent<STransform>().rotation.conjugated;
                    _localPosition = SQuaternion.GetRotVectorFromOrigin(value - pPos, cpq);
                }
                else
                {
                    _localPosition = value;
                }
            }
        }
        public SQuaternion rotation
        {
            get
            {
                if (gameObject != null && gameObject.parents != null)
                {
                    return gameObject.parents.GetComponent<STransform>().rotation * _localRotation;
                    // return _localRotation * gameObject.parents.GetComponent<STransform>().rotation;
                    // SVector2 pPos = gameObject.parents.GetComponent<STransform2D>().position;
                    // Fix64 pRot = gameObject.parents.GetComponent<STransform2D>().rotation;
                    // return pPos + _localPosition.GetRotVectorFromThis(pRot);

                }
                else
                {

                    return _localRotation;
                }

            }
            set
            {
                if (gameObject != null && gameObject.parents != null)
                {
                    localRotation =  gameObject.parents.GetComponent<STransform>().rotation.conjugated * value;
                }
                else
                {
                    localRotation = value;
                }
            }
        }

        public void RotateAroundAxis(SVector3 axis, Fix64 angle)
        {
            rotation = SQuaternion.AxisAngle(axis, angle) * rotation;
        }

        public void LookAt(SVector3 lookAtPoint)
        {
            SVector3 dir = lookAtPoint - position;
            //SVector3 norm = SVector3.Cross(forward, dir);
            if(dir.sqrtMagnitude < Fix64.One / (Fix64)10)
                return;
            
            SVector3 norm = SVector3.Cross(forward, dir);
            if(norm.sqrtMagnitude < Fix64.One / (Fix64)100 && SVector3.Dot(dir.normalized,forward) > (Fix64)0)
            {
                return;
            }
            else if(norm.sqrtMagnitude < Fix64.One / (Fix64)100 && SVector3.Dot(dir.normalized,forward) < (Fix64)0)
            {
                RotateAroundAxis(up, (Fix64)Fix64.Pi / (Fix64)2);
            }
            else
            {
                
                
                RotateAroundAxis(norm.normalized, SVector3.AngleFromTo(forward, dir));
            }
        }

        public STransform()
        {
            _localRotation = SQuaternion.AxisAngle(SVector3.up, (Fix64)0);
            _localPosition = SVector3.zero;
        }

    }

    public class STransform2D : SComponent
    {


        public FollowTag positionFollowTag = FollowTag.KeepRaletive;
        public FollowTag rotationFollowTag = FollowTag.KeepRaletive;

        private SVector2 _localPosition = SVector2.zero;

        public SVector2 position
        {
            get
            {
                if (gameObject != null && gameObject.parents != null)
                {
                    SVector2 pPos = gameObject.parents.GetComponent<STransform2D>().position;
                    Fix64 pRot = gameObject.parents.GetComponent<STransform2D>().rotation;
                    return pPos + _localPosition.GetRotVectorFromThis(pRot);
                }
                else
                {
                    return _localPosition;
                }
            }
            set
            {
                if (gameObject != null && gameObject.parents != null)
                {
                    SVector2 pPos = gameObject.parents.GetComponent<STransform2D>().position;
                    Fix64 pRot = gameObject.parents.GetComponent<STransform2D>().rotation;

                    _localPosition = (value - pPos).GetRotVectorFromThis(-pRot);
                    //return pPos + _localPosition.GetRotVectorFromThis(pRot);
                }
                else
                {
                    _localPosition = value;
                }
                // List<SGameObject> children = null;
                // List<SVector2> bPos = null;
                // List<Fix64> bRot = null;
                // if (gameObject != null)
                // {
                //     children = gameObject.children;
                //     bPos = new List<SVector2>(children.Count);
                //     bRot = new List<Fix64>(children.Count);
                //     for (int i = 0; i < children.Count; ++i)
                //     {
                //         bPos.Add(children[i].GetComponent<STransform2D>().localPosition);
                //         bRot.Add(children[i].GetComponent<STransform2D>().localRotation);
                //     }
                // }
                // _position = value;
                // if (children != null)
                // {
                //     for (int i = 0; i < children.Count; ++i)
                //     {
                //         children[i].GetComponent<STransform2D>().localPosition = bPos[i];
                //         children[i].GetComponent<STransform2D>().localRotation = bRot[i];
                //     }
                // }
            }
        }
        private SVector2 _localRotation = SVector2.right;

        public SVector2 localPosition
        {
            get
            {
                return _localPosition;
                // if (gameObject != null && gameObject.parents != null)
                // {
                //     SVector2 pPos = gameObject.parents.GetComponent<STransform2D>().position;
                //     Fix64 pRot = gameObject.parents.GetComponent<STransform2D>().rotation;
                //     return (position - pPos).GetRotVectorFromThis(-pRot);
                // }
                // else
                // {
                //     return position;
                // }
            }
            set
            {
                _localPosition = value;
                // if (gameObject != null && gameObject.parents != null)
                // {
                //     SVector2 pPos = gameObject.parents.GetComponent<STransform2D>().position;
                //     Fix64 pRot = gameObject.parents.GetComponent<STransform2D>().rotation;
                //     value.RotFromThis(pRot);
                //     position = pPos + value;
                //     //return (position - pPos).GetRotVectorFromThis(-pRot);
                // }
                // else
                // {
                //     position = value;
                // }
            }
        }

        public Fix64 localRotationInDeg
        {
            get
            {
                return localRotation * (Fix64)180 / (Fix64)Fix64.Pi;
            }
            set
            {
                localRotation = value * ((Fix64)Fix64.Pi / (Fix64)180);
            }
        }
        public Fix64 localRotation
        {
            get
            {
                return _localRotation.GetAngleFromX();
                // if (gameObject != null && gameObject.parents != null)
                // {
                //     return rotation - gameObject.parents.GetComponent<STransform2D>().rotation;
                // }
                // else
                // {
                //     return rotation;
                // }
            }
            set
            {
                _localRotation.RotFromX(value);
                // if (gameObject != null && gameObject.parents != null)
                // {
                //     rotation = value + gameObject.parents.GetComponent<STransform2D>().rotation;
                //     //return rotation - gameObject.parents.GetComponent<STransform2D>().rotation;
                // }
                // else
                // {
                //     rotation = value;
                // }
            }

        }

        public Fix64 rotationInDeg
        {
            get
            {
                return rotation * (Fix64)180 / (Fix64)Fix64.Pi;
            }
            set
            {
                rotation = value * ((Fix64)Fix64.Pi / (Fix64)180);
            }
        }
        public Fix64 rotation
        {
            get
            {
                if (gameObject != null && gameObject.parents != null)
                {
                    return localRotation + gameObject.parents.GetComponent<STransform2D>().rotation;
                }
                else
                {
                    return localRotation;
                }
            }
            set
            {

                if (gameObject != null && gameObject.parents != null)
                {
                    localRotation = value - gameObject.parents.GetComponent<STransform2D>().rotation;
                }
                else
                {
                    localRotation = value;
                }
                // List<SGameObject> children = null;
                // List<SVector2> bPos = null;
                // List<Fix64> bRot = null;
                // if (gameObject != null)
                // {
                //     children = gameObject.children;
                //     bPos = new List<SVector2>(children.Count);
                //     bRot = new List<Fix64>(children.Count);
                //     for (int i = 0; i < children.Count; ++i)
                //     {
                //         bPos.Add(children[i].GetComponent<STransform2D>().localPosition);
                //         bRot.Add(children[i].GetComponent<STransform2D>().localRotation);
                //     }
                // }
                // _rotation.RotFromX(value);
                // if (children != null)
                // {
                //     for (int i = 0; i < children.Count; ++i)
                //     {
                //         children[i].GetComponent<STransform2D>().localPosition = bPos[i];
                //         children[i].GetComponent<STransform2D>().localRotation = bRot[i];
                //     }
                // }


            }
        }
        public SVector2 right
        {
            get
            {
                return new SVector2(rotation);
            }
        }
        public SVector2 up
        {
            get
            {
                return right.GetRotVectorFromThis((Fix64)Fix64.Pi / (Fix64)2);
            }
        }

    }
}
