using UnityEngine;

// 격자 전체가 화면 비율(가로/세로)과 무관하게 중앙에 꽉 차게 보이도록
// Orthographic 카메라의 위치/크기를 런타임에 맞춥니다.
[RequireComponent(typeof(Camera))]
public class CameraGridFit : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;
    [SerializeField] private float padding = 0.5f;

    private Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
    }

    void Start()
    {
        Fit();
    }

    public void Fit()
    {
        if (gridManager == null)
        {
            gridManager = GridManager.Instance;
        }

        if (gridManager == null) return;

        cam.orthographic = true;

        float gridWidth = gridManager.width * gridManager.cellSize;
        float gridHeight = gridManager.height * gridManager.cellSize;

        Vector3 center = gridManager.transform.position + new Vector3(
            (gridManager.width - 1) * gridManager.cellSize * 0.5f,
            (gridManager.height - 1) * gridManager.cellSize * 0.5f,
            0f);

        transform.position = new Vector3(center.x, center.y, transform.position.z);

        float halfHeightNeeded = gridHeight * 0.5f + padding;
        float halfWidthNeeded = (gridWidth * 0.5f + padding) / cam.aspect;

        cam.orthographicSize = Mathf.Max(halfHeightNeeded, halfWidthNeeded);
    }
}
