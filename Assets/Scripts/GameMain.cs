using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomPhysics;

public class GameMain : MonoBehaviour {
    [SerializeField] private GameObject player = null;
    [SerializeField] InputControl _input = null;
    //[SerializeField] EnemyCreation _enemyCreation = null;
    private HandleGroundMove _playerMove;

    void Awake() {
        var levelManager = LevelManager.GetInstance();
        CollisionManager.GetInstance().Initialize();
        
        levelManager.Initialize();

        EventCommand.SharedData sharedData = new EventCommand.SharedData(
            player,
            levelManager.TileGrid,
            levelManager.LevelDictionary
        );
        EventManager.GetInstance().Initalize(sharedData);
        
        levelManager.LoadLevel(levelManager.CurrentLevelName);
    }

    void Start() {
        GroundController controller = player.GetComponent<GroundController>();
        controller.Initialize();

        _playerMove = new HandleGroundMove(controller, 45f, 65f, 100f);
        _playerMove.Initialize();

        PlayerFSM playerFSM = player.GetComponent<PlayerFSM>();
        playerFSM.Initalize();

        _input.Initalize(playerFSM, _playerMove);
    }

    void Update() {
        //_enemyCreation.Progress();
        _input.Progress();
        _playerMove.Progress();

        CollisionManager.GetInstance().Progress();
        //_particleManager.Progress();
    }

    void LateUpdate() {
        
    }
}