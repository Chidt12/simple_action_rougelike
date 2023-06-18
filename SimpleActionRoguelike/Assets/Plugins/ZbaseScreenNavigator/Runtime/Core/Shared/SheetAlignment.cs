using System;
using UnityEngine;

namespace ZBase.UnityScreenNavigator.Core
{
    public enum SheetAlignment
    {
        Left,
        Top,
        Right,
        Bottom,
        Center
    }

    internal static class SheetAlignmentExtensions
    {
        public static Vector3 ToPosition(this SheetAlignment self, RectTransform rectTransform)
        {
            var rect = rectTransform.rect;
            var width = rect.width;
            var height = rect.height;
            var z = rectTransform.localPosition.z;

            switch (self)
            {
                case SheetAlignment.Left: return new Vector3(-width, 0, z);
                case SheetAlignment.Top: return new Vector3(0, height, z);
                case SheetAlignment.Right: return new Vector3(width, 0, z);
                case SheetAlignment.Bottom: return new Vector3(0, -height, z);
                case SheetAlignment.Center: return new Vector3(0, 0, z);
            }

            throw new ArgumentOutOfRangeException(nameof(self), self, null);
        }
    }
}