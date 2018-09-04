using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using The_Storyteller.Models;
using The_Storyteller.Models.MCharacter;
using The_Storyteller.Models.MMap;

namespace The_Storyteller.Entities.Tools
{
    internal class Resources
    {
        private readonly Dictionary<string, string> text;

        public Resources(string filename)
        {
            using (var sr = new StreamReader(filename))
            {
                text = JsonConvert.DeserializeObject<Dictionary<string, string>>(sr.ReadToEnd());
            }
        }

        public string GetString(string resourceName, Character character = null, Region region = null)
        {
            if (!text.TryGetValue(resourceName, out var result))
                return "Something went wrong, I forgot what I had to say ...";

            if (region != null)
            {
                result = result.Replace("$REGIONNAME", region.Name);
                switch (region.Type)
                {
                    case RegionType.Desert: result = result.Replace("$REGIONTYPE", "desert region"); break;
                    case RegionType.Mountain: result = result.Replace("$REGIONTYPE", "mountainous region"); break;
                    case RegionType.Plain: result = result.Replace("$REGIONTYPE", "grassy plain"); break;
                    case RegionType.Sea: result = result.Replace("$REGIONTYPE", "sea"); break;
                    default: result = result.Replace("$REGIONTYPE", "unknown region"); break;
                }

                result = result.Replace("$REGIONCOORDINATE", $"[{region.GetCentralCase().Location.XPosition};{region.GetCentralCase().Location.YPosition}]");

                if(region.GetCentralCase().IsAvailable && region.GetCentralCase().IsBuildable())
                    result = result.Replace("$REGIONAVAILABLE", "currently without inhabitants but ready to welcome a new village");
                else if (region.GetCentralCase().IsAvailable && !region.GetCentralCase().IsBuildable())
                    result = result.Replace("$REGIONAVAILABLE", "currently without inhabitants and will remain so due to the difficult living conditions there");
                else
                    result = result.Replace("$REGIONAVAILABLE", "currently governed by a village where inhabitants lead peaceful lives");
            }

            if (character != null)
            {
                result = result.Replace("$NAME", character.Name);

                if (character.Sex == Sex.Male)
                {
                    result = result.Replace("$SEXPRONOUN", "he");
                    result = result.Replace("$SEX", "gentleman");
                }
                else
                {
                    result = result.Replace("$SEXPRONOUN", "she");
                    result = result.Replace("$SEX", "young lady");
                }
            }

            return result;
        }
    }
}