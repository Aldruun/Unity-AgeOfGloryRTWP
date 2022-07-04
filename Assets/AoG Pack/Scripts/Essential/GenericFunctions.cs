using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using Component = UnityEngine.Component;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

//using UnityEditor;

public enum BoundsCalculationMode
{
    RendererBased,
    ColliderBased
}

namespace GenericFunctions
{
    public static class DebugDraw
    {
        public static void Arrow(Vector3 start, Vector3 end, Color color)
        {
            Debug.DrawLine(start, end, color);
            Debug.DrawLine(end, -new Vector3(start.x, start.y, 0.2f));
            Debug.DrawLine(end, -new Vector3(start.x, start.y, -0.2f));
        }

        public static void Cube(Vector3 center, Vector3 size, Quaternion rotation, Color color)
        {
            Vector3 extents = size / 2;

            Vector3[] points = new Vector3[8];

            points[0] = rotation * Vector3.Scale(extents, new Vector3(1, 1, 1)) + center;
            points[1] = rotation * Vector3.Scale(extents, new Vector3(1, 1, -1)) + center;
            points[2] = rotation * Vector3.Scale(extents, new Vector3(1, -1, 1)) + center;
            points[3] = rotation * Vector3.Scale(extents, new Vector3(1, -1, -1)) + center;
            points[4] = rotation * Vector3.Scale(extents, new Vector3(-1, 1, 1)) + center;
            points[5] = rotation * Vector3.Scale(extents, new Vector3(-1, 1, -1)) + center;
            points[6] = rotation * Vector3.Scale(extents, new Vector3(-1, -1, 1)) + center;
            points[7] = rotation * Vector3.Scale(extents, new Vector3(-1, -1, -1)) + center;

            Debug.DrawLine(points[0], points[1], color);
            Debug.DrawLine(points[0], points[2], color);
            Debug.DrawLine(points[0], points[4], color);

            Debug.DrawLine(points[7], points[6], color);
            Debug.DrawLine(points[7], points[5], color);
            Debug.DrawLine(points[7], points[3], color);

            Debug.DrawLine(points[1], points[3], color);
            Debug.DrawLine(points[1], points[5], color);

            Debug.DrawLine(points[2], points[3], color);
            Debug.DrawLine(points[2], points[6], color);

            Debug.DrawLine(points[4], points[5], color);
            Debug.DrawLine(points[4], points[6], color);
        }

        public static void Ellipse(Vector3 center, Vector3 forward, Vector3 up, float radiusX, float radiusY,
            int segments, Color color, float duration = 0)
        {
            float angle = 0f;
            Quaternion rot = Quaternion.LookRotation(forward, up);
            Vector3 lastPoint = Vector3.zero;
            Vector3 thisPoint = Vector3.zero;

            for (int i = 0; i < segments + 1; i++)
            {
                thisPoint.x = Mathf.Sin(Mathf.Deg2Rad * angle) * radiusX;
                thisPoint.z = Mathf.Cos(Mathf.Deg2Rad * angle) * radiusY;

                if (i > 0) Debug.DrawLine(rot * lastPoint + center, rot * thisPoint + center, color, duration);

                lastPoint = thisPoint;
                angle += 360f / segments;
            }
        }

        public static void Circle(Vector3 center, Vector3 up, float radius, int segments, Color color,
            float duration = 0)
        {
            float angle = 0f;
            Quaternion rot = Quaternion.LookRotation(Vector3.forward, up);
            Vector3 lastPoint = Vector3.zero;
            Vector3 thisPoint = Vector3.zero;

            for (int i = 0; i < segments + 1; i++)
            {
                thisPoint.x = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
                thisPoint.z = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;

                if (i > 0) Debug.DrawLine(rot * lastPoint + center, rot * thisPoint + center, color, duration);

                lastPoint = thisPoint;
                angle += 360f / segments;
            }
        }

        public static void Rectangle(Vector3 center, float size, Color color)
        {
            float extents = size / 2;

            Debug.DrawLine(new Vector3(center.x - extents, center.y, center.z + extents),
                new Vector3(center.x + extents, center.y, center.z + extents), color);
            Debug.DrawLine(new Vector3(center.x - extents, center.y, center.z - extents),
                new Vector3(center.x + extents, center.y, center.z - extents), color);
            Debug.DrawLine(new Vector3(center.x + extents, center.y, center.z - extents),
                new Vector3(center.x + extents, center.y, center.z + extents), color);
            Debug.DrawLine(new Vector3(center.x - extents, center.y, center.z - extents),
                new Vector3(center.x - extents, center.y, center.z + extents), color);
        }

        public static void Cross(Vector3 center, float size, Color color)
        {
            float extents = size / 2;

            Debug.DrawLine(new Vector3(center.x - extents, center.y, center.z),
                new Vector3(center.x + extents, center.y, center.z), color);
            Debug.DrawLine(new Vector3(center.x, center.y, center.z + extents),
                new Vector3(center.x, center.y, center.z - extents), color);
        }
    }

    #region Do //////////////////////////////////////////////////////////

    public class Do : MonoBehaviour
    {
        public static void WrapInBoxCollider(GameObject parentObject)
        {
            BoxCollider bc = parentObject.GetComponent<BoxCollider>();
            if (bc == null)
                bc = parentObject.AddComponent<BoxCollider>();
            Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);
            bool hasBounds = false;
            Renderer[] renderers = parentObject.GetComponentsInChildren<Renderer>();
            foreach (Renderer render in renderers)
                if (hasBounds)
                {
                    bounds.Encapsulate(render.bounds);
                }
                else
                {
                    bounds = render.bounds;
                    hasBounds = true;
                }

