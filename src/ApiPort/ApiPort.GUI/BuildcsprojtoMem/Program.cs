﻿using BuildcsprojtoMem;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Locator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MSBuildAnalyzer
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            MSBuildLocator.RegisterDefaults();
            if (args.Length != 0)
            {
                string csProjPath = args[0];
                if (args.Length == 1)
                {
                    Temp.BuildIt(csProjPath);
                }

                if (args.Length > 1)
                {
                    string chosenConfig = args[1];
                    string chosenPlat = args[2];
                    Chosen.Configure(csProjPath, chosenConfig, chosenPlat);
                }
            }
        }
    }

    internal class Temp
    {
        private static List<string> configurations;
        private static List<string> platforms;

        public static List<string> Configurations { get => configurations; set => configurations = value; }

        public static List<string> Platforms { get => platforms; set => platforms = value; }

        public static void BuildIt(string csProjPath)
        {
            const string box1 = "Configuration";
            const string box2 = "Platform";
            Dictionary<string, string> dic = new Dictionary<string, string>
                {
                    { "Configuration", "Debug" },
                    { "Platform", "AnyCPU" }
                };
            ProjectCollection pc = new ProjectCollection(dic, null, ToolsetDefinitionLocations.Default);

            var project = pc.LoadProject(csProjPath);

            project.Build();
            Message(csProjPath);

       
                Configurations = project.ConditionedProperties[box1];
                Platforms = project.ConditionedProperties[box2];
                var con = new string[0];
                var pla = new string[0];
                Console.Write("Config:");
                foreach (var config in Configurations)
                {
                    con.Append(config);
                    Console.Write(" **" + config);
                }

                Console.WriteLine(" ");
                Console.Write("Plat:");
                foreach (var plat in Platforms)
                {
                    pla.Append(plat);
                    Console.Write(" **" + plat);
                }

                Console.WriteLine(" ");
                Console.Write("Assembly:");
                if (project.Properties.Any(n => n.Name == "TargetPath"))
                {
                    var myPath = System.Reflection.Assembly.GetEntryAssembly().Location;
                    var targetPath = project.GetProperty("TargetPath");
                    var targetPathString = targetPath.EvaluatedValue.ToString();
                    var assembly = Assembly.LoadFrom(targetPathString);
                    foreach (AssemblyName assemblyName in assembly.GetReferencedAssemblies())
                    {
                        Console.Write(" **" + Assembly.Load(assemblyName));
                    }

                    // Console.Write(" **" + assembly);
                }
            }
 
        public static void Message(string csProjPath)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>
                {
                    { "Configuration", "Debug" },
                    { "Platform", "AnyCPU" }
                };
            ProjectCollection pc = new ProjectCollection(dic, null, ToolsetDefinitionLocations.Default);

            var project = pc.LoadProject(csProjPath);
            Console.WriteLine("Build: {0}",project.Build());
        }
    }
}