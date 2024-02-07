using MathNet.Numerics.Distributions;

namespace EpidemicSpreadWithoutTensors
{
    public static class LamdaGammaIntegrals
    {
        public static float[] Integrals { get; private set; }
        
        public static void SetLamdaGammaIntegrals()
        {
            var scale = 5.15;
            var rate = 2.14;
            var b = rate * rate / scale;
            var a = scale / b;
            var res = new List<float>();

            for (int t = 1; t <= Params.Steps + 10; t++)
            {
                var cdfAtTimeT = Gamma.CDF(a, b, t);
                var cdfAtTimeTMinusOne = Gamma.CDF(a, b, t - 1);
                res.Add((float)(cdfAtTimeT - cdfAtTimeTMinusOne));
            }
            Integrals = res.ToArray();
        }
    }
}