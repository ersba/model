namespace EpidemicSpreadWithoutTensors.Model
{
    public static class Params
    {
        public static readonly int ChildUpperIndex = 1;
        
        public static readonly int AdultUpperIndex = 6;

        public static readonly float R0Value = 5.18f;
        
        public static readonly float MortalityRate = 0.1f;
        
        public static readonly float InitialInfectionRate = 0.05f;
        
        public static readonly int InfectedToRecoveredTime = 5;
        
        public static readonly int ExposedToInfectedTime = 3;
        
        public static readonly float EdgeAttribute = 1f;
        
        public static readonly int[] Mu = { 2, 4, 3 };
        
        public static readonly float[] Susceptibility = {0.35f, 0.69f, 1.03f, 1.03f, 1.03f, 1.03f, 1.27f, 1.52f};
        
        public static readonly float[] Infector = {0.0f, 0.33f, 0.72f, 0.0f, 0.0f};
    }
}