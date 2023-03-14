using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Container_PlayerData_EventNames
{
    GetPlayerCharData,SetPlayerCharData,
    GetNPCByUIIndex, GetNPCByIndex, GetAllNPCData, GetAllSelectedNPCData, GetNPCSelectedIndexs,
    SetNPCSelectIndex,

    GetMapIndex,
    MovShipIndex,
    UseResource,

    OnMapIndexChg,

    GetWareHousePageNum, WareHousePageNumChg,
    WareHouseItemChg,NPCDataChg,DateChg,PeopleChg, MouseItemChg, ResourceChange,
    NPCbackpack,
    NPCSelectChg, NPCListChg,AddNPC, AddNPCSlot
}

public enum EventNames 
{
    QuitStartScene,ArriveInGameScene,QuitInGameScene,ArriveStartScene, LoadSceneProcessGo, //场景相关主生命周期的事件名
    Save,
  LoadSave,UnLoadSave,LoadGame,UnLoadGame,
    SaveDone, LoadSaveDone, UnLoadSaveDone, LoadGameDone, UnLoadGameDone,//临时事件，每次跳scene监听都会被清空
    JumpToScene,//scenejumper 跳场景

  ThisSavePath,ThisSaveName, IsInSave,IsInGame, IsCheatMode,SetCheat,GetCurrentSaveData,//container_savedata
    GetCurrentMapPrefabs, OnMapTimeChg,StartGameFromMap,//container_mapprefab
    GetMovEngineByName,GetOriginMovEngineByName, GetAllMovEngineName,//container_movengine
  IsHereLoaded,GetTerrainHAt,GetChunkData,GetChunkObj,EBlockCanPlaceAt,//container_terrian
  GetBuildAbleData,//container_buildable
    SetEntityToCaChe, CreateEntityByName,RemoveEntity,//container_entity
    GetEVCbyUID,//container_evc

  GetStartEngine,
    GetLocalPlayer, GetAllPlayerPosi, SetDefaultSpawnPoint, GetDefaultSpawnPoint,PauseLocalPlayer, GetLocalPlayerOfflinePosi,

        GetMaxUUID,GetMapSize,

        //GetToolByItem,
    GetToolByName, GetBareHand, IsBareHand,//tool

    //loader事件
    SaveGameSetting,GetGameSetting,//loader_gamesetting
    GetBuffExcelData, GetBaseToolByItem, GetBaseToolNameByItem, GetToolExcelData, GetAmmoExcelData,//loader_Excel
    GetEntityByName,GetModelByName,//loader_prefabs
    GetNPCHeadImg, GetNPCBodyImg, GetNPCBody3DImg,//loader_NPC
    StrtoTexture, BMattoTexture,//loader_texture
    GetMovCommandByStr,//loader_Movcommand

    ItemIDtoName,ItemIDtoMax, ItemIDtoMaxSub, ItemIDtoSize, ItemtoTexture, ItemIDtoDes, GetAllItems, GetRandomItem,GetItemsByGroup,GetGroupByItemID,GetAllItemGroup,//loader_item

        GetEQObjByabsName, GetEQObjByItem,//loader_equip
        GetDefaultBBlockObj,GetMeshByIndex,GetMatByIndex,SetBBlock,GetBBlock,//loader_BBlock
    GetEBlockSizeByID, GetEBlockObjByID, GetEBlockObjByName,GetAllEBlockID,GetEBlockByLv, GetEBlockByLvAndClass, GetEBlockByID,//loader_eblock
    GetMapprefabDatas,GetMapGraphDatas, GetMapPrefabsDataByIndex,GetMapDataByName,GetGraphDataByName,IsMapNearBy,//loader_mapprefabsdata

        CreateSave, GetSaveDataByIndex, GetSaveDataByName, GetAllSaveData,CreateSaveAndGetName,
        OnDeleteSave,OnCreateSave,CreateSaveByName,SetSaveSetting,GetSaveSetting,//loader_savedata
    GetBullet,FireBullet,CastDamage,
        GetEffect,PlayEffect,GetEffectPrefabs,//loader_Effect
        PlaySoundAt,PlaySoundFor,

    GetCraftUUIDByGroup, GetCraftDataByUUID,//loader_craft
    GetFirePitDataByMat,GetFuelByID,//loader_firePit
    GetFurnanceDataByID,GetFurnanceByMats,//loader_Furnance

    SetMouseItem,GetMouseItem,DropItem,//item container
    AddRes,GetRes,SetRes, GetShipAtMapData,GetReserchData,SetReserchData, SetMapTime,//container_playerdata
    UpdateActiveChunk,ChunkUpdate,BigChunkLoaded,GetAllActiveChunkFile,//terrianContainer
    SetEBlock,GetEBlock,RemoveEBlock,//EBlockContainer

        //camera事件
    GetMouseLookAt, GetMouseLookObj, GetViewPosi,ShakeCamera, ChangeViewDir,Track


}
public enum PlayerEventName
{
    save,load,
    onHealthChangeBy, onDamage, onDie,onPowerChange, bpChg,

    onRespawn,pause,remove, SetActive,

    buildMVConnect, breakMVConnect,

    backpack,giveItem,takeItem,setItem,containItem,dropItem,dropBPItemAt, Getbp_Items,//backpack

    getLookAt,getLookAtObj,//eye

    setMovePauseInTime, setRotaPauseInTime,getMoveStat,//movement

    playAnima,onAnimationEvent,//animator

    onHandChgTool,setHandPauseInTime,getCurrentHandHolding,getMainHand,getLeftHand,
    getMainHandPos,getLeftHandPos, setHandActive, onToolUpdateItem,//hand

    onSpreadChg,//tool上传的事件

    entity_type, entity_hp, entity_food, entity_pow, entity_maxHp, entity_maxFood, entity_maxPow,
    entity_def, entity_spd, entity_runspd, entity_powUpRate, entity_hpUpRate,
    skill_pow, skill_know, skill_agi, skill_phy, skill_blade, skill_hammer, skill_bow,
    skill_rifle, skill_auto, skill_explo, skill_fire, skill_ice, skill_electro, skill_radio,
    skill_posion, skill_corrupt, skill_necroman, skill_heal, skill_build,//entityData

    onKey,

    move
}
public enum TestEventName
{
    test,eventa,eventb
}