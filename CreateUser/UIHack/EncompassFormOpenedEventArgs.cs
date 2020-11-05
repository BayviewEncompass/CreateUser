using System;
using System.Windows.Forms;

namespace PrimaryPlugin.UIHack
{
    public class EncompassFormOpenedEventArgs : EventArgs
    {
        private Form _Form;

        public Form OpenedForm
        {
            get { return _Form; }
            private set { _Form = value; }
        }

        public EncompassFormOpenedEventArgs(Form frm)
        {
            _Form = frm;
        }
        public EncompassFormOpenedEventArgs()
        {

        }


    }
}
