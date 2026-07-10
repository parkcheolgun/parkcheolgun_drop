using UnityEngine;

// 기획서 3.1: 슬롯에 고정되어 움직이지 않는 오브젝트(색이 있는 조각).
[RequireComponent(typeof(SpriteRenderer))]
public class BoardObjectController : MonoBehaviour
{
    [Header("오브젝트 속성")]
    public ColorType color = ColorType.Red;
    public DirectionType direction = DirectionType.All;

    [Header("보드 위치 (격자 좌표)")]
    public Vector2Int gridPosition;

    [Header("정면 표시 (선택, 방향이 All이 아닐 때만 사용)")]
    [SerializeField] private Transform directionMarker;

    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        ApplyVisual();
        transform.position = GridManager.Instance.GetWorldPosition(gridPosition);
        GridManager.Instance.RegisterObject(gridPosition, this);
    }

    private void ApplyVisual()
    {
        spriteRenderer.color = color.ToColor();

        if (directionMarker == null) return;

        if (direction == DirectionType.All)
        {
            directionMarker.gameObject.SetActive(false);
        }
        else
        {
            directionMarker.gameObject.SetActive(true);
            Vector2Int offset = direction.ToOffset();
            directionMarker.localPosition = new Vector3(offset.x, offset.y, 0f) * 0.35f;
        }
    }

    // 같은 색 홀이 정면에서 매칭되면 GridManager/HoleController가 호출해 오브젝트를 제거합니다.
    public void Consume()
    {
        GridManager.Instance.UnregisterObject(gridPosition);
        Destroy(gameObject);
    }
}
