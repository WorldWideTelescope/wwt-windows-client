using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.Text;

namespace TerraViewer
{
    [TypeConverter(typeof(ConstellationFilterConverter))]
    [SettingsSerializeAs(SettingsSerializeAs.String)]
    public class ConstellationFilter
    {
        public static Dictionary<String, int> BitIDs;
        public Int32[] Bits = new Int32[3];
        public Int32[] OldBits = new Int32[3];
        public BlendState BlendState = new BlendState(false, 1000, 0);
        public bool Internal = false;
        private void SaveBits()
        {
            for (var i = 0; i < 3; i++)
            {
                OldBits[i] = Bits[i];
            }
        }

        private bool IsChanged()
        {
            for (var i = 0; i < 3; i++)
            {
                if (OldBits[i] != Bits[i])
                {
                    return true;
                }
            }
            return false;
        }

        private void CheckChanged()
        {
            if (IsChanged())
            {
                FireChanged();
            }
        }

        public bool IsEnabled(string abbrev)
        {
            var bitID = BitIDs[abbrev];

            var index = bitID / 32;
            bitID = bitID % 32;

            return BlendState.State && ((1 << (bitID)) & Bits[index]) != 0;
        }

        public bool IsSet(string abbrev)
        {
            SaveBits();

            var bitID = BitIDs[abbrev];

            var index = bitID / 32;
            bitID = bitID % 32;

            return ((1 << (bitID)) & Bits[index]) != 0;
        }

        public void Set(string abbrev, bool state)
        {
            SaveBits();
            var bitID = BitIDs[abbrev];

            var index = bitID / 32;
            bitID = bitID % 32;

            if (state)
            {
                Bits[index] = Bits[index] | (1 << bitID);
            }
            else
            {
                Bits[index] = Bits[index] ^ (1 << bitID);
            }

            CheckChanged();
        }

        public void SetAll(bool state)
        {
            SaveBits();
            for (var bitID = 0; bitID < 89; bitID++)
            {

                var index = bitID / 32;
                var bit = bitID % 32;

                if (state)
                {
                    Bits[index] = Bits[index] | (1 << bit);
                }
                else
                {
                    Bits[index] = Bits[index] ^ (1 << bit);
                }
            }
            CheckChanged();
        }

        public void SetBits(byte[] bits)
        {
            SaveBits();
            for (var i = 0; i < 3; i++)
            {
                Bits[i] = bits[i * 4] + (bits[i * 4 + 1] << 8) + (bits[i * 4 + 2] << 16) + (bits[i * 4 + 3] << 24);
            }
            CheckChanged();
        }

        public byte[] GetBits()
        {
            var bits = new byte[12];

            var index = 0;
            for (var i = 0; i < 3; i++)
            {
                bits[index++] = (byte)(Bits[i]);
                bits[index++] = (byte)(Bits[i] >> 8);
                bits[index++] = (byte)(Bits[i] >> 16);
                bits[index++] = (byte)(Bits[i] >> 24);
            }

            return bits;

        }

        public void Clone(ConstellationFilter filter)
        {
            SaveBits();
            for (var i = 0; i < 3; i++)
            {
                Bits[i] = filter.Bits[i];
            }
            CheckChanged();
        }

        public ConstellationFilter Clone()
        {
            var newFilter = new ConstellationFilter();
            newFilter.Clone(this);
            return newFilter;
        }

        public void Combine(ConstellationFilter filter)
        {
            SaveBits();
            for (var i = 0; i < 3; i++)
            {
                Bits[i] = Bits[i] | filter.Bits[i];
            }
            CheckChanged();
        }

        public void Remove(ConstellationFilter filter)
        {
            SaveBits();
            for (var i = 0; i < 3; i++)
            {
                Bits[i] = Bits[i] & ~filter.Bits[i];
            }
            CheckChanged();
        }

        public static Dictionary<string, ConstellationFilter> Families = new Dictionary<string, ConstellationFilter>();

        static ConstellationFilter()
        {
            BitIDs = new Dictionary<string, int>();
            
        }

        public static void InitSets()
        {
            var all = AllConstellation;
            all.Internal = true;
            Families.Add("AllConstellation", all);
            Families.Add("Zodiacal", Zodiacal);
            Families.Add("Ursa Major Family", UrsaMajorFamily);
            Families.Add("Perseus Family", PerseusFamily);
            Families.Add("Hercules Family", HerculesFamily);
            Families.Add("Orion Family", OrionFamily);
            Families.Add("Heavenly Waters", HeavenlyWaters);
            Families.Add("Bayer Family", BayerFamily);
            Families.Add("La Caille Family", LaCaileFamily);
            LoadCustomFilters();
        }

        public static void SaveCustomFilters()
        {
            var sb = new StringBuilder();

            foreach (var kv in Families)
            {
                if (!kv.Value.Internal)
                {
                    sb.Append(kv.Key);
                    sb.Append(";");
                    sb.AppendLine(kv.Value.ToString());

                }
            }

            AppSettings.SettingsBase["SavedFilters"] = sb.ToString();
        }

        public static void LoadCustomFilters()
        {
            var savedFilders = AppSettings.SettingsBase["SavedFilters"].ToString();
            var lines = savedFilders.Split(new[] { '\n' });
            foreach (var line in lines)
            {
                try
                {
                    var parts = line.Split(new[] { ';' });
                    if (parts.Length > 1)
                    {
                        var filter = Parse(parts[1]);
                        Families.Add(parts[0], filter);
                    }
                }
                catch
                {
                }
            }
        }

        public static ConstellationFilter AllConstellation
        {
            get
            {
                var all = new ConstellationFilter();

                all.SetAll(true);

                return all;
            }

        }

