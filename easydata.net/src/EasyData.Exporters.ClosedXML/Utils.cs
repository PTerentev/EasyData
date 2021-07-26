﻿using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EasyData.Export
{
    internal static class Utils
    {
        private static Regex _formatRegex = new Regex("{0:(.*?)}", RegexOptions.Singleline);


        public static string GetFormat(string displayFormat)
        {
            if (_formatRegex.IsMatch(displayFormat))
                return _formatRegex.Match(displayFormat).Groups[1].Value;

            return "";
        }

        public static string GetExcelDisplayFormat(IDataExportSettings settings, string displayFormat)
        {
            if (_formatRegex.IsMatch(displayFormat)) {
                return _formatRegex.Replace(displayFormat, m => {
                    var format = m.Groups[1].Value;
      
                    var type = char.ToUpperInvariant(format[0]);
                    if (type == 'D' || type == 'C' || type == 'F') {
                        var digits = (format.Length > 1)
                          ? int.Parse(format.Substring(1))
                          : type == 'D' ? 1 : 2;

                        if (type == 'D') {
                            return new string('0', digits);
                        }

                        var floatFormat = "#0." + new string('0', digits);
                        if (type == 'C') {
                            return settings.Culture.NumberFormat.CurrencySymbol + floatFormat;
                        }
                        else if (type == 'F') {
                            return floatFormat;
                        }
                    }
                   
                    return format;
                });
            }

            return displayFormat;
        }

        public static string GetDateFormat(DataType dataType, IDataExportSettings settings, string displayFormat)
        {
            if (!string.IsNullOrEmpty(displayFormat)) {
                var dfmt = _formatRegex.Match(displayFormat).Groups[1].Value;
                if (dfmt == "d") {
                    return BuildShortDateTimeFormat(settings.Culture, DataType.Date);
                }
                else if (dfmt == "D") {
                    return BuildLongDateTimeFormat(settings.Culture, DataType.Date);
                }
                else if (dfmt == "f") {
                    return BuildShortDateTimeFormat(settings.Culture, DataType.DateTime);
                }
                else if (dfmt == "F") {
                    return BuildLongDateTimeFormat(settings.Culture, DataType.DateTime);
                }
                return dfmt;
            }

            return BuildShortDateTimeFormat(settings.Culture, dataType);
        }

        private static string BuildShortDateTimeFormat(CultureInfo culture, DataType type)
        {
            if (type == DataType.Date) {
                return culture.DateTimeFormat.ShortDatePattern;
            }
            else if (type == DataType.Time) {
                return culture.DateTimeFormat.ShortTimePattern;
            }
            
            return culture.DateTimeFormat.ShortDatePattern + " "
                    + culture.DateTimeFormat.ShortTimePattern;
        }

        private static string BuildLongDateTimeFormat(CultureInfo culture, DataType type)
        {
            if (type == DataType.Date) {
                return culture.DateTimeFormat.LongDatePattern;
            }
            else if (type == DataType.Time) {
                return culture.DateTimeFormat.LongTimePattern;
            }

            return culture.DateTimeFormat.LongDatePattern + " "
                    + culture.DateTimeFormat.LongTimePattern;
        }

        private static Regex _forbidSymbols = new Regex(string.Format("[{0}]", Regex.Escape(@":\/?*[]""")));

        public static string ToExcelSheetName(string title)
        {
            title = title ?? "";
            var result = _forbidSymbols.Replace(title, "");
            return !string.IsNullOrWhiteSpace(result)
                ? (result.Length > 30)
                    ? result.Substring(0, 30)
                    : result
                : "Sheet 1";
        }
    }
}
