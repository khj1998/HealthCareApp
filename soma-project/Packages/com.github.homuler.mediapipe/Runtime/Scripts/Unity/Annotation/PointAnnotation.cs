// Copyright (c) 2021 homuler
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using Mediapipe.Unity.CoordinateSystem;
using UnityEngine;

using mplt = Mediapipe.LocationData.Types;

namespace Mediapipe.Unity
{
#pragma warning disable IDE0065
  using Color = UnityEngine.Color;
#pragma warning restore IDE0065

  public class PointAnnotation : HierarchicalAnnotation
  {
    [SerializeField] private Color _color = Color.green;
    [SerializeField] private float _radius = 15.0f;
    private Material _material;

    private void Awake()
    {
      _material = GetComponent<Renderer>().material;
      _material.EnableKeyword("_EMISSION");
    }

    private void OnEnable()
    {
      ApplyColor(_color);
      ApplyRadius(_radius);
    }

    private void OnDisable()
    {
      ApplyRadius(0.0f);
    }

    public void SetColor(Color color)
    {
      _color = color;
      ApplyColor(_color);
    }

    public void SetRadius(float radius)
    {
      _radius = radius;
      ApplyRadius(_radius);
    }

    public void Draw(Vector3 position)
    {
      SetActive(true); // Vector3 is not nullable
      transform.localPosition = position;
    }

    public void Draw(Landmark target, Vector3 scale, bool visualizeZ = true)
    {
      if (ActivateFor(target))
      {
        var position = GetScreenRect().GetPoint(target, scale, rotationAngle, isMirrored);
        if (!visualizeZ)
        {
          position.z = 0.0f;
        }
        transform.localPosition = position;
      }
    }

    public void Draw(NormalizedLandmark target, bool visualizeZ = true)
    {
      if (ActivateFor(target))
      {
        var position = GetScreenRect().GetPoint(target, rotationAngle, isMirrored);
        if (!visualizeZ)
        {
          position.z = 0.0f;
        }
        transform.localPosition = position;
      }
    }

    public void Draw(NormalizedPoint2D target)
    {
      if (ActivateFor(target))
      {
        var position = GetScreenRect().GetPoint(target, rotationAngle, isMirrored);
        transform.localPosition = position;
      }
    }

    public void Draw(Point3D target, Vector2 focalLength, Vector2 principalPoint, float zScale, bool visualizeZ = true)
    {
      if (ActivateFor(target))
      {
        var position = GetScreenRect().GetPoint(target, focalLength, principalPoint, zScale, rotationAngle, isMirrored);
        if (!visualizeZ)
        {
          position.z = 0.0f;
        }
        transform.localPosition = position;
      }
    }

    public void Draw(AnnotatedKeyPoint target, Vector2 focalLength, Vector2 principalPoint, float zScale, bool visualizeZ = true)
    {
      if (visualizeZ)
      {
        Draw(target?.Point3D, focalLength, principalPoint, zScale, true);
      }
      else
      {
        Draw(target?.Point2D);
      }
    }

    public void Draw(mplt.RelativeKeypoint target, float threshold = 0.0f)
    {
      if (ActivateFor(target))
      {
        Draw(GetScreenRect().GetPoint(target, rotationAngle, isMirrored));
        SetColor(GetColor(target.Score, threshold));
      }
    }

    private void ApplyColor(Color color)
    {
      color *= Mathf.Pow(2.0f, /*Intensity*/ 6);
      _material.SetColor("_EmissionColor", color);
    }

    private void ApplyRadius(float radius)
    {
      transform.localScale = radius * Vector3.one;
    }

    private Color GetColor(float score, float threshold)
    {
      var t = (score - threshold) / (1 - threshold);
      var h = Mathf.Lerp(90, 0, t) / 360; // from yellow-green to red
      return Color.HSVToRGB(h, 1, 1);
    }
  }
}
