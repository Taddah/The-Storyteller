using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using The_Storyteller.Entities;

namespace The_Storyteller.Commands
{
    internal class Test
    {
        private readonly Dependencies dep;

        public Test(Dependencies d)
        {
            dep = d;
        }

        [Command("gen")]
        public async Task Confirmation(CommandContext ctx, string name)
        {
            var reg = dep.Entities.Map.GenerateNewRegion(9, ctx.Guild.Id, name, dep.Entities.Map.GetRandomRegionType());
            await ctx.RespondAsync($"region generated in {reg.GetCentralCase().Location.ToString()}");
        }
    }
}