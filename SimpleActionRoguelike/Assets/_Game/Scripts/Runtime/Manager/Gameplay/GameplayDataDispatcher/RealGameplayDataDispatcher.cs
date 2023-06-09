using Runtime.Definition;
using Runtime.Manager.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Gameplay
{
    public class RealGameplayDataDispatcher : GameplayDataDispatcher
    {
        public override int HeroLevel => 1;
        public override Dictionary<EquipmentType, EquipmentEquippedData> SelectedEquipments
        {
            get
            {
                Dictionary<EquipmentType, EquipmentEquippedData> selectedEquipments = new();
                var currentSelectWeapon = DataManager.Local.playerBasicLocalData.selectedWeapon;
                selectedEquipments.Add(EquipmentType.Weapon, new EquipmentEquippedData(EquipmentType.Weapon, RarityType.Common, (int)currentSelectWeapon, 1, 1));
                return selectedEquipments;
            }
        }
    }

}