using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;


namespace TerraViewer
{
    class ContextSearch
    {
        public ContextSearch()
        {
        }
        public static Dictionary<string, List<IPlace>> constellationObjects;
        public static void InitializeDatabase(bool sky)
        {
            constellationObjects = new Dictionary<string, List<IPlace>>();

            foreach (var abreviation in Constellations.FullNames.Keys)
            {
                constellationObjects.Add(abreviation, new List<IPlace>());

            }
            constellationObjects.Add("Mars", new List<IPlace>());
            constellationObjects.Add("Earth", new List<IPlace>());
            constellationObjects.Add("SolarSystem", new List<IPlace>());
            constellationObjects.Add("Constellations", new List<IPlace>());
            constellationObjects.Add("Community", new List<IPlace>());
        }

        public static void AddCatalogs(bool sky)
        {

            var dataSets = DataSetManager.GetDataSets();
            foreach (var d in dataSets.Values)
            {
                if (d.Sky == sky)
                {
                    if (d != null)
                    {
                        var placesList = d.GetPlaces();
                        foreach (var places in placesList.Values)
                        {
                            if (places != null)
                            {
                                var placeList = places.GetPlaceList();
                                foreach (IPlace place in placeList)
                                {
                                    if (place.StudyImageset != null && (place.StudyImageset.Projection == ProjectionType.Toast || place.StudyImageset.Projection == ProjectionType.Equirectangular))
                                    {
                                        continue;
                                    }
                                    AddPlaceToContextSearch(place);
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void AddFolderToSearch(Folder parent, bool sky)
        {

            foreach (var child in parent.Children)
            {
                if (child is IImageSet)
                {
                    var childImageset = (IImageSet)child;
                    if (Earth3d.ProjectorServer)
                    {
                        Earth3d.AddImageSetToTable(childImageset.GetHash(), childImageset);
                    }
                }
                if (child is IPlace)
                {
                    var place = (IPlace)child;
                    if (place.StudyImageset != null)
                    {
                        if (Earth3d.ProjectorServer)
                        {
                            Earth3d.AddImageSetToTable(place.StudyImageset.GetHash(), place.StudyImageset);
                        }
                    }
                    if (place.BackgroundImageSet != null)
                    {
                        if (Earth3d.ProjectorServer)
                        {
                            Earth3d.AddImageSetToTable(place.BackgroundImageSet.GetHash(), place.BackgroundImageSet);
                        }
                    }

                    if (place.StudyImageset != null && (place.StudyImageset.Projection == ProjectionType.Toast || place.StudyImageset.Projection == ProjectionType.Equirectangular))
                    {
                        continue;
                    }
                    AddPlaceToContextSearch(place);
                }
                if (child is Folder)
                {
                    AddFolderToSearch((Folder)child, sky);
                }
            }
        }

        public static void SetCommunitySearch(Folder searchFolder, bool clear)
        {
            if (clear)
            {
                constellationObjects["Community"].Clear();
            }
            constellationObjects["Community"].AddRange(searchFolder.Items);
        }

        public static void AddPlaceToContextSearch(IPlace place)
        {
            if (place.Classification == Classification.Constellation)
            {
                var constellationID = Constellations.Abbreviation(place.Name);
                place.Constellation = constellationID;
                constellationObjects["Constellations"].Add(place);
            }
            else if (place.Classification == Classification.SolarSystem)
            {
                var constellationID = Constellations.Containment.FindConstellationForPoint(place.RA, place.Dec);
                place.Constellation = constellationID;
                if (constellationObjects["SolarSystem"].Find(delegate(IPlace target) { return place.Name == target.Name; }) == null)
                {
                    constellationObjects["SolarSystem"].Add(place);
                }
            }
            else
            {
                var constellationID = "Error";

                if (place.Type == ImageSetType.Planet)
                {
                    if (place.Target == SolarSystemObjects.Undefined)
                    {
                        constellationID = "Mars";
                    }
                    else
                    {
                        constellationID = place.Target.ToString();
                    }
                }
                else if (place.Type == ImageSetType.Earth)
                {
                    constellationID = "Earth";
                }
                else
                {
                    constellationID = Constellations.Containment.FindConstellationForPoint(place.RA, place.Dec);
                }

                if (constellationID != "Error")
                {
                    place.Constellation = constellationID;
                    if (constellationObjects.ContainsKey(constellationID))
                    {
                        constellationObjects[constellationID].Add(place);
                    }
                }
            }
        }

        public static IPlace FindClosestMatch(string constellationID, double ra, double dec, double maxRadius)
        {
            if (!constellationObjects.ContainsKey(constellationID))
            {
                return null;
            }

            var tryIt = false;

            if (tryIt)
            {
                var data = DumpJSON();
            }

            var minDistance = 360.0 * 360.0;
            IPlace closestPlace = null;
            foreach (var place in constellationObjects[constellationID])
            {
                var test = ToJSON(place);


                var distanceRa = (ra - place.RA) * Math.Cos(dec / 180 * Math.PI) * 15;
                var distanceDec = (dec - place.Dec);
                var distanceSquared = (distanceDec * distanceDec) + (distanceRa * distanceRa);
                if (distanceSquared < minDistance)
                {
                    minDistance = distanceSquared;
                    closestPlace = place;
                }
            }

            foreach (var place in constellationObjects["SolarSystem"])
            {
                var distanceRa = (ra - place.RA) * Math.Cos(dec / 180 * Math.PI) * 15;
                var distanceDec = (dec - place.Dec);
                var distanceSquared = (distanceDec * distanceDec) + (distanceRa * distanceRa);
                if (distanceSquared < minDistance)
                {
                    minDistance = distanceSquared;
                    closestPlace = place;
                }
            }

            if ((maxRadius * maxRadius) > minDistance)
            {
                return closestPlace;
            }
            else
            {
                return null;
            }

        }

        public static string DumpJSON()
        {
            var sb = new StringBuilder();
            sb.AppendLine("{");
            sb.AppendLine("\"Constellations\" : [");
            var firstKey = true;
            foreach (var key in constellationObjects.Keys)
            {
                if (key.ToLower() != "mars" && constellationObjects[key].Count > 0)
                {

                    if (firstKey)
                    {
                        firstKey = false;
                    }
                    else
                    {
                        sb.Append(",");
                    }
                    sb.AppendLine("  {");
                    sb.AppendLine(string.Format(" \"{0}\" : \"{1}\"{2}", "name", key, ", \"places\": ["));
                    var firstLine = true;
                    foreach (var place in constellationObjects[key])
                    {
                        if (firstLine)
                        {
                            firstLine = false;
                        }
                        else
                        {
                            sb.AppendLine(",");
                        }
                        sb.Append(ToJSON(place));
                    }
                    sb.AppendLine("]");
                    sb.AppendLine("}");
                    //break;
                }
            }
            sb.AppendLine("]");
            sb.AppendLine("}");

            return sb.ToString();

        }

        public static string ToJSON(IPlace place)
        {
            var sb = new StringBuilder();

            sb.Append("  {");
            var names = "";
            var delim = "";
            foreach (var name in place.Names)
            {
                names += delim;
                names += name;
                delim = ";";
            }
            sb.Append(string.Format("{0}:\"{1}\"{2}", "n", names.Replace("\"", "'"), ","));
            sb.Append(string.Format("{0}:{1:###.####}{2}", "r", place.RA, ","));
            sb.Append(string.Format("{0}:{1:###.####}{2}", "d", place.Dec, ","));
            sb.Append(string.Format("{0}:{1}{2}", "c", (int)place.Classification, ","));
            sb.Append(string.Format("{0}:{1:0.#}{2}", "m", place.Magnitude, ","));
            sb.Append(string.Format("{0}:{1:###.#####}{2}", "z", place.ZoomLevel, ","));
            if (place.StudyImageset != null)
            {
                sb.Append(string.Format("{0}:{1}{2}", "fgi", ToJSON(place.StudyImageset), ","));
            }
            if (place.BackgroundImageSet != null)
            {
                sb.Append(string.Format("{0}:{1}{2}", "bgi", ToJSON(place.BackgroundImageSet), ","));
            }
            sb.Append(string.Format("{0}:{1}{2}", "i", (int)place.Type, ""));

            sb.Append("}");


            return sb.ToString();

        }

        public static string ToJSON(IImageSet imageset)
        {
            var sb = new StringBuilder();

            sb.Append("  {");

            sb.Append(string.Format("{0}:\"{1}\"{2}", "name", imageset.Name, ","));
            //   sb.Append(string.Format("{0}:{1:###.####}{2}", "r", place.RA, ","));

            // sb.Append(string.Format("{0}:\"{1}\"{2}", "DemUrl", imageset.DemUrl, ","));
            sb.Append(string.Format("{0}:\"{1}\"{2}", "bandPass", imageset.BandPass, ","));
            sb.Append(string.Format("{0}:\"{1}\"{2}", "baseLevel", imageset.BaseLevel, ","));
            sb.Append(string.Format("{0}:\"{1}\"{2}", "baseTileDegrees", imageset.BaseTileDegrees, ","));
            sb.Append(string.Format("{0}:\"{1}\"{2}", "bottomsUp", imageset.BottomsUp ? "true" :"false", ","));

            sb.Append(string.Format("{0}:\"{1}\"{2}", "centerX", imageset.CenterX, ","));
            sb.Append(string.Format("{0}:\"{1}\"{2}", "centerY", imageset.CenterY, ","));
            sb.Append(string.Format("{0}:\"{1}\"{2}", "creditsText", imageset.CreditsText, ","));
            sb.Append(string.Format("{0}:\"{1}\"{2}", "dataSetType", imageset.DataSetType, ","));
            // sb.Append(string.Format("{0}:\"{1}\"{2}", "ElevationModel", imageset.ElevationModel, ","));
            sb.Append(string.Format("{0}:\"{1}\"{2}", "levels", imageset.Levels, ","));
            sb.Append(string.Format("{0}:\"{1}\"{2}", "offsetX", imageset.OffsetX, ","));
            sb.Append(string.Format("{0}:\"{1}\"{2}", "offsetY", imageset.OffsetY, ","));
            sb.Append(string.Format("{0}:\"{1}\"{2}", "projection", imageset.Projection, ","));

            sb.Append(string.Format("{0}:\"{1}\"{2}", "quadTreeTileMap", imageset.QuadTreeTileMap, ","));
            sb.Append(string.Format("{0}:\"{1}\"{2}", "rotation", imageset.Rotation, ","));
            sb.Append(string.Format("{0}:\"{1}\"{2}", "thumbnailUrl", imageset.ThumbnailUrl, ","));
            sb.Append(string.Format("{0}:\"{1}\"{2}", "url", imageset.Url, ","));
            sb.Append(string.Format("{0}:\"{1}\"{2}", "widthFactor", imageset.WidthFactor, ","));
            sb.Append("}");


            return sb.ToString();

        }

     

        public static IPlace[] FindConteallationObjects(string constellationID, Coordinates[] corners, Classification type)
        {
            if (!constellationObjects.ContainsKey(constellationID))
            {
                return null;
            }

            if (corners != null)
            {
                var minRa = Math.Min(corners[0].RA, Math.Min(corners[1].RA, Math.Min(corners[2].RA, corners[3].RA)));
                var maxRa = Math.Max(corners[0].RA, Math.Max(corners[1].RA, Math.Max(corners[2].RA, corners[3].RA)));

                var minDec = Math.Min(corners[0].Dec, Math.Min(corners[1].Dec, Math.Min(corners[2].Dec, corners[3].Dec)));
                var maxDec = Math.Max(corners[0].Dec, Math.Max(corners[1].Dec, Math.Max(corners[2].Dec, corners[3].Dec)));

                var wrap = false;
                if (Math.Abs(maxRa - minRa) > 12)
                {
                    wrap = true;
                }

                var results = new List<IPlace>();

                foreach (var place in constellationObjects[constellationID])
                {
                    //TODO Need Serious fixing of RA dec range validation...
                    if ((type & place.Classification) != 0)
                    {
                        if (!wrap)
                        {
                            if (place.RA > minRa && place.RA < maxRa && place.Dec > minDec && place.Dec < maxDec)
                            {
                                if (place.IsTour)
                                {
                                    results.Insert(0, place);
                                }
                                else
                                {
                                    results.Add(place);
                                }
                            }
                        }
                        else
                        {
                            if ((place.RA < minRa || place.RA > maxRa) && place.Dec > minDec && place.Dec < maxDec)
                            {
                                if (place.IsTour)
                                {
                                    results.Insert(0, place);
                                }
                                else
                                {
                                    results.Add(place);
                                }
                            }
                        }

                    }
                }
                return results.ToArray();
            }
            else
            {
                var results = new List<IPlace>();

                foreach (var place in constellationObjects[constellationID])
                {
                    if ((type & place.Classification) != 0)
                    {
                        if (place.IsTour)
                        {
                            results.Insert(0, place);
                        }
                        else
                        {
                            results.Add(place);
                        }
                    }
                }
                return results.ToArray();
            }
        }
        public static IPlace[] FindConteallationObjectsInCone(string constellationID, double ra, double dec, float distance, Classification type)
        {
            var center = Coordinates.RADecTo3d(ra, dec,1);

            if (!constellationObjects.ContainsKey(constellationID))
            {
                return null;
            }

            if (distance > 0)
            {

                var results = new List<IPlace>();

                foreach (var place in constellationObjects[constellationID])
                {
                    Vector3d distanceVector;

                    //TODO Need Serious fixing of RA dec range validation...
                    if ((type & place.Classification) != 0)
                    {
                        distanceVector = Vector3d.Subtract(center, place.Location3d);

                        if (distanceVector.Length() < distance)
                        {
                            if (place.IsTour)
                            {
                                results.Insert(0, place);
                            }
                            else
                            {
                                results.Add(place);
                            }
                        }
                    }
                }
                return results.ToArray();
            }
            else
            {
                var results = new List<IPlace>();

                foreach (var place in constellationObjects[constellationID])
                {
                    if ((type & place.Classification) != 0)
                    {
                        if (place.IsTour)
                        {
                            results.Insert(0, place);
                        }
                        else
                        {
                            results.Add(place);
                        }
                    }
                }
                return results.ToArray();
            }
        }
        public static IPlace[] FindAllObjects(string constellationID, Classification type)
        {

            if (!constellationObjects.ContainsKey(constellationID))
            {
                return null;
            }

            var results = new List<IPlace>();

            foreach (var place in constellationObjects[constellationID])
            {
                if ((type & place.Classification) != 0)
                {

                    if (place.IsTour)
                    {
                        results.Insert(0, place);
                    }
                    else
                    {
                        results.Add(place);
                    }
                }
            }
            return results.ToArray();
        }
    }
}
