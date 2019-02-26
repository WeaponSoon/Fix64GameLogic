using FixMath.NET;
using System;
using System.Collections;
using System.Collections.Generic;
//using UnityEngine;

namespace SWPLogicLayerF
{

    public class SGameObject
    {
        private struct ComponentTypePair
        {
            public Type type;
            public SComponent component;
            public ComponentTypePair(Type t, SComponent c)
            {
                type = t;
                component = c;
            }
            public static bool operator ==(ComponentTypePair a, ComponentTypePair b)
            {
                return a.type.Equals(b.type);
            }
            public static bool operator !=(ComponentTypePair a, ComponentTypePair b)
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

                return this == (ComponentTypePair)obj;
            }
            public override int GetHashCode()
            {
                return type.GetHashCode();
            }
        }

        ~SGameObject()
        {
            SDebug.Log(_name + " has been destroyed");
        }
        public override string ToString()
        {
            return _name;
        }
        private string _name;

        public string name
        {
            get
            {
                return _name;
            }
        }

        private bool _isAlive = true;
        private bool _isExcuteDestroyed = false;
        public bool isAlive
        {
            get
            {
                return _isAlive;
            }
        }
        private SGameObject _parents;
        private List<SGameObject> _children = new List<SGameObject>();

        public List<SGameObject> children
        {
            get
            {
                //Debug.LogError("aa+"+_children.Count);
                List<SGameObject> ret = new List<SGameObject>(_children);
                return ret;
            }
        }

        public STransform transform
        {
            get; private set;
        }
        public STransform2D transform2D
        {
            get; private set;
        }
        public SGameObject parents
        {
            get
            {
                return _parents;
            }
            set
            {
                if (value == null || !value.isAlive)
                {
                    if (_parents != null)
                        this._parents.RemoveChild(this);
                    return;
                }
                value.AddChirld(this);
            }
        }
        private Dictionary<Type, SComponent> _componentList = new Dictionary<Type, SComponent>();
        private Queue<ComponentTypePair> _willAddComponent = new Queue<ComponentTypePair>();
        private Queue<ComponentTypePair> _willDeleteComponent = new Queue<ComponentTypePair>();
        private Queue<ComponentTypePair> _DeletedComponent = new Queue<ComponentTypePair>();
       
        private delegate void Command();

        private Queue<Command> _commandQueue = new Queue<Command>();
       
        #region Constructors
        public SGameObject()
        {
            SDebug.Assert(SFrameWork.frameWork != null);
            transform = AddComponent<STransform>();
            transform2D = AddComponent<STransform2D>();
            _parents = null;

            _name = "New_SGameObject_" + SFrameWork.frameWork.gameObjectNum;

            SFrameWork.frameWork.AddToList(this);
        }

        public SGameObject(string objname)
        {
            SDebug.Assert(SFrameWork.frameWork != null);
            transform = AddComponent<STransform>();
            transform2D = AddComponent<STransform2D>();
            _parents = null;

            _name = objname;

            SFrameWork.frameWork.AddToList(this);
        }

        public SGameObject(STransform transform)
        {
            SDebug.Assert(SFrameWork.frameWork != null);
            _componentList.Add(typeof(STransform), transform);
            this.transform = transform;
            transform2D = AddComponent<STransform2D>();
            _parents = null;

            _name = "New_SGameObject_" + SFrameWork.frameWork.gameObjectNum;
            SFrameWork.frameWork.AddToList(this);

        }
        public SGameObject(STransform2D transform2D)
        {
            SDebug.Assert(SFrameWork.frameWork != null);
            this.transform = AddComponent<STransform>();
            _componentList.Add(typeof(STransform2D), transform2D);
            this.transform2D = transform2D;
            _parents = null;

            _name = "New_SGameObject_" + SFrameWork.frameWork.gameObjectNum;
            SFrameWork.frameWork.AddToList(this);
        }

        public SGameObject(SGameObject parents)
        {
            SDebug.Assert(SFrameWork.frameWork != null);
            transform = AddComponent<STransform>();
            transform2D = AddComponent<STransform2D>();
            if (parents != null && parents.isAlive)
                this.parents = parents;
            else
                this.parents = null;
            transform2D.localPosition = SVector2.zero;
            transform2D.localRotation = (Fix64)0;

            _name = "New_SGameObject_" + SFrameWork.frameWork.gameObjectNum;
            SFrameWork.frameWork.AddToList(this);
        }

