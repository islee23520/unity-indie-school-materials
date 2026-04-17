using UnityEngine;

namespace Metroidvania.Session5
{
    public class ParallaxBackgroundMover : MonoBehaviour
    {
        [SerializeField] private Transform _cameraTransform;
        [SerializeField] private float _parallaxFactor = 0.4f;
        [SerializeField] private bool _lockY = true;

        private Vector3 _previousCameraPosition;

        private void Start()
        {
            if (_cameraTransform != null)
            {
                _previousCameraPosition = _cameraTransform.position;
            }
        }

        private void LateUpdate()
        {
            if (_cameraTransform == null)
            {
                return;
            }

            Vector3 delta = _cameraTransform.position - _previousCameraPosition;
            if (_lockY)
            {
                delta.y = 0f;
            }

            transform.position += delta * _parallaxFactor;
            _previousCameraPosition = _cameraTransform.position;
        }
    }
}
