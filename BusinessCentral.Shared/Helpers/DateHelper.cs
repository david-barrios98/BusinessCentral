using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessCentral.Shared.Helper
{
    public class DateHelper
    {
        public static string ConvertDateToStandardFormat(
            DateTime date,
            bool showHoursAndMinutes = false,
            bool onlyYearAndMonth = false
            )
        {
            string strDate, strYear, strMonth, strDay, strHours, strMinutes;

            strYear = date.Year.ToString();
            strMonth = date.Month.ToString();
            strDay = date.Day.ToString();
            strHours = date.Hour.ToString();
            strMinutes = date.Minute.ToString();

            strMonth = GetTwoDigits(strMonth);
            strDay = GetTwoDigits(strDay);
            strHours = GetTwoDigits(strHours);
            strMinutes = GetTwoDigits(strMinutes);

            strDate = strYear + strMonth + strDay;

            if (showHoursAndMinutes)
            {
                strDate = strDate + strHours + strMinutes;

            }
            else if (onlyYearAndMonth)
            {
                strDate = strYear + strMonth;
            }

            return strDate;
        }

        private static string GetTwoDigits(string value)
        {
            string digits;

            if (value.Length == 1)
                digits = $"0{value}";
            else
                digits = value;

            return digits;
        }

        public static string GetFormattedTime(int valueInSeconds)
        {
            int seconds = valueInSeconds % 60;
            int minutes = valueInSeconds / 60 % 60;
            int hours = valueInSeconds / 3600;

            return $"{TwoDigitTimeFormat(hours)}:{TwoDigitTimeFormat(minutes)}:{TwoDigitTimeFormat(seconds)}";
        }

        private static string TwoDigitTimeFormat(int value)
        {
            return value < 10 ? $"0{value}" : $"{value}";
        }


        public static string TimeString(TimeSpan difference)
        {
            if (difference.TotalSeconds < 60)
            {
                return "hace unos segundos";
            }
            else if (difference.TotalMinutes < 60)
            {
                int minutos = (int)difference.TotalMinutes;
                return $"hace {minutos} {(minutos == 1 ? "minuto" : "minutos")}";
            }
            else if (difference.TotalHours < 24)
            {
                int horas = (int)difference.TotalHours;
                return $"hace {horas} {(horas == 1 ? "hora" : "horas")}";
            }
            else
            {
                int dias = (int)difference.TotalDays;
                return $"hace {dias} {(dias == 1 ? "día" : "días")}";
            }
        }
    }
}
