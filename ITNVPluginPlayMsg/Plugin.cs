using System;
using System.Collections.Generic;
using System.Text;

using AgileSoftware;
using AgileSoftware.Configuration;
using AgileSoftware.Plugin;
using AgileSoftware.GUIHost;

namespace ITNVPluginPlayMsg
{
    /// <summary>
    /// <para>
    /// The Plugin class contains all the logic related to stopping and starting the plugin itself, and
    /// all plugin settings.
    /// </para>
    /// <para>
    /// For example, in the Email Plugin, the Plugin class contains all logic related to stopping and 
    /// starting the plugin itself, and all plugin settings.  The Worker class contains the logic that 
    /// controls email work items, and allows the user to interact with them.
    /// </para>
    /// </summary>
    public class Plugin : IASPIMClient2
    {
        /// <summary>
        /// Gets the run state of the plugin.
        /// </summary>
        public RunState RunState
        {
            get
            {
                return runState;
            }
        }

        /// <summary>
        /// Gets the PimBroker that contains this plugin.
        /// </summary>
        public static IASPIMBroker PimBroker
        {
            get
            {
                return pimBroker;
            }

        }

        /// <summary>
        /// Gets the GuiHost that contains this plugin.
        /// </summary>
        public static IASGUIHost3 GuiHost
        {
            get
            {
                return guiHost;
            }
        }

        /// <summary>
        /// Gets the configuration of this plugin.
        /// </summary>
        public static Configuration PluginConfiguration
        {
            get
            {
                return pluginConfiguration;
            }
        }


        public static AgileSoftware.Multimedia.IASMediaController MediaController
        {
            get
            {
                return mediaController;
            }
        }

        public static AgileSoftware.Developer.ASXMLStation XMLStation
        {
            get
            {
                return xMLStation;
            }
        }

        public static AgileSoftware.Developer.ASXMLClient XMLClient
        {
            get
            {
                return xMLClient;
            }
        }

        public static AgileSoftware.ASGUIHVoicePlugin.IVoicePlugin2 VoicePlugin
        {
            get
            {
                return voicePlugin;
            }
        }

        public static string ConfigSectionName
        {
            get
            {
                return configsectionname;
            }
        }

        /// <summary>
        /// Disposes this object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Gets the references of the objects exposed by other plug-ins. This method is invoked by the Host between 
        /// invoking the Initialise and Run methods.
        /// </summary>
        /// <returns></returns>
        public ErrorCode Prepare()
        {
            //Get the GUIHost plugin
            guiHost = GetPluginInstance<AgileSoftware.GUIHost.IASGUIHost3>();

            //Get the MediaController 
            mediaController = GetPluginInstance<AgileSoftware.Multimedia.IASMediaController>();

            //Get the XMLStation and XMLClient control 
            AgileSoftware.GUIHost.ITelephony telephonyPlugin = GetPluginInstance<AgileSoftware.GUIHost.ITelephony>();
            xMLStation = (AgileSoftware.Developer.ASXMLStation)telephonyPlugin.XMLStation;
            xMLClient = (AgileSoftware.Developer.ASXMLClient)telephonyPlugin.XMLClient;

            //Get the Voice Plugin 
            voicePlugin = GetPluginInstance<AgileSoftware.ASGUIHVoicePlugin.IVoicePlugin2>();



            return ErrorCode.NoError;

        }


        /// <summary>
        /// Changes the run state of the plugin, either starting or stopping it. This method is called by the CCE Desktop host after 
        /// calling the IASPIMClient.Initialise methods of all the plugins which have been loaded successfully.
        /// </summary>
        /// <param name="state">The target state into which the plugin will be placed.</param>
        /// <returns>Returns an <see cref="AgileSoftware.Plugin.ErrorCode"/> describing whether the change succeeded or failed.</returns>
        public AgileSoftware.Plugin.ErrorCode Run(AgileSoftware.Plugin.RunState state)
        {
            AgileSoftware.Plugin.ErrorCode result = AgileSoftware.Plugin.ErrorCode.NoError;

            if (state == AgileSoftware.Plugin.RunState.Start)
            {
                // try to start the plugin
                result = StartPlugin();
                if (result != AgileSoftware.Plugin.ErrorCode.UnknowError)
                {
                    runState = AgileSoftware.Plugin.RunState.Start;
                }
            }
            else if (state == AgileSoftware.Plugin.RunState.Stop)
            {
                // try to stop the plugin
                result = StopPlugin();
                if (result != AgileSoftware.Plugin.ErrorCode.UnknowError)
                {
                    runState = AgileSoftware.Plugin.RunState.Stop;
                }
            }

            return result;
        }

