﻿using System;
using System.IO;
using EpidemicSpreadWithoutTensors.Model;
using Mars.Components.Starter;
using Mars.Interfaces.Model;


namespace EpidemicSpreadWithoutTensors
{
    internal static class Program
    {
        private static void Main()
        {
            var description = new ModelDescription();
            
            description.AddLayer<InfectionLayer>();
            
            description.AddAgent<Host, InfectionLayer>();
            
            // use config.json 
            var file = File.ReadAllText("config.json");
            var config = SimulationConfig.Deserialize(file);
            Params.Steps = (int)(config.Globals.Steps?? 0);
            Params.AgentCount = config.AgentMappings[0].InstanceCount ?? 0;
            var starter = SimulationStarter.Start(description, config);
            var handle = starter.Run();
            Console.WriteLine("Successfully executed iterations: " + handle.Iterations);
            
            starter.Dispose();
        }
    }
}