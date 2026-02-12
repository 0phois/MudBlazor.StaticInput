using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudBlazor.StaticInput.Enums
{
    public static class EnumExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToEnumDescriptionString(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            if (field is null)
            {
                return value.ToString().ToLower();
            }

            var attributes = Attribute.GetCustomAttributes(field, typeof(DescriptionAttribute), false) as DescriptionAttribute[];

            return attributes is { Length: > 0 }
                ? attributes[0].Description
                : value.ToString().ToLower();
        }

    }
}
