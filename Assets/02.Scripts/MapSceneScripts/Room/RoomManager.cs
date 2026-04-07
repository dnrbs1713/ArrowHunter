using UnityEngine;
using System.Collections.Generic;
using ArrowClash.Common;
using System;
using Unity.VisualScripting;


public class RoomManager : MonoBehaviour
{
    public static RoomManager instance;

    public static event Action OnRoomChanged;

    [Header("방 풀")]
    public List<RoomSO> roomPools;
    public RoomSO startRoomSO;

    public RoomInstance CurrentRoom { get; private set; }
    public int ClearCount { get; private set; } = 0;

    private RoomGenerator _generator;
    public Dictionary<Vector2Int, RoomInstance> _visitedRooms
        = new Dictionary<Vector2Int, RoomInstance>();
    
    // 다시 private로 바꿀 것
    public Dictionary<Vector2Int, GameObject> _roomObjects
        = new Dictionary<Vector2Int, GameObject>();

    private Vector2Int _currentPos = Vector2Int.zero;

    // 현재 방 문 — SetupCurrentRoom()에서 자동 탐색
    private PortalController _portalUp;
    private PortalController _portalDown;
    private PortalController _portalLeft;
    private PortalController _portalRight;

    private Vector3 _nextRoomWorldPos = Vector3.zero;

    private void Awake()
    {
        if (instance != null) { Destroy(gameObject); return; }
        instance = this;
        DontDestroyOnLoad(gameObject);

        _generator = new RoomGenerator(roomPools);
        CurrentRoom = new RoomInstance(startRoomSO, _currentPos, _nextRoomWorldPos);
        _visitedRooms[_currentPos] = CurrentRoom;
        
    }


    // ************* MapSceneInit에서 호출 ***************//
    public void RegisterRoomObject(GameObject roomObj)
    {
        foreach (var kv in _roomObjects)
            kv.Value.SetActive(false);
        _roomObjects[_currentPos] = roomObj;
        BattleData.currentRoomWorldPos = roomObj.transform.position;
        RefreshPortals(roomObj);
        SetupPortals(CurrentRoom);
    }
    
    public GameObject GetRoomObject()
    {
        if (_roomObjects.TryGetValue(_currentPos, out GameObject obj))
        {
            if(obj == null)
            {
                _roomObjects.Remove(_currentPos);
                return null;
            }
            return obj;
        }
        return null;
    }

    // 다음 방 월드 위치 반환
    public Vector3 GetNextRoomWorldPos() => _nextRoomWorldPos;


    // ************* [방 상태 관리] ***************//

    public void OnRoomCleared()
    {
        if (CurrentRoom.isCleared) return;

        CurrentRoom.isCleared = true;
        ClearCount++;
        _generator.UpdateDifficulty(ClearCount);

        SetupPortals(CurrentRoom);
    }

    public PortalController GetPortal(Direction dir)
    {
        return dir switch
        {
            Direction.Up => _portalUp,
            Direction.Down => _portalDown,
            Direction.Left => _portalLeft,
            Direction.Right => _portalRight,
            _ => null
        };
    }

    public void MoveToRoom(Direction dir)
    {
        Vector2Int nextGridPos = _currentPos + DirToVector(dir);

        if (!_visitedRooms.TryGetValue(nextGridPos, out RoomInstance nextRoom))
        {
            RoomSO nextSO = _generator.GenerateRoom(dir);
            float size = CurrentRoom.data.roomSize;

            Vector3 nextWorldPos = CurrentRoom.worldPos + DirToVector3(dir) * size;

            nextRoom = new RoomInstance(nextSO, nextGridPos, nextWorldPos);

            _visitedRooms[nextGridPos] = nextRoom;
        }
        // 다음 방 위치 미리 계산 — MapSceneInit에서 사용
        //_nextRoomWorldPos = GetNextRoomPosition(_currentPos, nextPos, dir);

        _currentPos = nextGridPos;
        CurrentRoom = nextRoom;

        BattleData.enterDirection = dir;

        Debug.Log($"[RoomManager] 이동 → {dir} / 방: {CurrentRoom.RoomType} // {_visitedRooms.Count}");
        
        OnRoomChanged?.Invoke();
    }

