using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnityEditor
{
	[CreateAssetMenu]
	[CustomGridBrush(false, true, false, "Prefab Brush")]
	public class PrefabBrush : GridBrushBase
	{
		private const float k_PerlinOffset = 100000f;
		public GameObject[] m_Prefabs;
		public float m_PerlinScale = 0.5f;
		public int m_Z;
        //Desired parent - Jeremy K.
        public string parentTag;
        public bool useSpriteRendererScale;

		public override void Paint(GridLayout grid, GameObject brushTarget, Vector3Int position)
		{
            Paint(grid, brushTarget, position, new Vector3Int(1, 1, 1));
		}

        //Allow scaling - Jeremy K.
        public void Paint(GridLayout grid, GameObject brushTarget, Vector3Int position, Vector3Int scale)
        {
            // Do not allow editing palettes
			if (brushTarget.layer == 31)
				return;
            
            //Force to desired parent - Jeremy K.
            if (!string.IsNullOrEmpty(parentTag))
            {
                GameObject parent = GameObject.FindGameObjectWithTag(parentTag);
                if (parent != null)
                    brushTarget = parent;
            }

            BoxErase(grid, brushTarget, new BoundsInt(position, scale));

			int index = Mathf.Clamp(Mathf.FloorToInt(GetPerlinValue(position, m_PerlinScale, k_PerlinOffset)*m_Prefabs.Length), 0, m_Prefabs.Length - 1);
			GameObject prefab = m_Prefabs[index];
			GameObject instance = (GameObject) PrefabUtility.InstantiatePrefab(prefab);
			Undo.RegisterCreatedObjectUndo((Object)instance, "Paint Prefabs");

			if (instance != null)
			{
				instance.transform.SetParent(brushTarget.transform);
                SpriteRenderer sr = instance.GetComponent<SpriteRenderer>();

                if (useSpriteRendererScale && sr != null)
                {
                    sr.size = new Vector2(scale.x, scale.y);
                    float x_pivot = sr.sprite.pivot.x / sr.sprite.pixelsPerUnit * scale.x;
                    float y_pivot = sr.sprite.pivot.y / sr.sprite.pixelsPerUnit * scale.y;
                    instance.transform.position = new Vector3(position.x + x_pivot, position.y + y_pivot);
                }
                else
                {
                    instance.transform.localScale = scale;
                    instance.transform.position = grid.LocalToWorld(grid.CellToLocalInterpolated(new Vector3Int(position.x, position.y, m_Z) + new Vector3(.5f * scale.x, .5f * scale.y, .5f * scale.z)));
                }
            }
        }

        public GameObject GetParent(GameObject defaultParent)
        {
            if (!string.IsNullOrEmpty(parentTag))
            {
                GameObject desiredParent = GameObject.FindGameObjectWithTag(parentTag);
                if (desiredParent != null)
                    return desiredParent;
            }
            return defaultParent;
        }

        //Scale instead of drawing multiple - Jeremy K.
        public override void BoxFill(GridLayout gridLayout, GameObject brushTarget, BoundsInt position)
        {
            Paint(gridLayout, brushTarget, position.position, position.size);
        }

        public override void BoxErase(GridLayout gridLayout, GameObject brushTarget, BoundsInt position)
        {
            brushTarget = GetParent(brushTarget);
            base.BoxErase(gridLayout, brushTarget, position);
        }

        public override void Erase(GridLayout grid, GameObject brushTarget, Vector3Int position)
		{
			// Do not allow editing palettes
			if (brushTarget.layer == 31)
				return;

            brushTarget = GetParent(brushTarget);

			Transform erased = GetObjectInCell(grid, brushTarget.transform, new Vector3Int(position.x, position.y, m_Z));
			if (erased != null)
				Undo.DestroyObjectImmediate(erased.gameObject);
		}

		private static Transform GetObjectInCell(GridLayout grid, Transform parent, Vector3Int position)
		{
			int childCount = parent.childCount;
			Vector3 min = grid.LocalToWorld(grid.CellToLocalInterpolated(position));
			Vector3 max = grid.LocalToWorld(grid.CellToLocalInterpolated(position + Vector3Int.one));
			Bounds bounds = new Bounds((max + min)*.5f, max - min);

			for (int i = 0; i < childCount; i++)
			{
				Transform child = parent.GetChild(i);
				if (bounds.Contains(child.position))
					return child;
			}
			return null;
		}

		private static float GetPerlinValue(Vector3Int position, float scale, float offset)
		{
			return Mathf.PerlinNoise((position.x + offset)*scale, (position.y + offset)*scale);
		}
	}

	[CustomEditor(typeof(PrefabBrush))]
	public class PrefabBrushEditor : GridBrushEditorBase
	{
		private PrefabBrush prefabBrush { get { return target as PrefabBrush; } }

		private SerializedProperty m_Prefabs;
		private SerializedObject m_SerializedObject;

		protected void OnEnable()
		{
			m_SerializedObject = new SerializedObject(target);
			m_Prefabs = m_SerializedObject.FindProperty("m_Prefabs");
		}

		public override void OnPaintInspectorGUI()
		{
			m_SerializedObject.UpdateIfRequiredOrScript();
			prefabBrush.m_PerlinScale = EditorGUILayout.Slider("Perlin Scale", prefabBrush.m_PerlinScale, 0.001f, 0.999f);
			prefabBrush.m_Z = EditorGUILayout.IntField("Position Z", prefabBrush.m_Z);
				
			EditorGUILayout.PropertyField(m_Prefabs, true);
			m_SerializedObject.ApplyModifiedPropertiesWithoutUndo();
		}
	}
}
