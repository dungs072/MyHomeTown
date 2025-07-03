using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Unity.Mathematics;
using DashboardContainerElements;
using UnityEditor.Callbacks;

public class TaskGeneratorWindow : EditorWindow
{
    // config the editor here
    private readonly Vector2 NODE_SIZE = new(200, 200);
    private readonly Vector2 DEFAULT_OFFSET = new(250, 150);

    // end of the config 
    private TaskData selectedTask;
    private List<StepDataNode> stepNodes = new();
    private StepDataNode selectedNode;
    private Vector2 scrollPos;
    private bool isDragging = false;
    private Vector2 dragOffset;

    [MenuItem("Tools/Task Graph")]
    public static void OpenWindow()
    {
        TaskGeneratorWindow window = GetWindow<TaskGeneratorWindow>();
        var taskData = Selection.activeObject as TaskData;
        window.titleContent = new GUIContent("Task Graph");
        window.selectedTask = taskData;
        window.titleContent = new GUIContent("Task Graph");
        window.CreateStepNodes();

    }

    [OnOpenAsset(1)]
    public static bool OnOpenTaskAsset(int instanceID, int line)
    {
        TaskData taskData = EditorUtility.InstanceIDToObject(instanceID) as TaskData;
        if (taskData != null)
        {
            OpenWindow();

            return true;
        }
        return false;
    }
    private void OnEnable()
    {
        Selection.selectionChanged += OnSelectionChange;
    }
    private void OnSelectionChange()
    {
        selectedTask = Selection.activeObject as TaskData;
        if (selectedTask == null)
        {
            Debug.LogWarning("No TaskData selected. Please select a TaskData asset.");
        }
        else
        {
            CreateStepNodes();
            Repaint();
        }
    }
    private void CreateStepNodes()
    {
        stepNodes.Clear();
        int i = 0;
        foreach (var step in selectedTask.Steps)
        {
            if (step == null) continue;
            var position = new Vector2(100 + DEFAULT_OFFSET.x * i, 100);
            var node = new StepDataNode(step, position);
            stepNodes.Add(node);
            i++;
        }
    }

    private void OnGUI()
    {
        if (selectedTask == null)
        {
            EditorGUILayout.LabelField("Please select a TaskData asset in the Project window.");
            return;
        }
        Start();

        Draw();

        End();

    }
    //! handle start here
    private void Start()
    {
        RegisterEvents();
    }

    private void RegisterEvents()
    {
        Event e = Event.current;
        Vector2 mousePosition = e.mousePosition;

        if (e.type == EventType.MouseDown && e.button == 0)
        {
            selectedNode = GetNodeAtPosition(mousePosition);
            if (selectedNode != null)
            {
                isDragging = true;
                dragOffset = mousePosition - selectedNode.Position;
                GUI.changed = true;
            }
            else
            {
                selectedNode = null;
            }
        }

        // if (e.type == EventType.MouseUp && e.button == 0)
        // {
        //     isDragging = false;
        // }

        // if (e.type == EventType.MouseDrag && e.button == 0 && isDragging && selectedNode != null)
        // {
        //     selectedNode.Position = mousePosition - dragOffset;
        //     GUI.changed = true;
        //     e.Use();
        //     EditorUtility.SetDirty(selectedTask);
        // }
    }
    //! handle middle here
    private void Draw()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        GUILayoutUtility.GetRect(4000, 4000);
        DrawHeaderUIButtons();
        DrawNodes();
        DrawConnections();
        EditorGUILayout.EndScrollView();
    }
    private void DrawHeaderUIButtons()
    {
        if (GUI.Button(new Rect(10, 10, 100, 30), "Add Step"))
        {
            var newStepData = new StepData()
            {
                StepName = "New Step",
            };
            var position = new Vector2(100 + DEFAULT_OFFSET.x * selectedTask.Steps.Count, 100);
            var rect = new Rect(position, NODE_SIZE);
            var newNode = new StepDataNode(newStepData, position);

            selectedTask.Steps.Add(newStepData);
            stepNodes.Add(newNode);
            EditorUtility.SetDirty(selectedTask);
        }

        if (selectedNode != null && GUI.Button(new Rect(120, 10, 100, 30), "Delete Step"))
        {
            var currentPosition = Event.current.mousePosition + scrollPos;
            var selectedNode = GetNodeAtPosition(currentPosition);
            if (selectedNode == null) return;
            var stepData = selectedNode.Data;
            selectedTask.Steps.Remove(stepData);
            stepNodes.Remove(selectedNode);
            EditorUtility.SetDirty(selectedTask);
        }
    }

    private void DrawNodes()
    {
        foreach (var node in stepNodes)
        {
            DrawNode(node);
        }
    }
    private void DrawNode(StepDataNode node)
    {
        if (node == null) return;
        Rect nodeRect = new Rect(node.Position, NODE_SIZE);
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

    private void DrawConnections()
    {
        for (int i = 0; i < stepNodes.Count - 1; i++)
        {
            var currentNode = stepNodes[i];
            var nextNode = stepNodes[i + 1];
            if (currentNode == null || nextNode == null) continue;

            Vector3 startPos = currentNode.Position + new Vector2(200, 50);
            Vector3 endPos = nextNode.Position + new Vector2(0, 50);

            Handles.DrawBezier(startPos, endPos,
                startPos + Vector3.right * 50,
                endPos + Vector3.left * 50,
                Color.white, null, 4f);
        }
    }
    //! handle end here
    private void End()
    {
        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(selectedTask);
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
            if (IsMouseOverNode(node, position))
            {
                return node;
            }
        }
        return null;
    }
    private bool IsMouseOverNode(StepDataNode node, Vector2 mousePosition)
    {
        return node != null &&
               mousePosition.x >= node.Position.x &&
               mousePosition.x <= node.Position.x + NODE_SIZE.x &&
               mousePosition.y >= node.Position.y &&
               mousePosition.y <= node.Position.y + NODE_SIZE.y;

    }
}
