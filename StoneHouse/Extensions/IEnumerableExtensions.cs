using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StoneHouse.Extensions
{
    //Extensions must be of static class
    public static class IEnumerableExtensions
    {
        //Extension method to convert IEnumerable of geeneric type to select list item 
        //'SelectListItem' is the return type
        //'T' stands for generic type
        public static IEnumerable<SelectListItem> ToSelectListItem<T>(this IEnumerable<T> items, int selectedValue)
        {

            //pass the collection of IEnumerable item called 'items'
            //adn use link to iterate and convert it into 'SelectListItem'
            return from item in items
                   select new SelectListItem
                   {
                       Text = item.GetPropertyValue("Name"),
                       Value = item.GetPropertyValue("Id"),
                       Selected = item.GetPropertyValue("Id").Equals(selectedValue.ToString())

                   };
        }
    

    public static IEnumerable<SelectListItem> ToSelectListItemString<T>(this IEnumerable<T> items, string selectedValue)
    {
        if (selectedValue == null)
        {
            selectedValue = "";
        }

        //pass the collection of IEnumerable item called 'items'
        //adn use link to iterate and convert it into 'SelectListItem'
        return from item in items
               select new SelectListItem
               {
                   Text = item.GetPropertyValue("Name"),
                   Value = item.GetPropertyValue("Id"),
                   Selected = item.GetPropertyValue("Id").Equals(selectedValue.ToString())

               };
    }
}
}

