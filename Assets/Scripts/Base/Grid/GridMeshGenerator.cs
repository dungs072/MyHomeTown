using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(GridSystem))]
public class GridMeshGenerator : MonoBehaviour
{
    [SerializeField] Material gridMaterial;
    private int rows = 10;
    private int cols = 10;
    private float cellSize = 1f;

    private GridSystem gridSystem;
    void Awake()
    {
        gridSystem = GetComponent<GridSystem>();
        rows = gridSystem.TotalVerticalSlot;
        cols = gridSystem.TotalHorizontalSlot;
        cellSize = gridSystem.Size;
    }
    void Start()
    {
        GenerateGridMesh();
    }

    void GenerateGridMesh()
    {
        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        int totalLines = (rows + 1) * 2 + (cols + 1) * 2;
        Vector3[] vertices = new Vector3[totalLines * 2];
        int[] indices = new int[totalLines * 2];

        int index = 0;

        // Vertical lines (centered)
        for (int x = -cols / 2; x <= cols / 2; x++)
        {
            vertices[index] = new Vector3(x * cellSize, 0, -rows / 2 * cellSize);
            vertices[index + 1] = new Vector3(x * cellSize, 0, rows / 2 * cellSize);
            indices[index] = index;
            indices[index + 1] = index + 1;
            index += 2;
        }

        // Horizontal lines (centered)
        for (int y = -rows / 2; y <= rows / 2; y++)
        {
            vertices[index] = new Vector3(-cols / 2 * cellSize, 0, y * cellSize);
            vertices[index + 1] = new Vector3(cols / 2 * cellSize, 0, y * cellSize);
            indices[index] = index;
            indices[index + 1] = index + 1;
            index += 2;
        }

        mesh.vertices = vertices;
        mesh.SetIndices(indices, MeshTopology.Lines, 0);
        mesh.RecalculateBounds();

        MeshRenderer renderer = GetComponent<MeshRenderer>();
        renderer.material = gridMaterial;
    }
}
