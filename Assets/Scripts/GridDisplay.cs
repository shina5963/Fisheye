using UnityEngine;

public class GridDisplay : MonoBehaviour
{
    public GameObject gridPrefab; // 碁盤の目を生成するプレハブ
    public int gridSize = 10; // 碁盤の目の細かさ
    public Color gridColor = Color.white; // 碁盤の目の色
    public float gridDistance = 5.0f; // 碁盤の目を奥に移動する距離
    public bool isGridActive = true; // 碁盤の目の表示切り替え
    public float lineWidth = 0.05f; // 碁盤の目の線幅

    private GameObject gridObject; // 碁盤の目のオブジェクト
    public Camera fishEye;
    public Vector3 initScale= new Vector3(100, 100, 100);
    void Start()
    {
        CreateGrid();
        UpdateGridProperties();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ToggleGrid();
        }
    }

    void CreateGrid()
    {
        if (gridObject != null)
        {
            Destroy(gridObject);
        }

        gridObject = new GameObject("Grid");
        gridPrefab = gridObject;
        gridObject.transform.SetParent(fishEye.transform);

        gridObject.transform.localPosition = new Vector3(0, 0, 0.02f);
        gridObject.transform.localRotation = Quaternion.identity;
        gridObject.transform.localScale = new Vector3(1, 1 ,1);
        
        LineRenderer lineRenderer;

        // 縦線
        for (int x = -gridSize; x <= gridSize; x++)
        {
            lineRenderer = CreateLine();
            lineRenderer.SetPosition(0, new Vector3(x, -gridSize, 0));
            lineRenderer.SetPosition(1, new Vector3(x, gridSize, 0));
        }

        // 横線
        for (int y = -gridSize; y <= gridSize; y++)
        {
            lineRenderer = CreateLine();
            lineRenderer.SetPosition(0, new Vector3(-gridSize, y, 0));
            lineRenderer.SetPosition(1, new Vector3(gridSize, y, 0));
        }
        gridObject.transform.localScale = initScale;

    }

    LineRenderer CreateLine()
    {
        GameObject line = new GameObject("Line");
        line.transform.SetParent(gridObject.transform);
        line.transform.localPosition = new Vector3(0, 0, 0);
        line.transform.localRotation = Quaternion.identity;

        LineRenderer lineRenderer = line.AddComponent<LineRenderer>();
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.useWorldSpace = false; // ローカル座標で描画
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = gridColor;
        lineRenderer.endColor = gridColor;
        return lineRenderer;
    }

    void UpdateGridProperties()
    {
        if (gridObject != null)
        {
            gridObject.SetActive(isGridActive);

            foreach (LineRenderer lineRenderer in gridObject.GetComponentsInChildren<LineRenderer>())
            {
                lineRenderer.startColor = gridColor;
                lineRenderer.endColor = gridColor;

                // 線幅を一括設定
                lineRenderer.startWidth = lineWidth;
                lineRenderer.endWidth = lineWidth;
            }
        }
    }

    public void SetGridSize(int size)
    {
        gridSize = size;
        CreateGrid();
    }

    public void SetGridColor(Color color)
    {
        gridColor = color;
        UpdateGridProperties();
    }

    public void SetGridDistance(float distance)
    {
        gridDistance = distance;
        if (gridObject != null)
        {
            gridObject.transform.localPosition = fishEye.transform.forward * gridDistance;
        }
    }

    public void SetLineWidth(float width)
    {
        lineWidth = width;
        UpdateGridProperties();
    }

    public void ToggleGrid()
    {
        isGridActive = !isGridActive;
        UpdateGridProperties();
    }
}
