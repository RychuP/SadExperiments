using SadRogue.Primitives;

namespace SadExperimentsV9.TestConsoles
{
    internal class Workshop : TestConsole
    {
        public Workshop()
        {
            var handle = Activator.CreateInstanceFrom("SadExperimentsV9.dll", "Donut3d");
            var obj = handle?.Unwrap();
        }
    }
}
