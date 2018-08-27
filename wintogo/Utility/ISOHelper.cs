using System.Management.Automation;

namespace wintogo.Utility
{
    public class ISOHelper
    {
        public static  void MountISO(string path)
        {
            using (PowerShell PowerShellInstance = PowerShell.Create())
            {
                // use "AddScript" to add the contents of a script file to the end of the execution pipeline.
                // use "AddCommand" to add individual commands/cmdlets to the end of the execution pipeline.
                PowerShellInstance.AddScript("param($path) Mount-DiskImage -ImagePath $path");
                
                // use "AddParameter" to add a single parameter to the last command/script on the pipeline.
                PowerShellInstance.AddParameter("path", path);
                PowerShellInstance.Invoke();

            }
        }
        public static void DismountISO(string path)
        {
            using (PowerShell PowerShellInstance = PowerShell.Create())
            {
                // use "AddScript" to add the contents of a script file to the end of the execution pipeline.
                // use "AddCommand" to add individual commands/cmdlets to the end of the execution pipeline.
                PowerShellInstance.AddScript("param($path) Dismount-DiskImage -ImagePath $path");

                // use "AddParameter" to add a single parameter to the last command/script on the pipeline.
                PowerShellInstance.AddParameter("path", path);
                PowerShellInstance.Invoke();

            }

        }
    }
}
