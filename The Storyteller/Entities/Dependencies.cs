using System.Threading;
using DSharpPlus.Interactivity;
using The_Storyteller.Entities.Tools;

namespace The_Storyteller.Entities
{
    internal class Dependencies
    {
        internal InteractivityModule Interactivity { get; set; }
        internal CancellationTokenSource Cts { get; set; }
        internal EmbedGenerator Embed { get; set; }
        internal Resources Resources { get; set; }

        ///////////////////////////////////////////
        internal EntityManager Entities { get; set; }
    }
}