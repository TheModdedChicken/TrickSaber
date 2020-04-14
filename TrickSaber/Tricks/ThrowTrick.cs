﻿using System.Collections;
using IPA.Utilities;
using UnityEngine;

namespace TrickSaber
{
    public class ThrowTrick : Trick
    {
        private float _controllerSnapThreshold = 0.3f;
        private float _saberRotSpeed;
        private float _velocityMultiplier = 1;

        private GameObject _vrGameCore;

        public override TrickAction TrickAction => TrickAction.Throw;

        public override void OnTrickStart()
        {
            SaberTrickModel.ChangeToTrickModel();
            SaberTrickModel.Rigidbody.isKinematic = false;

            SaberTrickModel.Rigidbody.velocity = MovementController.GetAverageVelocity() * 3 * _velocityMultiplier;
            _saberRotSpeed = MovementController.SaberSpeed * _velocityMultiplier;
            if (MovementController.AngularVelocity.x > 0) _saberRotSpeed *= 150;
            else _saberRotSpeed *= -150;
            //if (PluginConfig.Instance.SlowmoDuringThrow) _saberRotSpeed *= PluginConfig.Instance.SlowmoMultiplier;
            //SaberTrickModel.Rigidbody.angularVelocity = Vector3.zero;
            SaberTrickModel.Rigidbody.AddRelativeTorque(Vector3.right * _saberRotSpeed, ForceMode.Acceleration);
        }

        public override void OnTrickUpdate()
        {
        }

        public override void OnTrickEndRequested()
        {
            SaberTrickModel.Rigidbody.velocity = Vector3.zero;
            StartCoroutine(ReturnSaber(PluginConfig.Instance.ReturnSpeed));
        }

        public override void OnInit()
        {
            _controllerSnapThreshold = PluginConfig.Instance.ControllerSnapThreshold;
            _velocityMultiplier = PluginConfig.Instance.ThrowVelocity;
            _vrGameCore = GameObject.Find("VRGameCore");
        }

        public IEnumerator ReturnSaber(float speed)
        {
            Vector3 position = SaberTrickModel.TrickModel.transform.position;
            float distance = Vector3.Distance(position, MovementController.ControllerPosition);
            while (distance > _controllerSnapThreshold)
            {
                distance = Vector3.Distance(position, MovementController.ControllerPosition);
                var direction = MovementController.ControllerPosition - position;
                float force;
                if (distance < 1f) force = 10f;
                else force = speed * distance;
                force = Mathf.Clamp(force, 0, 200);
                SaberTrickModel.Rigidbody.velocity = direction.normalized * force;
                position = SaberTrickModel.TrickModel.transform.position;
                yield return new WaitForEndOfFrame();
            }

            ThrowEnd();
        }

        private void ThrowEnd()
        {
            SaberTrickModel.Rigidbody.isKinematic = true;
            SaberTrickModel.ChangeToActualSaber();
            Reset();
        }
    }
}