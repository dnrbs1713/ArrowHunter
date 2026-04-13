using UnityEngine;
using ArrowClash.Common;
public class RoomInstance
{
    // 생성자
    public RoomSO data;
    public Vector2Int gridPos; // position에서 이름 변경 (가독성)
    public Vector3 worldPos;   // 월드 좌표를 데이터에 포함!
    public bool isCleared = false;
    public MonsterSpawner spawner;
    // [확장성] 타일의 클리어 여부나 방문 여부 등을 저장할 수 있음
    //public bool IsCleared { get; set; }
    //public bool IsVisited { get; set; }


    public RoomInstance(RoomSO data, Vector2Int girdPos, Vector3 worldpos)
    {
        this.data = data;
        this.gridPos = girdPos;
        this.worldPos = worldpos;
    }

    public bool IsOpen(Direction dir) => data.IsOpen(dir);
    public RoomType RoomType => data.roomType;
}
