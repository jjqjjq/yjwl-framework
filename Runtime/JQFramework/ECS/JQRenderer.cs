using UnityEngine;

namespace JQFramework.ECS
{
    public class JQRenderer
    {
        private MeshRenderer _meshRenderer; //颜色部件

        private Material _material;

        public Material Material => _material;

        private Material _oldMaterial;

        public JQRenderer(MeshRenderer meshRenderer)
        {
            _meshRenderer = meshRenderer;
            _oldMaterial = meshRenderer.material;
            _material = new Material(meshRenderer.material);
            meshRenderer.material = _material;
        }

        public bool enabled
        {
            get
            {
                return _meshRenderer.enabled;
            }
            set
            {
                _meshRenderer.enabled = value;
            }
        }

        public void Dispose()
        {
            if (_material != null)
            {
                _meshRenderer.material = _oldMaterial;
                Object.Destroy(_material);
                _material = null;
            }
            _oldMaterial = null;
            _meshRenderer = null;
        }
    }
}