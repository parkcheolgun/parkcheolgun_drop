using UnityEngine;

public class BlockController : MonoBehaviour
{
    [Header("그리드 이동 범위 설정 (9x9)")]
    // 블록이 이동할 수 있는 최소, 최대 좌표입니다.
    // 0칸부터 8칸까지 총 9칸을 이동하도록 기본 설정했습니다.
    public float minX = 0f;
    public float maxX = 8f;
    public float minY = 0f;
    public float maxY = 8f;

    // 드래그 중인지 확인하는 변수
    private bool isDragging = false;
    // 마우스 클릭 시 블록 중심과의 거리 차이
    private Vector3 offset;

    // 1. 마우스(터치)로 블록을 누르는 순간 실행됩니다.
    void OnMouseDown()
    {
        isDragging = true;
        // 클릭한 마우스 위치와 실제 블록 위치의 차이를 기억해둡니다. (자연스러운 드래그를 위해)
        offset = transform.position - GetMouseWorldPosition();
    }

    // 2. 마우스를 누른 채로 움직일 때 계속 실행됩니다.
    void OnMouseDrag()
    {
        if (isDragging)
        {
            // 마우스 위치에 아까 기억해둔 차이(offset)를 더해 목표 위치를 잡습니다.
            Vector3 targetPosition = GetMouseWorldPosition() + offset;

            // 핵심 1: 반올림을 사용해 1칸 단위로 '스냅(Snap)' 되게 만듭니다.
            float snappedX = Mathf.Round(targetPosition.x);
            float snappedY = Mathf.Round(targetPosition.y);

            // 핵심 2: 블록이 설정한 9x9 그리드(min ~ max) 밖으로 나가지 않게 가둡니다.
            snappedX = Mathf.Clamp(snappedX, minX, maxX);
            snappedY = Mathf.Clamp(snappedY, minY, maxY);

            // 계산된 최종 위치로 블록을 이동시킵니다.
            transform.position = new Vector3(snappedX, snappedY, transform.position.z);
        }
    }

    // 3. 마우스(터치)에서 손을 떼는 순간 실행됩니다.
    void OnMouseUp()
    {
        isDragging = false; // 드래그 종료
    }

    // [보조 함수] 화면상 마우스 위치를 게임 세상 좌표로 바꿔주는 기능입니다.
    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePoint = Input.mousePosition;
        // 2D 카메라의 거리만큼 Z축을 맞춰줍니다.
        mousePoint.z = Mathf.Abs(Camera.main.transform.position.z);
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }
}