    public Vector3 GetCurrentRoomSpawnerPos()
    {
        if (_roomObjects.TryGetValue(_currentPos, out GameObject roomObj))
        {
            var spawner = roomObj.GetComponentInChildren<MonsterSpawner>();
            if (spawner != null)
                return spawner.transform.position;
        }

        return Vector3.zero;
    }

    public void ClearRoomObjects()
    {
        _roomObjects.Clear();
        _nextRoomWorldPos = CurrentRoom.worldPos;
        Debug.Log($"확인 {_nextRoomWorldPos}");
    }

    //************ [private] ****************//
    private void RefreshPortals(GameObject roomObj)
    {
        _portalUp = _portalDown = _portalLeft = _portalRight = null;

        PortalController[] portals = roomObj.GetComponentsInChildren<PortalController>();
        foreach (var portal in portals)
        {
            switch (portal.direction)
            {
                case Direction.Up: _portalUp = portal; break;
                case Direction.Down: _portalDown = portal; break;
                case Direction.Left: _portalLeft = portal; break;
                case Direction.Right: _portalRight = portal; break;
            }
        }
    }
    private void SetPlayerSpawnPosition()
    {
        var player = GameObject.FindWithTag("PLAYER");

        Direction enterDir = BattleData.enterDirection;

        if (enterDir == Direction.None)
        {
            player.transform.position = new Vector3(0f, 1f, 0f);
            return;
        }

        Direction spawnDir = RoomSO.Opposite(enterDir);
        PortalController spawnPortal = GetPortal(spawnDir);

        if (spawnPortal != null)
            player.transform.position = spawnPortal.transform.position + Vector3.up;
    }


    private Vector3 GetRoomWorldPosition(Vector2Int pos)
    {
        float size = CurrentRoom.data.roomSize;
        return new Vector3(pos.x * size, 0f, pos.y * size);
    }
       
    private Vector3 GetNextRoomPosition(Vector2Int currentPos, Vector2Int nextPos, Direction dir)
    {
        float size = _visitedRooms[currentPos].data.roomSize;
        Vector3 currentWorldPos = _roomObjects.TryGetValue(currentPos, out var obj)
            ? obj.transform.position
            : Vector3.zero;

        return currentWorldPos + DirToVector3(dir) * size;
    }

    private void SetupPortals(RoomInstance room)
    {
        SetPortal(_portalUp, Direction.Up, room);
        SetPortal(_portalDown, Direction.Down, room);
        SetPortal(_portalLeft, Direction.Left, room);
        SetPortal(_portalRight, Direction.Right, room);
    }

    private void SetPortal(PortalController portal, Direction dir, RoomInstance room)
    {
        if (portal == null) return;

        Direction reverse = RoomSO.Opposite(BattleData.enterDirection);

        if (!room.IsOpen(dir))
        {
            portal.gameObject.SetActive(false);
            return;
        }

        portal.gameObject.SetActive(true);

        if (room.isCleared && reverse != dir) portal.SetOpen();
        else portal.SetLocked();
    }


    private Vector2Int DirToVector(Direction dir)
    {
        return dir switch
        {
            Direction.Up => Vector2Int.up,
            Direction.Down => Vector2Int.down,
            Direction.Left => Vector2Int.left,
            Direction.Right => Vector2Int.right,
            _ => Vector2Int.zero
        };
    }
    private Vector3 DirToVector3(Direction dir)
    {
        return dir switch
        {
            Direction.Up => Vector3.forward,
            Direction.Down => Vector3.back,
            Direction.Left => Vector3.left,
            Direction.Right => Vector3.right,
            _ => Vector3.zero
        };
    }
}
