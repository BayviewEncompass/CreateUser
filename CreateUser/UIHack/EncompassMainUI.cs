using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace PrimaryPlugin.UIHack
{
    public static class EncompassMainUI
    {
        public static event EncompassFormOpenedHandler FormOpened;
        private static Dictionary<Form, IntPtr> _OpenForms;
        private static System.Timers.Timer mainUITimer = null;

        public static Form MainUI
        {
            get
            {
                return Application.OpenForms[0];
            }
        }

        static EncompassMainUI()
        {
            try
            {
                try
                {
                    _OpenForms = new Dictionary<Form, IntPtr>();
                }
                catch (Exception)
                {
                    //handle Exception
                }


                try
                {
                    Application.OpenForms[0].Deactivate += EncompassMainUI_Deactivate;
                }
                catch (Exception)
                {
                    //handle Exception
                }


                try
                {
                    mainUITimer = new System.Timers.Timer(300);
                    mainUITimer.Elapsed += OnTimer;
                    mainUITimer.AutoReset = false;
                    mainUITimer.SynchronizingObject = Application.OpenForms[0];
                    mainUITimer.Enabled = true;
                }
                catch (Exception)
                {
                    //handle Exception
                }

                try
                {
                    CheckAndAdd();
                }
                catch (Exception)
                {
                    //handle Exception
                }

            }
            catch (Exception)
            {
                //handle Exception
            }
        }

        private static void EncompassMainUI_Deactivate(object sender, EventArgs e)
        {
            try
            {
                CheckAndAdd();
            }
            catch (Exception)
            {
                //handle Exception
            }
        }

        private static void OnTimer(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                CheckAndAdd();
            }
            catch (Exception)
            {
                //handle Exception
            }

            try
            {
                CheckDictionary();
            }
            catch (Exception)
            {
                //handle Exception
            }
            try
            {
                if (mainUITimer != null)
                {
                    mainUITimer.Enabled = true;
                }
            }
            catch (Exception)
            {
                //handle Exception
            }
        }

        private static void CheckDictionary()
        {
            try
            {
                if (_OpenForms != null)
                {
                    foreach (var s in _OpenForms.Where(p => p.Key == null || p.Key.IsDisposed).ToList())
                    {
                        try
                        {
                            _OpenForms.Remove(s.Key);
                        }
                        catch (Exception)
                        {
                            //handle Exception
                        }
                    }
                }
            }
            catch (Exception)
            {
                //handle Exception
            }
        }

        private static void CheckAndAdd()
        {
            try
            {
                if (Application.OpenForms != null && _OpenForms != null)
                {
                    foreach (Form _form in Application.OpenForms)
                    {
                        if (_form != null && _form.IsDisposed == false)
                        {
                            if (!_OpenForms.Keys.Contains(_form))
                            {
                                AddNewForm(_form);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                //handle Exception
            }
        }

        private static void AddNewForm(Form _form)
        {
            try
            {
                if (_form != null && _form.IsDisposed == false)
                {
                    _form.FormClosing += _form_FormClosing;
                    _OpenForms.Add(_form, _form.Handle);
                    FormOpenEventTrigger(_form);
                }
            }
            catch (Exception)
            {
                //handle Exception
            }
        }

        private static void _form_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (sender != null)
                {
                    Form _form = (Form)sender;
                    if (_form != null && _form.IsDisposed == false)
                    {
                        if (_OpenForms != null && _OpenForms.Keys.Contains(_form))
                        {
                            _OpenForms.Remove(_form);
                        }
                        _form.FormClosing -= _form_FormClosing;
                    }
                }
            }
            catch (Exception)
            {
                //handle Exception
            }
        }

        private static void FormOpenEventTrigger(Form _form)
        {
            try
            {
                if (_form != null && _form.IsDisposed == false)
                {
                    EncompassFormOpenedEventArgs eventArgs = new EncompassFormOpenedEventArgs(_form);
                    FormOpened.Invoke(null, eventArgs);
                }
            }
            catch (Exception)
            {
                //handle Exception
            }
        }


    }

}
