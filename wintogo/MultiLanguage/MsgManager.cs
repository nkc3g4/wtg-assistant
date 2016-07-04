using System;
using System.Globalization;
using System.Threading;
//using System.Threading.Tasks;
using wintogo.MultiLanguage;

namespace wintogo
{

    public static class MsgManager
    {
        public static CultureInfo ci = Thread.CurrentThread.CurrentCulture;
        public static System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ResourceLang));
        public static string GetResString(string rname, CultureInfo culi)
        {
            return resources.GetString(rname, culi).Replace("\\n", Environment.NewLine);
        }
        public static string GetResString(string rescourceName)
        {
            return resources.GetString(rescourceName, ci).Replace("\\n", Environment.NewLine);

        }

    }


}
