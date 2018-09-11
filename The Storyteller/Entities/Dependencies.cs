using System.Threading;
using DSharpPlus.Interactivity;
using The_Storyteller.Entities.Tools;

namespace The_Storyteller.Entities
{
    /// <summary>
    /// Dépendance injecté dans Commands accessible depuis toutes les commandes
    /// Entities contient l'accès à tous les "Managers" (entité du jeu)
    /// </summary>
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