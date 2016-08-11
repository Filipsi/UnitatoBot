using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitatoBot {

    public static class SymbolFactory {

        public enum Emoji {
            Checklist,
            BoxUnchecked,
            BoxChecked,
            Tada
        }

        public enum Emoticon {
            Magic,
            Dance,
            Praise,
            Greet,
            Pleased,
            WaitWhat
        }

        public static string AsString(Emoji emoji) {
            switch(emoji) {
                case Emoji.Checklist:
                    return ":pencil:";
                case Emoji.BoxUnchecked:
                    return ":white_large_square:";
                case Emoji.BoxChecked:
                    return ":white_check_mark:";
                case Emoji.Tada:
                    return ":tada:";
            }

            return string.Empty;
        }

        public static string AsString(Emoticon emoji) {
            switch(emoji) {
                case Emoticon.Magic:
                    return "(ﾉ◕ヮ◕)ﾉ*:・ﾟ✧";
                case Emoticon.Dance:
                    return "（〜^∇^)〜";
                case Emoticon.Praise:
                    return "ヽ(´▽｀)ノ";
                case Emoticon.Greet:
                    return "(•̀ᴗ•́)و";
                case Emoticon.Pleased:
                    return "(　＾∇＾)";
                case Emoticon.WaitWhat:
                    return "(⊙.☉)7";
            }

            return string.Empty;
        }

    }

}
