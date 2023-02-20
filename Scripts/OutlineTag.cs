using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EasyOutline
{
    public class OutlineTag : MonoBehaviour
    {
        [HideInInspector] public MeshFilter[] meshFilters;
        [HideInInspector] public Renderer[] meshRenderers;

        void Awake()
        {
            meshFilters = gameObject.GetComponentsInChildren<MeshFilter>();
            foreach (var meshFilter in meshFilters)
            {
                meshFilter.mesh.SetUVs(2, CalculateSmoothNormals(meshFilter.mesh));
            }
            
            meshRenderers = gameObject.GetComponentsInChildren<Renderer>();
        }

        private List<Vector3> CalculateSmoothNormals(Mesh mesh)
        {
            // 위치가 같은 버텍스들을 그룹으로 묶는다.
            var verticesGroups = new Dictionary<Vector3, List<KeyValuePair<int, Vector3>>>();
            for (int i = 0; i < mesh.vertices.Length; ++i)
            {
                var vertex = mesh.vertices[i];
                List<KeyValuePair<int, Vector3>> group;

                if (verticesGroups.ContainsKey(vertex) == false)
                {
                    verticesGroups[vertex] = new List<KeyValuePair<int, Vector3>>();
                }

                group = verticesGroups[vertex];
                group.Add(new KeyValuePair<int, Vector3>(i, vertex));
            }

            var smoothNormals = new List<Vector3>(mesh.normals);

            // 묶은 버텍스 그룹과 입력받은 버텍스의 개수가 같다면 위치가 같은 버텍스가 없다는 것을 뜻하므로 즉시 리턴한다. 
            if (verticesGroups.Count == mesh.vertices.Length)
                return smoothNormals;
            
            // 위치가 같은 버텍스들이 노말의 평균 구한다.
            foreach (var verticesGroup in verticesGroups)
            {
                var group = verticesGroup.Value;

                if (group.Count != 1)
                {
                    var smoothNormal = Vector3.zero;

                    foreach (var vertex in group)
                    {
                        smoothNormal += mesh.normals[vertex.Key];
                    }

                    smoothNormal.Normalize();

                    foreach (var vertex in group)
                    {
                        smoothNormals[vertex.Key] = smoothNormal;
                    }
                }
            }

            return smoothNormals;
        }
    }
}
