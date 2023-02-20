using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EasyOutline
{
    public class OutlineManager : MonoBehaviour
    {
        private static readonly int s_VisibleColor = Shader.PropertyToID("_VisibleColor");
        private static readonly int s_InvisibleColor = Shader.PropertyToID("_InvisibleColor");
        private static readonly int s_Thickness = Shader.PropertyToID("_Thickness");
        
        [SerializeField] private Color _visibleOutlineColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        [SerializeField] private Color _invisibleOutlineColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        [SerializeField] private Color _visibleOverlayColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        [SerializeField] private Color _invisibleOverlayColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        [SerializeField] private float _thickness = 0.05f;
        
        private Material _maskMaterial;
        private Material _outlineMaterial;
        private Material _overlayMaterial;
        
        private List<Material> _materials = new List<Material>();
        
        void Awake()
        {
            _maskMaterial = Instantiate(Resources.Load<Material>("Materials/Mask"));
            _outlineMaterial = Instantiate(Resources.Load<Material>("Materials/Outline"));
            _overlayMaterial = Instantiate(Resources.Load<Material>("Materials/Overlay"));

            ApplyProperties();
            CreateOverlayPlane();
        }

        private void OnDestroy()
        {
            Destroy(_maskMaterial);
            Destroy(_outlineMaterial);
            Destroy(_overlayMaterial);
        }
        
        private void ApplyProperties()
        {
            _maskMaterial.SetFloat(s_Thickness, _thickness);
            
            _outlineMaterial.SetFloat(s_Thickness, _thickness);
            _outlineMaterial.SetColor(s_VisibleColor, _visibleOutlineColor);
            _outlineMaterial.SetColor(s_InvisibleColor, _invisibleOutlineColor);

            _overlayMaterial.SetFloat(s_Thickness, _thickness);
            _overlayMaterial.SetColor(s_VisibleColor, _visibleOverlayColor);
            _overlayMaterial.SetColor(s_InvisibleColor, _invisibleOverlayColor);
        }
        
        private void CreateOverlayPlane()
        {
            var mainCamera = Camera.main;
            var camTransform = mainCamera.transform;
            var clipPoint = mainCamera.nearClipPlane + 0.05f;
            
            GameObject plane = new GameObject("Plane");
            MeshFilter meshFilter = (MeshFilter)plane.AddComponent(typeof(MeshFilter));
            var planeSize = GetCameraClipPlaneSize(mainCamera, clipPoint);
            meshFilter.mesh = CreatePlaneMesh(planeSize.x, planeSize.y);
            MeshRenderer meshRenderer = plane.AddComponent<MeshRenderer>();
            meshRenderer.material = _overlayMaterial;
            
            plane.transform.SetParent(mainCamera.transform, false);
            plane.transform.forward = -camTransform.forward;
            plane.transform.position += camTransform.forward * clipPoint;
        }
        
        Vector2 GetCameraClipPlaneSize(Camera camera, float clipPoint)
        {
            if (camera.orthographic)
            {
                var orthographicSize = camera.orthographicSize;
                if (camera.pixelWidth < camera.pixelHeight)
                {
                    var ratio = (float)camera.pixelWidth / (float)camera.pixelHeight;
                    return new Vector2(orthographicSize * ratio, orthographicSize);    
                }
                else
                {
                    var ratio = (float)camera.pixelWidth / (float)camera.pixelHeight;
                    return new Vector2(orthographicSize * ratio, orthographicSize);
                }
            }
            else
            {
                float angle = camera.fieldOfView * 0.5f;                // get angle
                angle = angle * Mathf.Deg2Rad;                          // convert tor radians
                float h = (Mathf.Tan(angle) * clipPoint);               // calc height
                float w = (h / camera.pixelHeight) * camera.pixelWidth; // deduct width
                return new Vector2(w, h);    
            }
        }
        
        private Mesh CreatePlaneMesh(float width, float height)
        {
            Mesh m = new Mesh();
            m.name = "Plane";
            m.vertices = new Vector3[] {
                new Vector3(-width, -height, 0.01f),
                new Vector3(width, -height, 0.01f),
                new Vector3(width, height, 0.01f),
                new Vector3(-width, height, 0.01f)
            };
            m.uv = new Vector2[] {
                new Vector2 (0, 0),
                new Vector2 (0, 1),
                new Vector2(1, 1),
                new Vector2 (1, 0)
            };
            m.triangles = new int[] { 0, 1, 2, 0, 2, 3};
            m.RecalculateNormals();
         
            return m;
        }

        public void EnableOutline(OutlineTag obj)
        {
            foreach (var meshRenderer in obj.meshRenderers)
            {
                _materials.Clear();
                _materials.AddRange(meshRenderer.sharedMaterials);
                _materials.Add(_maskMaterial);
                _materials.Add(_outlineMaterial);
                meshRenderer.materials = _materials.ToArray();
            }
        }
        
        public void DisableOutline(OutlineTag obj)
        {
            foreach (var meshRenderer in obj.meshRenderers)
            {
                _materials.Clear();
                _materials.AddRange(meshRenderer.sharedMaterials);
                _materials.Remove(_maskMaterial);
                _materials.Remove(_outlineMaterial);
                meshRenderer.materials = _materials.ToArray();
            }
        }

#if UNITY_EDITOR
        private void Update()
        {
            ApplyProperties();
        }
#endif
    }
}
