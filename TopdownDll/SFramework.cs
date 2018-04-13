using System;
using System.Collections;
using System.Collections.Generic;
//using UnityEngine;

namespace SWPLogicLayerF
{


    public class NoSFrameworkException : Exception
    {

    }
    public class SDebug
    {

        public static void Assert(bool condition)
        {
            if (!condition)
                throw new NoSFrameworkException();
        }
        public delegate void ShowInfo(object msg);

        public static ShowInfo Log = _Log;
        public static ShowInfo LogWarning = _LogWarning;
        public static ShowInfo LogError = _LogError;

        public static void _Log(object msg)
        {
#if NOT_IN_UNITY
            
            ConsoleColor bef = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
#endif
            Console.WriteLine(msg);
#if NOT_IN_UNITY
            Console.ForegroundColor = bef;
#endif

        }
        public static void _LogWarning(object msg)
        {
#if NOT_IN_UNITY && DEBUG
            ConsoleColor bef = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
#endif
            Console.WriteLine(msg);
#if NOT_IN_UNITY && DEBUG
            Console.ForegroundColor = bef;
#endif
        }
        public static void _LogError(object msg)
        {
#if NOT_IN_UNITY && DEBUG
            ConsoleColor bef = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
#endif
            Console.WriteLine(msg);
#if NOT_IN_UNITY && DEBUG
            Console.ForegroundColor = bef;
#endif
        }
    }

    public class SingleTon<T> where T : new()
    {
        private static object mutex = new object();
        private static T _single;
        public static T single
        {
            get
            {
                if (_single == null)
                {
                    lock (mutex)
                    {
                        if (_single == null)
                        {
                            _single = new T();
                        }
                    }
                }
                return _single;
            }
        }

    }
    public class SFrameWork
    {
        private static object mutex = new object();
        public static SFrameWork _framework = null;
        public static SFrameWork frameWork
        {
            get
            {
                return _framework;
            }
        }
        public static bool InitFramework()
        {
            lock (mutex)
            {
                if (_framework == null)
                {
                    _framework = new SFrameWork();
                    return true;
                }
                else
                {
                    SDebug.LogWarning("the Framework has been inited");
                    return false;
                }
            }

        }
        public static void UninitFramework()
        {
            _framework.OnDestroy();
            _framework = null;
        }

        internal void DestroyGameObject(SGameObject obj)
        {
            obj.OnDestroy();
            _gameObjectList.Remove(obj);
        }

        private SFrameWork()
        {

        }

        private Queue<SGameObject> _willAddSGOQueue = new Queue<SGameObject>();
        private Queue<SGameObject> _wilDeleteSGOQueue = new Queue<SGameObject>();
        private Queue<SGameObject> _DeletedSGOQueue = new Queue<SGameObject>();
        delegate void Command();
        private Queue<Command> _cmdQueue = new Queue<Command>();
        private List<SGameObject> _gameObjectList = new List<SGameObject>();
        private SPhysics _physics = new SPhysics();
        public SPhysics physics
        {
            get
            {
                return _physics;
            }
        }        

        public int gameObjectNum
        {
            get
            {
                return _gameObjectList.Count;
            }
        }

        public SGameObject FindObject(string objname)
        {

            foreach (var i in _gameObjectList)
            {
                if (i.name.Equals(objname))
                    return i;
            }
            return null;
        }
        public List<SGameObject> FindObjects(string objname)
        {
            List<SGameObject> rets = new List<SGameObject>();
            foreach (var i in _gameObjectList)
            {
                if (i.name.Equals(objname))
                {
                    rets.Add(i);
                }
            }
            return rets;
        }

        public delegate bool Condition(SGameObject obj);
        public SGameObject FindObject(Condition condition)
        {
            if (condition == null)
                return null;
            foreach (var i in _gameObjectList)
            {
                if (condition(i))
                    return i;
            }
            return null;
        }
        public List<SGameObject> FindObjects(Condition condition)
        {
            if (condition == null)
                return null;
            List<SGameObject> rets = new List<SGameObject>();
            foreach (var i in _gameObjectList)
            {
                if (condition(i))
                {
                    rets.Add(i);
                }
            }
            return rets;
        }
        
        internal void AddToList(SGameObject sGameObject)
        {
            _willAddSGOQueue.Enqueue(sGameObject);
            _cmdQueue.Enqueue(AddToList);
            //_gameObjectList.Add(sGameObject);
        }
        private void AddToList()
        {
            if(_willAddSGOQueue.Count > 0)
            {
                var tem = _willAddSGOQueue.Dequeue();
                tem.Start();
                _gameObjectList.Add(tem);
            }
        }
        private void Remove(SGameObject obj)
        {
            _wilDeleteSGOQueue.Enqueue(obj);
            _cmdQueue.Enqueue(Remove);
        }
        private void Remove()
        {
            if(_wilDeleteSGOQueue.Count > 0)
            {
                var tem = _wilDeleteSGOQueue.Dequeue();
                tem.OnDestroy();
                _DeletedSGOQueue.Enqueue(tem);
            }
        }
        public void Destroy(SGameObject obj)
        {
            
            List<SGameObject> tem = obj.children;
            for(int i = tem.Count - 1; i >= 0; --i)
            {
                Destroy(tem[i]);
            }
            // foreach (var i in obj.children)
            // {
            //     Destroy(i);
            // }
            // obj.OnDestroy();
            Remove(obj);
        }
        public void Update()
        {
            while(_cmdQueue.Count > 0)
            {
                var cmd = _cmdQueue.Dequeue();
                cmd();
            }
            while(_DeletedSGOQueue.Count > 0)
            {
                var tem = _DeletedSGOQueue.Dequeue();
                tem.OnDelete();
                _gameObjectList.Remove(tem);
            }
            _DeletedSGOQueue.Clear();
            foreach (var i in _gameObjectList)
            {
                i.BeforeAllUpdate();
            }
            foreach (var i in _gameObjectList)
            {
                i.BeforePhysicsUpdate();
            }
            _physics.Update();
            foreach (var i in _gameObjectList)
            {
                i.Update();
            }
            foreach (var i in _gameObjectList)
            {
                i.LateUpdate();
            }
        }

        public void OnDestroy()
        {
            foreach (var i in _gameObjectList)
            {
                i.OnDestroy();
            }
            _gameObjectList.Clear();
        }
    }
}