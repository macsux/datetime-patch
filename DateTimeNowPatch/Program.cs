using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Harmony;

namespace DateTimeNowPatch
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"Before patch: {DateTime.Now}");
            var harmony = HarmonyInstance.Create("com.github.harmony.rimworld.mod.example");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            Console.WriteLine($"After patch: {DateTime.Now}");
            Console.ReadLine();
        }
    }

    [Harmony]
    class Patch
    {
        static MethodBase TargetMethod() => AccessTools.Property(typeof(DateTime), nameof(DateTime.Now)).GetMethod;
        
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instr) =>
            new[]
            {
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Patch), nameof(MyCustomDateTime))),
                new CodeInstruction(OpCodes.Ret)
            };

        public static DateTime MyCustomDateTime() =>
            TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time"));
    }
}
