using Players.Components;
using UnityEngine;

namespace Players.Settings.ScriptableObjects {

    // Example of scriptable object settings
    [CreateAssetMenu(fileName = "PlayerStatsSettingsScriptable", menuName = "ScriptableObjects/Settings/PlayerStatsSettingsScriptable")]
    public class PlayerStatsSettingsScriptable : ScriptableObject {
        public PlayerAttackComponent PlayerAttack;
        public PlayerAttackCountComponent PlayerAttackCount;
        public PlayerAttackSpeedComponent PlayerAttackSpeed;
    }
}