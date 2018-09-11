using System;
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

        public string GetString(string resourceName, Character character = null, 
            Region region = null, Case mCase = null)
        {
            if (!text.TryGetValue(resourceName, out var result))
                return "Something went wrong, I forgot what I had to say ...";

            if (region != null)
            {
                result = ReplaceRegionData(region, result);
            }

            if (character != null)
            {
                result = ReplaceCharacterData(character, result);
            }

            if(mCase != null)
            {
                result = ReplaceCaseData(mCase, result);
            }

            return result;
        }

        private string ReplaceCaseData(Case mCase, string result)
        {
            switch (mCase.Type)
            {
                case CaseType.Desert: result = result.Replace("$CASETYPE", "desert"); break;
                case CaseType.Mountain: result = result.Replace("$CASETYPE", "mountainous"); break;
                case CaseType.Land: result = result.Replace("$CASETYPE", "land"); break;
                case CaseType.Forest: result = result.Replace("$CASETYPE", "forest"); break;
                case CaseType.Water: result = result.Replace("$CASETYPE", "aquatic"); break;
                default: result = result.Replace("$CASETYPE", "unknown"); break;
            }

            return result;
        }

        private static string ReplaceCharacterData(Character character, string result)
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

            return result;
        }

        private static string ReplaceRegionData(Region region, string result)
        {
            if(region.Name != null)
                result = result.Replace("$REGIONNAME", region.Name);

            
            switch (region.Type)
            {
                case RegionType.Desert: result = result.Replace("$REGIONTYPE", "desert region"); break;
                case RegionType.Mountain: result = result.Replace("$REGIONTYPE", "mountainous region"); break;
                case RegionType.Plain: result = result.Replace("$REGIONTYPE", "verdant plain"); break;
                case RegionType.Sea: result = result.Replace("$REGIONTYPE", "sea"); break;
                default: result = result.Replace("$REGIONTYPE", "unknown region"); break;
            }

            
            if (region.GetCentralCase() != null)
            {
                result = result.Replace("$REGIONCOORDINATE", $"[{region.GetCentralCase().Location.XPosition};{region.GetCentralCase().Location.YPosition}]");

                if (region.GetCentralCase().IsAvailable && region.GetCentralCase().IsBuildable())
                    result = result.Replace("$REGIONAVAILABLE", "currently without inhabitants but ready to welcome a new village");
                else if (region.GetCentralCase().IsAvailable && !region.GetCentralCase().IsBuildable())
                    result = result.Replace("$REGIONAVAILABLE", "currently without inhabitants and will remain so due to the difficult living conditions there");
                else
                    result = result.Replace("$REGIONAVAILABLE", "currently governed by a village where inhabitants lead peaceful lives");
            }
            
            
            return result;
        }

        /// <summary>
        /// Enlève les charactères relatif au markdown sur le nom de région
        /// </summary>
        /// <param name="regionName"></param>
        /// <returns></returns>
        public string RemoveMarkdown(string regionName)
        {
            regionName = regionName.Replace("*", "");
            regionName = regionName.Replace("`", "");
            regionName = regionName.Replace("_", "");

            return regionName;
        }
    }
}