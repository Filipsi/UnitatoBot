using CSharpVerbalExpressions;

namespace UnitatoBot {

    public static class Expressions {

        public static VerbalExpressions CommandParser = new VerbalExpressions()
            .StartOfLine()
            .AnyOf("/:")
            .BeginCapture("command")
            .SomethingBut(" ")
            .EndCapture()
            .Maybe(":")
            .Maybe(" ")
            .Anything()
            .EndOfLine();

        public static VerbalExpressions CommandArgsParser = new VerbalExpressions()
            .StartOfLine()
            .SomethingBut(" ")
            .Then(" ")
            .BeginCapture("args")
            .Something()
            .Maybe(" ")
            .Anything()
            .Maybe(" ")
            .EndCapture()
            .EndOfLine();

    }

}
