using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    [Header("그리드 생성 설정")]
    // 복사해서 깔아줄 그리드 타일 (붕어빵 틀)
    public GameObject tilePrefab;

    // 가로 6칸, 세로 6칸 설정
    public int width = 6;
    public int height = 6;

    void Start()
    {
        // 게임이 시작되자마자 그리드를 깔아줍니다.
        GenerateGrid();
    }

    void GenerateGrid()
    {
        if (tilePrefab == null)
        {
            Debug.LogError("GridBase 오브젝트(타일 프리팹)를 연결해 주세요!");
            return;
        }

        // 가로(X)로 6번, 세로(Y)로 6번 반복하며 총 36개의 타일을 생성합니다.
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // 각 칸의 정수 좌표(0,0 / 0,1 / 1,0 등)를 계산합니다.
                Vector3 spawnPosition = new Vector3(x, y, 0);

                // 계산된 위치에 타일 붕어빵을 하나 찍어냅니다.
                GameObject newTile = Instantiate(tilePrefab, spawnPosition, Quaternion.identity);

                // Hierarchy 창이 지저분해지지 않도록 이 오브젝트 아래로 깔끔하게 묶어줍니다.
                newTile.transform.parent = this.transform;
                newTile.name = $"Tile_{x}_{y}"; // 이름도 알아보기 쉽게 변경
            }
        }
    }
}