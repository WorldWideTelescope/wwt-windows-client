using MicrosoftInternal.AdvancedCollections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraViewer
{
    public class Catalogs
    {

        public static TreeDictionary<string, IPlace> AutoCompleteList = new TreeDictionary<string, IPlace>(true);

        public static IPlace FindCatalogObject(string name)
        {
            InitSearchTable();
            foreach (KeyValuePair<string, IPlace> kv in AutoCompleteList.StartFromKey(name.ToLower(), TraversalStartingPoint.EqualOrMore, TraversalDirection.LowToHigh))
            {
                if (kv.Key.ToLower().Replace(" ", "") == name.ToLower().Replace(" ", ""))
                {
                    if (kv.Value.StudyImageset == null)
                    {
                        return kv.Value;
                    }
                }

            }
            return null;


        }

        public static IPlace FindCatalogObjectExact(string name)
        {
            InitSearchTable();

            string shortName = name.ToLower().Replace(" ", "");

            foreach (KeyValuePair<string, IPlace> kv in AutoCompleteList.StartFromKey(name.ToLower(), TraversalStartingPoint.EqualOrMore, TraversalDirection.LowToHigh))
            {
                IPlace place = kv.Value as IPlace;

                if (place != null && place.Name.ToLower().Replace(" ", "") == shortName)
                {
                    //if (kv.Value.StudyImageset == null)
                    {
                        return kv.Value;
                    }
                }

            }

            foreach (KeyValuePair<string, IPlace> kv in AutoCompleteList.StartFromKey(name.ToLower(), TraversalStartingPoint.EqualOrMore, TraversalDirection.LowToHigh))
            {
                if (kv.Key.ToLower().Replace(" ", "") == shortName)
                {
                    if (kv.Value.StudyImageset == null)
                    {
                        return kv.Value;
                    }
                }

            }

            foreach (KeyValuePair<string, IPlace> kv in AutoCompleteList.StartFromKey(name.ToLower(), TraversalStartingPoint.EqualOrMore, TraversalDirection.LowToHigh))
            {
                if (kv.Key.ToLower().Replace(" ", "").StartsWith(shortName))
                {
                    //if (kv.Value.StudyImageset == null)
                    {
                        return kv.Value;
                    }
                }

            }
            return null;


        }

        public static bool Initializing = false;
        public static void InitSearchTable()
        {
            if (!searchTableInitialized && !Initializing)
            {
                LoadSearchTable();
            }
        }

        static bool searchTableInitialized = false;
        static System.Threading.Mutex LoadSearchMutex = new System.Threading.Mutex();
        static public void LoadSearchTable()
        {
            Initializing = true;
            try
            {
                LoadSearchMutex.WaitOne();
                if (searchTableInitialized)
                {
                    return;
                }
                foreach (string abreviation in ContextSearch.constellationObjects.Keys)
                {
                    foreach (IPlace place in ContextSearch.constellationObjects[abreviation])
                    {
                        try
                        {
                            foreach (string name in place.Names)
                            {
                                AddParts(name, place);
                            }

                            if (place.Classification != Classification.Unidentified)
                            {
                                AddParts(place.Classification.ToString(), place);
                            }
                        }
                        catch
                        {
                        }
                    }
                }
            }
            catch
            {
            }
            finally
            {
                searchTableInitialized = true;
                LoadSearchMutex.ReleaseMutex();
            }
            Initializing = false;
        }

        public static void AddParts(string key, IPlace place)
        {

            key = key.ToLower();
            AutoCompleteList.Add(key, place);
            string[] parts = key.Split(new char[] { ' ' });
            if (parts.Length > 1)
            {
                foreach (string part in parts)
                {
                    if (!string.IsNullOrEmpty(part))
                    {
                        AutoCompleteList.Add(part, place);
                    }
                }
            }
        }

        public static string CleanSearchString(string searchString)
        {
            searchString = searchString.ToLower();

            if (searchString.Contains("ngc "))
            {
                for (int i = 0; i < 10; i++)
                {
                    searchString = searchString.Replace(string.Format("ngc {0}", i), string.Format("ngc{0}", i));
                }
            }

            if (searchString.Contains("ic "))
            {
                for (int i = 0; i < 10; i++)
                {
                    searchString = searchString.Replace(string.Format("ic {0}", i), string.Format("ic{0}", i));
                }
            }
            if (searchString.Contains("hr "))
            {
                for (int i = 0; i < 10; i++)
                {
                    searchString = searchString.Replace(string.Format("ic {0}", i), string.Format("ic{0}", i));
                }
            }
            if (searchString.Contains("hd "))
            {
                for (int i = 0; i < 10; i++)
                {
                    searchString = searchString.Replace(string.Format("ic {0}", i), string.Format("ic{0}", i));
                }
            }
            if (searchString.Contains("sao "))
            {
                for (int i = 0; i < 10; i++)
                {
                    searchString = searchString.Replace(string.Format("ic {0}", i), string.Format("ic{0}", i));
                }
            }
            return searchString;
        }
    }
}
