using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class TaskGeneratorWindow : EditorWindow
{
    private TaskData currentGraph;
    private Vector2 drag;
    private Vector2 offset = new Vector2(250, 150);
    private StepData selectedStep;
    private StepData editingStep;

    [MenuItem("Tools/Task Graph")]
    public static void OpenWindow()
    {
        TaskGeneratorWindow window = GetWindow<TaskGeneratorWindow>();
        window.titleContent = new GUIContent("Task Graph");
    }

    private void OnGUI()
    {
        ProcessEvents(Event.current);

        if (currentGraph != null)
        {
            DrawNodes();
            DrawConnections();
        }

        if (GUI.Button(new Rect(10, 10, 100, 30), "Load Graph"))
        {
            currentGraph = Selection.activeObject as TaskData;
            if (currentGraph != null)
            {
                AutoLayoutSteps();
            }
        }

        if (GUI.Button(new Rect(120, 10, 100, 30), "Add Node"))
        {
            if (currentGraph != null)
            {
                var newNode = new StepData()
                {
                    StepName = "New Step",
                    Position = new Vector2(100 + offset.x * currentGraph.Steps.Count, 100)
                };
                currentGraph.Steps.Add(newNode);
                EditorUtility.SetDirty(currentGraph);
            }
        }

        if (selectedStep != null && GUI.Button(new Rect(230, 10, 100, 30), "Delete Node"))
        {
            currentGraph.Steps.Remove(selectedStep);
            selectedStep = null;
            EditorUtility.SetDirty(currentGraph);
        }

        if (editingStep != null)
        {
            GUILayout.BeginArea(new Rect(10, 50, 200, 100), EditorStyles.helpBox);
            GUILayout.Label("Edit Step:");
            editingStep.StepName = EditorGUILayout.TextField("Name:", editingStep.StepName);
            if (GUILayout.Button("Done"))
            {
                editingStep = null;
                EditorUtility.SetDirty(currentGraph);
            }
            GUILayout.EndArea();
        }
    }

    private void DrawNodes()
    {
        foreach (var step in currentGraph.Steps)
        {
            if (step == null) continue;

            Rect nodeRect = new(step.Position, new Vector2(200, 100));
            GUIStyle style = new GUIStyle(GUI.skin.box);
            if (step == selectedStep)
                style.normal.background = MakeTex(1, 1, Color.cyan);

            GUI.Box(nodeRect, step.StepName, style);

            if (nodeRect.Contains(Event.current.mousePosition))
            {
                if (Event.current.type == EventType.MouseDown)
                {
                    if (Event.current.button == 0)
                    {
                        selectedStep = step;
                        if (Event.current.clickCount == 2)
                        {
                            editingStep = step;
                        }
                        GUI.changed = true;
                        Event.current.Use();
                    }
                }
            }

            // Drag node
            if (Event.current.type == EventType.MouseDrag && Event.current.button == 0 && step == selectedStep)
            {
                step.Position += Event.current.delta;
                GUI.changed = true;
                Event.current.Use();
                EditorUtility.SetDirty(currentGraph);
            }
        }
    }

    private void DrawConnections()
    {
        for (int i = 0; i < currentGraph.Steps.Count - 1; i++)
        {
            var currentStep = currentGraph.Steps[i];
            var nextStep = currentGraph.Steps[i + 1];
            if (currentStep == null || nextStep == null) continue;

            Vector3 startPos = currentStep.Position + new Vector2(200, 50);
            Vector3 endPos = nextStep.Position + new Vector2(0, 50);

            Handles.DrawBezier(startPos, endPos,
                startPos + Vector3.right * 50,
                endPos + Vector3.left * 50,
                Color.white, null, 4f);
        }
    }

    private void ProcessEvents(Event e)
    {
        drag = Vector2.zero;

        if (e.type == EventType.MouseDrag && e.button == 2)
        {
            drag = e.delta;
            if (currentGraph != null)
            {
                foreach (var node in currentGraph.Steps)
                {
                    node.Position += drag;
                }
                GUI.changed = true;
            }
        }
    }

    private void AutoLayoutSteps()
    {
        for (int i = 0; i < currentGraph.Steps.Count; i++)
        {
            var step = currentGraph.Steps[i];
            if (step == null) continue;

            if (step.Position == Vector2.zero)
            {
                step.Position = new Vector2(100 + offset.x * i, 100);
            }
        }
    }

    private Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; i++) pix[i] = col;

        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }
}