        public static ConstellationFilter Zodiacal
        {
            get
            {
                var zodiacal = new ConstellationFilter();
                zodiacal.Set("ARI", true);
                zodiacal.Set("TAU", true);
                zodiacal.Set("GEM", true);
                zodiacal.Set("CNC", true);
                zodiacal.Set("LEO", true);
                zodiacal.Set("VIR", true);
                zodiacal.Set("LIB", true);
                zodiacal.Set("SCO", true);
                zodiacal.Set("SGR", true);
                zodiacal.Set("CAP", true);
                zodiacal.Set("AQR", true);
                zodiacal.Set("PSC", true);
                zodiacal.Internal = true;
                return zodiacal;
            }

        }

        public static ConstellationFilter UrsaMajorFamily
        {
            get
            {
                var uma = new ConstellationFilter();
                uma.Set("UMA", true);
                uma.Set("UMI", true);
                uma.Set("DRA", true);
                uma.Set("CVN", true);
                uma.Set("BOO", true);
                uma.Set("COM", true);
                uma.Set("CRB", true);
                uma.Set("CAM", true);
                uma.Set("LYN", true);
                uma.Set("LMI", true);
                uma.Internal = true;
                return uma;
            }
        }

        public static ConstellationFilter PerseusFamily
        {
            get
            {
                var Perseus = new ConstellationFilter();
                Perseus.Set("CAS", true);
                Perseus.Set("CEP", true);
                Perseus.Set("AND", true);
                Perseus.Set("PER", true);
                Perseus.Set("PEG", true);
                Perseus.Set("CET", true);
                Perseus.Set("AUR", true);
                Perseus.Set("LAC", true);
                Perseus.Set("TRI", true);
                Perseus.Internal = true;

                return Perseus;
            }

        }

        public static ConstellationFilter HerculesFamily
        {
            get
            {
                var hercules = new ConstellationFilter();
                hercules.Set("HER", true);
                hercules.Set("SGE", true);
                hercules.Set("AQL", true);
                hercules.Set("LYR", true);
                hercules.Set("CYG", true);
                hercules.Set("VUL", true);
                hercules.Set("HYA", true);
                hercules.Set("SEX", true);
                hercules.Set("CRT", true);
                hercules.Set("CRV", true);
                hercules.Set("OPH", true);
                hercules.Set("SER1", true);
                hercules.Set("SER2", true);
                hercules.Set("SCT", true);
                hercules.Set("CEN", true);
                hercules.Set("LUP", true);
                hercules.Set("CRA", true);
                hercules.Set("ARA", true);
                hercules.Set("TRA", true);
                hercules.Set("CRU", true);
                hercules.Internal = true;

                return hercules;
            }

        }
        public static ConstellationFilter OrionFamily
        {
            get
            {
                var orion = new ConstellationFilter();
                orion.Set("ORI", true);
                orion.Set("CMA", true);
                orion.Set("CMI", true);
                orion.Set("MON", true);
                orion.Set("LEP", true);
                orion.Internal = true;

                return orion;
            }

        }
        public static ConstellationFilter HeavenlyWaters
        {
            get
            {
                var waters = new ConstellationFilter();
                waters.Set("DEL", true);
                waters.Set("EQU", true);
                waters.Set("ERI", true);
                waters.Set("PSA", true);
                waters.Set("CAR", true);
                waters.Set("PUP", true);
                waters.Set("VEL", true);
                waters.Set("PYX", true);
                waters.Set("COL", true);
                waters.Internal = true;
                return waters;
            }
        }
        public static ConstellationFilter BayerFamily
        {
            get
            {
                var bayer = new ConstellationFilter();
                bayer.Set("HYA", true);
                bayer.Set("DOR", true);
                bayer.Set("VOL", true);
                bayer.Set("APS", true);
                bayer.Set("PAV", true);
                bayer.Set("GRU", true);
                bayer.Set("PHE", true);
                bayer.Set("TUC", true);
                bayer.Set("IND", true);
                bayer.Set("CHA", true);
                bayer.Set("MUS", true);
                bayer.Internal = true;
                return bayer;
            }

        }
        public static ConstellationFilter LaCaileFamily
        {
            get
            {
                var LaCaile = new ConstellationFilter();
                LaCaile.Set("NOR", true);
                LaCaile.Set("CIR", true);
                LaCaile.Set("TEL", true);
                LaCaile.Set("MIC", true);
                LaCaile.Set("SCL", true);
                LaCaile.Set("FOR", true);
                LaCaile.Set("CAE", true);
                LaCaile.Set("HOR", true);
                LaCaile.Set("OCT", true);
                LaCaile.Set("MEN", true);
                LaCaile.Set("RET", true);
                LaCaile.Set("PIC", true);
                LaCaile.Set("ANT", true);
                LaCaile.Internal = true;
                return LaCaile;
            }

        }

        public bool SettingsOwned = false;
        private void FireChanged()
        {
            if (SettingsOwned)
            {
                PulseMe.PulseForSettingsUpdate();
            }
        }

        public override string ToString()
        {
            return string.Format("{0},{1},{2}", Bits[0], Bits[1], Bits[2]);
        }

        public static ConstellationFilter Parse(string val)
        {
            var parts = val.Split(new[] { ',' });

            var cf = new ConstellationFilter();
            try
            {
                for (var i = 0; i < 3; i++)
                {
                    cf.Bits[i] = Int32.Parse(parts[i]);
                }
            }
            catch
            {
            }

            return cf;
        }
    }

    public class ConstellationFilterConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                var cf = ConstellationFilter.Parse(value as string);
                cf.SettingsOwned = true;
                return cf;
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return value.ToString();
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }


    }
}
