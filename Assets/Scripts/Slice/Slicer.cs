using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Assets.Scripts.Slice

{
    class Slicer
    {
        /// <summary>
        /// Slice the object by the plane 
        /// </summary>
        /// <param name="plane"></param>
        /// <param name="objectToCut"></param>
        /// <returns></returns>
        public static GameObject[] Slice(Plane plane, GameObject objectToCut)
        {
            foreach (var needsCleanup in objectToCut.GetComponents<ISliceCleanup>())
                needsCleanup.Cleanup();
            
            //Get the current mesh and its verts and tris
            Mesh mesh = objectToCut.GetComponent<MeshFilter>().mesh;
            var a = mesh.GetSubMesh(0);
            Sliceable sliceable = objectToCut.GetComponent<Sliceable>();

            if (sliceable == null)
            {
                throw new NotSupportedException("Cannot slice non sliceable object, add the sliceable script to the object or inherit from sliceable to support slicing");
            }

            //Create left and right slice of hollow object
            SlicesMetadata slicesMeta = new SlicesMetadata(plane, mesh, sliceable.IsSolid, sliceable.ReverseWireTriangles, sliceable.ShareVertices, sliceable.SmoothVertices);

            GameObject positiveObject = CreateMeshGameObject(objectToCut);
            positiveObject.name = string.Format("{0}_positive", objectToCut.name);

            GameObject negativeObject = CreateMeshGameObject(objectToCut);
            negativeObject.name = string.Format("{0}_negative", objectToCut.name);

            var positiveSideMeshData = slicesMeta.PositiveSideMesh;
            positiveSideMeshData.RecalculateBounds();
            var negativeSideMeshData = slicesMeta.NegativeSideMesh;

            positiveObject.GetComponent<MeshFilter>().mesh = positiveSideMeshData;
            negativeObject.GetComponent<MeshFilter>().mesh = negativeSideMeshData;

            SetupCollidersAndRigidBodys(ref positiveObject, positiveSideMeshData, sliceable.UseGravity);
            SetupCollidersAndRigidBodys(ref negativeObject, negativeSideMeshData, sliceable.UseGravity);

            return new GameObject[] { positiveObject, negativeObject };
        }

        /// <summary>
        /// Creates the default mesh game object.
        /// </summary>
        /// <param name="originalObject">The original object.</param>
        /// <returns></returns>
        private static GameObject CreateMeshGameObject(GameObject originalObject)
        {
            var originalMaterial = originalObject.GetComponent<MeshRenderer>().materials;

            GameObject meshGameObject = new GameObject();
            Sliceable originalSliceable = originalObject.GetComponent<Sliceable>();

            meshGameObject.AddComponent<MeshFilter>();
            meshGameObject.AddComponent<MeshRenderer>();
            Sliceable sliceable = meshGameObject.AddComponent<Sliceable>();

            sliceable.IsSolid = originalSliceable.IsSolid;
            sliceable.ReverseWireTriangles = originalSliceable.ReverseWireTriangles;
            sliceable.UseGravity = originalSliceable.UseGravity;

            meshGameObject.GetComponent<MeshRenderer>().materials = originalMaterial;

            meshGameObject.transform.localScale = originalObject.transform.localScale;
            meshGameObject.transform.rotation = originalObject.transform.rotation;
            meshGameObject.transform.position = originalObject.transform.position;

            meshGameObject.tag = originalObject.tag;

            ControllableObject originalCobj = originalObject.GetComponent<ControllableObject>();

            if (originalCobj != null)
            {
                ControllableObject obj = meshGameObject.AddComponent<ControllableObject>();
                obj.objectRenderer = obj.GetComponent<MeshRenderer>();
                
                // ---- FADER SETUP ----
                ObjectFader fader = meshGameObject.AddComponent<ObjectFader>();
                fader.targetAlpha = 0;
                fader.duration = 0.5f;
                fader.targetRenderer = meshGameObject.GetComponent<MeshRenderer>();
                obj.onAttached.AddListener(fader.SetFading);
                // ---------------------
                
                // ---- HIGHLIGHTER SETUP ----
                ObjectHighlighter highlighter = meshGameObject.AddComponent<ObjectHighlighter>();
                highlighter.duration = 0.5f;
                highlighter.intensity = 4f;
                highlighter.meshRenderer = meshGameObject.GetComponent<MeshRenderer>();
                var lookEvent = meshGameObject.AddComponent<LookEvent>();
                lookEvent.targetRenderer = meshGameObject.GetComponent<MeshRenderer>();
                lookEvent.lookStrength = 0.99f;
                lookEvent.onLookChanged.AddListener(highlighter.SetHighlighted);
                // ---- HIGHLIGHTER SETUP ----
                
                obj.properties = originalCobj.properties;
                Rigidbody rb = meshGameObject.AddComponent<Rigidbody>();
                rb.mass = originalCobj.body.mass * 0.7F;
                rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
                rb.interpolation = RigidbodyInterpolation.Interpolate;
                obj.body = rb;
            } else
            {
                Rigidbody originalrb = originalObject.GetComponent<Rigidbody>();
                if (originalrb != null)
                {
                    Rigidbody rb = meshGameObject.AddComponent<Rigidbody>();
                    rb.mass = originalrb.mass * 0.7F;
                }
            }




            return meshGameObject;
        }

        /// <summary>
        /// Add mesh collider and rigid body to game object
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="mesh"></param>
        private static void SetupCollidersAndRigidBodys(ref GameObject gameObject, Mesh mesh, bool useGravity)
        {
            MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
            meshCollider.sharedMesh = mesh;
            meshCollider.convex = true;

            var rb = gameObject.GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody>();
            }
            rb.useGravity = useGravity;
        }
    }
}