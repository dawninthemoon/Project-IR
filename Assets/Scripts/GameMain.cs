using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomPhysics;

public class GameMain : MonoBehaviour {
    [SerializeField] private GameObject player = null;
    [SerializeField] InputControl _input = null;
    //[SerializeField] EnemyCreation _enemyCreation = null;

    void Awake() {
        //var levelManager = LevelManager.GetInstance();

        //AssetManager.GetInstance().Initalize();
        CollisionManager.GetInstance().Initalize();
        
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

    void Start() {
        GroundController controller = player.GetComponent<GroundController>();
        controller.Initalize();

        HandleGroundMove handleMove = new HandleGroundMove(controller);
        handleMove.Initalize();

        PlayerFSM playerFSM = player.GetComponent<PlayerFSM>();
        playerFSM.Initalize();

        _input.Initalize(playerFSM, handleMove);
    }

    void Update() {
        //_enemyCreation.Progress();
        _input.Progress();
        //_player.Progress();
        //_particleManager.Progress();
    }

    void LateUpdate() {
        
    }
}