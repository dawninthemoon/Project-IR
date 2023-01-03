using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class EventCommand {
    public class SetPlayer : EventCommandInterface {
        public IEnumerator Execute(SharedData data, SharedVariable variable) {
            string prevLevelName = LevelManager.GetInstance().PrevLevelName;
            string currentLevelName = LevelManager.GetInstance().CurrentLevelName;
            string doorID = data.ExecuterEntity.fields[0].value;

            if (!prevLevelName.Equals(currentLevelName) && !prevLevelName.Equals(doorID)) yield break;

            var player = data.Player;
            LevelInfo level = data.LevelDictionary[LevelManager.GetInstance().CurrentLevelName];
            var entity = data.ExecuterEntity;

            Vector3 worldPosition = data.Grid.GetWorldPosition(entity.row, entity.column);
            worldPosition.y += 0.5f;
            player.SetActive(true);
            player.transform.position = worldPosition;
            
            yield break;
        }
    }

    public class SetEnemy : EventCommandInterface {
        static string EnemyNameKey = "enemyName";
        public IEnumerator Execute(SharedData data, SharedVariable variable) {/*
            string enemyName = null;
            Vector3 position = Vector3.zero;
            var entity = data.ExecuterEntity;
            foreach (var field in entity.fields) {
                if (field.name.Equals(EnemyNameKey)) {
                    enemyName = field.value;
                    position = data.Grid.GetWorldPosition(entity.row, entity.column);
                    break;
                }
            }*/
            //var enemy = data.EnemyCreation.CreateEnemy(enemyName);
            //enemy.transform.position = position;

            yield break;
        }
    }

    public interface EventCommandInterface {
        IEnumerator Execute(SharedData data, SharedVariable variable);
    }
}