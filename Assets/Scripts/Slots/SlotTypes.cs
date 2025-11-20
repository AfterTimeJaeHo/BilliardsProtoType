using System;

namespace Aftertime.MyTinyStreamer.Slots
{
    // 슬롯 심볼 정의
    public enum SlotSymbol
    {
        None = 0,
        Shield = 1,
        Sword = 2,
        Magic = 3
    }

    public static class SlotSymbolHelper
    {
        // 심볼 표시용 텍스트 반환 (한글 아이콘 느낌)
        public static string GetDisplayText(SlotSymbol symbol)
        {
            switch (symbol)
            {
                case SlotSymbol.Shield: return "방패";
                case SlotSymbol.Sword: return "칼";
                case SlotSymbol.Magic: return "마법";
                default: return "-";
            }
        }

        // 3종 중 하나를 균등 반환
        public static SlotSymbol NextRandom(System.Random rng)
        {
            int r = rng.Next(0, 3);
            if (r == 0) return SlotSymbol.Shield;
            if (r == 1) return SlotSymbol.Sword;
            return SlotSymbol.Magic;
        }
    }
}