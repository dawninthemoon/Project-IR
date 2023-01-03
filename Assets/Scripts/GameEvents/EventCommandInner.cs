using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomTilemap;

public partial class EventCommand {
    public class SharedData {
        public CustomGrid<TileObject> Grid { get; }
        public Dictionary<string, LevelInfo> LevelDictionary { get; }
        public EntityInfo ExecuterEntity { get; set; }
        public GameObject Player { get; set; }
        public SharedData(GameObject player, CustomGrid<TileObject> grid, Dictionary<string, LevelInfo> info) {
            Player = player;
            Grid = grid;
            LevelDictionary = info;
        }
    }
    public class SharedVariable {

    }
}