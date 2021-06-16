using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace SimpleShapeGrammar.Classes
{
    // --- properties ---
    // --- constructors --
    // --- methods ---

    // --- variables ---
    // --- input ---
    // --- solve ---
    // --- output ---

    public static class SH_UtilityClass
    {
        


        /*
        public static T DeepCopy<T>(this T self) where T : class

        {

            var ret = Activator.CreateInstance(typeof(T), true) as T;

            var type = self.GetType();

            var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic);

            foreach (var field in fields)

                field.SetValue(ret, field.GetValue(self));

            return ret;

        }*/
        public static T DeepCopy<T>(T target)
        {
            T result;
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream m = new MemoryStream();

            try
            {
                bf.Serialize(m, target);
                m.Position = 0;
                result = (T)bf.Deserialize(m);
            }
            finally
            {
                m.Close();
            }



            return result;
        }




    }
}
