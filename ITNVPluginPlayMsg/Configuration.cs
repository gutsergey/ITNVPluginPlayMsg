using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using AgileSoftware.Configuration;
using System.Reflection;

namespace ITNVPluginPlayMsg
{

    public class IVRMessage
    {
        public IVRMessage()
        {
            this.number = "";
            this.msg = "";
            this.desc = "";
        }
        public string number;   // message index number
        public string msg;      // msg vaw file
        public string desc;

    }

    public class Configuration
    {
        private string pluginConfigurationSectionName = string.Empty;

        public GeneralSection General;
        public class GeneralSection
        {
            private string section;
            public string WindowsTitle;
            public bool SingleInstance;
            public string SingleInstanceMessage;
            public string Language;

            public GeneralSection(Configuration conf)
            {
                section = "General";
                this.Language = conf.GetACCDataString(section, "Language", "en-US");
                if (this.Language.Trim().Length <= 0) this.Language = "en-US";
            }
        }


        public PlayMsgMessagesSection PlayMsgMessages;
        public class PlayMsgMessagesSection
        {
            private string section;
            public List<IVRMessage> Messages = null;
            public PlayMsgMessagesSection(Configuration conf, string listname)
            {
                Messages = new List<IVRMessage>();

                section = conf.pluginConfigurationSectionName + "List" + listname;

                IVRMessage im;

                for (int i = 1; i < 100; i++)
				{
					string msg = conf.GetACCDataString(section, "Msg" + i.ToString("00"), "");

                    if (msg.Trim().Length > 0)
					{
                        PrintConfiguration(section + "->Msg" + i.ToString("00"), "" + msg);
						string[] x = msg.Split(new char[] { ';', ',' });
						im = new IVRMessage();
						im.number = i.ToString("00");

						im.msg = x[0];
						im.desc = x.Length > 1 ? x[1] : x[0];
                        Messages.Add(im);
					}

				}
			}
        }

        public PlayMsgButtonsSection PlayMsgButtons;
        public class PlayMsgButtonsSection
        {
            private string section;
            public bool Enabled;
            public bool ShowIVRLines;
            public string ConferenceNumber;
            public string MessagesList;

            public string WebServiceURL = "";
            public ITNVPlayMsgManager.ITNVPlayMsgManager Playmsgmng = null;

            private string langstr = "";
            public List<string> Language = null;


            public PlayMsgButtonsSection(Configuration conf)
            {
                string s;
                section = conf.pluginConfigurationSectionName;

                this.Enabled = conf.GetACCDataBoolean(section, "Enabled", false);
                PrintConfiguration("Enabled: ", "" + this.Enabled);

                this.ShowIVRLines = conf.GetACCDataBoolean(section, "ShowIVRLines", false);
                PrintConfiguration("ShowIVRLines: ", "" + this.ShowIVRLines);

                this.ConferenceNumber = conf.GetACCDataString(section, "ConferenceNumber", "");
                PrintConfiguration("ConferenceNumber: ", "" + this.ConferenceNumber);


                this.WebServiceURL = conf.GetACCDataString(section, "WebServiceURL", "");
                PrintConfiguration("WebServiceURL: ", "" + WebServiceURL);
                //Playmsgmng = new ITNVPlayMsgManager.ITNVPlayMsgManager(WebServiceURL);



                this.langstr = conf.GetACCDataString(section, "Languages", "HEB");
                PrintConfiguration("Languages: ", "" + this.langstr);

                this.Language = new List<string>(langstr.Split(new char[]{';',','}));

                MessagesList = conf.GetACCDataString(section, "MessagesList", "01");
                PrintConfiguration("MessagesList: ", "" + this.MessagesList);
            }
        }

        /// <summary>
        /// Gets the configuraiton settings from the GUIHost. 
        /// </summary>
        public void LoadConfiguration(string configurationSectionName)
        {
            this.pluginConfigurationSectionName = configurationSectionName;
            this.PlayMsgButtons = new PlayMsgButtonsSection(this);
            this.PlayMsgMessages = new PlayMsgMessagesSection(this, PlayMsgButtons.MessagesList);

            this.General = new GeneralSection(this);
        }

        private static void PrintConfiguration(string name, string value)
        {
            string prefix = Plugin.ConfigSectionName + "->";
            Plugin.PimBroker.ErrorLogging.AddStartupInformation(prefix + name +":" + value);
        }
        private string GetACCDataString(string sectionName, string configKey, string defaultValue)
        {
            try
            {
                string tmp = Plugin.PimBroker.Configuration.ConfigurationSections[sectionName].ConfigurationItems[configKey].Value.Trim();
                if (tmp == null || tmp.Trim().Length == 0)
                    tmp = defaultValue;

                return tmp;
            }
            catch
            {
                return defaultValue;
            }

        }

        private int GetACCDataInterger(string sectionName, string configKey, int defaultValue)
        {
            try
            {
                string tmp = Plugin.PimBroker.Configuration.ConfigurationSections[sectionName].ConfigurationItems[configKey].Value.Trim();
                int valueInteger = Convert.ToInt32(tmp);

                return valueInteger;
            }
            catch
            {
                return defaultValue;
            }
        }

        private bool GetACCDataBoolean(string sectionName, string configKey, bool defaultValue)
        {
            try
            {
                string tmp = Plugin.PimBroker.Configuration.ConfigurationSections[sectionName].ConfigurationItems[configKey].Value.Trim();
                if (tmp == null || tmp.Length == 0)
                    return defaultValue;

                if (string.Compare(tmp, "true", true) == 0 || string.Compare(tmp, "1", true) == 0)
                    return true;

                if (string.Compare(tmp, "false", true) == 0 || string.Compare(tmp, "0", true) == 0)
                    return false;

                return defaultValue;
            }
            catch
            {
                return defaultValue;
            }

        }

        private void UpdateConfigData(string sectionName, string configKey, string configValue)
        {
            Plugin.PimBroker.Configuration.ConfigurationSections.EditConfigurationItem(sectionName, configKey, configValue);
        }
    }

}
