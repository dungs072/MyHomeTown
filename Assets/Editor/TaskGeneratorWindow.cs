using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class TaskGeneratorWindow : EditorWindow
{
    private TaskData currentGraph;
    private Vector2 drag;
    private Vector2 offset = new Vector2(250, 150); // Offset spacing between nodes

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
                AutoLayoutSteps(); // Set positions once when loaded
            }
        }
    }

    private void DrawNodes()
    {
        foreach (var step in currentGraph.Steps)
        {
            if (step == null) continue;
            Rect nodeRect = new(step.Position, new Vector2(200, 100));
            GUI.Box(nodeRect, step.StepName);
        }
    }

    private void DrawConnections()
    {
        foreach (var currentStep in currentGraph.Steps)
        {
            if (currentStep == null) continue;

            foreach (var nextStep in currentStep.NextSteps)
            {
                if (nextStep == null) continue;

                Vector3 startPos = currentStep.Position + new Vector2(200, 50);
                Vector3 endPos = nextStep.Position + new Vector2(0, 50);

                Handles.DrawBezier(startPos, endPos,
                    startPos + Vector3.right * 50,
                    endPos + Vector3.left * 50,
                    Color.white, null, 4f);
            }
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

            if (step.Position == Vector2.zero) // only auto-layout if not set
            {
                step.Position = new Vector2(100 + offset.x * i, 100);
            }
        }
    }
}
