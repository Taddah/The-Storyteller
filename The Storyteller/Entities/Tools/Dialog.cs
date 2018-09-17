using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using The_Storyteller.Models.MCharacter;
using The_Storyteller.Models.MMap;
using The_Storyteller.Models.MVillage;

namespace The_Storyteller.Entities.Tools
{
    /// <summary>
    /// Classe de ressource pour les strings
    /// </summary>
    internal class Dialog
    {
        private readonly Dictionary<string, string> text;

        public Dialog(string filename)
        {
            using (var sr = new StreamReader(filename))
            {
                text = JsonConvert.DeserializeObject<Dictionary<string, string>>(sr.ReadToEnd());
            }
        }

        /// <summary>
        /// Récupère un string par son nom depuis le fichier json ou ils sont sauvegardé
        /// Remplace les variables $NAME par la valeur à afficher
        /// </summary>
        /// <param name="resourceName">Nom du string à récupérer</param>
        /// <param name="character">Optionnel. Pour afficher infos relative aux Character</param>
        /// <param name="region">Optionnel. Pour afficher infos relative aux Region</param>
        /// <param name="mCase">Optionnel. Pour afficher infos relative aux Case</param>
        /// <returns></returns>
        public string GetString(string resourceName, Character character = null, 
            Region region = null, Case mCase = null, Village village = null)
        {
            if (!text.TryGetValue(resourceName, out var result))
                return "Something went wrong, I forgot what I had to say ...";

            result = result.Replace("$PREFIX", Config.Instance.Prefix);

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

            if(village != null)
            {
                result = ReplaceVillageData(village, result);
            }

            return result;
        }

        /// <summary>
        /// Remplace les données de type Case
        /// </summary>
        /// <param name="mCase"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private string ReplaceCaseData(Case mCase, string result)
        {
            switch (mCase.Type)
            {
                case CaseType.Desert: result = result.Replace("$CASE_TYPE", "desert"); break;
                case CaseType.Mountain: result = result.Replace("$CASE_TYPE", "mountainous"); break;
                case CaseType.Land: result = result.Replace("$CASE_TYPE", "land"); break;
                case CaseType.Forest: result = result.Replace("$CASE_TYPE", "forest"); break;
                case CaseType.Water: result = result.Replace("$CASE_TYPE", "aquatic"); break;
                default: result = result.Replace("$CASE_TYPE", "unknown"); break;
            }

            return result;
        }

        /// <summary>
        /// Remplace les données de type Character
        /// </summary>
        /// <param name="character"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private static string ReplaceCharacterData(Character character, string result)
        {
            if(character.Name != null)
                result = result.Replace("$CHARACTER_NAME", character.Name);

            if (character.Sex == Sex.Male)
            {
                result = result.Replace("$CHARACTER_SEXPRONOUN", "he");
                result = result.Replace("$CHARACTER_SEX", "gentleman");
            }
            else
            {
                result = result.Replace("$CHARACTER_SEXPRONOUN", "she");
                result = result.Replace("$CHARACTER_SEX", "young lady");
            }

            if(character.OriginRegionName != null)
            {
                result = result.Replace("CHARACTER_ORIGINEREGION", character.OriginRegionName);
            }

            return result;
        }

        /// <summary>
        /// Remplace les données de type Region
        /// </summary>
        /// <param name="region"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private static string ReplaceRegionData(Region region, string result)
        {
            if(region.Name != null)
                result = result.Replace("$REGION_NAME", region.Name);

            
            switch (region.Type)
            {
                case RegionType.Desert: result = result.Replace("$REGION_TYPE", "desert region"); break;
                case RegionType.Mountain: result = result.Replace("$REGION_TYPE", "mountainous region"); break;
                case RegionType.Plain: result = result.Replace("$REGION_TYPE", "verdant plain"); break;
                case RegionType.Sea: result = result.Replace("$REGION_TYPE", "sea"); break;
                default: result = result.Replace("$REGION_TYPE", "unknown region"); break;
            }

            
            if (region.GetCentralCase() != null)
            {
                result = result.Replace("$REGION_COORDINATE", $"[{region.GetCentralCase().Location.XPosition};{region.GetCentralCase().Location.YPosition}]");

                if (region.GetCentralCase().IsAvailable && region.GetCentralCase().IsBuildable())
                    result = result.Replace("$REGION_AVAILABLE", "currently without inhabitants but ready to welcome a new village");
                else if (region.GetCentralCase().IsAvailable && !region.GetCentralCase().IsBuildable())
                    result = result.Replace("$REGION_AVAILABLE", "currently without inhabitants and will remain so due to the difficult living conditions there");
                else
                    result = result.Replace("$REGION_AVAILABLE", "currently governed by a village where inhabitants lead peaceful lives");
            }
            
            
            return result;
        }

        /// <summary>
        /// Remplace les données de type Village
        /// </summary>
        /// <param name="village"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private static string ReplaceVillageData(Village village, string result)
        {
            if (village.Name != null)
                result = result.Replace("$VILLAGE_NAME", village.Name);
            
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
            regionName = regionName.Replace("/", "");
            regionName = regionName.Replace("\\", "");

            return regionName;
        }
    }
}