using Cysharp.Threading.Tasks;
using Runtime.Constants;
using Runtime.Manager.Audio;
using Runtime.Manager.Data;
using Runtime.Message;
using System;
using System.Linq;
using UnityEngine;

namespace Runtime.UI
{
    public class ModalGameplayControlSettings : BaseModal
    {
        [SerializeField] private ToggleObject[] _soundObjects;
        [SerializeField] private ToggleObject[] _musicObjects;
        [SerializeField] private SelectGameObject[] _selectGameObjects;

        public enum SelectedRow
        {
            Music = 0,
            Sound = 1,
        }

        [Serializable]
        public class SelectGameObject
        {
            public GameObject selectGameObject;
            public SelectedRow selectedRow;
        }

        private SelectedRow _currentSelectedRow;
        private GameObject _selectedGameObject;

        public async override UniTask Initialize(Memory<object> args)
        {
            await base.Initialize(args);

            foreach (var item in _selectGameObjects)
            {
                item.selectGameObject.SetActive(false);
            }

            UpdataSelectUI(SelectedRow.Music);
            UpdateSoundUI();
        }

        protected override void OnKeyPress(InputKeyPressMessage message)
        {
            base.OnKeyPress(message);
            if(message.KeyPressType == KeyPressType.Down)
            {
                var selectedRow = _currentSelectedRow + 1;
                UpdataSelectUI(selectedRow);
            }
            else if (message.KeyPressType == KeyPressType.Up)
            {
                var selectedRow = _currentSelectedRow - 1;
                UpdataSelectUI(selectedRow);
            }
            else if (message.KeyPressType == KeyPressType.Right)
            {
                if(_currentSelectedRow == SelectedRow.Music)
                {
                    if (DataManager.Local.playerBasicLocalData.musicSettings < Constant.MAX_CONFIG_SOUND)
                    {
                        DataManager.Local.playerBasicLocalData.musicSettings++;
                        UpdateAllSettings();
                    }
                }
                else if (_currentSelectedRow == SelectedRow.Sound)
                {
                    if (DataManager.Local.playerBasicLocalData.sfxSettings < Constant.MAX_CONFIG_SOUND)
                    {
                        DataManager.Local.playerBasicLocalData.sfxSettings++;
                        UpdateAllSettings();
                    }
                }
            }
            else if (message.KeyPressType == KeyPressType.Left)
            {
                if (_currentSelectedRow == SelectedRow.Music)
                {
                    if(DataManager.Local.playerBasicLocalData.musicSettings > 0) 
                    {
                        DataManager.Local.playerBasicLocalData.musicSettings--;
                        UpdateAllSettings();
                    }
                }
                else if (_currentSelectedRow == SelectedRow.Sound)
                {
                    if(DataManager.Local.playerBasicLocalData.sfxSettings > 0)
                    {
                        DataManager.Local.playerBasicLocalData.sfxSettings--;
                        UpdateAllSettings();
                    }
                }
            }
        }

        private void UpdateAllSettings()
        {
            DataManager.Local.SavePlayerData();
            AudioManager.Instance.UpdateSettings();
            UpdateSoundUI();
        }

        private void UpdateSoundUI()
        {
            var musicValue = DataManager.Local.playerBasicLocalData.musicSettings;
            var soundValue = DataManager.Local.playerBasicLocalData.sfxSettings;

            for (int i = 0; i < _musicObjects.Length; i++)
            {
                _musicObjects[i].Toggle(i < musicValue);
            }

            for (int i = 0; i < _soundObjects.Length; i++)
            {
                _soundObjects[i].Toggle(i < soundValue);
            }
        }

        private void UpdataSelectUI(SelectedRow selectedRow)
        {
            var selectedObject = _selectGameObjects.FirstOrDefault(x => x.selectedRow == selectedRow);
            if(selectedObject != null)
            {
                _currentSelectedRow = selectedRow;
                if (_selectedGameObject)
                    _selectedGameObject.SetActive(false);
                _selectedGameObject = selectedObject.selectGameObject;
                _selectedGameObject.SetActive(true);
            }
        }
    }
}