using Mars.Components.Environments;
using Mars.Components.Layers;
using Mars.Core.Data;
using Mars.Interfaces.Annotations;
using Mars.Interfaces.Data;
using Mars.Interfaces.Layers;

namespace EpidemicSpreadWithoutTensors.Model

{
    public class InfectionLayer : AbstractLayer
    { 
        public ContactGraphEnvironment ContactEnvironment { get; private set; }
        
        public IAgentManager AgentManager { get; private set; }
        
        public override bool InitLayer(LayerInitData layerInitData, RegisterAgent registerAgentHandle,
            UnregisterAgent unregisterAgentHandle)
        {
            var initiated = base.InitLayer(layerInitData, registerAgentHandle, unregisterAgentHandle);
            ContactEnvironment = new ContactGraphEnvironment();
            AgentManager = layerInitData.Container.Resolve<IAgentManager>();
            AgentManager.Spawn<Host, InfectionLayer>().ToList();
            ContactEnvironment.ReadCSV();
            LamdaGammaIntegrals.SetLamdaGammaIntegrals();
            return initiated;
        }
    }
}