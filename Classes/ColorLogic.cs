using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RCModelColors.Classes
{
    public class ColorLogic
    {
        DBInteraction DBInteraction = new DBInteraction();
        public void SetColors(int S, int L)
        {
            List<PropItem> list = DBInteraction.ReadDatabase();

            int increment = list.Count != 0 ? (int)(360 / list.Count() - 1) : 0;

            ArrayList numbers = new ArrayList();

            for (int i = 0; i < list.Count; i++)
            {
                numbers.Add(i);
            }

            Random rnd = new Random();

            foreach (PropItem item in list)
            {
                int number = rnd.Next(0, numbers.Count);

                item.Hue = (int)numbers[number] * increment;
                numbers.RemoveAt(number);

                item.Saturation = S;
                item.Lightness = L;

                DBInteraction.Store(item);
            }
        }
        public void UpdateColors(int S, int L)
        {
            List<PropItem> list = DBInteraction.ReadDatabase();

            foreach (PropItem item in list)
            {
                item.Saturation = S;
                item.Lightness = L;
                DBInteraction.Store(item);
            }
        }
    }
}
