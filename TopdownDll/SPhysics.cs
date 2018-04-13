using System;
using System.Collections;
using System.Collections.Generic;

namespace SWPLogicLayerF
{
    public class SPhysics
    {
        internal SPhysics()
        {

        }

        internal void Update()
        {
            
        }

    }

    public interface SICollider
    {
        
    }

    public class SSphereCollider : SComponent, SICollider
    {

    }
    public class SCapsuleCollider : SComponent, SICollider
    {
        
    }
}