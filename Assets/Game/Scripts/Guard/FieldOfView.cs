using System;
using UnityEngine;

namespace Game.Scripts.Guard
{
    public class FieldOfView : MonoBehaviour
    {
        [SerializeField] private float detectionRadius;
        
        private Mesh _mesh;
        private MeshRenderer _meshRenderer;
        private Vector3 _origin;
        private float _startingAngle;
        private float _fov;

        private void Awake()
        {
            _meshRenderer = GetComponent<MeshRenderer>();
        }

        private void Start()
        {
            _mesh = new Mesh();
            GetComponent<MeshFilter>().mesh = _mesh;
            _mesh.MarkDynamic();
            _fov = 360f;
        }

        private void LateUpdate()
        {
            var rayCount = 100;
            var angle = _startingAngle;
            var angleIncrease = _fov / rayCount;
            float viewDistance = detectionRadius;

            Vector3[] vertices = new Vector3[rayCount + 2];
            Vector2[] uv = new Vector2[vertices.Length];
            int[] triangles = new int[(rayCount * 3)];

            vertices[0] = _origin;
            uv[0] = new Vector2(0.5f, 0.5f);
            for (var i = 0; i < rayCount; i++)
            {
                bool isVertexHit;
                Vector3 vertex;
                var vectorFromAngle = GetVectorFromAngle(angle);
                var hit = Physics2D.Raycast(_origin, vectorFromAngle, viewDistance,
                    LayerMask.GetMask("Obstacle"));
                if (hit.collider == null)
                {
                    //No hit
                    vertex = _origin + vectorFromAngle * viewDistance;
                    isVertexHit = false;
                }
                else
                {
                    //Hit
                    vertex = hit.point;
                    isVertexHit = true;
                }

                if (isVertexHit)
                {
                    var coef =  vertex.magnitude / (_origin + vectorFromAngle * viewDistance).magnitude;
                    uv[i + 1] = Vector2.Lerp(uv[0], vectorFromAngle , coef);
                }
                else
                    uv[i + 1] = vectorFromAngle;


                vertices[i + 1] = vertex;

                angle -= angleIncrease;
            }

            for (int i = 0; i < rayCount - 1; i++)
            {
                var triangleIndex = i * 3;
                triangles[triangleIndex + 0] = 0;
                triangles[triangleIndex + 1] = i + 1;
                triangles[triangleIndex + 2] = i + 2;
            }

            var lastTriangleIndex = (rayCount-1) * 3;
            triangles[lastTriangleIndex + 0] = 0;
            triangles[lastTriangleIndex + 1] = rayCount;
            triangles[lastTriangleIndex + 2] = 1;

            _mesh.Clear();
            _mesh.vertices = vertices;
            _mesh.uv = uv;
            _mesh.triangles = triangles;
            _mesh.RecalculateNormals();
            _mesh.RecalculateBounds();
        }

        public void SetMaterial(Material material)
        {
            _meshRenderer.material = material;
        }
        
        public void SetOrigin(Vector3 origin)
        {
            _origin = origin;
        }

        public void SetAimDirection(Vector3 direction)
        {
            _startingAngle = GetAngleFromVector(direction) - _fov / 2f;
        }

        private Vector3 GetVectorFromAngle(float angle)
        {
            //angle 0 -> 360
            float angleRad = angle * Mathf.Deg2Rad;
            return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
        }

        private float GetAngleFromVector(Vector3 direction)
        {
            direction.Normalize();
            var n = MathF.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            if (n < 0) n += 360;
            return n;
        }
    }
}