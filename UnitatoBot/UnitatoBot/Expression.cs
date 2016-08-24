using CSharpVerbalExpressions;

namespace UnitatoBot {

    public static class Expression {

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

        public static VerbalExpressions CommandArgumentParser = new VerbalExpressions()
            .AddModifier('m')
            .AddModifier('s')
            .StartOfLine()
            .SomethingBut(" ")
            .Then(" ")
            .BeginCapture("args")
            .Something()
            .EndCapture();

    }

}
