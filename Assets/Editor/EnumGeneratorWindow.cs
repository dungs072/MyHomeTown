using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class EnumGeneratorWindow : EditorWindow
{
    private string enumName = "NewEnum";
    private List<string> enumEntries = new List<string> { "Entry1", "Entry2" };
    private Vector2 scroll;
    private string loadedFilePath = null;

    [MenuItem("Tools/Enum Generator")]
    public static void OpenWindow()
    {
        GetWindow<EnumGeneratorWindow>("Enum Generator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Enum Generator", EditorStyles.boldLabel);

        enumName = EditorGUILayout.TextField("Enum Name", enumName);

        GUILayout.Label("Enum Values:");
        scroll = EditorGUILayout.BeginScrollView(scroll);
        for (int i = 0; i < enumEntries.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            enumEntries[i] = EditorGUILayout.TextField(enumEntries[i]);
            if (GUILayout.Button("X", GUILayout.Width(20)))
            {
                enumEntries.RemoveAt(i);
                i--;
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();

        if (GUILayout.Button("Add Entry"))
        {
            enumEntries.Add("NewEntry");
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Generate New Enum File"))
        {
            GenerateNewEnum();
        }

        if (GUILayout.Button("Append to Existing Enum"))
        {
            AppendToEnum();
        }

        if (GUILayout.Button("Edit Existing Enum File"))
        {
            LoadAndEditEnum();
        }

        if (!string.IsNullOrEmpty(loadedFilePath))
        {
            GUILayout.Space(10);
            if (GUILayout.Button("Save Changes to Loaded Enum"))
            {
                SaveEditedEnum();
            }
        }
    }

    private void GenerateNewEnum()
    {
        string path = EditorUtility.SaveFilePanelInProject("Save Enum", enumName, "cs", "Choose location to save the enum");
        if (string.IsNullOrEmpty(path)) return;

        using (StreamWriter writer = new StreamWriter(path))
        {
            writer.WriteLine("public enum " + enumName);
            writer.WriteLine("{");
            for (int i = 0; i < enumEntries.Count; i++)
            {
                writer.WriteLine("    " + enumEntries[i] + (i < enumEntries.Count - 1 ? "," : ""));
            }
            writer.WriteLine("}");
        }

        AssetDatabase.Refresh();
        Debug.Log("Enum created at: " + path);
    }

    private void AppendToEnum()
    {
        string path = EditorUtility.OpenFilePanel("Select Enum File", Application.dataPath, "cs");
        if (string.IsNullOrEmpty(path)) return;

        string[] lines = File.ReadAllLines(path);
        List<string> newLines = new List<string>();
        bool insideEnum = false;
        bool inserted = false;

        for (int i = 0; i < lines.Length; i++)
        {
            string trimmed = lines[i].Trim();

            if (trimmed.StartsWith("public enum") || trimmed.StartsWith("enum"))
            {
                insideEnum = true;
            }

            if (insideEnum && trimmed == "}")
            {
                if (!inserted)
                {
                    foreach (var entry in enumEntries)
                    {
                        newLines.Add("    " + entry + ",");
                    }
                    inserted = true;
                }
            }

            newLines.Add(lines[i]);
        }

        File.WriteAllLines(path, newLines);
        AssetDatabase.Refresh();
        Debug.Log("Enum updated at: " + path);
    }

    private void LoadAndEditEnum()
    {
        string path = EditorUtility.OpenFilePanel("Select Enum File to Edit", Application.dataPath, "cs");
        if (string.IsNullOrEmpty(path)) return;

        string[] lines = File.ReadAllLines(path);
        enumEntries.Clear();
        enumName = "";
        loadedFilePath = path;

        bool insideEnum = false;
        foreach (string line in lines)
        {
            string trimmed = line.Trim();

            // Find enum name
            if (trimmed.StartsWith("public enum") || trimmed.StartsWith("enum"))
            {
                Match m = Regex.Match(trimmed, @"enum\s+(\w+)");
                if (m.Success)
                    enumName = m.Groups[1].Value;
                insideEnum = true;
                continue;
            }

            if (insideEnum)
            {
                if (trimmed == "}")
                    break;

                // Parse enum entries
                if (!string.IsNullOrEmpty(trimmed))
                {
                    string cleaned = trimmed.TrimEnd(',').Trim();
                    if (!string.IsNullOrWhiteSpace(cleaned))
                        enumEntries.Add(cleaned);
                }
            }
        }

        Debug.Log("Enum loaded: " + enumName);
    }

    private void SaveEditedEnum()
    {
        if (string.IsNullOrEmpty(loadedFilePath))
        {
            Debug.LogError("No file loaded.");
            return;
        }

        using (StreamWriter writer = new StreamWriter(loadedFilePath))
        {
            writer.WriteLine("public enum " + enumName);
            writer.WriteLine("{");
            for (int i = 0; i < enumEntries.Count; i++)
            {
                writer.WriteLine("    " + enumEntries[i] + (i < enumEntries.Count - 1 ? "," : ""));
            }
            writer.WriteLine("}");
        }

        AssetDatabase.Refresh();
        Debug.Log("Enum saved to: " + loadedFilePath);
        loadedFilePath = null;
    }
}
