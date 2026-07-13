using UnityEngine;

// 기획서 v2 §3.2, §4, §5, §9.14: 플레이어가 드래그로 옮기는 유일한 조작 대상.
// 다중 셀 홀은 shapeCells와 같은 순서로 배치된 자식 SpriteRenderer들을 통해
// 시각적으로 표현합니다 (앵커 셀 = 루트, 나머지 셀 = 자식 오브젝트).
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
public class HoleController : MonoBehaviour
{
    [Header("홀 속성")]
    public ColorType color = ColorType.Red;

    [Tooltip("홀을 구성하는 셀의 상대 좌표(앵커 기준). 기본값은 1x1.")]
    public Vector2Int[] shapeCells = { Vector2Int.zero };

    [Header("보드 위치 (격자 좌표, 앵커 셀 기준)")]
    public Vector2Int gridPosition;

    [Header("정원 표시 (선택)")]
    [SerializeField] private TextMesh capacityLabel;

    private SpriteRenderer[] cellRenderers;
    private int remainingCapacity;
    private bool isDragging;
    private Vector3 dragStartWorldPos;

    public int RemainingCapacity => remainingCapacity;

    void Awake()
    {
        cellRenderers = GetComponentsInChildren<SpriteRenderer>(true);
    }

    void Start()
    {
        remainingCapacity = shapeCells.Length;

        Color tint = color.ToColor();
        tint.a = cellRenderers.Length > 0 ? cellRenderers[0].color.a : 1f;
        foreach (SpriteRenderer sr in cellRenderers)
        {
            sr.color = tint;
        }

        UpdateCapacityLabel();
        transform.position = GridManager.Instance.GetWorldPosition(gridPosition);
        GameManager.Instance?.RegisterHole(this);
    }

    void OnMouseDown()
    {
        isDragging = true;
        dragStartWorldPos = GetMouseWorldPosition();
    }

    void OnMouseUp()
    {
        if (!isDragging) return;
        isDragging = false;

        Vector3 delta = GetMouseWorldPosition() - dragStartWorldPos;
        DirectionType? direction = ResolveDragDirection(delta);

        if (direction.HasValue)
        {
            TryMove(direction.Value);
        }
    }

    private DirectionType? ResolveDragDirection(Vector3 delta)
    {
        float minDragDistance = GridManager.Instance.cellSize * 0.3f;
        if (delta.magnitude < minDragDistance) return null;

        return Mathf.Abs(delta.x) > Mathf.Abs(delta.y)
            ? (delta.x > 0 ? DirectionType.Right : DirectionType.Left)
            : (delta.y > 0 ? DirectionType.Up : DirectionType.Down);
    }

    // 기획서 v2 §4, §9.14.2: 인접 칸으로 한 칸 이동을 시도합니다.
    // 다중 셀 홀은 이동 후 모든 셀을 검사해 하나라도 벽이면 전체 이동을 취소합니다("전부 이동" 또는 "전부 취소").
    public void TryMove(DirectionType direction)
    {
        Vector2Int offset = direction.ToOffset();
        Vector2Int targetAnchor = gridPosition + offset;

        if (!CanMoveTo(targetAnchor))
        {
            transform.position = GridManager.Instance.GetWorldPosition(gridPosition);
            return;
        }

        gridPosition = targetAnchor;
        transform.position = GridManager.Instance.GetWorldPosition(gridPosition);
        ResolveMatchesAtCurrentPosition();
    }

    private bool CanMoveTo(Vector2Int targetAnchor)
    {
        foreach (Vector2Int cellOffset in shapeCells)
        {
            Vector2Int cell = targetAnchor + cellOffset;

            if (!GridManager.Instance.IsInsideBoard(cell))
            {
                return false;
            }

            if (GridManager.Instance.TryGetObject(cell, out BoardObjectController obj) && obj.color != color)
            {
                return false; // 다른 색 오브젝트 = 벽
            }
        }

        return true;
    }

    // 기획서 v2 §9.14.1: 매칭은 오직 색상 일치 여부로만 판정됩니다(진입 방향 무관).
    private void ResolveMatchesAtCurrentPosition()
    {
        foreach (Vector2Int cellOffset in shapeCells)
        {
            Vector2Int cell = gridPosition + cellOffset;

            if (!GridManager.Instance.TryGetObject(cell, out BoardObjectController obj)) continue;
            if (obj.color != color) continue;

            obj.Consume();
            remainingCapacity--;
        }

        UpdateCapacityLabel();

        if (remainingCapacity <= 0)
        {
            GameManager.Instance?.NotifyHoleCleared(this);
            Destroy(gameObject);
        }
    }

    private void UpdateCapacityLabel()
    {
        if (capacityLabel != null)
        {
            capacityLabel.text = remainingCapacity.ToString();
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = Mathf.Abs(Camera.main.transform.position.z);
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }
}
