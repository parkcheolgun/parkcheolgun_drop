using UnityEngine;

// 기획서 3.2, 4, 5: 플레이어가 드래그로 옮기는 유일한 조작 대상.
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

    private SpriteRenderer spriteRenderer;
    private int remainingCapacity;
    private bool isDragging;
    private Vector3 dragStartWorldPos;

    public int RemainingCapacity => remainingCapacity;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        remainingCapacity = shapeCells.Length;
        Color tint = color.ToColor();
        tint.a = spriteRenderer.color.a; // 프리팹에 설정된 반투명(홀 느낌)을 유지합니다.
        spriteRenderer.color = tint;
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

    // 기획서 4.2~4.5: 인접 칸으로 한 칸 이동을 시도하고, 벽이면 취소, 아니면 매칭을 판정합니다.
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
        ResolveMatchesAtCurrentPosition(direction);
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

    private void ResolveMatchesAtCurrentPosition(DirectionType? incomingDirection = null)
    {
        foreach (Vector2Int cellOffset in shapeCells)
        {
            Vector2Int cell = gridPosition + cellOffset;

            if (!GridManager.Instance.TryGetObject(cell, out BoardObjectController obj)) continue;
            if (obj.color != color) continue;

            bool facingMatch = obj.direction == DirectionType.All
                || (incomingDirection.HasValue && obj.direction == incomingDirection.Value.Opposite());

            if (facingMatch)
            {
                obj.Consume();
                remainingCapacity--;
            }
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
