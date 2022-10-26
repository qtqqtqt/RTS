using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace RTS.Combat
{
    public class Targetable : NetworkBehaviour
    {
        [SerializeField] Transform aimAtPoint = null;

        public Transform GetAimAtPoint()
        {
            return aimAtPoint;
        }
    }
}
