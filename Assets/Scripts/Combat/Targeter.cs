using System.Collections;
using System.Collections.Generic;
using Mirror;
using RTS.Buildings;
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

        public override void OnStartServer()
        {
            GameOverHandler.ServerOnGameOver += ServerHandleGameOver;
        }

        public override void OnStopServer()
        {
            GameOverHandler.ServerOnGameOver -= ServerHandleGameOver;
        }

        public Transform GetTargetAimAtPoint()
        {
            return target.GetAimAtPoint();
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

        [Server]
        private void ServerHandleGameOver()
        {
            ClearTarget();
        }

        #endregion

        #region Client



        #endregion
    }
}
