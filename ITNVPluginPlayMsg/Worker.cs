using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace ITNVPluginPlayMsg
{
    /// <summary>
    /// <para>
    /// The Worker class contains all the logic that is not related to the AS GUI Host plugin architecture.
    /// </para>
    /// <para>
    /// For example, in the Email Plugin, the Worker class contains the logic that controls email work items, and
    /// allows the user to interact with them.  The logic related to stopping and starting the plugin itself, and
    /// all plugin settings, lives in the Plugin class instead.
    /// </para>
    /// </summary>
    internal class Worker
    {
        /// <summary>
        /// Starts working. 
        /// </summary>
        internal void StartWorking()
        {
            //Hook up document window events
            //Plugin.GuiHost.WindowFocused += new AgileSoftware.GUIHost.WindowStateChangedEventHandler(GuiHost_WindowFocused);
            //Plugin.GuiHost.WindowClosed += new AgileSoftware.GUIHost.WindowStateChangedEventHandler(GuiHost_WindowClosed);
            //Plugin.GuiHost.DocumentWindowCreated += new AgileSoftware.GUIHost.WindowStateChangedEventHandler(GuiHost_DocumentWindowCreated);

            //Create buttons in the tool strip bar.
            //ToolPlayMsgButtons.StartWorking();

            //Create menu items in the host window.
            //MenuStripItems.StartWorking();

            if (Plugin.PluginConfiguration.PlayMsgButtons.Enabled)
            {

                if (Plugin.PluginConfiguration.PlayMsgButtons.WebServiceURL.Length > 0)
                    Plugin.PluginConfiguration.PlayMsgButtons.Playmsgmng = new ITNVPlayMsgManager.ITNVPlayMsgManager(Plugin.PluginConfiguration.PlayMsgButtons.WebServiceURL);
                ToolStripButtonsPlayIVRMsg.StartWorking();

                ///ToolButtonPlayIVRMsg.StartWorking();
                // ToolButtonPlayIVTMsg.thisInstance.toolButton.Enabled = false;
            }


            Plugin.PimBroker.ErrorLogging.AddErrorToListInformation(Plugin.ConfigSectionName + "->" + this.GetType().Name + "-> " + MethodBase.GetCurrentMethod().Name +"---------------->");
            //Hook the telephony events fired by XMLClient and XMLStation.
            TelephonyEventHandler.StartWorking();



            Plugin.PimBroker.ErrorLogging.AddErrorToListInformation(Plugin.ConfigSectionName + "->" + this.GetType().Name + "-> " + MethodBase.GetCurrentMethod().Name + "<----------------");
        }

        //void GuiHost_DocumentWindowCreated(object sender, AgileSoftware.GUIHost.WindowStateChangedEventArgs arg)
        //{

        //}

        //void GuiHost_WindowClosed(object sender, AgileSoftware.GUIHost.WindowStateChangedEventArgs arg)
        //{
        //}

        //void GuiHost_WindowFocused(object sender, AgileSoftware.GUIHost.WindowStateChangedEventArgs arg)
        //{
        //}

        internal void StopWorking()
        {
        }
    }

}
