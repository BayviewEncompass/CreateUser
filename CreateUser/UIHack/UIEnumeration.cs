using System;
using System.Windows.Forms;

namespace PrimaryPlugin.UIHack
{
    class UIEnumeration
    {
        public UIEnumeration()
        {
        }
        public static Control FindControlByPath(Control ctrl_parent, string sPath)
        {
            Control control;
            if (ctrl_parent != null)
            {
                string[] strArrays = new string[] { "\\", "/" };
                string[] strArrays1 = sPath.Split(strArrays, StringSplitOptions.RemoveEmptyEntries);
                Control ctrlParent = ctrl_parent;
                string[] strArrays2 = strArrays1;
                int num = 0;
                while (num < (int)strArrays2.Length)
                {
                    string str = strArrays2[num];
                    if ((!str.StartsWith("[") ? true : !str.EndsWith("]")))
                    {
                        ctrlParent = UIEnumeration.FindFirstChildWithName(ctrlParent, str);
                    }
                    else
                    {
                        string str1 = str.Substring(1, str.Length - 2);
                        ctrlParent = UIEnumeration.FindFirstChildWithText(ctrlParent, str1);
                    }
                    if (ctrlParent != null)
                    {
                        num++;
                    }
                    else
                    {
                        control = null;
                        return control;
                    }
                }
                control = ctrlParent;
            }
            else
            {
                control = null;
            }
            return control;
        }

        private static Control FindFirstChildWithName(Control ctrl_parent, string sName)
        {
            Control control;
            foreach (Control control1 in ctrl_parent.Controls)
            {
                if ((control1 == null || control1.Name == null ? false : control1.Name.Equals(sName, StringComparison.OrdinalIgnoreCase)))
                {
                    control = control1;
                    return control;
                }
            }
            control = null;
            return control;
        }

        private static Control FindFirstChildWithText(Control ctrl_parent, string sText)
        {
            Control control;
            foreach (Control control1 in ctrl_parent.Controls)
            {
                if ((control1 == null || control1.Text == null ? false : control1.Text.Equals(sText, StringComparison.OrdinalIgnoreCase)))
                {
                    control = control1;
                    return control;
                }
            }
            control = null;
            return control;
        }
    }
}
