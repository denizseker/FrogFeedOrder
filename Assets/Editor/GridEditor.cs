using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Default_Cell))]
public class GridEditor : Editor
{
#if UNITY_EDITOR
    private string[] entityOptions = { "None", "Frog", "Grape", "Arrow" };
    private int selectedEntityType = 0;

    public override void OnInspectorGUI()
    {
        Default_Cell myScript = (Default_Cell)target;

        // Entity selection
        GUILayout.Space(10);
        EditorGUILayout.LabelField("Select Entity to Add", EditorStyles.boldLabel);
        selectedEntityType = EditorGUILayout.Popup(selectedEntityType, entityOptions);

        // Prefab previews and buttons
        DisplayPrefabOptions(myScript);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Delete Last Cell"))
        {
            myScript.DeleteCell();
        }
        if (GUILayout.Button("Delete All Cells"))
        {
            myScript.DeleteAllCells();
        }
        GUILayout.EndHorizontal();

        //Rotate button only show up when active cell have arrow or frog
        if (myScript.activeCell != null)
        {
            var cell = myScript.activeCell.GetComponent<Entity_Cell>();
            var entity = cell.entityOnCell;

            if (entity != null && (entity.GetComponent<Frog>() != null || entity.GetComponent<Arrow>() != null))
            {
                if (GUILayout.Button("Rotate Entity"))
                {
                    myScript.RotateEntity();
                }
            }
        }


        DrawDefaultInspector();
    }

    private void DisplayPrefabOptions(Default_Cell myScript)
    {
        if (myScript.cellPrefabs == null) return;

        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();

        foreach (var prefab in myScript.cellPrefabs)
        {
            if (prefab == null) continue;

            var preview = AssetPreview.GetAssetPreview(prefab);
            if (preview != null)
            {
                GUILayout.BeginVertical();
                if (GUILayout.Button(preview, GUILayout.Width(64), GUILayout.Height(64)))
                {
                    myScript.AddCell(prefab, entityOptions[selectedEntityType]);
                }
                GUILayout.Space(0);
                GUILayout.EndVertical();
            }
        }

        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
    }
#endif
}
