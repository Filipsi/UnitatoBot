using CSharpVerbalExpressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
