using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace RTS.Combat
{
    public class Targeter : NetworkBehaviour
    {
        Targetable target;

        public Targetable GetTarget()
        {
            return target;
        }

        #region Server

        [Command]
        public void CmdSetTarget(GameObject targetGameObject)
        {
            if (!targetGameObject.TryGetComponent(out Targetable target)) return;

            this.target = target;
        }

        [Server]
        public void ClearTarget()
        {
            target = null;
        }

        #endregion

        #region Client



        #endregion
    }
}
