using System;
using System.IO;
using Hyena.SExpEngine;

public class SExpEvaluator
{
    //public static void Main(string [] args)
    //{
    //    using(var stream = new FileStream(args[0], FileMode.Open)) {
    //        using(var reader = new StreamReader(stream)) {
    //            var evaluator = new Evaluator();
				//var result = evaluator.EvaluateString(reader.ReadToEnd()).Flatten();

    //            if(evaluator.Success) {
    //                Console.WriteLine("[[=== Result Tree ===]]");
    //                if(!result.Empty) {
    //                    result.Dump();
    //                } else {
    //                    Console.WriteLine("No result");
    //                }
    //            } else {
    //                Console.WriteLine(evaluator.ErrorMessage);
    //                foreach(var exception in evaluator.Exceptions) {
    //                    Console.WriteLine(exception.Message);
    //                }
    //            }
	   //    }
    //    }
    //}
}