            if (hasBounds)
            {
                bc.center = bounds.center - parentObject.transform.position;
                bc.size = bounds.size;
            }
            else
            {
                bc.size = bc.center = Vector3.zero;
                bc.size = Vector3.zero;
            }
        }
    }

    #endregion Do //////////////////////////////////////////////////////////

    public static class QuaternionUtil
    {
        public static bool IsNaN(Quaternion q)
        {
            return float.IsNaN(q.x * q.y * q.z * q.w);
        }

        public static string Stringify(Quaternion q)
        {
            return q.x + "," + q.y + "," + q.z + q.w;
        }

        public static string ToDetailedString(this Quaternion v)
        {
            return string.Format("<{0}, {1}, {2}, {3}>", v.x, v.y, v.z, v.w);
        }

        public static Quaternion Normalize(Quaternion q)
        {
            double mag = Math.Sqrt(q.w * q.w + q.x * q.x + q.y * q.y + q.z * q.z);
            q.w = (float)(q.w / mag);
            q.x = (float)(q.x / mag);
            q.y = (float)(q.y / mag);
            q.z = (float)(q.z / mag);
            return q;
        }

        #region Transform

        ///// <summary>
        ///// Create a LookRotation for a non-standard 'forward' axis.
        ///// </summary>
        ///// <param name="dir"></param>
        ///// <param name="forwardAxis"></param>
        ///// <returns></returns>
        //public static Quaternion AltForwardLookRotation(Vector3 dir, Vector3 forwardAxis)
        //{
        //    return Quaternion.LookRotation(dir) * Quaternion.FromToRotation(forwardAxis, Vector3.forward);
        //}
        public static void GetAngleAxis(this Quaternion q, out Vector3 axis, out float angle)
        {
            if (q.w > 1)
                q = QuaternionUtil.Normalize(q);

            //get as doubles for precision
            double qw = (double)q.w;
            double qx = (double)q.x;
            double qy = (double)q.y;
            double qz = (double)q.z;
            double ratio = Math.Sqrt(1.0d - qw * qw);

            angle = (float)(2.0d * Math.Acos(qw)) * Mathf.Rad2Deg;
            if (ratio < 0.001d)
            {
                axis = new Vector3(1f, 0f, 0f);
            }
            else
            {
                axis = new Vector3(
                    (float)(qx / ratio),
                    (float)(qy / ratio),
                    (float)(qz / ratio));
                axis.Normalize();
            }
        }

        public static void GetShortestAngleAxisBetween(Quaternion a, Quaternion b, out Vector3 axis, out float angle)
        {
            Quaternion dq = Quaternion.Inverse(a) * b;
            if (dq.w > 1)
                dq = QuaternionUtil.Normalize(dq);

            //get as doubles for precision
            double qw = (double)dq.w;
            double qx = (double)dq.x;
            double qy = (double)dq.y;
            double qz = (double)dq.z;
            double ratio = Math.Sqrt(1.0d - qw * qw);

            angle = (float)(2.0d * Math.Acos(qw)) * Mathf.Rad2Deg;
            if (ratio < 0.001d)
            {
                axis = new Vector3(1f, 0f, 0f);
            }
            else
            {
                axis = new Vector3(
                    (float)(qx / ratio),
                    (float)(qy / ratio),
                    (float)(qz / ratio));
                axis.Normalize();
            }
        }

        #endregion Transform
    }

    #region Create /////////////////////////////////////////////////////////////

    public class Create : MonoBehaviour
    {
        public static string UniqueID()
        {
            return Guid.NewGuid().ToString();
        }

        public static void OneShotSFX(AudioSource audioSource, AudioClip audioClip, float pitch = 1.0f)
        {
            if (audioClip == null)
                return;

            // create an audiosource and attach it to the gameobject
            audioSource.clip = audioClip;

            // Make sure the settings of the audiosource are ok for one shot usage
            audioSource.loop = false;
            audioSource.pitch = pitch;
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 1;

            //if(backwards) {
            //    audioSource.time = audioClip.length;
            //    audioSource.pitch = -audioSource.pitch;
            //}

            // Action!
            audioSource.Play();
        }

        public static void OneShotSFXWithAudioSource(AudioSource audioSource, AudioClip audioClip, Vector3 position,
            bool backwards = false)
        {
            if (audioClip == null)
                return;

            // Create a gameobject and set its position
            GameObject go = new GameObject();
            go.transform.position = position;

            // create an audiosource and attach it to the gameobject
            AudioSource audiosource = go.AddComponent<AudioSource>();

            // Make sure the settings of the audiosource are ok for one shot usage
            audiosource.loop = audioSource.loop;
            audiosource.volume = audioSource.volume;
            audiosource.pitch = audioSource.pitch;
            audiosource.playOnAwake = audioSource.playOnAwake;
            audiosource.spatialBlend = audioSource.spatialBlend;
            audiosource.rolloffMode = audioSource.rolloffMode;
            audiosource.minDistance = audioSource.minDistance;
            audiosource.maxDistance = audioSource.maxDistance;

            audiosource.clip = audioClip;

            if (backwards)
            {
                audiosource.time = audioClip.length;
                audiosource.pitch = -audiosource.pitch;
            }

            // Action!
            audiosource.Play();

            // Destroy the audio object after the clip has played
            Object.Destroy(go, audiosource.clip.length);
        }

        public static void OneShotSFX(AudioClip audioClip, Vector3 position, float volume, float pitch = 1,
            float maxDistance = 25, bool backwards = false)
        {
            if (audioClip == null)
                return;
            // Create a gameobject and set its position
            GameObject go = new GameObject();
            go.transform.position = position;

            // create an audiosource and attach it to the gameobject
            AudioSource audiso = go.AddComponent<AudioSource>();
            audiso.clip = audioClip;

            // Make sure the settings of the audiosource are ok for one shot usage
            audiso.loop = false;
            audiso.volume = volume;
            audiso.pitch = pitch;
            audiso.playOnAwake = false;
            audiso.spatialBlend = 1;
            audiso.rolloffMode = AudioRolloffMode.Linear;
            audiso.minDistance = 0;
            audiso.maxDistance = maxDistance;

            if (backwards)
            {
                audiso.time = audioClip.length;
                audiso.pitch = -audiso.pitch;
            }

            // Action!
            audiso.Play();

            // Destroy the audio object after the clip has played
            Object.Destroy(go, audiso.clip.length);
        }

        public static void OneShotSFX(AudioSource audioSource, AudioClip audioClip, float volume, float pitch = 1,
            float maxDistance = 25, bool backwards = false)
        {
            if (audioClip == null)
                return;

            // create an audiosource and attach it to the gameobject
            audioSource.clip = audioClip;

            // Make sure the settings of the audiosource are ok for one shot usage
            audioSource.loop = false;
            audioSource.volume = volume;
            audioSource.pitch = pitch;
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 1;
            //audioSource.rolloffMode = AudioRolloffMode.Linear;
            //audioSource.minDistance = 0;
            audioSource.maxDistance = maxDistance;

            if (backwards)
            {
                audioSource.time = audioClip.length;
                audioSource.pitch = -audioSource.pitch;
            }

            // Action!
            audioSource.Play();
        }
    }

    #endregion Create /////////////////////////////////////////////////////////////

    #region Get /////////////////////////////////////////////////////////////

    public class Get : MonoBehaviour
    {
        public static List<T> AssetsByType<T>() where T : Object
        {
            //UnityEngine.Debug.Log(string.Format("t:{0}", typeof(T)));
            //UnityEngine.Debug.Log(string.Format("t:{0}", typeof(T).ToString()));
            //UnityEngine.Debug.Log(string.Format("t:{0}", typeof(T).Name));

            List<T> assets = new List<T>();
            string[] guids = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(T) /*.ToString()*/));
            for (int i = 0; i < guids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
                if (asset != null)
                    assets.Add(asset);
            }

            return assets;
        }

        public static Vector2 VectorFromAngle(float angle, float radius)
        {
            //angle = Mathf.PI * 2;
            float x = Mathf.Sin(angle) * radius;
            float y = Mathf.Cos(angle) * radius;

            return new Vector2(x, y);
        }

        public static Vector2 RandomPointOnUnitCircle(float radius)
        {
            float angle = Random.Range(0f, Mathf.PI * 2);
            float x = Mathf.Sin(angle) * radius;
            float y = Mathf.Cos(angle) * radius;

            return new Vector2(x, y);
        }

        public static Vector3 RandomOnUnitCircle2(Vector3 origin, Vector3 targetPoint, float radius)
        {
            Vector3 randomPointOnCircle = Random.insideUnitCircle;
            randomPointOnCircle.Normalize();
            randomPointOnCircle *= radius;

            Vector3 dir = targetPoint - origin;
            //randomPointOnCircle = new Vector3(randomPointOnCircle.x, randomPointOnCircle.y, dir.z);
            Vector3 targetDir = Quaternion.AngleAxis(Random.Range(-radius, radius), Vector3.forward) * dir;
            for (int i = 0; i < 50; i++)
            {
                targetDir = Quaternion.AngleAxis(Random.Range(-radius, radius), Vector3.forward) * dir;

                Debug.DrawLine(origin, targetDir, Color.cyan, 1f);
            }

            return randomPointOnCircle;
        }

        public static Vector3 RandomOnUnitSphere(float radius)
        {
            Vector3 randomPointOnCircle = Random.insideUnitSphere;
            randomPointOnCircle.Normalize();
            randomPointOnCircle *= radius;
            return randomPointOnCircle;
        }

        public static Vector3 GetPointOnUnitSphereCap(Quaternion targetDirection, float angle)
        {
            float angleInRad = Random.Range(0.0f, angle) * Mathf.Deg2Rad;
            Vector2 PointOnCircle = Random.insideUnitCircle.normalized * Mathf.Sin(angleInRad);
            Vector3 V = new Vector3(PointOnCircle.x, PointOnCircle.y, Mathf.Cos(angleInRad));
            return targetDirection * V;
        }

        public static Vector3 GetPointOnUnitSphereCap(Vector3 targetDirection, float angle)
        {
            //for(int i = 0; i < 50; i++)
            //{
            //    Vector3 targetPoint = GetPointOnUnitSphereCap(Quaternion.LookRotation(targetDirection), angle);

            //    UnityEngine.Debug.DrawLine(targetPoint + new Vector3(0, 0, 0.1f), targetPoint, Color.cyan, 1f);
            //}
            return Get.GetPointOnUnitSphereCap(Quaternion.LookRotation(targetDirection), angle);
        }

        public static float Normalized01(float value, float min, float max)
        {
            return (value - min) / (max - min);
        }

        public static T ClosestMonoBehaviourAroundPoint<T>(Vector3 requestPoint, float radius) where T : MonoBehaviour
        {
            Collider[] colliders = Physics.OverlapSphere(requestPoint, radius);
            List<T> listOfCertainType = new List<T>();

            for (int i = 0; i < colliders.Length; i++)
            {
                Collider col = colliders[i];

                T behaviour = col.GetComponentInChildren<T>();
                if (behaviour != null)
                    listOfCertainType.Add(behaviour);
            }

            if (listOfCertainType.Count == 0)
                return null;

            return listOfCertainType.OrderBy(d => Vector3.Distance(requestPoint, d.transform.position)).ToList()[0];
        }

        public static T ClosestTypeFromList<T>(Vector3 fromPosition, List<T> listOfCertainType) where T : Component
        {
            if (listOfCertainType == null || listOfCertainType.Count == 0)
                return null;

            return listOfCertainType.OrderBy(d => Vector3.Distance(fromPosition, d.transform.position)).ToList()[0];
        }

        public static T ClosestTypeInScene<T>(Vector3 fromPosition, float searchRadius) where T : Component
        {
            T[] objectsOfType = Object.FindObjectsOfType<T>();

            if (objectsOfType == null || objectsOfType.Length == 0)
                return null;

            return objectsOfType.OrderBy(d => Vector3.Distance(fromPosition, d.transform.position)).ToList()[0];
        }

        public static T ClosestObjectFromArray<T>(Vector3 fromPosition, T[] listOfCertainType) where T : Component
        {
            if (listOfCertainType == null || listOfCertainType.Length == 0)
                return null;

            return listOfCertainType.OrderBy(d => Vector3.Distance(fromPosition, d.transform.position))
                .FirstOrDefault();
        }

        public static GameObject ClosestGameObjectFromList(Vector3 fromPosition, List<GameObject> listOfCertainType)
        {
            if (listOfCertainType == null || listOfCertainType.Count == 0)
                return null;

            return listOfCertainType.OrderBy(d => Vector3.Distance(fromPosition, d.transform.position)).ToList()[0];
        }

        public static Vector3 ClosestPositionFromList(Vector3 fromPosition, List<Vector3> listOfPositions)
        {
            if (listOfPositions == null || listOfPositions.Count == 0)
                return new Vector3(-1000, -1000, -1000);

            return listOfPositions.OrderBy(pos => Vector3.Distance(fromPosition, pos)).ToList()[0];
        }

        public static List<Type> SubClassesOf<T>() where T : class
        {
            List<Type> types = new List<Type>();

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.FullName.StartsWith("Mono.Cecil"))
                    continue;

                if (assembly.FullName.StartsWith("UnityScript"))
                    continue;

                if (assembly.FullName.StartsWith("Boo.Lan"))
                    continue;

                if (assembly.FullName.StartsWith("System"))
                    continue;

                if (assembly.FullName.StartsWith("I18N"))
                    continue;

                if (assembly.FullName.StartsWith("UnityEngine"))
                    continue;

                if (assembly.FullName.StartsWith("UnityEditor"))
                    continue;

                if (assembly.FullName.StartsWith("mscorlib"))
                    continue;

                foreach (Type type in assembly.GetTypes())
                {
                    if (!type.IsClass)
                        continue;

                    if (type.IsAbstract)
                        continue;

                    if (!type.IsSubclassOf(typeof(T)))
                        continue;

                    types.Add(type);
                }
            }

            return types;
        }

        //public static List<Type> SubClassesOf<TBaseType>() where TBaseType : UnityEngine.Component {
        //    return System.Reflection.Assembly.GetExecutingAssembly()
        //    .GetTypes()
        //    .Where(t => t.BaseType != null && t.BaseType.IsGenericType &&
        //                t.BaseType.GetGenericTypeDefinition() == typeof(TBaseType)).ToList();

        //    //Type baseType = typeof(TBaseType);
        //    //Assembly assembly = baseType.Assembly;

        //    //return assembly.GetTypes().Where(t => t.IsSubclassOf(baseType)).ToList();
        //}

        // Asset related
        //public static List<T> AssetsByType<T>() where T : UnityEngine.Object {
        //    //UnityEngine.Debug.Log(string.Format("t:{0}", typeof(T)));
        //    //UnityEngine.Debug.Log(string.Format("t:{0}", typeof(T).ToString()));
        //    //UnityEngine.Debug.Log(string.Format("t:{0}", typeof(T).Name));

        //    List<T> assets = new List<T>();
        //    string[] guids = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(T)/*.ToString()*/));
        //    for(int i = 0; i < guids.Length; i++) {
        //        string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
        //        T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
        //        if(asset != null) {
        //            assets.Add(asset);
        //        }
        //    }
        //    return assets;
        //}

        public static float YAngle(Transform self, Vector3 target)
        {
            return Vector3.SignedAngle(self.forward, target - self.position, Vector3.up);
        }

        public static float XAngle(Transform self, Vector3 target)
        {
            return Vector3.SignedAngle(self.forward, target - self.position, Vector3.right);
        }

        // Super-Duper Function!
        public static Quaternion YAngleTo(Transform rotator, Vector3 targetPosition)
        {
            // Look at including x and z leaning
            rotator.LookAt(targetPosition);

            // Euler angles are easier to deal with. You could use Quaternions here also
            // C# requires you to set the entire rotation variable. You can't set the individual x and z (UnityScript can), so you make a temp Vec3 and set it back
            Vector3 eulerAngles = rotator.rotation.eulerAngles;
            eulerAngles.x = 0;
            eulerAngles.z = 0;

            // Set the altered rotation back
            return Quaternion.Euler(eulerAngles);
        }

        public static Vector3 RandomPointOnCircle(Vector3 center, float radius, bool placeOnGround)
        {
            // create random angle between 0 to 360 degrees
            float ang = Random.value * 360;
            Vector3 pos;
            pos.x = center.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);

            if (placeOnGround)
            {
                RaycastHit[] hits = Physics.RaycastAll(center.Shifted(0, 2, 0), Vector3.down, Mathf.Infinity);

                foreach (RaycastHit hit in hits)
                    if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
                    {
                        //UnityEngine.Debug.Log("Placing object on ground");
                        pos.y = hit.point.y;
                        pos.z = center.z;
                        return pos;
                    }
            }

            pos.y = center.y;
            pos.z = center.z;

            return pos;
        }

        public static Bounds RendererBounds(Transform t)
        {
            Quaternion currentRotation = t.rotation;
            Vector3 currentScale = t.localScale;
            t.rotation = Quaternion.Euler(0f, 0f, 0f);
            t.localScale = Vector3.one;
            Bounds bounds = new Bounds(t.position, Vector3.zero);
            foreach (Renderer renderer in t.GetComponentsInChildren<Renderer>())
                bounds.Encapsulate(renderer.bounds);

            Vector3 localCenter = bounds.center - t.position;
            bounds.center = localCenter;

            t.rotation = currentRotation;
            t.localScale = currentScale;

            return bounds;
        }

        public static Bounds TotalBounds(Transform t)
        {
            Bounds combinedBounds = new();
            Renderer[] renderers = t.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers)
                if (renderer != t.GetComponent<Renderer>())
                    combinedBounds.Encapsulate(renderer.bounds);

            return combinedBounds;
        }

        public static Bounds LocalBounds(Transform t)
        {
            Quaternion currentRotation = t.rotation;
            t.rotation = Quaternion.Euler(0f, 0f, 0f);
            Bounds bounds = new Bounds(t.position, Vector3.zero);
            foreach (Renderer renderer in t.GetComponentsInChildren<Renderer>())
                bounds.Encapsulate(renderer.bounds);
            Vector3 localCenter = bounds.center - t.position;
            bounds.center = localCenter;
            t.rotation = currentRotation;

            return bounds;
        }

        public static Bounds TotalMeshFilterBounds(Transform objectTransform)
        {
            MeshFilter meshFilter = objectTransform.GetComponent<MeshFilter>();
            Bounds result = meshFilter != null ? meshFilter.mesh.bounds : new Bounds();

            foreach (Transform transform in objectTransform)
            {
                Bounds bounds = Get.TotalMeshFilterBounds(transform);
                result.Encapsulate(bounds.min);
                result.Encapsulate(bounds.max);
            }

            Vector3 scaledMin = result.min;
            scaledMin.Scale(objectTransform.localScale);
            result.min = scaledMin;
            Vector3 scaledMax = result.max;
            scaledMax.Scale(objectTransform.localScale);
            result.max = scaledMax;
            return result;
        }

        public static string[] AllLinesFromTextfile(string textfile)
        {
            TextAsset txtAsset = (TextAsset)Resources.Load("Text Files/" + textfile);

            //quote = dataPairs[Random.Range(0, lineNum++)][Random.Range(0, lineNum++)];

            return txtAsset.text.Split('\n');
        }

        public static Vector3 RandomPositionOnPlane(Transform plane)
        {
            Vector3 size = plane.GetComponent<Renderer>().bounds.size;
            Vector3 pos = plane.position;
            Vector3 randomDestination = new Vector3(Random.Range(pos.x - size.x / 2, pos.x + size.x / 2),
                pos.y, Random.Range(pos.z - size.z / 2, pos.z + size.z / 2));

            return randomDestination;
        }

        public static Quaternion TargetRotation(Transform thisTransform, Vector3 targetPosition)
        {
            float angle = Vector3.Angle(targetPosition - thisTransform.position, thisTransform.forward);
            Quaternion targetRotation = thisTransform.rotation * Quaternion.AngleAxis(angle, Vector3.up);
            return targetRotation;
        }

        public static Color RandomColor()
        {
            Color newColor = new Color(Random.value, Random.value, Random.value);
            return newColor;
        }

        public static bool RandomBool(float trueChance = 0.5f)
        {
            if (Random.value <= trueChance)
                return true;
            return false;
        }

        public static Vector3 RandomPositionAroundTransform(Transform t, float range, float yDown, float yUp)
        {
            Vector3 storedVector3 = t.position;
            return new Vector3(Random.Range(storedVector3.x - range, storedVector3.x + range),
                Random.Range(storedVector3.y + yDown, storedVector3.y + yUp),
                Random.Range(storedVector3.z - range, storedVector3.z + range));
        }

        public static float DistanceBetween(Vector3 v1, Vector3 v2)
        {
            float fDistanceBetween = Vector3.Distance(v1, v2);

            return fDistanceBetween;
        }

        public static Vector3 Direction(Vector3 v1, Vector3 v2) // v1 = target, v2 = objectSelf
        {
            return v1 - v2;
        }

        public static float DotProduct(Vector3 targetPosition, Transform objectSelf)
        {
            Vector3 dir = (targetPosition - objectSelf.position).normalized;

            return Vector3.Dot(dir, objectSelf.forward);
        }

        public static List<Transform> AllChildren(Transform t)
        {
            List<Transform> aList = new List<Transform>();

            foreach (Transform trfm in t.GetComponentsInChildren(typeof(Transform)))
                aList.Add(trfm);
            return aList;
        }

        public static List<GameObject> AllChildren(GameObject g)
        {
            List<GameObject> aList = new List<GameObject>();

            foreach (Transform trfm in g.GetComponentsInChildren(typeof(Transform)))
                aList.Add(trfm.gameObject);
            return aList;
        }

        public static bool IsInFOV(Transform origin, Vector3 targetPosition, float angle)
        {
            Vector3 relativPosition = targetPosition - origin.position;
            float height = relativPosition.y;
            relativPosition.y = 0;

            if (Mathf.Abs(Vector3.Angle(relativPosition, origin.forward)) < angle) //if(Mathf.Abs(height) < maxHeight) {
                return true;
            //}
            //else {
            //    return false;
            //}
            return false;
        }

        public static bool IsInFront(Transform origin, Vector3 targetPosition, float viewAngle)
        {
            Vector3 targetDir = targetPosition - origin.position;
            float angle = Vector3.Angle(targetDir, origin.forward);

            return angle < viewAngle;
        }

        public static bool IsInFront2(Transform origin, Vector3 targetPosition, float dotAngle)
        {
            Vector3 forward = origin.TransformDirection(Vector3.forward);
            Vector3 toOther = targetPosition - origin.position;

            return Vector3.Dot(forward, toOther) > dotAngle;
        }

        public static bool IsBehind(Vector3 targetPosition, Transform objectSelf)
        {
            if (Get.DotProduct(targetPosition, objectSelf) < 0.3f)
                return true;
            return false;

            //float angel = Vector3.Angle(objectSelf.forward, Get.Direction(targetPosition, objectSelf.position));

            //if(Mathf.Abs(angel) > 90 && Mathf.Abs(angel) < 270)
            //    //Debug.Log("target is in front of me");

            //if(Mathf.Abs(angel) < 90 || Mathf.Abs(angel) > 270)
            //    //Debug.Log("target is behind me");
        }

        //####################################################################################//
        ////////////////////////////// RAY/LINE CASTING ////////////////////////////////////////
        //************************************************************************************//

        //checks if targetObject is blocking objectSelf 's path to its desiredPosition

        public static IEnumerable<T> DerivedTypes<T>(params object[] args) where T : class
        {
            List<T> objects = new List<T>();
            foreach (Type type in Assembly.GetAssembly(typeof(T)).GetTypes()
                         .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(T))))
                objects.Add(Activator.CreateInstance(type, args) as T);

            return objects;
        }
    }

    #endregion Get /////////////////////////////////////////////////////////////

    public static class MouseHelper
    {
        private static Vector2 m_CurrentMousePos;
        private static Vector2 m_LastMousePos;
        private static Vector2 m_CurrentDelta;
        private static readonly Vector2[] m_DragStartVector = new Vector2[3];
        private static readonly Vector2[] m_DragVector = new Vector2[3];
        private static int m_LastFrame = -1;

        public static Vector2 mousePosition
        {
            get
            {
                MouseHelper.Update();
                return MouseHelper.m_CurrentMousePos;
            }
        }

        public static Vector2 lastMousePosition
        {
            get
            {
                MouseHelper.Update();
                return MouseHelper.m_LastMousePos;
            }
        }

        public static Vector2 mouseDelta
        {
            get
            {
                MouseHelper.Update();
                return MouseHelper.m_CurrentDelta;
            }
        }

        public static Vector2 GetDragStartPoint(int aIndex)
        {
            MouseHelper.Update();
            return MouseHelper.m_DragStartVector[aIndex];
        }

        public static Vector2 GetDragOffset(int aIndex)
        {
            MouseHelper.Update();
            return MouseHelper.m_DragVector[aIndex];
        }

        static MouseHelper()
        {
            // force initialization on first access
            MouseHelper.m_CurrentMousePos = Input.mousePosition;
            MouseHelper.Update();
            MouseHelper.m_LastFrame = -1;
        }

        private static void Update()
        {
            if (MouseHelper.m_LastFrame >= Time.frameCount)
                return;
            if (MouseHelper.m_LastFrame < Time.frameCount - 1)
                MouseHelper.m_CurrentMousePos = Input.mousePosition;
            MouseHelper.m_LastFrame = Time.frameCount;
            MouseHelper.m_LastMousePos = MouseHelper.m_CurrentMousePos;
            MouseHelper.m_CurrentMousePos = Input.mousePosition;
            MouseHelper.m_CurrentDelta = MouseHelper.m_CurrentMousePos - MouseHelper.m_LastMousePos;
            for (int i = 0; i < MouseHelper.m_DragStartVector.Length; i++)
            {
                if (Input.GetMouseButtonDown(i))
                    MouseHelper.m_DragStartVector[i] = MouseHelper.m_CurrentMousePos;
                if (Input.GetMouseButton(i))
                    MouseHelper.m_DragVector[i] = MouseHelper.m_CurrentMousePos - MouseHelper.m_DragStartVector[i];
            }
        }
    }

    #region Extensions //////////////////////////////////////////////////////////

    public static class GenericExtensions
    {
        public static void PrintListElements<T>(this T[] arrayOfTypes)
        {
            string debugResult = string.Join(", ",
                arrayOfTypes.Select(i => i.ToString()).ToArray());

            Debug.Log(debugResult);
        }

        public static void PrintListElements<T>(this HashSet<T> arrayOfTypes, string prefix)
        {
            string debugResult = string.Join(", ",
                arrayOfTypes.Select(i => i.ToString()).ToArray());

            Debug.Log(prefix + ": " + debugResult);
        }

        public static string ElementsToString<T>(this List<T> arrayOfTypes, string prefix)
        {
            string debugResult = string.Join(", ",
                arrayOfTypes.Select(i => i.ToString()).ToArray());

            return debugResult;
        }

        public static string ElementsToString<T>(this T[] arrayOfTypes, string prefix)
        {
            string debugResult = string.Join(", ",
                arrayOfTypes.Select(i => i.ToString()).ToArray());

            return debugResult;
        }
    }

    public static class ReflectionExtensions
    {
        public static Type GetUnderlyingType(this MemberInfo member)
        {
            switch (member.MemberType)
            {
                case MemberTypes.Event:
                    return ((EventInfo)member).EventHandlerType;

                case MemberTypes.Field:
                    return ((FieldInfo)member).FieldType;

                case MemberTypes.Method:
                    return ((MethodInfo)member).ReturnType;

                case MemberTypes.Property:
                    return ((PropertyInfo)member).PropertyType;

                default:
                    throw new ArgumentException
                    (
                        "Input MemberInfo must be of type EventInfo, FieldInfo, MethodInfo, or PropertyInfo"
                    );
            }
        }

        public static Type[] GetAllDerivedTypes(this AppDomain aAppDomain, Type aType)
        {
            List<Type> result = new List<Type>();
            Assembly[] assemblies = aAppDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                Type[] types = assembly.GetTypes();
                foreach (Type type in types)
                    if (type.IsSubclassOf(aType))
                        result.Add(type);
            }

            return result.ToArray();
        }
    }

    public static class ComponentExtensions
    {
        /// <summary>
        ///     Add the specified component if it does not exist.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="component"></param>
        public static void EnsureHasComponent<T>(this Component component) where T : Component
        {
            if (component.GetComponent<T>() == null)
                component.gameObject.AddComponent<T>();
        }

        /// <summary>
        ///     Add and return the specified component.
        /// </summary>
        public static T ForceGetComponent<T>(this Component component) where T : Component
        {
            T t = null;
            if (component.GetComponent<T>() == null)
                t = component.gameObject.AddComponent<T>();
            else
                t = component.GetComponent<T>();

            return t;
        }

        /// <summary>
        ///     Add and return the specified component.
        /// </summary>
        public static T ForceGetComponent<T>(this GameObject gameObject) where T : Component
        {
            T t = null;
            if (gameObject.GetComponent<T>() == null)
                return gameObject.AddComponent<T>();
            t = gameObject.GetComponent<T>();

            return t;
        }

        /// <summary>
        ///     Add and return the specified component.
        /// </summary>
        public static T ForceGetComponent<T>(this Transform transform) where T : Component
        {
            if (transform.GetComponent<T>() == null)
                return transform.gameObject.AddComponent<T>();

            return transform.GetComponent<T>();
        }

        public static T GetComponentInDirectChildren<T>(this Component parent) where T : Component
        {
            foreach (Transform transform in parent.transform)
            {
                T component;
                if ((component = transform.GetComponent<T>()) != null)
                    return component;
            }

            return null;
        }

        public static T[] GetComponentsInDirectChildren<T>(this Component parent) where T : Component
        {
            List<T> tmpList = new List<T>();

            foreach (Transform transform in parent.transform)
            {
                T component;
                if ((component = transform.GetComponent<T>()) != null)
                    tmpList.Add(component);
            }

            return tmpList.ToArray();
        }
    }

    public static class EssentialExtensions
    {
        public static Vector3 CombinedBoundsSize(this Transform transform, BoundsCalculationMode calculationMode)
        {
            Bounds combinedBounds = new Bounds();

            switch (calculationMode)
            {
                case BoundsCalculationMode.RendererBased:

                    Renderer[] renderers = transform.GetComponentsInChildren<Renderer>();

                    foreach (Renderer render in renderers)
                        combinedBounds.Encapsulate(render.bounds);

                    break;

                case BoundsCalculationMode.ColliderBased:

                    Collider[] colliders = transform.GetComponentsInChildren<Collider>();

                    foreach (Collider collider in colliders)
                        combinedBounds.Encapsulate(collider.bounds);

                    break;
            }

            return combinedBounds.size;
        }

        public static bool StartsWithNumber(this string input)
        {
            if (!char.IsNumber(input[0]))
                return false;
            return true;
        }

        public static float Normalized(this float value, float min, float max)
        {
            return (value - min) / (max - min);
        }

        public static float Normalized(this int value, int min, int max)
        {
            return (float)(value - min) / (max - min);
        }

        public static float Remap(this float value, float from1, float to1, float from2, float to2)
        {
            return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        }
    }

    public static class EditorExtensions
    {
        public static bool KeyPressed<T>(this T s, string controlName, KeyCode key, out T fieldValue)
        {
            fieldValue = s;
            if (GUI.GetNameOfFocusedControl() == controlName)
            {
                if (Event.current.type == EventType.KeyUp &&
                    Event.current.keyCode == key)
                    return true;
                return false;
            }

            return false;
        }
    }

    public static class BooleanExtensions
    {
        public static bool targetPositionToTheLeft(this Transform t, Vector3 targetPosition)
        {
            Vector3 relativePoint = t.InverseTransformPoint(targetPosition);
            if (relativePoint.x < 0.0f)
                return true;
            return false;
        }

        public static bool targetPositionToTheRight(this Transform t, Vector3 targetPosition)
        {
            Vector3 relativePoint = t.InverseTransformPoint(targetPosition);
            if (relativePoint.x > 0.0f)
                return true;
            return false;
        }
    }

    public static class RendererExtensions
    {
        public static bool IsVisibleFrom(this Renderer renderer, Camera camera)
        {
            Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
            return GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
        }
    }

    public static class IntegerExtensions
    {
        public static void IncrementToMaximum(this int i, int maximum)
        {
            if (i < maximum)
                i++;
        }

        public static void DecrementToMinimum(this int i, int minimum)
        {
            if (i > minimum)
                i--;
        }
    }

    public static class Vector3Extensions
    {
        public static Vector3 DirectionTo(this Vector3 origin, Vector3 destination)
        {
            return Vector3.Normalize(destination - origin);
        }

        public static bool IsBehind(this Vector3 origin, Vector3 targetPoint)
        {
            return Vector3.Dot(origin, targetPoint) < 0;
        }

        public static Vector3 With(this Vector3 original, float? x = null, float? y = null, float? z = null)
        {
            float newX = x.HasValue ? x.Value : original.x;
            float newY = y.HasValue ? y.Value : original.y;
            float newZ = z.HasValue ? z.Value : original.z;

            return new Vector3(newX, newY, newZ);
        }

        public static bool EqualToVector3Path(this List<Vector3> vector3Path1, List<Vector3> vector3Path2)
        {
            if (vector3Path1.Count != vector3Path2.Count)
                return false;
            for (int pointIndex = 0; pointIndex < vector3Path1.Count; pointIndex++)
                if (vector3Path1[pointIndex] != vector3Path2[pointIndex])
                    return false;
            return true;
        }

        public static bool EqualToVector3Path(this List<Vector3> vector3Path1, Vector3[] vector3Path2)
        {
            if (vector3Path1.Count != vector3Path2.Length)
                return false;
            for (int pointIndex = 0; pointIndex < vector3Path1.Count; pointIndex++)
                if (vector3Path1[pointIndex] != vector3Path2[pointIndex])
                    return false;
            return true;
        }

        public static bool EqualToVector3Path(this Vector3[] vector3Path1, List<Vector3> vector3Path2)
        {
            if (vector3Path1.Length != vector3Path2.Count)
                return false;
            for (int pointIndex = 0; pointIndex < vector3Path1.Length; pointIndex++)
                if (vector3Path1[pointIndex] != vector3Path2[pointIndex])
                    return false;
            return true;
        }

        public static float Distance(this Vector3 thisPosition, Vector3 targetPosition)
        {
            return Get.DistanceBetween(targetPosition, thisPosition);
        }

        public static Vector3 Shifted(this Vector3 v3, float x, float y, float z)
        {
            Vector3 temp = v3;
            temp.x += x;
            temp.y += y;
            temp.z += z;
            v3 = temp;
            return v3;
        }

        public static void Shift(this Vector3 v3, float x, float y, float z)
        {
            Vector3 temp = v3;
            temp.x += x;
            temp.y += y;
            temp.z += z;
            v3 = temp;
        }

        //public static Vector3 v3 { get; set; }
    }

    public static class RotationExtensions
    {
        // Super-Duper Function!
        public static float YAngleTo(this Transform rotator, Vector3 targetPosition)
        {
            // Euler angles are easier to deal with. You could use Quaternions here also
            // C# requires you to set the entire rotation variable. You can't set the individual x and z (UnityScript can), so you make a temp Vec3 and set it back
            Vector3 eulerAngles = rotator.rotation.eulerAngles;
            eulerAngles.x = 0;
            eulerAngles.z = 0;

            // Set the altered rotation back
            return Quaternion.Euler(eulerAngles).y;
        }

        // Super-Duper Function!
        public static Quaternion RotateOnYAngleTo(this Transform rotator, Vector3 targetPosition)
        {
            // Look at including x and z leaning
            rotator.LookAt(targetPosition);

            // Euler angles are easier to deal with. You could use Quaternions here also
            // C# requires you to set the entire rotation variable. You can't set the individual x and z (UnityScript can), so you make a temp Vec3 and set it back
            Vector3 eulerAngles = rotator.rotation.eulerAngles;
            eulerAngles.x = 0;
            eulerAngles.z = 0;

            // Set the altered rotation back
            return Quaternion.Euler(eulerAngles);
        }

        public static Quaternion Shifted(this Quaternion q, float x, float y, float z, float w)
        {
            Quaternion temp = q;

            temp.x += x;
            temp.y += y;
            temp.z += z;
            temp.w += w;

            q = temp;

            return q;
        }
    }

    public static class GameObjectExtensions
    {
    }

    public static class TransformExtensions
    {
        public static Transform FindParentWithNameContainingString(this Transform childTransform, string searchString)
        {
            Transform t = childTransform;
            while (t.parent != null)
            {
                if (t.parent.name.ToLower().Contains(searchString))
                    return t.parent;
                t = t.parent.transform;
            }

            return null; // Could not find a parent with given tag.
        }

        public static Vector3 DirectionTo(this Transform origin, Vector3 destination)
        {
            return Vector3.Normalize(destination - origin.position);
        }

        public static void RotateTowards(this Transform thisTransform, Vector3 dir, float rotationSpeed)
        {
            Vector3 lookPos = dir - thisTransform.position;
            lookPos.y = 0;

            if (lookPos == Vector3.zero)
                return;

            Quaternion rotation = Quaternion.LookRotation(lookPos);
            thisTransform.rotation = Quaternion.Slerp(thisTransform.rotation, rotation, Time.deltaTime * rotationSpeed);
        }

        public static bool RotateUntilFacing(this Transform thisTransform, Vector3 dir, float rotationSpeed)
        {
            Vector3 lookPos = dir - thisTransform.position;
            lookPos.y = 0;

            if (lookPos == Vector3.zero)
                return false;

            Quaternion rotation = Quaternion.LookRotation(lookPos);
            thisTransform.rotation = Quaternion.Slerp(thisTransform.rotation, rotation, Time.deltaTime * rotationSpeed);

            return thisTransform.IsFacing(dir, 10);
        }

        public static bool IsFacing(this Transform looker, Vector3 targetPos, float FOVAngle)
        {
            // FOVAngle has to be less than 180
            float checkAngle =
                Mathf.Min(FOVAngle, 359.9999f) /
                2; // divide by 2 isn't necessary, just a bit easier to understand when looking at the angles.

            float dot = Vector3.Dot(looker.forward,
                (targetPos - looker.position).normalized); // credit to fafase for this

            float viewAngle =
                (1 - dot) * 90; // convert the dot product value into a 180 degree representation (or *180 if you don't divide by 2 earlier)

            if (viewAngle <= checkAngle)
                return true;
            return false;
        }

        public static Transform GetRandomTransformWithTag(this Transform t, string tag)
        {
            GameObject[] goList = GameObject.FindGameObjectsWithTag(tag);
            List<GameObject> finalGoList = new List<GameObject>();
            foreach (GameObject go in goList)
                if (go.transform.name != t.name)
                    finalGoList.Add(go);
            return finalGoList[Random.Range(0, finalGoList.Count)].transform;
        }

        public static Vector3 GetDirection(this Transform t, Transform targetTransform)
        {
            return targetTransform.position - t.position;
        }

        public static Vector3 GetDirection(this Vector3 pos, Vector3 targetPosition)
        {
            return targetPosition - pos;
        }

        public static Vector3 GetDirection(this Transform t, Vector3 targetPosition)
        {
            return targetPosition - t.position;
        }

        public static Vector3 GetDirection(this Vector3 pos, Transform targetTransform)
        {
            return targetTransform.position - pos;
        }

        public static bool HasChild(this Transform t, string sChildName, bool caseSensitiv)
        {
            if (t.Child(sChildName, false, caseSensitiv))
                return true;
            return false;
        }

        public static Transform Child(this Transform tParent, string sChildName, bool checkIfContainsName,
            bool caseSensitive)
        {
            string parentName = !caseSensitive ? tParent.name.ToLower() : tParent.name;
            string childName = !caseSensitive ? sChildName.ToLower() : sChildName;

            // check if the current bone is the bone we're looking for, if so return it
            if (checkIfContainsName)
            {
                if (parentName.Contains(!caseSensitive ? childName.ToLower() : childName))
                    return tParent;
            }
            else
            {
                if (parentName == (!caseSensitive ? childName.ToLower() : childName))
                    return tParent;
            }

            Transform found = null;

            // search through child transforms for the bone we're looking for
            for (int i = 0; i < tParent.childCount; ++i)
            {
                // the recursive step; repeat the search one step deeper in the hierarchy
                found = TransformExtensions.Child(tParent.GetChild(i), sChildName, checkIfContainsName, caseSensitive);

                // a transform was returned by the search above that is not null,
                // it must be the bone we're looking for
                if (found != null)
                    return found;
            }

            // transform with name was not found
            return null;
        }

        public static Transform FindDeepChild(this Transform parent, string name)
        {
            Transform result = parent.Find(name);
            if (result != null)
                return result;
            foreach (Transform child in parent)
            {
                result = child.FindDeepChild(name);
                if (result != null)
                    return result;
            }

            return null;
        }

        public static void Hide(this Transform t)
        {
            if (t.GetComponent<Renderer>().enabled)
                t.GetComponent<Renderer>().enabled = false;
        }

        public static void Unhide(this Transform t)
        {
            if (!t.GetComponent<Renderer>().enabled)
                t.GetComponent<Renderer>().enabled = true;
        }

        public static void Disable(this Transform t)
        {
            if (t.gameObject.activeSelf)
                t.gameObject.SetActive(false);
        }

        public static void ShiftPositionX(this Transform t, float offsetX)
        {
            Vector3 temp = t.position;
            temp.x += offsetX;
            t.position = temp;
        }

        public static void ShiftPositionY(this Transform t, float offsetY)
        {
            Vector3 temp = t.position;
            temp.y += offsetY;
            t.position = temp;
        }

        public static void ShiftPositionZ(this Transform t, float offsetZ)
        {
            Vector3 temp = t.position;
            temp.z += offsetZ;
            t.position = temp;
        }
    }

    public static class AudioExtensions
    {
        public static void ProduceSound(this AudioSource audiso, string soundCategory, string soundFileName)
        {
            if (soundCategory.IndexOf("human", StringComparison.InvariantCultureIgnoreCase) >= 0)
                soundCategory = "Human Sounds/";
            if (soundCategory.IndexOf("magic", StringComparison.InvariantCultureIgnoreCase) >= 0)
                soundCategory = "Magic Sounds/";
            if (soundCategory.IndexOf("gui", StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                soundCategory.IndexOf("interface", StringComparison.InvariantCultureIgnoreCase) >= 0)
                soundCategory = "GUI Sounds/";
            //soundCategory = Directory.GetFiles();
            Debug.Log(Application.dataPath);
            audiso.clip =
                Resources.Load("Sound Library/" + soundCategory + soundFileName, typeof(AudioClip)) as AudioClip;
            audiso.Play();
        }

        //public static AudioSource PlayClipAt(AudioClip clip, Vector3 pos)
        //{
        //    GameObject tempGO = new GameObject("TempAudio"); // create the temp object
        //    tempGO.transform.position = pos; // set its position
        //    AudioSource aSource = tempGO.AddComponent<AudioSource>(); // add an audio source
        //    aSource.clip = clip; // define the clip
        //    // set other aSource properties here, if desired
        //    aSource.Play(); // start the sound
        //    Destroy(tempGO, clip.length); // destroy object after clip duration
        //    return aSource; // return the AudioSource reference
        //}
    }

    public static class ArrayExtensions
    {
        public static object GetRandomObjectFromArray(this object[] array)
        {
            return array[Random.Range(0, array.Length)];
        }

        public static Vector3 GetRandomVector3FromArray(this Vector3[] array)
        {
            return array[Random.Range(0, array.Length)];
        }

        public static GameObject GetRandomGameObjectFromArray(this GameObject[] array)
        {
            return array[Random.Range(0, array.Length)];
        }

        public static Transform GetRandomVector3FromArray(this Transform[] array)
        {
            return array[Random.Range(0, array.Length)];
        }
    }

    public static class ListExtensions
    {
        public static List<T> Shuffle<T>(this List<T> ts)
        {
            int count = ts.Count;
            int last = count - 1;
            for (int i = 0; i < last; ++i)
            {
                int r = Random.Range(i, count);
                T tmp = ts[i];
                ts[i] = ts[r];
                ts[r] = tmp;
            }

            return ts;
        }

        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0)
                return attributes[0].Description;
            return value.ToString();
        }
    }

    public static class InterfaceExtensions
    {
        public static T GetInterface<T>(this GameObject inObj) where T : class
        {
            return inObj.GetComponents<Component>().OfType<T>().FirstOrDefault();
        }

        public static IEnumerable<T> GetInterfaces<T>(this GameObject inObj) where T : class
        {
            return inObj.GetComponents<Component>().OfType<T>();
        }

        //public static T GetInterface<T>(this GameObject inObj) where T : class
        //{
        //    if(!typeof(T).IsInterface)
        //    {
        //        return null;
        //    }

        //    return inObj.GetComponents<UnityEngine.Component>().OfType<T>().FirstOrDefault();
        //}

        //public static IEnumerable<T> GetInterfaces<T>(this GameObject inObj) where T : class
        //{
        //    if(!typeof(T).IsInterface)
        //    {
        //        return Enumerable.Empty<T>();
        //    }

        //    return inObj.GetComponents<UnityEngine.Component>().OfType<T>();
        //}
    }

    public static class RegexExtensions
    {
        public static string SplitWords(this string value)
        {
            return Regex.Replace(value, @"((?<=\p{Ll})\p{Lu})|((?!\A)\p{Lu}(?>\p{Ll}))", " $0");
        }

        public static string SplitCamelCase(this string value)
        {
            return Regex.Replace(value, "(?<=[a-z])([A-Z])", " $1", RegexOptions.Compiled).Trim();
        }
    }

    public static class StringExtensions
    {
        public static string FirstCharToUpper(this string input)
        {
            return input switch
            {
                null => throw new ArgumentNullException(nameof(input)),
                "" => throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input)),
                _ => input.First().ToString().ToUpper() + input.Substring(1)
            };
        }
    }

    public static class ColorExtensions
    {
        public static string ColorToHex(Color32 color)
        {
            string hex = color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
            return hex;
        }

        public static Color HexToColor(string hex)
        {
            hex = hex.Trim();
            if (hex[0] == '#')
                hex = hex.Substring(1);

            byte r = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
            byte g = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
            byte b = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);
            return new Color32(r, g, b, 255);
        }

        public static string ToRGBHex(Color c)
        {
            return string.Format("#{0:X2}{1:X2}{2:X2}", ColorExtensions.ToByte(c.r), ColorExtensions.ToByte(c.g),
                ColorExtensions.ToByte(c.b));
        }

        private static byte ToByte(float f)
        {
            f = Mathf.Clamp01(f);
            return (byte)(f * 255);
        }

        public static Color32 RandomColor()
        {
            return new Color32((byte)Random.Range(0, 255), (byte)Random.Range(0, 255), (byte)Random.Range(0, 255),
                255);
        }

        public static Color32 AverageColorFromTexture(Texture2D tex)
        {
            Color32[] texColors = tex.GetPixels32();

            int total = texColors.Length;

            float r = 0;
            float g = 0;
            float b = 0;

            for (int i = 0; i < total; i++)
            {
                r += texColors[i].r;

                g += texColors[i].g;

                b += texColors[i].b;
            }

            return new Color32((byte)(r / total), (byte)(g / total), (byte)(b / total), 0);
        }
    }

    public static class ShaderExtensions
    {
        public enum BlendMode
        {
            Opaque,
            Cutout,
            Fade,
            Transparent
        }

        public static void SetRenderMode(this Material standardShaderMaterial, BlendMode blendMode)
        {
            switch (blendMode)
            {
                case BlendMode.Opaque:
                    standardShaderMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    standardShaderMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    standardShaderMaterial.SetInt("_ZWrite", 1);
                    standardShaderMaterial.DisableKeyword("_ALPHATEST_ON");
                    standardShaderMaterial.DisableKeyword("_ALPHABLEND_ON");
                    standardShaderMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    standardShaderMaterial.renderQueue = -1;
                    break;

                case BlendMode.Cutout:
                    standardShaderMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    standardShaderMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    standardShaderMaterial.SetInt("_ZWrite", 1);
                    standardShaderMaterial.EnableKeyword("_ALPHATEST_ON");
                    standardShaderMaterial.DisableKeyword("_ALPHABLEND_ON");
                    standardShaderMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    standardShaderMaterial.renderQueue = 2450;
                    break;

                case BlendMode.Fade:
                    standardShaderMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    standardShaderMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    standardShaderMaterial.SetInt("_ZWrite", 0);
                    standardShaderMaterial.DisableKeyword("_ALPHATEST_ON");
                    standardShaderMaterial.EnableKeyword("_ALPHABLEND_ON");
                    standardShaderMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    standardShaderMaterial.renderQueue = 3000;
                    break;

                case BlendMode.Transparent:
                    standardShaderMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    standardShaderMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    standardShaderMaterial.SetInt("_ZWrite", 0);
                    standardShaderMaterial.DisableKeyword("_ALPHATEST_ON");
                    standardShaderMaterial.DisableKeyword("_ALPHABLEND_ON");
                    standardShaderMaterial.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                    standardShaderMaterial.renderQueue = 3000;
                    break;
            }
        }
    }

    public static class RectTransformExtensions
    {
        public static bool Contains(this RectTransform rectTransform1, RectTransform rectTransform2)
        {
            if (rectTransform1.rect.position.x <= rectTransform2.rect.position.x &&
                rectTransform1.rect.position.x + rectTransform1.rect.size.x >=
                rectTransform2.rect.position.x + rectTransform2.rect.size.x &&
                rectTransform1.rect.position.y <= rectTransform2.rect.position.y &&
                rectTransform1.rect.position.y + rectTransform1.rect.size.y >=
                rectTransform2.rect.position.y + rectTransform2.rect.size.y)
                return true;
            return false;
        }

        public static bool Overlaps(this RectTransform a, RectTransform b)
        {
            return a.WorldRect().Overlaps(b.WorldRect());
        }

        public static bool Overlaps(this RectTransform a, RectTransform b, bool allowInverse)
        {
            return a.WorldRect().Overlaps(b.WorldRect(), allowInverse);
        }

        public static Rect WorldRect(this RectTransform rectTransform)
        {
            Vector2 sizeDelta = rectTransform.sizeDelta;
            float rectTransformWidth = sizeDelta.x * rectTransform.lossyScale.x;
            float rectTransformHeight = sizeDelta.y * rectTransform.lossyScale.y;

            Vector3 position = rectTransform.position;
            return new Rect(position.x - rectTransformWidth / 2f, position.y - rectTransformHeight / 2f,
                rectTransformWidth, rectTransformHeight);
        }

        public static bool OverlapsExtended(this RectTransform rectTrans1, RectTransform rectTrans2)
        {
            Rect rect1 = new Rect(rectTrans1.localPosition.x, rectTrans1.localPosition.y, rectTrans1.rect.width,
                rectTrans1.rect.height);
            Rect rect2 = new Rect(rectTrans2.localPosition.x, rectTrans2.localPosition.y, rectTrans2.rect.width,
                rectTrans2.rect.height);

            return rect1.Overlaps(rect2);
        }
    }

    //public static class RectTransformExtensions {
    //    public static bool OverlapsWithOther(this RectTransform rectTrans1, RectTransform rectTrans2) {
    //        Rect rect1 = new Rect(rectTrans1.localPosition.x, rectTrans1.localPosition.y, rectTrans1.rect.width, rectTrans1.rect.height);
    //        Rect rect2 = new Rect(rectTrans2.localPosition.x, rectTrans2.localPosition.y, rectTrans2.rect.width, rectTrans2.rect.height);

    //        return rect1.Overlaps(rect2);
    //    }
    //}

    #endregion Extensions //////////////////////////////////////////////////////////
}