using GameGlobal;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;


namespace GameObjects
{

    public class PropertyComparer : IComparer<GameObject>
    {
        private bool isNumber;
        private string propertyName;
        private bool SmallToBig;
        private int itemID;

        public PropertyComparer(string propertyName, bool isNumber, bool SmallToBig)
        {
            this.propertyName = propertyName;
            this.isNumber = isNumber;
            this.SmallToBig = SmallToBig;
            this.itemID = -1;
        }

        public PropertyComparer(string propertyName, bool isNumber, bool SmallToBig, int itemID)
        {
            this.propertyName = propertyName;
            this.isNumber = isNumber;
            this.SmallToBig = SmallToBig;
            this.itemID = itemID;
        }

        private static readonly Regex dateMatcher = new Regex("^([0-9]+)年(1?[0-9])月([123]?[0-9])日$", RegexOptions.Compiled);
        private static readonly Regex slashMatcher = new Regex("^([0-9]+)/([0-9]+)$", RegexOptions.Compiled);
        private static readonly Regex numberFirstMatcher = new Regex("^([0-9]+)", RegexOptions.Compiled);

        private static bool TryParseNumber(string s, out double value)
        {
            if (long.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out long longVal))
            {
                value = longVal;
                return true;
            }
            if (double.TryParse(s, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out double doubleVal)
                && !double.IsNaN(doubleVal))
            {
                value = doubleVal;
                return true;
            }
            value = 0;
            return false;
        }

        private static int GetStringCategory(string s, out long k1, out long k2, out long k3)
        {
            Match m = slashMatcher.Match(s);
            if (m.Success
                && long.TryParse(m.Groups[1].Value, NumberStyles.None, CultureInfo.InvariantCulture, out long left)
                && long.TryParse(m.Groups[2].Value, NumberStyles.None, CultureInfo.InvariantCulture, out long right))
            {
                k1 = right;
                k2 = left;
                k3 = 0;
                return 0;
            }

            m = dateMatcher.Match(s);
            if (m.Success
                && long.TryParse(m.Groups[1].Value, NumberStyles.None, CultureInfo.InvariantCulture, out long year)
                && long.TryParse(m.Groups[2].Value, NumberStyles.None, CultureInfo.InvariantCulture, out long month)
                && long.TryParse(m.Groups[3].Value, NumberStyles.None, CultureInfo.InvariantCulture, out long day))
            {
                k1 = year;
                k2 = month;
                k3 = day;
                return 1;
            }

            m = numberFirstMatcher.Match(s);
            if (m.Success
                && long.TryParse(m.Groups[1].Value, NumberStyles.None, CultureInfo.InvariantCulture, out long num))
            {
                k1 = num;
                k2 = 0;
                k3 = 0;
                return 2;
            }

            k1 = 0;
            k2 = 0;
            k3 = 0;
            return 3;
        }

        public int Compare(GameObject x, GameObject y)
        {
            if (ReferenceEquals(x, y))
            {
                return 0;
            }
            if (x == null)
            {
                return this.SmallToBig ? -1 : 1;
            }
            if (y == null)
            {
                return this.SmallToBig ? 1 : -1;
            }
            if (x.Equals(y))
            {
                return 0;
            }

            object objX, objY;
            if (itemID < 0)
            {
                objX = StaticMethods.GetPropertyValue(x, this.propertyName);
                objY = StaticMethods.GetPropertyValue(y, this.propertyName);
            }
            else
            {
                objX = StaticMethods.GetMethodValue(x, this.propertyName, new object[] { this.itemID });
                objY = StaticMethods.GetMethodValue(y, this.propertyName, new object[] { this.itemID });
            }

            string xStr = objX?.ToString() ?? string.Empty;
            string yStr = objY?.ToString() ?? string.Empty;
            int result;

            if (this.isNumber)
            {
                bool xOk = TryParseNumber(xStr, out double xNum);
                bool yOk = TryParseNumber(yStr, out double yNum);
                if (xOk && yOk)
                {
                    result = xNum.CompareTo(yNum);
                }
                else if (xOk != yOk)
                {
                    // 非数值（如 "--"、"----"）统一排在数值一侧，满足反对称与传递性
                    result = xOk ? 1 : -1;
                }
                else
                {
                    result = string.Compare(xStr, yStr, StringComparison.CurrentCulture);
                }
            }
            else
            {
                int catX = GetStringCategory(xStr, out long x1, out long x2, out long x3);
                int catY = GetStringCategory(yStr, out long y1, out long y2, out long y3);

                if (catX != catY)
                {
                    result = catX.CompareTo(catY);
                }
                else
                {
                    result = x1.CompareTo(y1);
                    if (result == 0) result = x2.CompareTo(y2);
                    if (result == 0) result = x3.CompareTo(y3);
                    if (result == 0) result = string.Compare(xStr, yStr, StringComparison.CurrentCulture);
                }
            }

            int sign = result < 0 ? -1 : (result > 0 ? 1 : 0);
            return this.SmallToBig ? sign : -sign;
        }
    }
}

