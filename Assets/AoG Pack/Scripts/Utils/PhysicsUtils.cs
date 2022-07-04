using UnityEngine;

namespace PhysicsUtils
{
    public static class BoundsExtensions
    {
        public static Bounds GetChildRendererBounds(this Transform t)
        {
            var renderers = t.GetComponentsInChildren<Renderer>();

            if (renderers.Length > 0)
            {
                var bounds = renderers[0].bounds;

                foreach (var renderer in renderers) bounds.Encapsulate(renderer.bounds);

                return bounds;
            }

            return new Bounds();
        }
    }

    public static class ColliderExtensions
    {
        public static void AddBoxColliderAndEncapsulate(this Transform t)
        {
            var hasBounds = false;
            var bounds = new Bounds(Vector3.zero, Vector3.zero);

            for (var i = 0; i < t.childCount; ++i)
            {
                var childRenderer = t.GetChild(i).GetComponent<Renderer>();
                if (childRenderer != null)
                {
                    if (hasBounds)
                    {
                        bounds.Encapsulate(childRenderer.bounds);
                    }
                    else
                    {
                        bounds = childRenderer.bounds;
                        hasBounds = true;
                    }
                }
            }

            var collider = t.GetComponent<BoxCollider>();
            if (collider == null) collider = t.gameObject.AddComponent<BoxCollider>();
            collider.center = bounds.center - t.transform.position;
            collider.size = bounds.size;
        }
    }
}