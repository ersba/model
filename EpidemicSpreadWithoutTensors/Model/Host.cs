using Mars.Interfaces.Agents;
using Mars.Interfaces.Annotations;
using Mars.Interfaces.Layers;

namespace EpidemicSpreadWithoutTensors.Model 
{
    public class Host : IAgent<InfectionLayer>
    {
        [PropertyDescription]
        public int Index { get; set; }
        
        [PropertyDescription]
        public int MyAgeGroup { get; set; }
        
        [PropertyDescription]
        public int MyStage { get; set; }
        
        [PropertyDescription] 
        public UnregisterAgent UnregisterHandle { get; set; }
        
        public int InfectedTime { get; set; }
        
        private int _meanInteractions;
        
        private int _nextStageTime;

        private int _infinityTime;

        private bool _exposedToday;

        private float _susceptibility;

        public void Init(InfectionLayer layer)
        {
            _infectionLayer = layer;
            _infectionLayer.ContactEnvironment.Insert(this);
            _infinityTime = Params.Steps + 1;
            InitStage();
            InitTimeVariables();
            InitMeanInteractions();
            _susceptibility = Params.Susceptibility[MyAgeGroup];
        }

        public void Tick()
        {
            _exposedToday = false;
            Interact();
            if (MyStage != (int)Stage.Recovered) Progress();
            if (MyStage == (int)Stage.Mortality) Die();
        }

        private void InitStage()
        {
            Random random = new Random();
            if (random.NextDouble() < Params.InitialInfectionRate) MyStage = (int)Stage.Infected;
        }

        private void InitMeanInteractions()
        {
            var childAgent = MyAgeGroup <= Params.ChildUpperIndex;
            var adultAgent = MyAgeGroup > Params.ChildUpperIndex && MyAgeGroup <= Params.AdultUpperIndex;
            var elderAgent = MyAgeGroup > Params.AdultUpperIndex;
            
            if (childAgent) _meanInteractions = Params.Mu[0];
            else if (adultAgent) _meanInteractions = Params.Mu[1];
            else if (elderAgent) _meanInteractions = Params.Mu[2];
        }

        private void InitTimeVariables()
        {
            switch (MyStage)
            {   
                case (int) Stage.Susceptible:
                    InfectedTime = _infinityTime;
                    _nextStageTime = _infinityTime;
                    break;
                case (int)Stage.Exposed:
                    InfectedTime = 0;
                    _nextStageTime = Params.Steps + Params.ExposedToInfectedTime;
                    break;
                case (int)Stage.Infected:
                    InfectedTime = 1 - Params.ExposedToInfectedTime;
                    _nextStageTime = 1 + Params.InfectedToRecoveredTime;
                    break;
                case (int)Stage.Recovered:
                    InfectedTime = _infinityTime;
                    _nextStageTime = _infinityTime;
                    break;
                case (int)Stage.Mortality:
                    InfectedTime = _infinityTime;
                    _nextStageTime = _infinityTime;
                    break;
            }
        }

        private void Interact()
        {
            if(MyStage == (int) Stage.Susceptible){
                foreach (Host host in _infectionLayer.ContactEnvironment.GetNeighbors(Index))
                {
                    if (host.MyStage is (int) Stage.Infected or (int) Stage.Exposed)
                    {
                        var infector = Params.Infector[host.MyStage];
                        var bN = Params.EdgeAttribute;
                        var integral =
                            LamdaGammaIntegrals.Integrals[
                                Math.Abs(_infectionLayer.GetCurrentTick() - host.InfectedTime)];
                        var result = Params.R0Value * _susceptibility * infector * bN * integral / _meanInteractions;

                        Random random = new Random();
                        if (!(random.NextDouble() < result)) continue;
                        _exposedToday = true;
                        return;
                    }
                }
            }
        }

        private void Die()
        {
            // UnregisterHandle.Invoke(_infectionLayer, this);
        }

        private void Progress()
        {
            var nextStage = UpdateStage();
            UpdateNextStageTime();
            MyStage = nextStage;
            if (_exposedToday) InfectedTime = (int)_infectionLayer.GetCurrentTick();
        }

        private void UpdateNextStageTime()
        {
            if (_exposedToday)
            {
                _nextStageTime = (int) (_infectionLayer.GetCurrentTick() + 1
                                                                         + Params.ExposedToInfectedTime);
            } else if (_nextStageTime == _infectionLayer.GetCurrentTick())
            {
                if (MyStage == (int) Stage.Exposed)
                    _nextStageTime = (int) (_infectionLayer.GetCurrentTick()
                                            + Params.InfectedToRecoveredTime);
                else
                    _nextStageTime = _infinityTime;
            }
        }

        private int UpdateStage()
        {
            if (_exposedToday) return (int)Stage.Exposed;
            switch (MyStage)
            {
                case (int)Stage.Susceptible:
                    return (int)Stage.Susceptible;
                case (int)Stage.Exposed:
                    if (_infectionLayer.GetCurrentTick() >= _nextStageTime) return (int)Stage.Infected;
                    return (int) Stage.Exposed;
                case (int)Stage.Infected:
                    if (_infectionLayer.GetCurrentTick() >= _nextStageTime)
                    {
                        Random random = new Random();
                        if (random.NextDouble() < Params.MortalityRate) return (int)Stage.Mortality;
                        return (int)Stage.Recovered;
                    }
                    return (int)Stage.Infected;
                case (int)Stage.Recovered:
                    return (int)Stage.Recovered;
            }
            return (int)Stage.Mortality;
        }

        private InfectionLayer _infectionLayer;

        public Guid ID { get; set; }
    }
    
    
}