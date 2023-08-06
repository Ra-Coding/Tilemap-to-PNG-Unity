using UnityEngine;
using UnityEditor;

namespace RaCoding.TilemapToPng
{
    [CustomEditor(typeof(TilemapToPngExporter))]
    public class TilemapToPngExporterEditor : Editor
    {
        string pngName = "";

        public override void OnInspectorGUI()
        {
            TilemapToPngExporter tilemapToPngExporter = (TilemapToPngExporter)target;

            DrawDefaultInspector();

            if (tilemapToPngExporter.Image == null)
            {
                if (GUILayout.Button("Create PNG"))
                {
                    tilemapToPngExporter.Pack();
                }
            }
            else
            {
                GUILayout.Label("File Name");
                pngName = GUILayout.TextField(pngName);
                if (pngName.Length > 0)
                {
                    if (GUILayout.Button("Export PNG"))
                    {
                        tilemapToPngExporter.ExportAsPng(pngName);
                    }
                }
            }
        }
    }
}