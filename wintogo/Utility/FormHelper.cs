using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wintogo.Utility
{
    public static class FormHelper
    {
        public static void Closewp()
        {
            if (WTGModel.wp != null)
            {
                try
                {
                    WTGModel.wp.Invoke(new Action(() =>
                    {
                        WTGModel.wp.IsUserClosing = false;
                        WTGModel.wp.Close();
                    }));

                }
                catch (Exception ex) {
                    Console.WriteLine(ex.ToString());
                }
            }
        }
    }
}
