using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Steps
{
    public class FootPrintLine : MonoBehaviour
    {
        [SerializeField] private GameObject footStepR;
        [SerializeField] private GameObject footStepL;
        [SerializeField] private Material footStepMaterial;
        [SerializeField] private float spacingForward;
        [SerializeField] private float spacingSide;
        [SerializeField] private Transform[] pathPoints;

        private List<GameObject> _footprints = new List<GameObject>();

        private void Start()
        {
            footStepR.GetComponent<Renderer>().material = footStepMaterial;
            footStepL.GetComponent<Renderer>().material = footStepMaterial;
            footStepMaterial.SetColor("_Color", GetComponentInParent<FootStepColorController>()._grayColor);
            GenerateFootprintsAlongPath();
        }

        private void GenerateFootprintsAlongPath()
        {
            if (pathPoints.Length < 2) return;

            bool isRight = true;
            for (int i = 0; i < pathPoints.Length - 1; i++)
            {
                Vector2 start = pathPoints[i].position;
                Vector2 end = pathPoints[i + 1].position;
                Vector2 direction = (end - start).normalized;
                float segmentLength = Vector2.Distance(start, end);
                int footprintsCount = Mathf.FloorToInt(segmentLength / spacingForward);

                for (int j = 0; j < footprintsCount; j++)
                {
                    Vector2 position = start + direction * (j * spacingForward);
                    var angle = GetAngleFromVector(direction);
                    var angleOffset = (angle % 90) * 90;
                    Quaternion rotation = Quaternion.Euler(0, 0, angle - 90);
                    Vector2 perpendicular = Vector2.Perpendicular(direction).normalized;
                    perpendicular *= spacingSide;
                    GameObject footprint = Instantiate(isRight ? footStepR : footStepL,
                        isRight ? position - perpendicular : position + perpendicular, rotation, transform);
                    _footprints.Add(footprint);
                    isRight = !isRight;
                }
            }
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