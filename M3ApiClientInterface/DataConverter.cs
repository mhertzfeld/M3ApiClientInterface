using System;
using System.Globalization;
using System.Linq;
using System.Text;


namespace M3ApiClientInterface
{
    public static class DataConverter
    {
        public static DateTime ToDateTime(String _ToConvert, String format = "yyyyMMdd")
        {
            if ((_ToConvert == null) || (_ToConvert.Equals("0", StringComparison.InvariantCultureIgnoreCase)))
            { return default(DateTime); }
            else
            {
                if (format == null)
                { return DateTime.Parse(_ToConvert); }
                else
                { return DateTime.ParseExact(_ToConvert, format, CultureInfo.InvariantCulture); }
            }
        }

        public static DateTime? ToDateTimeNullable(String _ToConvert, String format = "yyyyMMdd")
        {
            if ((_ToConvert == null) || (_ToConvert.Equals("0", StringComparison.InvariantCultureIgnoreCase)))
            { return null; }
            else
            {
                if (format == null)
                { return DateTime.Parse(_ToConvert); }
                else
                { return DateTime.ParseExact(_ToConvert, format, CultureInfo.InvariantCulture); }
            }
        }

        public static Decimal ToDecimal(String _ToConvert)
        {
            return Decimal.Parse(_ToConvert);
        }

        public static Decimal? ToDecimalNullable(String _ToConvert)
        {
            if (_ToConvert == null)
            { return null; }
            else
            { return Decimal.Parse(_ToConvert); }
        }

        public static Int16 ToInt16(String _ToConvert)
        {
            return Int16.Parse(_ToConvert);
        }

        public static Int16? ToInt16Nullable(String _ToConvert)
        {
            if (_ToConvert == null)
            { return null; }
            else
            { return Int16.Parse(_ToConvert); }
        }

        public static Int32 ToInt32(String _ToConvert)
        {
            return Int32.Parse(_ToConvert);
        }

        public static Int32? ToInt32Nullable(String _ToConvert)
        {
            if (_ToConvert == null)
            { return null; }
            else
            { return Int32.Parse(_ToConvert); }
        }

        public static Int64 ToInt64(String _ToConvert)
        {
            return Int64.Parse(_ToConvert);
        }

        public static Int64? ToInt64Nullable(String _ToConvert)
        {
            if (_ToConvert == null)
            { return null; }
            else
            { return Int64.Parse(_ToConvert); }
        }

        public static String ToString(String _ToConvert, Boolean _Trim = true)
        {
            if (_ToConvert == null)
            { return null; }
            else
            {
                if (_Trim)
                {
                    String _Trimmed_ToConvert = _ToConvert.Trim();

                    if (_Trimmed_ToConvert.Length == 0)
                    { return null; }
                    else
                    { return _Trimmed_ToConvert; }
                }
                else
                {
                    if (_ToConvert.Length == 0)
                    { return null; }
                    else
                    { return _ToConvert; }
                }
            }
        }

        public static UInt16 ToUInt16(String _ToConvert)
        {
            return UInt16.Parse(_ToConvert);
        }

        public static UInt16? ToUInt16Nullable(String _ToConvert)
        {
            if (_ToConvert == null)
            { return null; }
            else
            { return UInt16.Parse(_ToConvert); }
        }

        public static UInt32 ToUInt32(String _ToConvert)
        {
            return UInt32.Parse(_ToConvert);
        }

        public static UInt32? ToUInt32Nullable(String _ToConvert)
        {
            if (_ToConvert == null)
            { return null; }
            else
            { return UInt32.Parse(_ToConvert); }
        }

        public static UInt64 ToUInt64(String _ToConvert)
        {
            return UInt64.Parse(_ToConvert);
        }

        public static UInt64? ToUInt64Nullable(String _ToConvert)
        {
            if (_ToConvert == null)
            { return null; }
            else
            { return UInt64.Parse(_ToConvert); }
        }
    }
}