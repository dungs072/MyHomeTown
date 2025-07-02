using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Unity.Mathematics;
using DashboardContainerElements;

public class TaskGeneratorWindow : EditorWindow
{
    // config the editor here
    private readonly Vector2 NODE_SIZE = new Vector2(200, 200);

    // end of the config 
    private TaskData currentGraph;
    private List<StepDataNode> stepNodes = new List<StepDataNode>();

    private Vector2 offset = new Vector2(250, 150);
    private StepDataNode selectedNode;
    private Vector2 scrollPos;

    [MenuItem("Tools/Task Graph")]
    public static void OpenWindow()
    {
        TaskGeneratorWindow window = GetWindow<TaskGeneratorWindow>();
        window.titleContent = new GUIContent("Task Graph");
    }

    private void OnGUI()
    {

        var result = TryToGetSelectedTaskData();
        if (!result) return;

        Start();

        Middle();

        End();

    }
    private bool TryToGetSelectedTaskData()
    {
        currentGraph = Selection.activeObject as TaskData;
        if (currentGraph == null)
        {
            GUILayout.Label("No TaskData selected. Please select a TaskData asset.");
            return false;
        }
        return true;
    }
    //! handle start here
    private void Start()
    {
        InitHeaderUIButtons();
        CreateStepNodes();
        RegisterEvents();
    }
    private void CreateStepNodes()
    {
        stepNodes.Clear();
        int i = 0;
        foreach (var step in currentGraph.Steps)
        {
            if (step == null) continue;
            var position = new Vector2(100 + offset.x * i, 100);
            //? edit the size of the node here
            var nodeRect = new Rect(position, NODE_SIZE);
            var node = new StepDataNode(step, nodeRect);
            stepNodes.Add(node);
            i++;
        }
    }
    private void RegisterEvents()
    {
        // deselect step when clicking outside
        if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
        {

            Vector2 mousePosition = Event.current.mousePosition;
            selectedNode = GetNodeAtPosition(mousePosition);
            if (selectedNode == null)
            {
                GUI.changed = true;

            }

        }
    }
    //! handle middle here
    private void Middle()
    {

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        GUILayoutUtility.GetRect(4000, 4000);

        if (currentGraph)
        {
            DrawNodes();
            DrawConnections();

        }

        EditorGUILayout.EndScrollView();
    }
    private void InitHeaderUIButtons()
    {
        if (GUI.Button(new Rect(10, 10, 100, 30), "Add Step"))
        {
            var newStepData = new StepData()
            {
                StepName = "New Step",
            };
            var position = new Vector2(100 + offset.x * currentGraph.Steps.Count, 100);
            var rect = new Rect(position, NODE_SIZE);
            var newNode = new StepDataNode(newStepData, rect);

            currentGraph.Steps.Add(newStepData);
            stepNodes.Add(newNode);
            EditorUtility.SetDirty(currentGraph);
        }

        if (selectedNode != null && GUI.Button(new Rect(120, 10, 100, 30), "Delete Step"))
        {
            var currentPosition = Event.current.mousePosition + scrollPos;
            var selectedNode = GetNodeAtPosition(currentPosition);
            if (selectedNode == null) return;
            var stepData = selectedNode.Data;
            currentGraph.Steps.Remove(stepData);
            stepNodes.Remove(selectedNode);
            EditorUtility.SetDirty(currentGraph);
        }
    }
    //! handle end here
    private void End()
    {

    }
    private void DrawNodes()
    {
        foreach (var node in stepNodes)
        {

            DrawNode(node);
            RegisterNodeClickEvents(node);
        }
    }
    private void DrawNode(StepDataNode node)
    {
        if (node == null) return;
        Rect nodeRect = node.NodeRect;
        GUIStyle style = new GUIStyle(GUI.skin.box);
        if (node == selectedNode)
        {
            style.normal.background = MakeTex(1, 1, Color.cyan);
            style.fontStyle = FontStyle.Bold;
        }

        GUI.Box(nodeRect, "", style);
        // Draw the step name
        float fieldWidth = nodeRect.width - 20;
        float textFieldWidth = fieldWidth;

        var textFieldRect = new Rect(
            nodeRect.x + (nodeRect.width - fieldWidth) / 2,
            nodeRect.y + 30,
            textFieldWidth,
            20
        );
        var stepData = node.Data;
        stepData.StepName = EditorGUI.TextField(textFieldRect, stepData.StepName);

        // Description (Editable)
        var descriptionRect = new Rect(nodeRect.x + 10, nodeRect.y + 60, nodeRect.width - 20, 40);
        stepData.Description = EditorGUI.TextArea(descriptionRect, stepData.Description ?? "Description");

        // Duration (Editable)
        var durationRect = new Rect(nodeRect.x + 10, nodeRect.y + 110, nodeRect.width - 20, 18);
        stepData.Duration = EditorGUI.FloatField(durationRect, "Duration(s)", stepData.Duration);

        // WorkContainerType (Editable enum popup)
        var workContainerRect = new Rect(nodeRect.x + 10, nodeRect.y + 130, nodeRect.width - 20, 18);
        stepData.WorkContainerType = (WorkContainerType)EditorGUI.EnumPopup(workContainerRect, "Container", stepData.WorkContainerType);

    }
    private void RegisterNodeClickEvents(StepDataNode node)
    {
        // Drag node
        if (Event.current.type == EventType.MouseDrag && Event.current.button == 0 && node == selectedNode)
        {
            var nodeRect = node.NodeRect;
            nodeRect.position += Event.current.delta;
            GUI.changed = true;
            Event.current.Use();
            EditorUtility.SetDirty(currentGraph);
        }

    }

    private void DrawConnections()
    {
        for (int i = 0; i < stepNodes.Count - 1; i++)
        {
            var currentNode = stepNodes[i];
            var nextNode = stepNodes[i + 1];
            if (currentNode == null || nextNode == null) continue;

            Vector3 startPos = currentNode.NodeRect.position + new Vector2(200, 50);
            Vector3 endPos = nextNode.NodeRect.position + new Vector2(0, 50);

            Handles.DrawBezier(startPos, endPos,
                startPos + Vector3.right * 50,
                endPos + Vector3.left * 50,
                Color.white, null, 4f);
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
    private StepDataNode GetNodeAtPosition(Vector2 position)
    {
        foreach (var node in stepNodes)
        {
            if (node.NodeRect.Contains(position))
            {
                return node;
            }
        }
        return null;
    }

}