        /// <summary>
        /// Initialises the plugin. This method is called by the CCE Desktop host when it is started. It is the first method of the plugin 
        /// being invoked by the host application, or say, it is the starting point of a plugin.
        /// </summary>
        /// <param name="asPIMBroker">The PIM broker to be used by the plugin.</param>
        /// <param name="configurationSectionName">The name of the confiugration section in which the plugin will store data.</param>
        /// <returns>Returns an <see cref="AgileSoftware.Plugin.ErrorCode"/> describing the success, or otherwise, of initialisation.</returns>
        public AgileSoftware.Plugin.ErrorCode Initialise(AgileSoftware.Plugin.IASPIMBroker asPIMBroker, string configurationSectionName)
        {
            configsectionname = configurationSectionName;
            asPIMBroker.ErrorLogging.AddErrorToListInformation("ITNVPluginPlayMsg.Plugin.Initialise: initializing.");

            AgileSoftware.Plugin.ErrorCode result = AgileSoftware.Plugin.ErrorCode.NoError;

            //Get the PIMBroker object
            pimBroker = asPIMBroker;

            //Get the configuration settings for this plugin
            pluginConfiguration.LoadConfiguration(configurationSectionName);

            pimBroker.ErrorLogging.AddErrorToListInformation("ITNVPluginPlayMsg.Plugin.Initialise: initialized with result {0}.", result);

            return result;
        }

        #region Private Members
        private ErrorCode StartPlugin()
        {
            pimBroker.ErrorLogging.AddStartupInformation("ITNVPluginPlayMsg.Plugin.StartPlugin: starting.");

            AgileSoftware.Plugin.ErrorCode result = ErrorCode.NoError;

            if (guiHost == null) return ErrorCode.UnknowError;

            try
            {
                worker = new Worker();

                //Notify the Worker object that the plugin has started.
                worker.StartWorking();
            }
            catch (Exception e)
            {
                pimBroker.ErrorLogging.AddErrorToListFatal(
                    "ITNVPluginPlayMsg.Plugin.StartPlugin: an exception was thrown while creating a new worker object.  {0}",
                    e.ToString());

                result = ErrorCode.UnknowError;
            }

            pimBroker.ErrorLogging.AddStartupInformation("ITNVPluginPlayMsg.Plugin.StartPlugin: started with result {0}.", result);
            return result;
        }

        private ErrorCode StopPlugin()
        {
            pimBroker.ErrorLogging.AddStatusInformation("ITNVPluginPlayMsg.Plugin.StopPlugin: stopping.");

            //Notifies the Worker class that the plugin has been stopped
            if (this.worker != null) this.worker.StopWorking();

            AgileSoftware.Plugin.ErrorCode result = ErrorCode.NoError;

            pimBroker.ErrorLogging.AddStatusInformation("ITNVPluginPlayMsg.Plugin.StopPlugin: stopped with result {0}.", result);

            return result;
        }

        private static TPlugin GetPluginInstance<TPlugin>() where TPlugin : class
        {
            Object pluginObject;
            string typeVersion;

            if (pimBroker.GetObjectReference(typeof(TPlugin).FullName,
                null, out pluginObject, out typeVersion) == AgileSoftware.Plugin.ErrorCode.NoError)
            {
                if (pluginObject != null)
                {
                    return (pluginObject as TPlugin);
                }
            }

            pimBroker.ErrorLogging.AddErrorToListFatal(
                "ITNVPluginPlayMsg.Plugin.GetPluginInstance: failed to obtain plugin of type '{0}'.",
                typeof(TPlugin).ToString());

            return null;
        }
        #endregion

        #region Protected Members
        #endregion

        #region Internal Members
        /// <summary>
        /// Disposes this object.
        /// </summary>
        /// <param name="disposing">If true, disposes managed resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // dispose managed resources here
            }

            // dispose unmanaged resources here
        }
        #endregion

        #region Fields
        private static string configsectionname;
        private static IASGUIHost3 guiHost;
        private static AgileSoftware.Plugin.IASPIMBroker pimBroker;
        private static Configuration pluginConfiguration = new Configuration();
        private static AgileSoftware.Multimedia.IASMediaController mediaController;
        private static AgileSoftware.Developer.ASXMLStation xMLStation;
        private static AgileSoftware.Developer.ASXMLClient xMLClient;
        private static AgileSoftware.ASGUIHVoicePlugin.IVoicePlugin2 voicePlugin;
        private AgileSoftware.Plugin.RunState runState;
        private Worker worker;
        #endregion
    }
}
