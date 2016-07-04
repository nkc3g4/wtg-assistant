using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace wintogo
{
    public class PropertyGridFolderBrowserDialogItem : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)

        {
            IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
            if (edSvc != null)
            {
                // 可以打开任何特定的对话框  
                FolderBrowserDialog dialog = new FolderBrowserDialog();
                if (dialog.ShowDialog().Equals(DialogResult.OK))
                {
                    return dialog.SelectedPath;
                    //if (dialog.SelectedPath.Length != 3)
                    //{
                    //    MessageBox.Show(MsgManager.GetResString("Msg_UDRoot", MsgManager.ci));
                    //    return string.Empty;
                    //}
                    //else
                    //{
                    //    return dialog.SelectedPath;
                    //}
                }
            }
            return value;
        }

    }

}
