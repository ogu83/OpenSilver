﻿using EnvDTE;
using Microsoft.VisualStudio.TemplateWizard;
using OpenSilver.TemplateWizards.AppCustomizationWindow;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace OpenSilver.TemplateWizards
{
    class AppCustomizationWizard : IWizard
    {
        public void BeforeOpeningFile(ProjectItem projectItem)
        {

        }

        public void ProjectFinishedGenerating(Project project)
        {

        }

        public void ProjectItemFinishedGenerating(ProjectItem projectItem)
        {

        }

        public void RunFinished()
        {

        }

        public void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams)
        {
            XElement openSilverInfo = XElement.Parse(replacementsDictionary["$wizarddata$"]);

            XNamespace defaultNamespace = openSilverInfo.GetDefaultNamespace();

            string openSilverAPI = openSilverInfo.Element(defaultNamespace + "Api").Value;
            string openSilverType = openSilverInfo.Element(defaultNamespace + "Type").Value;

            if (openSilverType == "Application")
            {
                // For now, we have nothing to configure in "Library", so we only show the configuration window in "Application" mode
                AppConfigurationWindow window = new AppConfigurationWindow(openSilverType);

                bool? result = window.ShowDialog();
                if (!result.HasValue || !result.Value)
                {
                    throw new WizardBackoutException("OpenSilver project creation was cancelled by user");
                }

                switch (window.BlazorVersion)
                {
                    case BlazorVersion.Net5:
                        replacementsDictionary.Add("$blazortargetframework$", "net5.0");
                        replacementsDictionary.Add("$blazorpackagesversion$", "5.0.7");
                        break;
                    case BlazorVersion.Net6:
                        replacementsDictionary.Add("$blazortargetframework$", "net6.0");
                        replacementsDictionary.Add("$blazorpackagesversion$", "6.0.0-preview.5.21301.17");
                        break;
                }
            }

            if (openSilverAPI == "Silverlight")
            {
                replacementsDictionary.Add("$opensilverpackagename$", "OpenSilver");
            }
            else if (openSilverAPI == "UWP")
            {
                replacementsDictionary.Add("$opensilverpackagename$", "OpenSilver.UWPCompatible");
            }
            else
            {
                throw new ArgumentNullException($"Unknown OpenSilver API '{openSilverAPI}'");
            }

            replacementsDictionary.Add("$opensilverpackageversion$", "1.0.0-alpha-021");
        }

        public bool ShouldAddProjectItem(string filePath)
        {
            return true;
        }
    }
}
