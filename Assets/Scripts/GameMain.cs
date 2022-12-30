using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomPhysics;

public class GameMain : MonoBehaviour {
    [SerializeField] private GameEntityBase player = null;
    [SerializeField] InputControl _input = null;
    //[SerializeField] EnemyCreation _enemyCreation = null;
    private HandleGroundMove _playerMove;

    void Awake() {
        //var levelManager = LevelManager.GetInstance();

        //AssetManager.GetInstance().Initalize();
        CollisionManager.GetInstance().Initalize();
        
        player.Assign();

        /*
        levelManager.Initalize();
        //_enemyCreation.Initalize();

        EventCommand.SharedData sharedData = new EventCommand.SharedData(
            _player,
            levelManager.TileGrid,
            levelManager.LevelDictionary,
            _enemyCreation
        );
        EventManager.GetInstance().Initalize(sharedData);

        levelManager.LoadLevel(levelManager.CurrentLevelName);*/
    }

    void Start() 
    {
        
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