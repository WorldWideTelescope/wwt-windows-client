using System;
using System.Collections.Generic;

namespace TerraViewer
{
    public class SearchCriteria
    {
        public SearchCriteria(string searchString)
        {
            searchString = searchString.Trim().ToLower();
            Target = searchString;
            MagnitudeMin = -100.0;
            MagnitudeMax = 100.0;




            List<string> keywords = new List<string>(searchString.ToLower().Split(new char[] { ' ' }));
            if (keywords.Count > 1)
            {
                for (int i = keywords.Count - 1; i > -1; i--)
                {
                    if (keywords[i] == ">" && i > 0 && i < keywords.Count - 1)
                    {
                        if (keywords[i - 1] == "m" || keywords[i - 1] == "mag" || keywords[i - 1] == "magnitude")
                        {
                            try
                            {
                                MagnitudeMin = Convert.ToDouble(keywords[i + 1]);
                            }
                            catch
                            {
                            }
                            keywords.RemoveAt(i + 1);
                            keywords.RemoveAt(i);
                            keywords.RemoveAt(i - 1);
                            i -= 2;
                            continue;
                        }
                    }

                    if (keywords[i] == "<" && i > 0 && i < keywords.Count - 2)
                    {
                        if (keywords[i - 1] == "m" || keywords[i - 1] == "mag" || keywords[i - 1] == "magnitude")
                        {
                            try
                            {
                                MagnitudeMax = Convert.ToDouble(keywords[i + 1]);
                            }
                            catch
                            {
                            }
                            keywords.RemoveAt(i + 1);
                            keywords.RemoveAt(i);
                            keywords.RemoveAt(i - 1);
                            i -= 2;
                            continue;
                        }
                    }
                    bool brokeOut = false;

                    foreach (string classId in Enum.GetNames(typeof(Classification)))
                    {
                        if (keywords[i] == classId.ToLower())
                        {
                            Classification |= (Classification)Enum.Parse(typeof(Classification), classId);
                            keywords.RemoveAt(i);
                            brokeOut = true;
                            break;
                        }

                    }

                    if (brokeOut)
                    {
                        continue;
                    }

                    if (Constellations.Abbreviations.ContainsKey(keywords[i].ToUpper()))
                    {
                        Constellation = keywords[i].ToUpper();
                        keywords.RemoveAt(i);
                        continue;
                    }

                    if (Constellations.FullNames.ContainsKey(keywords[i].ToUpper()))
                    {
                        Constellation = Constellations.Abbreviation(keywords[i].ToUpper());
                        keywords.RemoveAt(i);
                        continue;
                    }
                }
                //keywords.Add(searchString);
                Keywords = keywords;
                string spacer = "";
                Target = "";
                foreach (string keyword in Keywords)
                {
                    Target += spacer + keyword;
                    spacer = " ";
                }
            }
            else
            {
                Keywords = null;
            }

            if (Classification == 0)
            {
                Classification = Classification.Unfiltered;
            }
        }
        public string Target;
        public string Constellation;
        public List<string> Keywords;
        public Classification Classification = 0;
        public double MagnitudeMin;
        public double MagnitudeMax;
    }
}
