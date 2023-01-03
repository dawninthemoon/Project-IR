using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomPhysics;

public class GameMain : MonoBehaviour {
    [SerializeField] private GameEntityBase player = null;
    //[SerializeField] EnemyCreation _enemyCreation = null;
    private HandleGroundMove _playerMove;

    void Awake() {
        var levelManager = LevelManager.GetInstance();

        CollisionManager.GetInstance().Initialize();
        
        player.Assign();

        levelManager.Initialize();
        //_enemyCreation.Initalize();

        EventCommand.SharedData sharedData = new EventCommand.SharedData(
            player,
            levelManager.TileGrid,
            levelManager.LevelDictionary
            //_enemyCreation
        );
        EventManager.GetInstance().Initalize(sharedData);

        levelManager.LoadLevel(levelManager.CurrentLevelName);
    }

    void Start() {
        player.Initialize();

    }

    void Update() {
        ActionKeyInputManager.GetInstance().progress(Time.deltaTime);

        player.Progress(Time.deltaTime);

        CollisionManager.GetInstance().Progress();
    }

    void LateUpdate() {
        
    }
}