using UnityEngine;
using ArrowClash.Common;
public class RoomInstance
{
    // 생성자
    public RoomSO data;
    public Vector2Int position;
    public bool isCleared = false;
    public MonsterSpawner spawner;
    // [확장성] 타일의 클리어 여부나 방문 여부 등을 저장할 수 있음
    //public bool IsCleared { get; set; }
    //public bool IsVisited { get; set; }


    public RoomInstance(RoomSO data, Vector2Int position)
    {
        this.data = data;
        this.position = position;
    }

    public bool IsOpen(Direction dir) => data.IsOpen(dir);
    public RoomType RoomType => data.roomType;
}
