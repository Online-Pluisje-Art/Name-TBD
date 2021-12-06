using UnityEngine;
using OperationBlackwell.Core;

namespace OperationBlackwell.LevelTransitions {
	public class Plane : MonoBehaviour {
		[SerializeField] private float transtionSpeed_;
        private Vector3 velocity_ = Vector3.zero;

		private void Update() {
            transform.position = Vector3.SmoothDamp(transform.position, new Vector3(1315, transform.position.y, transform.position.z), ref velocity_, transtionSpeed_ * Time.deltaTime);
        }
	}
}
