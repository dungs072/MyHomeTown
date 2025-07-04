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
    private Vector2 dragOffset;
    private Rect headerRect;

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
        Undo.undoRedoPerformed += OnUndoRedoPerformed;
    }
    void OnDisable()
    {
        Selection.selectionChanged -= OnSelectionChange;
        Undo.undoRedoPerformed -= OnUndoRedoPerformed;
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
    private void OnUndoRedoPerformed()
    {
        if (selectedTask != null)
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
    #region Start
    private void Start()
    {
        RegisterEvents();
    }

    private void RegisterEvents()
    {
        Event e = Event.current;
        Vector2 mousePositionOnWindow = e.mousePosition;
        Vector2 currentMousePosition = mousePositionOnWindow + scrollPos;
        if (e.type == EventType.MouseDown && e.button == 0)
        {
            if (IsMouseOverHeader(currentMousePosition)) return;
            selectedNode = GetNodeAtPosition(currentMousePosition);
            if (selectedNode != null)
            {
                dragOffset = currentMousePosition - selectedNode.Position;
                GUI.changed = true;
            }
            else
            {
                dragOffset = mousePositionOnWindow + scrollPos;
            }
        }

        var isDragByMouseLeft = e.type == EventType.MouseDrag && e.button == 0;
        if (isDragByMouseLeft && selectedNode != null)
        {
            selectedNode.Position = currentMousePosition - dragOffset;
            GUI.changed = true;
            EditorUtility.SetDirty(selectedTask);
        }

        if (isDragByMouseLeft && selectedNode == null)
        {
            scrollPos = dragOffset - mousePositionOnWindow;
            GUI.changed = true;
        }
    }
    #endregion

    #region Draw
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
        headerRect = new Rect(0, 0, position.width, 30);
        var headerStyle = new GUIStyle(GUI.skin.box)
        {
            alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Bold,
            fontSize = 16,
        };
        GUI.Box(headerRect, "Task Graph Header", headerStyle);

        GUILayout.BeginArea(headerRect);
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Add Step", GUILayout.Width(100), GUILayout.Height(30)))
        {
            HandleAddStep();
        }

        if (selectedNode != null && GUILayout.Button("Delete step", GUILayout.Width(100), GUILayout.Height(30)))
        {
            HandleRemoveStep();
        }

        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }
    private void HandleAddStep()
    {
        Undo.RecordObject(selectedTask, "Undo Add Step Data");
        var newStepData = new StepData()
        {
            StepName = "New Step",
        };
        var position = new Vector2(100 + DEFAULT_OFFSET.x * selectedTask.Steps.Count, 100);
        var newNode = new StepDataNode(newStepData, position);

        selectedTask.Steps.Add(newStepData);
        stepNodes.Add(newNode);
    }
    private void HandleRemoveStep()
    {
        Undo.RecordObject(selectedTask, "Undo Delete Step Data");
        selectedTask.Steps.Remove(selectedNode.Data);
        stepNodes.Remove(selectedNode);
        selectedNode = null;
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
        Undo.RecordObject(selectedTask, "Undo Edit Step Data");

        // Step Name (Editable)

        EditorGUI.BeginChangeCheck();
        var stepName = EditorGUI.TextField(textFieldRect, stepData.StepName);
        // Description (Editable)

        var descriptionRect = new Rect(nodeRect.x + 10, nodeRect.y + 60, nodeRect.width - 20, 40);
        var description = EditorGUI.TextArea(descriptionRect, stepData.Description ?? "Description");
        // Duration (Editable)

        var durationRect = new Rect(nodeRect.x + 10, nodeRect.y + 110, nodeRect.width - 20, 18);
        var duration = EditorGUI.FloatField(durationRect, "Duration(s)", stepData.Duration);
        // WorkContainerType (Editable enum popup)

        var workContainerRect = new Rect(nodeRect.x + 10, nodeRect.y + 130, nodeRect.width - 20, 18);
        var workContainerType = (WorkContainerType)EditorGUI.EnumPopup(workContainerRect, "Container", stepData.WorkContainerType);

        if (EditorGUI.EndChangeCheck())
        {
            stepData.StepName = stepName;
            stepData.Description = description;
            stepData.Duration = duration;
            stepData.WorkContainerType = workContainerType;
        }
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
    #endregion

    #region End
    private void End()
    {
        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(selectedTask);
        }
    }
    #endregion



    #region Utility Methods

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
    private bool IsMouseOverHeader(Vector2 mousePosition)
    {
        return mousePosition.y <= headerRect.yMax && mousePosition.x <= headerRect.xMax;
    }
    #endregion
}
