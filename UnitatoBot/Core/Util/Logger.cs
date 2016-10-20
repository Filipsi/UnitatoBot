using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotCore.Util {

    public static class Logger {

        private static StringBuilder Builder = new StringBuilder();
        private static byte Depth = 0;

        public static void Log(string entry, params object[] args) {
            Print(ConsoleColor.Gray, entry, args);
        }

        public static void Info(string entry, params object[] args) {
            Print(ConsoleColor.DarkGray, entry, args);
        }

        public static void Warn(string entry, params object[] args) {
            Print(ConsoleColor.Yellow, "[Warning] " + entry, args);
        }

        public static void Error(string entry, params object[] args) {
            Print(ConsoleColor.Red, "[Error] " + entry, args);
        }

        private static void Print(ConsoleColor color, string entry, params object[] args) {
            WriteLineColored(GetDepthIndentation() + string.Format(entry, args), color);
        }

        public static void SectionStart() {
            Depth++;
        }

        public static void SectionEnd() {
            if(Depth > 0)
                Depth--;
            else
                Warn("Logger: Wrong SectionEnd detected, Depth was not changed.");
        }

        // Util

        private static void WriteLineColored(string text, ConsoleColor color) {
            ConsoleColor last = Console.ForegroundColor;

            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ForegroundColor = last;
        }

        private static string GetDepthIndentation() {
            Builder.Clear();

            for(byte i = 0; i < Depth; i++) {
                Builder.Append(" ");
            }

            return Builder.ToString();
        }

    }

}
