using UnityEngine;
using System.Collections; 
using System.Collections.Generic;
using TMPro;
using ArrowClash.Common;
using UnityEngine.Rendering.Universal;

public class MapSceneInit : MonoBehaviour
{
    private void OnEnable()
    {
        RoomManager.OnRoomChanged += InitRoom;
    }

    private void OnDisable()
    {
        RoomManager.OnRoomChanged -= InitRoom;
    }

    private void Start()
    {
        RoomManager.instance.ClearRoomObjects();
        InitRoom();
    }

    private void InitRoom()
    {
        Debug.Log(RoomManager.instance._roomObjects.Count);
        // 1. 방 생성 or 재활성화
        GameObject roomObj = SpawnRoom();
        if(roomObj == null)
        {
            Debug.LogWarning("[MapSceneInit] 방 생성 실패");
            return;
        }

        // 2. RoomManager에 등록
        RoomManager.instance.RegisterRoomObject(roomObj);

        // 3. 전투 승리 후 복귀 시 문 열기
        if (BattleData.hasPendingBattleVictory)
        {
            RoomManager.instance.MarkCurrentRoomMonsterDefeated(BattleData.defeatedEnemyStat);
            BattleData.isVictory = false;
            BattleData.ClearPendingBattleVictory();
        }

        // 4. 플레이어 위치 설정
        SetPlayerPosition();

        // 5. 몬스터 스폰
        MonsterSpawner spawner = roomObj.GetComponentInChildren<MonsterSpawner>();
        if (spawner != null)
            spawner.SpawnMonsters(RoomManager.instance.CurrentRoom);

        LockPlayerMovementAfterRoomInit();
    }

    private GameObject SpawnRoom()
    {
        RoomInstance current = RoomManager.instance.CurrentRoom;
        if(current.data.roomPrefab == null) return null;

        // 이미 생성된 방이면 재활성화
        GameObject existing = RoomManager.instance.GetRoomObject();
        if(existing != null)
        {
            existing.SetActive(true);
            return existing;
        }

        // 없으면 새로 생성
        // _nextRoomWorldPos가 있으면 포탈 기반
        // 없으면 좌표 기반 기본 위치
        /*
        Vector3 worldPos = BattleData.isVictory
            ? BattleData.currentRoomWorldPos
            : RoomManager.instance.GetNextRoomWorldPos();
        */
        Vector3 worldPos = current.worldPos;

        return Instantiate(current.data.roomPrefab, worldPos, Quaternion.identity);
    }

    private void SetPlayerPosition()
    {
        MapPlayerController player = GetMapPlayer();
        if (player == null) return;

        Transform playerTransform = player.transform;

        Direction enterDir = BattleData.enterDirection;

        if (enterDir == Direction.None)
        {
            playerTransform.position = new Vector3(0f, 1f, 0f);
            return;
        }

        if (BattleData.playerMapPosition != Vector3.zero)
        {
            playerTransform.position = BattleData.playerMapPosition;
            BattleData.playerMapPosition = Vector3.zero;
            return;
        }

        Direction spawnDir = RoomSO.Opposite(enterDir);
        PortalController spawnPortal = RoomManager.instance.GetPortal(spawnDir);

        if (spawnPortal != null)
            playerTransform.position = spawnPortal.transform.position + Vector3.up;
        else
            playerTransform.position = new Vector3(0f, 1f, 0f);
    }


    private void LockPlayerMovementAfterRoomInit()
    {
        GameObject player = GameObject.FindWithTag("PLAYER");
        if (player == null) return;

        MapPlayerController controller = player.GetComponent<MapPlayerController>();
        if (controller != null)
            controller.LockMovement(1f);
    }

    private MapPlayerController GetMapPlayer()
    {
        return FindFirstObjectByType<MapPlayerController>();
    }

}
