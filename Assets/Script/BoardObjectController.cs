using UnityEngine;

// 기획서 v2 §3.1: 슬롯에 고정되어 움직이지 않는 오브젝트(색이 있는 조각).
// v2 개정: "바라보는 방향" 속성은 폐지되었습니다. 오브젝트는 색상 하나로만 구분됩니다.
[RequireComponent(typeof(SpriteRenderer))]
public class BoardObjectController : MonoBehaviour
{
    [Header("오브젝트 속성")]
    public ColorType color = ColorType.Red;

    [Header("보드 위치 (격자 좌표)")]
    public Vector2Int gridPosition;

    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        spriteRenderer.color = color.ToColor();
        transform.position = GridManager.Instance.GetWorldPosition(gridPosition);
        GridManager.Instance.RegisterObject(gridPosition, this);
    }

    // 같은 색 홀이 이 칸에 들어오면 GridManager/HoleController가 호출해 오브젝트를 제거합니다.
    public void Consume()
    {
        GridManager.Instance.UnregisterObject(gridPosition);
        Destroy(gameObject);
    }
}
