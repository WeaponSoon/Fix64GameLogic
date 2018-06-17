using System.Collections;
using System.Collections.Generic;
//using UnityEngine;

namespace SWPLogicLayerF{
	public class SComponent {
		internal SGameObject _gameObject;
		public SGameObject gameObject{
			get{
				return _gameObject;
			}
		}
		
		public virtual void BeforePhysicsUpdate()
		{

		}
		public virtual void OnCollisionEnter(SCollisionResult res)
		{

		}
		public virtual void OnCollisionStay(SCollisionResult res)
		{

		}
		public virtual void OnCollisionEnd(SCollisionResult res)
		{

		}
		public virtual void Update()
		{

		}
		public virtual void LateUpdate()
		{
			
		}

		public virtual void OnDestroy()
		{
			
		}

	}
}

