using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Unity.Mathematics;
using DashboardContainerElements;
using UnityEditor.Callbacks;
using System.Linq;


public class TaskGeneratorWindow : EditorWindow
{
    // --- Config ---
    private static readonly Vector2 NODE_SIZE = new(200, 250);
    private static readonly Vector2 DEFAULT_OFFSET = new(250, 150);
    private static readonly Vector2 CANVAS_SIZE = new(4000, 4000);

    // --- State ---
    private TaskData selectedTask;
    private StepData selectedNode;
    private Vector2 scrollPos;
    private Vector2 dragOffset;
    private Rect headerRect;
    private StepData linkingParentNode;

    // --- Zoom ---
    private float zoomScale = 1f;
    private const float ZOOM_MIN = 0.2f;
    private const float ZOOM_MAX = 2.0f;

    #region Unity Methods
    [MenuItem("Tools/Task Graph")]
    public static void OpenWindow()
    {
        var window = GetWindow<TaskGeneratorWindow>();
        window.titleContent = new GUIContent("Task Graph");
        window.selectedTask = Selection.activeObject as TaskData;
        window.SetUpNodePositions();
    }

    [OnOpenAsset(1)]
    public static bool OnOpenTaskAsset(int instanceID, int line)
    {
        var taskData = EditorUtility.InstanceIDToObject(instanceID) as TaskData;
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

    private void OnDisable()
    {
        Selection.selectionChanged -= OnSelectionChange;
        Undo.undoRedoPerformed -= OnUndoRedoPerformed;
    }

    private void OnSelectionChange()
    {
        selectedTask = Selection.activeObject as TaskData;
        if (selectedTask != null)
            Repaint();
    }

    private void OnUndoRedoPerformed()
    {
        if (selectedTask != null)
            Repaint();
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
    #endregion

    #region Event Handling
    private void Start()
    {
        RegisterEvents();
        HandleZoom();
    }

    private void RegisterEvents()
    {
        var e = Event.current;
        Vector2 mousePosWindow = e.mousePosition;
        Vector2 mousePosCanvas = (mousePosWindow + scrollPos) / zoomScale;

        if (e.type == EventType.MouseDown && e.button == 0)
        {
            selectedNode = GetNodeAtPosition(mousePosCanvas);
            if (selectedNode != null)
            {
                dragOffset = mousePosCanvas - selectedNode.position;
                GUI.changed = true;
            }
            else
            {
                linkingParentNode = null;
                dragOffset = mousePosWindow + scrollPos;
            }
        }

        bool isDragging = e.type == EventType.MouseDrag && e.button == 0;
        if (isDragging && selectedNode != null)
        {
            selectedNode.position = mousePosCanvas - dragOffset;
            GUI.changed = true;
            EditorUtility.SetDirty(selectedTask);
        }
        else if (isDragging && selectedNode == null)
        {
            scrollPos = dragOffset - mousePosWindow;
            GUI.changed = true;
        }
    }

    private void HandleZoom()
    {
        var e = Event.current;
        if (e.type != EventType.ScrollWheel) return;

        float oldZoom = zoomScale;
        float zoomDelta = -e.delta.y * 0.01f;
        zoomScale = Mathf.Clamp(zoomScale + zoomDelta, ZOOM_MIN, ZOOM_MAX);

        Vector2 zoomCenter = e.mousePosition;
        Vector2 zoomPos = (zoomCenter + scrollPos) / oldZoom;
        scrollPos = zoomPos * zoomScale - zoomCenter;

        e.Use();
        Repaint();
    }
    #endregion

    #region Node Positioning
    private void SetUpNodePositions()
    {
        for (int i = 0; i < selectedTask.Steps.Count; i++)
        {
            var stepNode = selectedTask.Steps[i];
            var children = selectedTask.StepsDictionary[stepNode];
            if (children == null || children.Count == 0) continue;
            for (int j = 0; j < children.Count; j++)
            {
                var childNode = children[j];
                if (childNode == null) continue;
                if (childNode.position == Vector2.zero)
                    childNode.position = stepNode.position + new Vector2(DEFAULT_OFFSET.x * (j + 1), DEFAULT_OFFSET.y);
            }
        }
    }
    #endregion

    #region Drawing
    private void Draw()
    {
        var zoomArea = new Rect(0, 0, position.width, position.height);
        scrollPos = GUI.BeginScrollView(zoomArea, scrollPos, new Rect(0, 0, CANVAS_SIZE.x * zoomScale, CANVAS_SIZE.y * zoomScale));
        Matrix4x4 oldMatrix = GUI.matrix;
        GUIUtility.ScaleAroundPivot(Vector2.one * zoomScale, Vector2.zero);
        var viewSize = CANVAS_SIZE * zoomScale;
        GUILayout.BeginArea(new Rect(0, 0, viewSize.x, viewSize.y));
        DrawNodes();
        DrawConnections();
        GUILayout.EndArea();
        GUI.matrix = oldMatrix;
        GUI.EndScrollView();
        DrawHeaderUIButtons();
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
        var displayHeaderName = selectedTask != null ? selectedTask.TaskName : "No Task Selected";
        GUI.Box(headerRect, $"Task: {displayHeaderName}", headerStyle);

        GUILayout.BeginArea(headerRect);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Step", GUILayout.Width(100), GUILayout.Height(30)))
            HandleAddStep();
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }

    private void HandleAddStep()
    {
        Undo.RecordObject(selectedTask, "Undo Add Step Data");
        selectedTask.CreateStep(null, Vector2.zero);
    }

    private void DrawNodes()
    {
        foreach (var node in selectedTask.Steps.ToList())
            DrawNode(node);
    }

    private void DrawNode(StepData node)
    {
        if (node == null) return;
        float baseNodeHeight = node.isMinimized ? 40f : 250f;
        float itemFieldHeight = 20f;
        float itemFieldPadding = 2f;
        float extraHeightPerItem = itemFieldHeight + itemFieldPadding;
        int totalItemCount = node.NeedItems.Count + node.PossibleCreateItems.Count;
        float extraHeight = node.isMinimized ? 0f : totalItemCount * extraHeightPerItem + 80f;
        float totalHeight = baseNodeHeight + extraHeight;
        Rect nodeRect = new Rect(node.position, new Vector2(NODE_SIZE.x, totalHeight));
        GUIStyle style = new GUIStyle(GUI.skin.box);
        if (node == selectedNode)
        {
            style.normal.background = MakeTex(1, 1, Color.cyan);
            style.fontStyle = FontStyle.Bold;
        }
        GUI.Box(nodeRect, "", style);

        // Minimize/Maximize button
        var toggleRect = new Rect(nodeRect.x + nodeRect.width - 30, nodeRect.y + 5, 25, 20);
        if (GUI.Button(toggleRect, node.isMinimized ? "+" : "-"))
        {
            node.isMinimized = !node.isMinimized;
            GUI.changed = true;
        }

        // Draw header (always)
        var textFieldRect = new Rect(nodeRect.x + 10, nodeRect.y + 10, nodeRect.width - 50, 20);
        node.StepName = EditorGUI.TextField(textFieldRect, node.StepName);

        // Only draw details if not minimized
        if (!node.isMinimized)
        {
            DrawNodeFields(node, nodeRect);
            DrawNeedItems(node, nodeRect);
            DrawPossibleCreateItems(node, nodeRect);
            DrawFunctions(node, nodeRect);
        }
    }
    private void DrawNodeFields(StepData node, Rect nodeRect)
    {
        float fieldWidth = nodeRect.width - 20;
        //float textFieldWidth = fieldWidth;
        // var textFieldRect = new Rect(nodeRect.x + (nodeRect.width - fieldWidth) / 2, nodeRect.y + 30, textFieldWidth, 20);
        Undo.RecordObject(selectedTask, "Undo Edit Step Data");
        EditorGUI.BeginChangeCheck();
        //var stepName = EditorGUI.TextField(textFieldRect, node.StepName);
        var descriptionRect = new Rect(nodeRect.x + 10, nodeRect.y + 60, nodeRect.width - 20, 40);
        var description = EditorGUI.TextArea(descriptionRect, node.Description ?? "Description");
        var durationRect = new Rect(nodeRect.x + 10, nodeRect.y + 110, nodeRect.width - 20, 18);
        var duration = EditorGUI.FloatField(durationRect, "Duration(s)", node.Duration);
        var workContainerRect = new Rect(nodeRect.x + 10, nodeRect.y + 130, nodeRect.width - 20, 18);
        var workContainerType = (WorkContainerType)EditorGUI.EnumPopup(workContainerRect, "Container", node.WorkContainerType);
        var taskNameRect = new Rect(nodeRect.x + 10, nodeRect.y + 150, nodeRect.width - 20, 18);
        var taskName = (TaskName)EditorGUI.EnumPopup(taskNameRect, "Task", node.TaskName);
        var needPermissionRect = new Rect(nodeRect.x + 10, nodeRect.y + 170, nodeRect.width - 20, 18);
        var needPermissionToGiveItems = EditorGUI.Toggle(needPermissionRect, "Need Permission to Give Items", node.NeedPermissionToGiveItems);
        if (EditorGUI.EndChangeCheck())
        {
            //node.StepName = stepName;
            node.Description = description;
            node.Duration = duration;
            node.WorkContainerType = workContainerType;
            node.TaskName = taskName;
            node.NeedPermissionToGiveItems = needPermissionToGiveItems;
        }
    }

    private void DrawNeedItems(StepData node, Rect nodeRect)
    {
        float itemFieldHeight = 20f;
        float itemFieldPadding = 2f;
        float itemListStartY = nodeRect.y + 190;
        for (int i = 0; i < node.NeedItems.Count; i++)
        {
            var item = node.NeedItems[i];
            float itemY = itemListStartY + i * (itemFieldHeight + itemFieldPadding);
            var keyRect = new Rect(nodeRect.x + 10, itemY, nodeRect.width / 2 - 20, itemFieldHeight);
            var newKey = (ItemKey)EditorGUI.EnumPopup(keyRect, item.itemKey);
            var amountRect = new Rect(nodeRect.x + nodeRect.width / 2, itemY, nodeRect.width / 2 - 30, itemFieldHeight);
            var newAmount = EditorGUI.IntField(amountRect, item.amount);
            var deleteRect = new Rect(nodeRect.x + nodeRect.width - 20, itemY, 16, itemFieldHeight);
            if (GUI.Button(deleteRect, "X"))
            {
                Undo.RecordObject(selectedTask, "Remove Need Item");
                node.NeedItems.RemoveAt(i);
                EditorUtility.SetDirty(selectedTask);
                break;
            }
            if (newKey != item.itemKey || newAmount != item.amount)
            {
                Undo.RecordObject(selectedTask, "Edit Need Item");
                item.itemKey = newKey;
                item.amount = newAmount;
                EditorUtility.SetDirty(selectedTask);
            }
        }
        float afterNeedItemsY = itemListStartY + node.NeedItems.Count * (itemFieldHeight + itemFieldPadding);
        var addButtonRect = new Rect(nodeRect.x + 10, afterNeedItemsY, nodeRect.width - 20, itemFieldHeight);
        if (GUI.Button(addButtonRect, "Add Need Item"))
        {
            Undo.RecordObject(selectedTask, "Add Need Item");
            node.NeedItems.Add(new ItemRequirement());
            EditorUtility.SetDirty(selectedTask);
        }
    }

    private void DrawPossibleCreateItems(StepData node, Rect nodeRect)
    {
        float itemFieldHeight = 20f;
        float itemFieldPadding = 2f;
        float itemListStartY = nodeRect.y + 190 + node.NeedItems.Count * (itemFieldHeight + itemFieldPadding) + itemFieldHeight + 5f;
        for (int i = 0; i < node.PossibleCreateItems.Count; i++)
        {
            var item = node.PossibleCreateItems[i];
            float itemY = itemListStartY + i * (itemFieldHeight + itemFieldPadding);
            var keyRect = new Rect(nodeRect.x + 10, itemY, nodeRect.width / 2 - 20, itemFieldHeight);
            var newKey = (ItemKey)EditorGUI.EnumPopup(keyRect, item.itemKey);
            var amountRect = new Rect(nodeRect.x + nodeRect.width / 2, itemY, nodeRect.width / 2 - 30, itemFieldHeight);
            var newAmount = EditorGUI.IntField(amountRect, item.amount);
            var deleteRect = new Rect(nodeRect.x + nodeRect.width - 20, itemY, 16, itemFieldHeight);
            if (GUI.Button(deleteRect, "X"))
            {
                Undo.RecordObject(selectedTask, "Remove Possible Create Item");
                node.PossibleCreateItems.RemoveAt(i);
                EditorUtility.SetDirty(selectedTask);
                break;
            }
            if (newKey != item.itemKey || newAmount != item.amount)
            {
                Undo.RecordObject(selectedTask, "Edit Possible Create Item");
                item.itemKey = newKey;
                item.amount = newAmount;
                EditorUtility.SetDirty(selectedTask);
            }
        }
        float addPossibleButtonY = itemListStartY + node.PossibleCreateItems.Count * (itemFieldHeight + itemFieldPadding);
        var addPossibleButtonRect = new Rect(nodeRect.x + 10, addPossibleButtonY, nodeRect.width - 20, itemFieldHeight);
        if (GUI.Button(addPossibleButtonRect, "Add Possible Create Item"))
        {
            Undo.RecordObject(selectedTask, "Add Possible Create Item");
            node.PossibleCreateItems.Add(new ItemRequirement());
            EditorUtility.SetDirty(selectedTask);
        }
    }

    private void DrawFunctions(StepData node, Rect nodeRect)
    {
        float itemFieldHeight = 20f;
        float itemFieldPadding = 2f;
        float startY = nodeRect.y + 220;
        startY += node.NeedItems.Count * (itemFieldHeight + itemFieldPadding) + 22f;
        startY += node.PossibleCreateItems.Count * (itemFieldHeight + itemFieldPadding) + 22f;
        DrawLinkFunction(node, nodeRect, startY);
        DrawCreateChildFunction(node, nodeRect, startY + 20f);
        DrawDeleteFunction(node, nodeRect, startY + 40f);
    }

    private void DrawLinkFunction(StepData node, Rect nodeRect, float y)
    {
        var linkButtonRect = new Rect(nodeRect.x + 10, y, nodeRect.width - 20, 18);
        if (linkingParentNode == null)
        {
            if (GUI.Button(linkButtonRect, "Link"))
                linkingParentNode = node;
        }
        else if (linkingParentNode == node)
        {
            if (GUI.Button(linkButtonRect, "Cancel link"))
                linkingParentNode = null;
        }
        else if (linkingParentNode.IsChildExist(node.UniqueID))
        {
            if (GUI.Button(linkButtonRect, "Unlink"))
            {
                Undo.RecordObject(selectedTask, "Undo Unlink Step Data");
                selectedTask.UnlinkStep(linkingParentNode, node);
                linkingParentNode = null;
            }
        }
        else if (GUI.Button(linkButtonRect, "Child"))
        {
            Undo.RecordObject(selectedTask, "Undo Link Step Data");
            selectedTask.LinkStep(linkingParentNode, node);
            linkingParentNode = null;
        }
    }

    private void DrawCreateChildFunction(StepData node, Rect nodeRect, float y)
    {
        var createChildButtonRect = new Rect(nodeRect.x + 10, y, nodeRect.width - 20, 18);
        if (GUI.Button(createChildButtonRect, "Create Child"))
        {
            Undo.RecordObject(selectedTask, "Undo Create Child Step Data");
            var position = node.position + DEFAULT_OFFSET;
            selectedTask.CreateStep(node, position);
            GUI.changed = true;
        }
    }

    private void DrawDeleteFunction(StepData node, Rect nodeRect, float y)
    {
        var deleteButtonRect = new Rect(nodeRect.x + 10, y, nodeRect.width - 20, 18);
        if (GUI.Button(deleteButtonRect, "Delete Step"))
        {
            Undo.RecordObject(selectedTask, "Undo Delete Step Data");
            selectedTask.RemoveStep(node);
            selectedNode = null;
            EditorUtility.SetDirty(selectedTask);
            Repaint();
        }
    }

    private void DrawConnections()
    {
        var stepNodes = selectedTask.Steps;
        for (int i = 0; i < stepNodes.Count; i++)
        {
            var currentNode = stepNodes[i];
            var children = selectedTask.StepsDictionary[currentNode];
            if (children == null || children.Count == 0) continue;
            foreach (var child in children)
                DrawConnection(currentNode, child);
        }
    }

    private void DrawConnection(StepData fromNode, StepData toNode)
    {
        if (fromNode == null || toNode == null) return;
        float x = NODE_SIZE.x;
        float y = fromNode.isMinimized ? 40f : NODE_SIZE.y;
        float yTo = toNode.isMinimized ? 40f : NODE_SIZE.y;
        Vector3 startPos = fromNode.position + new Vector2(x, y / 2);
        Vector3 endPos = toNode.position + new Vector2(0, yTo / 2);
        Handles.DrawBezier(startPos, endPos,
            startPos + Vector3.right * 50,
            endPos + Vector3.left * 50,
            Color.white, null, 4f);
    }
    #endregion

    #region End
    private void End()
    {
        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(selectedTask);
    }
    #endregion



    #region Utility Methods
    private Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = Enumerable.Repeat(col, width * height).ToArray();
        var result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }

    private StepData GetNodeAtPosition(Vector2 position)
    {
        return selectedTask.Steps.FirstOrDefault(node => IsMouseOverNode(node, position));
    }

    private bool IsMouseOverNode(StepData node, Vector2 mousePosition)
    {
        float baseNodeHeight = node.isMinimized ? 40f : 250f;
        float extraHeight = node.isMinimized ? 0f : node.NeedItems.Count * 22f + 80f;
        float dynamicHeight = baseNodeHeight + extraHeight;
        return node != null &&
               mousePosition.x >= node.position.x &&
               mousePosition.x <= node.position.x + NODE_SIZE.x &&
               mousePosition.y >= node.position.y &&
               mousePosition.y <= node.position.y + dynamicHeight;
    }
    #endregion
}
