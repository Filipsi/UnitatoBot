using CSharpVerbalExpressions;

namespace UnitatoBot {

    public static class Expressions {

        public static VerbalExpressions CommandParser = new VerbalExpressions()
            .StartOfLine()
            .Then("!")
            .BeginCapture("command")
            .SomethingBut(" ")
            .EndCapture()
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