        public SGameObject(SGameObject parents, string name)
        {
            SDebug.Assert(SFrameWork.frameWork != null);
            transform = AddComponent<STransform>();
            transform2D = AddComponent<STransform2D>();
            if (parents != null && parents.isAlive)
                this.parents = parents;
            else
                this.parents = null;
            transform2D.localPosition = SVector2.zero;
            transform2D.localRotation = (Fix64)0;

            _name = name;
            SFrameWork.frameWork.AddToList(this);
        }
        #endregion


        #region GameObject Tree
        public bool RemoveChild(SGameObject child)
        {

            if (_children.Contains(child))
            {
                child._parents = null;
                _children.Remove(child);
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool AddChirld(SGameObject child)
        {

            if (child == null || child == this || _children.Contains(child))
            {
                return false;
            }
            if (child._parents != null)
            {
                child._parents.RemoveChild(child);
            }
            child._parents = this;
            _children.Add(child);

            return true;
        }

        public SGameObject FindChild(string objname)
        {
            foreach (var i in _children)
            {
                if (i.name.Equals(objname))
                    return i;
            }
            return null;
        }
        public List<SGameObject> FindChildren(string objname)
        {
            List<SGameObject> rets = new List<SGameObject>();
            foreach (var i in _children)
            {
                if (i.name.Equals(objname))
                    rets.Add(i);
            }
            return rets;
        }
        public SGameObject FindChild(SFrameWork.Condition condition)
        {
            if (condition == null)
                return null;
            foreach (var i in _children)
            {
                if (condition(i))
                    return i;
            }
            return null;
        }
        public List<SGameObject> FindChildren(SFrameWork.Condition condition)
        {
            if (condition == null)
                return null;
            List<SGameObject> rets = new List<SGameObject>();
            foreach (var i in _children)
            {
                if (condition(i))
                    rets.Add(i);
            }
            return rets;
        }


        #endregion

        #region Component Operate  
        public SComponent GetComponent(Type type)
        {
            if (!isAlive)
                return null;
            if (HasComponentInWillDel(type))
            {
                return null;
            }
            if (_componentList.ContainsKey(type))
            {
                return _componentList[type];
            }
            if (HasComponentInWillAdd(type))
            {
                var willAddArray = _willAddComponent.ToArray();
                for (int i = 0; i < willAddArray.Length; ++i)
                {
                    if (willAddArray[i].type == type)
                    {
                        return willAddArray[i].component;
                    }
                }
            }
            return null;
        }

        public T GetComponent<T>() where T : SComponent, new()
        {
            if (!isAlive)
                return null;
            Type type = typeof(T);
            if (HasComponentInWillDel(type))
            {
                return null;
            }
            if (_componentList.ContainsKey(type))
            {
                return _componentList[type] as T;
            }
            if (HasComponentInWillAdd(type))
            {
                var willAddArray = _willAddComponent.ToArray();
                for(int i = 0; i < willAddArray.Length; ++i)
                {
                    if(willAddArray[i].type == type)
                    {
                        return willAddArray[i].component as T;
                    }
                }
            }
            return null;
        }

        public bool HasComponent(Type type)
        {
            if (!isAlive)
                return false;
            return (_componentList.ContainsKey(type) || HasComponentInWillAdd(type)) && !HasComponentInWillDel(type);
        }
        public bool HasComponent<T>() where T : SComponent, new()
        {
            if (!isAlive)
                return false;
            Type type = typeof(T);
            return (_componentList.ContainsKey(type) || HasComponentInWillAdd(type)) && !HasComponentInWillDel(type);
        }
        private bool HasComponentInWillAdd(Type t)
        {
            var tem = new ComponentTypePair(t, null);
            return _willAddComponent.Contains(tem);
        }
        private bool HasComponentInWillDel(Type t)
        {
            var tem = new ComponentTypePair(t, null);
            return _willDeleteComponent.Contains(tem);
        }
        private void AddComponent()
        {
            ComponentTypePair tem = _willAddComponent.Dequeue();
            _componentList.Add(tem.type, tem.component);
        }
        public SComponent AddComponent(Type type)
        {
            if (!isAlive)
                return null;
            if (!HasComponent(type))
            {
                
                SComponent sComponent = Activator.CreateInstance(type) as SComponent;
                if (sComponent != null)
                {

                    sComponent._gameObject = this;
                    _willAddComponent.Enqueue(new ComponentTypePair(type, sComponent));
                    _commandQueue.Enqueue(AddComponent);
                    //_componentList.Add(type, sComponent);
                    return sComponent;
                }
                else
                {
                    SDebug.LogError("Create Component Instance Failed");
                }
            }
            else
            {
                SDebug.LogError("this kind of component has already been added");
            }
            return null;

        }

        public T AddComponent<T>() where T : SComponent, new()
        {
            if (!isAlive)
                return null;
            Type type = typeof(T);
            if (!HasComponent(type))
            {
                T t = new T();
                t._gameObject = this;
                _willAddComponent.Enqueue(new ComponentTypePair(type, t));
                _commandQueue.Enqueue(AddComponent);
                //_componentList.Add(type, t);
                return t;
            }
            else
            {
                SDebug.LogError("this kind of component has already been added");
            }
            return null;
        }
        private void RemoveComponent()
        {
            Type t = _willDeleteComponent.Dequeue().type;
            _componentList.Remove(t);
        }

        public void RemoveComponent(Type type)
        {
            if (!isAlive)
                return;
            if(HasComponent(type))
            {
                _willDeleteComponent.Enqueue(new ComponentTypePair(type, null));
                _commandQueue.Enqueue(RemoveComponent);
            }
            // if (_componentList.ContainsKey(type))
            // {
            //     _componentList.Remove(type);
            // }
            else
            {
                SDebug.LogError("this kind of component has not been added yet");
            }
        }

        public void RemoveComponent<T>() where T : SComponent, new()
        {
            if (!isAlive)
                return;
            Type type = typeof(T);
            if(HasComponent(type))
            {
                _willDeleteComponent.Enqueue(new ComponentTypePair(type, null));
                _commandQueue.Enqueue(RemoveComponent);
            }
            // if (_componentList.ContainsKey(type))
            // {
            //     _componentList.Remove(type);
            // }
            else
            {
                SDebug.LogError("this kind of component has not been added yet");
            }
        }
        #endregion

        #region GameObject Lifecircle
        public void Awake()
        {

        }
        public void Start()
        {

        }
        internal void BeforeAllUpdate()
        {
            while(_commandQueue.Count > 0)
            {
                _commandQueue.Dequeue()();
            }
        }
        internal void BeforePhysicsUpdate()
        {
            if (isAlive)
                foreach (var i in _componentList)
                {
                    i.Value.BeforePhysicsUpdate();
                }
        }
        internal void OnCollisionEnter(SCollisionResult res)
        {
            if(isAlive)
            {
                foreach (var i in _componentList)
                {
                    i.Value.OnCollisionEnter(res);
                }
            }
        }
        internal void OnCollisionStay(SCollisionResult res)
        {
            if(isAlive)
            {
                foreach (var i in _componentList)
                {
                    i.Value.OnCollisionStay(res);
                }
            }
        }
        internal void OnCollisionEnd(SCollisionResult res)
        {
            if(isAlive)
            {
                foreach(var i in _componentList)
                {
                    i.Value.OnCollisionEnd(res);
                }
            }
        }
        internal void Update()
        {
            if (isAlive)
                foreach (var i in _componentList)
                {
                    i.Value.Update();
                }
        }
        internal void LateUpdate()
        {
            if (isAlive)
                foreach (var i in _componentList)
                {
                    i.Value.LateUpdate();
                }
        }
        internal void OnDestroy()
        {
            if (!_isExcuteDestroyed)
            {
                foreach (var i in _componentList)
                {
                    i.Value.OnDestroy();
                }
                _isExcuteDestroyed = true;
            }
        }

        internal void OnDelete()
        {
            if (_componentList != null)
            {
                parents = null;
                _componentList.Clear();
                _componentList = null;
            }
            _isAlive = false;

        }
        #endregion

    }

}
