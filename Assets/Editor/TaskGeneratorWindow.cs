using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Unity.Mathematics;
using DashboardContainerElements;
using UnityEditor.Callbacks;
using System.Linq;

public class TaskGeneratorWindow : EditorWindow
{
    // config the editor here
    private readonly Vector2 NODE_SIZE = new(200, 250);
    private readonly Vector2 DEFAULT_OFFSET = new(250, 150);
    private readonly Vector2 CANVAS_SIZE = new(4000, 4000);

    // end of the config 
    private TaskData selectedTask;
    private StepData selectedNode;
    private Vector2 scrollPos;
    private Vector2 dragOffset;
    private Rect headerRect;
    // linking steps
    private StepData linkingParentNode = null;

    // zoom
    private float zoomScale = 1f;
    private const float ZOOM_MIN = 0.2f;
    private const float ZOOM_MAX = 2.0f;

    [MenuItem("Tools/Task Graph")]
    public static void OpenWindow()
    {
        TaskGeneratorWindow window = GetWindow<TaskGeneratorWindow>();
        var taskData = Selection.activeObject as TaskData;
        window.titleContent = new GUIContent("Task Graph");
        window.selectedTask = taskData;
        window.SetUpNodePositions();
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
            Repaint();
        }
    }
    private void OnUndoRedoPerformed()
    {
        if (selectedTask != null)
        {
            //CreateStepNodes();
            Repaint();
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
        HandleZoom();
    }

    private void RegisterEvents()
    {
        Event e = Event.current;
        Vector2 mousePositionOnWindow = e.mousePosition;
        Vector2 currentMousePosition = (mousePositionOnWindow + scrollPos) / zoomScale;
        if (e.type == EventType.MouseDown && e.button == 0)
        {
            selectedNode = GetNodeAtPosition(currentMousePosition);
            if (selectedNode != null)
            {
                dragOffset = currentMousePosition - selectedNode.Position;
                GUI.changed = true;
            }
            else
            {
                linkingParentNode = null;
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
    private void HandleZoom()
    {
        var e = Event.current;
        if (e.type == EventType.ScrollWheel)
        {
            float oldZoom = zoomScale;
            float zoomDelta = -e.delta.y * 0.01f;
            zoomScale += zoomDelta;
            zoomScale = Mathf.Clamp(zoomScale, ZOOM_MIN, ZOOM_MAX);

            Vector2 zoomCenter = e.mousePosition;
            Vector2 zoomPos = (zoomCenter + scrollPos) / oldZoom;
            scrollPos = zoomPos * zoomScale - zoomCenter;

            e.Use();
            Repaint();
        }
    }

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
                if (childNode.Position == Vector2.zero)
                {
                    childNode.Position = stepNode.Position + new Vector2(DEFAULT_OFFSET.x * (j + 1), DEFAULT_OFFSET.y);
                }
            }
        }
    }
    #endregion

    #region Draw
    private void Draw()
    {
        var zoomArea = new Rect(0, 0, position.width, position.height);
        scrollPos = GUI.BeginScrollView(zoomArea, scrollPos, new Rect(0, 0, CANVAS_SIZE.x * zoomScale, CANVAS_SIZE.y * zoomScale));
        Matrix4x4 oldMatrix = GUI.matrix;
        // Apply zoom scale matrix
        GUIUtility.ScaleAroundPivot(Vector2.one * zoomScale, Vector2.zero);
        var viewSize = CANVAS_SIZE * zoomScale;
        GUILayout.BeginArea(new Rect(0, 0, viewSize.x, viewSize.y));
        DrawNodes();
        DrawConnections();
        GUILayout.EndArea();
        GUI.matrix = oldMatrix;
        GUI.EndScrollView();

        // header UI
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
        {
            HandleAddStep();
        }

        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }
    private void HandleAddStep()
    {
        Undo.RecordObject(selectedTask, "Undo Add Step Data");
        var position = new Vector2(0, 0);
        selectedTask.CreateStep(null, position);
    }

    private void DrawNodes()
    {
        foreach (var node in selectedTask.Steps.ToList())
        {
            DrawNode(node);
        }
    }
    private void DrawNode(StepData node)
    {
        if (node == null) return;
        float baseNodeHeight = 250f;
        float itemFieldHeight = 20f;
        float itemFieldPadding = 2f;
        float extraHeightPerItem = itemFieldHeight + itemFieldPadding;

        // Include space for each NeedItem + add button + bottom functions
        int totalItemCount = node.NeedItems.Count;
        float extraHeight = totalItemCount * extraHeightPerItem + 80f; // 80 includes add + 3 buttons
        float totalHeight = baseNodeHeight + extraHeight;

        Rect nodeRect = new Rect(node.Position, new Vector2(NODE_SIZE.x, totalHeight));
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
        Undo.RecordObject(selectedTask, "Undo Edit Step Data");

        // Step Name (Editable)

        EditorGUI.BeginChangeCheck();
        var stepName = EditorGUI.TextField(textFieldRect, node.StepName);
        // Description (Editable)

        var descriptionRect = new Rect(nodeRect.x + 10, nodeRect.y + 60, nodeRect.width - 20, 40);
        var description = EditorGUI.TextArea(descriptionRect, node.Description ?? "Description");
        // Duration (Editable)

        var durationRect = new Rect(nodeRect.x + 10, nodeRect.y + 110, nodeRect.width - 20, 18);
        var duration = EditorGUI.FloatField(durationRect, "Duration(s)", node.Duration);
        // WorkContainerType (Editable enum popup)

        var workContainerRect = new Rect(nodeRect.x + 10, nodeRect.y + 130, nodeRect.width - 20, 18);
        var workContainerType = (WorkContainerType)EditorGUI.EnumPopup(workContainerRect, "Container", node.WorkContainerType);

        if (EditorGUI.EndChangeCheck())
        {
            node.StepName = stepName;
            node.Description = description;
            node.Duration = duration;
            node.WorkContainerType = workContainerType;
        }


        float itemListStartY = nodeRect.y + 160;
        // Draw each need item
        for (int i = 0; i < node.NeedItems.Count; i++)
        {
            var item = node.NeedItems[i];
            float itemY = itemListStartY + i * (itemFieldHeight + itemFieldPadding);

            // Item Key Enum
            var keyRect = new Rect(nodeRect.x + 10, itemY, nodeRect.width / 2 - 20, itemFieldHeight);
            var newKey = (NeedItemKey)EditorGUI.EnumPopup(keyRect, item.itemKey);

            // Amount Field
            var amountRect = new Rect(nodeRect.x + nodeRect.width / 2, itemY, nodeRect.width / 2 - 30, itemFieldHeight);
            var newAmount = EditorGUI.IntField(amountRect, item.amount);

            // Delete button
            var deleteRect = new Rect(nodeRect.x + nodeRect.width - 20, itemY, 16, itemFieldHeight);
            if (GUI.Button(deleteRect, "X"))
            {
                Undo.RecordObject(selectedTask, "Remove Need Item");
                node.NeedItems.RemoveAt(i);
                EditorUtility.SetDirty(selectedTask);
                break; // prevent layout issues after modifying the list
            }

            // Apply changes
            if (newKey != item.itemKey || newAmount != item.amount)
            {
                Undo.RecordObject(selectedTask, "Edit Need Item");
                item.itemKey = newKey;
                item.amount = newAmount;
                EditorUtility.SetDirty(selectedTask);
            }
        }

        // Add button below list
        var addButtonY = itemListStartY + node.NeedItems.Count * (itemFieldHeight + itemFieldPadding);
        var addButtonRect = new Rect(nodeRect.x + 10, addButtonY, nodeRect.width - 20, itemFieldHeight);
        if (GUI.Button(addButtonRect, "Add Need Item"))
        {
            Undo.RecordObject(selectedTask, "Add Need Item");
            node.NeedItems.Add(new ItemData());
            EditorUtility.SetDirty(selectedTask);
        }

        // functions
        DrawFunctions(node, nodeRect);
    }
    private void DrawFunctions(StepData node, Rect nodeRect)
    {
        float itemFieldHeight = 20f;
        float itemFieldPadding = 2f;
        float startY = nodeRect.y + 160 + node.NeedItems.Count * (itemFieldHeight + itemFieldPadding) + 22f; // 22 = space for Add button

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
            {
                linkingParentNode = node;
            }
        }
        else
        {
            if (linkingParentNode == node)
            {
                if (GUI.Button(linkButtonRect, "Cancel link"))
                {
                    linkingParentNode = null;
                }
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
    }
    private void DrawCreateChildFunction(StepData node, Rect nodeRect, float y)
    {
        var createChildButtonRect = new Rect(nodeRect.x + 10, y, nodeRect.width - 20, 18);
        if (GUI.Button(createChildButtonRect, "Create Child"))
        {
            Undo.RecordObject(selectedTask, "Undo Create Child Step Data");
            var position = node.Position + DEFAULT_OFFSET;
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
            {
                DrawConnection(currentNode, child);
            }

        }
    }
    private void DrawConnection(StepData fromNode, StepData toNode)
    {
        if (fromNode == null || toNode == null) return;
        float x = NODE_SIZE.x;
        float y = NODE_SIZE.y;
        Vector3 startPos = fromNode.Position + new Vector2(x, y / 2);
        Vector3 endPos = toNode.Position + new Vector2(0, y / 2);

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
    private StepData GetNodeAtPosition(Vector2 position)
    {
        foreach (var node in selectedTask.Steps)
        {
            if (IsMouseOverNode(node, position))
            {
                return node;
            }
        }
        return null;
    }
    private bool IsMouseOverNode(StepData node, Vector2 mousePosition)
    {
        float baseNodeHeight = 250f;
        float extraHeight = node.NeedItems.Count * 22f + 80f; // use same logic as DrawNode
        float dynamicHeight = baseNodeHeight + extraHeight;

        return node != null &&
               mousePosition.x >= node.Position.x &&
               mousePosition.x <= node.Position.x + NODE_SIZE.x &&
               mousePosition.y >= node.Position.y &&
               mousePosition.y <= node.Position.y + dynamicHeight;
    }
    #endregion
}
