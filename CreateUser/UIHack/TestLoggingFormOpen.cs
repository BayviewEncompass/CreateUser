using EllieMae.Encompass.Automation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace PrimaryPlugin.UIHack
{
    class TestLoggingFormOpen : IPluginClass
    {
        Form fm;

        public TestLoggingFormOpen()
        {
            SetupAndRun();
        }

        public void SetupAndRun()
        {
            //Only runs for SuperAdmins
            if (EncompassApplication.CurrentUser.Personas.Contains(EncompassApplication.Session.Users.Personas.GetPersonaByName("Super Administrator")) && EncompassApplication.CurrentUser.ID == "josephwaligorski")
            {
                
                EncompassMainUI.FormOpened += EncompassMainUI_FormOpened;
            }
        }

        private void EncompassMainUI_FormOpened(object sender, EncompassFormOpenedEventArgs e)
        {


            try
            {
                //I always check to make sure form isn't null or disposed, to prevent exceptions
                if (e.OpenedForm != null && e.OpenedForm.IsDisposed == false)
                {
                    ControlExport(e.OpenedForm);
                    fm = e.OpenedForm;
                    e.OpenedForm.Click += OpenedForm_Click;

                    
                }
            }
            catch (Exception ex)
            {
                //handle Exception
            }
        }

        void OpenedForm_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Do you want to save changes?", "Confirmation", MessageBoxButtons.YesNoCancel);
            if (result == DialogResult.Yes)
            {
                ControlExport(fm);
            }
            else if (result == DialogResult.No)
            {
                //...
            }
            else
            {
                //...
            }  
        }

        

        private string CleanString(string text)
        {
            string regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            Regex r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));

            return r.Replace(text, "");
        }

        private void ControlExport(Form form)
        {
            //This saves a CSV file with all of the controls on the form, making it easier to identify how to modify the form.
            if (form != null)
            {
                //saves to C:\Temp folder
                string path = @"C:\Temp\" + CleanString(form.Name) + ".csv";

                if (!File.Exists(path))
                {
                    List<Control> allControls = getControls(form);

                    // Create a file to write to. 
                    string createText = "Type,Name,Parent,Text," + Environment.NewLine;
                    File.WriteAllText(path, createText);

                    foreach (Control controlList in allControls)
                    {
                        StringBuilder controlText = new StringBuilder(); ;
                        controlText.Append(controlList.GetType().FullName + ",");
                        controlText.Append(controlList.Name + ",");
                        controlText.Append(controlList.Parent.Name + ",");
                        controlText.Append(controlList.Text + ",");
                        controlText.Append(Environment.NewLine);

                        File.AppendAllText(path, controlText.ToString());

                        StringBuilder controlText2 = new StringBuilder(); ;
                        if (controlList.Name.Contains("gvTemplates"))
                        {
                            MessageBox.Show("Docs");
                            EllieMae.EMLite.UI.GridView gv = (EllieMae.EMLite.UI.GridView)controlList;
                            foreach (var item in gv.Items)
                            {
                                controlText2.AppendLine(item.Text);
                                File.AppendAllText(@"C:\Temp\listOfDocs.csv", controlText2.ToString());
                            }
                        }
                    }
                }
            }
        }

        private List<Control> getControls(Control where)
        {
            List<Control> controles = new List<Control>();

            try
            {
                foreach (Control c in where.Controls)
                {
                    controles.Add(c);

                    if (c.Controls.Count > 0)
                    {
                        controles.AddRange(getControls(c));
                        
                    }
                }
            }
            catch (Exception ex)
            {
                //handle Exception
            }
            return controles;
        }

    }
}
