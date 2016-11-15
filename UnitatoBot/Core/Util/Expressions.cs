using CSharpVerbalExpressions;

namespace BotCore.Util {

    public static class Expressions {

        public static VerbalExpressions CommandParser = new VerbalExpressions()
            .StartOfLine()
            .Then("!")
            .BeginCapture("ExecutionDispacher")
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
