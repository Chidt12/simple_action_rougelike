using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEditor.Localization;
using UnityEditor.Localization.Plugins.Google;

namespace GameEditor.Localization
{
    [GlobalConfig("Assets/_Game/Localization")]
    public class LocalizationPullTool : GlobalConfig<LocalizationPullTool>
    {
        #region Class Methods

        [ListDrawerSettings(HideAddButton = true)]
        public List<LocalizationSelect> localizations;

        [HorizontalGroup("Buttons")]
        [Button("Sync Localize", ButtonSizes.Medium)]
        public void SyncStringTables()
        {
            var tableStringCollections = LocalizationEditorSettings.GetStringTableCollections();

            if (localizations == null || localizations.Count == 0)
            {
                localizations = tableStringCollections.Select(x => new LocalizationSelect(x, x.name)).ToList();
                return;
            }

            foreach (var tableCollection in tableStringCollections)
            {
                var localizationSelect = localizations.FirstOrDefault(x => x.name == tableCollection.name);
                if (localizationSelect != null)
                    localizationSelect.tableCollection = tableCollection;
            }

            if (localizations.Count > 0)
            {
                for (int i = 0; i < localizations.Count; i++)
                {
                    var localization = localizations[i];
                    if (!tableStringCollections.Any(x => x.name == localization.name))
                        localizations.Remove(localization);
                }
            }
        }

        [HorizontalGroup("Buttons")]
        [Button("Pull All", ButtonSizes.Medium)]
        public void PullAll()
        {
            var stringTables = localizations.Select(x => x.tableCollection).ToList();
            PullTables(stringTables);
        }

        [HorizontalGroup("Buttons")]
        [Button("Pull Selected", ButtonSizes.Medium)]
        public void PullSelected()
        {
            var stringTables = localizations.Where(x => x.selected).Select(x => x.tableCollection).ToList();
            PullTables(stringTables);
        }

        private void PullTables(List<StringTableCollection> stringTables)
        {
            foreach (var stringTable in stringTables)
            {
                var extensions = stringTable.Extensions;
                foreach (var extension in extensions)
                {
                    if (extension is GoogleSheetsExtension)
                    {
                        var googleSheetExtension = extension as GoogleSheetsExtension;
                        var googleSheets = new GoogleSheets(googleSheetExtension.SheetsServiceProvider);
                        googleSheets.SpreadSheetId = googleSheetExtension.SpreadsheetId;
                        googleSheets.PullIntoStringTableCollection(googleSheetExtension.SheetId, stringTable, googleSheetExtension.Columns, true);
                    }
                }
            }
        }

        #endregion Class Methods

        #region Class In Class

        [Serializable]
        public class LocalizationSelect
        {
            [ReadOnly]
            public StringTableCollection tableCollection;
            [ReadOnly]
            public string name;
            public bool selected;

            public LocalizationSelect(StringTableCollection tableCollection, string name, bool selected = true)
            {
                this.tableCollection = tableCollection;
                this.name = name;
                this.selected = selected;
            }
        }

        #endregion Class In Class
    }
    public class LocalizationToolsEditorWindow : OdinMenuEditorWindow
    {
        #region Class Methods

        [MenuItem("Tools/Localization/Tools")]
        private static void OpenWindow()
        {
            var window = GetWindow<LocalizationToolsEditorWindow>();
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 600);
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            OdinMenuTree tree = new OdinMenuTree(supportsMultiSelect: true) {
                { "Pull Tool", LocalizationPullTool.Instance, EditorIcons.Bell },
                { "All Localizations", null , EditorIcons.House },
            };

            tree.AddAllAssetsAtPath("All Localizations", "Assets/_Game/Localization/StringTables", typeof(StringTableCollection), true, true).SortMenuItemsByName();
           

            var customMenuStyle = new OdinMenuStyle {
                BorderPadding = 0f,
                AlignTriangleLeft = true,
                TriangleSize = 16f,
                TrianglePadding = 0f,
                Offset = 20f,
                Height = 23,
                IconPadding = 0f,
                BorderAlpha = 0.323f
            };

            tree.DefaultMenuStyle = customMenuStyle;

            tree.Config.DrawSearchToolbar = true;
            tree.Config.DrawScrollView = true;

            return tree;
        }

        #endregion Class Methods
    }
}