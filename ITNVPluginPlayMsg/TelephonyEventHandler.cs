using System;
using System.Collections.Generic;
using System.Text;
using AgileSoftware.Developer.Station;
using AgileSoftware.Developer.CSTA;
using AgileSoftware.Developer.CSTA.PrivateData;

namespace ITNVPluginPlayMsg
{
    public class TelephonyEventHandler
    {
        //It hooks the telephony events fired by XMLClient and XMLStation.
        internal static void StartWorking()
        {

            Plugin.XMLStation.MonitorStarted += new EventHandler(XMLStation_MonitorStarted);

            //Hanlde the XMLStation StationCallChanged event. 
            Plugin.XMLStation.StationCallChanged += new AgileSoftware.Developer.Station.StationCallChangedEventHandler(XMLStation_StationCallChanged);

            //Handle the XMLStation AgentGetStateReturn event
            Plugin.XMLStation.AgentGetStateReturn += new AgileSoftware.Developer.Station.AgentGetStateReturnEventHandler(XMLStation_AgentGetStateReturn);
        }
        private static void XMLStation_MonitorStarted(object sender, EventArgs arg)
        {
            try
            {
                if (Plugin.PluginConfiguration.PlayMsgButtons.Enabled)
                {
                    ToolStripButtonsPlayIVRMsg.PhoneNumber = Plugin.XMLStation.StationDN;
                }
            }
            catch (Exception exc)
            {
                Plugin.PimBroker.ErrorLogging.AddErrorToListInformation("TelephonyEventHandler, XMLStation_MonitorStarted, Message: " + exc.Message);
                Plugin.PimBroker.ErrorLogging.AddErrorToListInformation("TelephonyEventHandler, XMLStation_MonitorStarted, StackTrace: " + exc.StackTrace);
            }
        }
        static void XMLStation_AgentGetStateReturn(object sender, AgileSoftware.Developer.Station.AgentGetStateReturnEventArgs arg)
        {
            //Put your code here to handle the agent state changes ...

            switch (arg.AgentChangeType)
            {
                case AgileSoftware.Developer.Station.enAgentChangeType.agentLoggedIn:
                    break;
                case AgileSoftware.Developer.Station.enAgentChangeType.agentLoggedOut:
                    break;
                case AgileSoftware.Developer.Station.enAgentChangeType.agentStateChanged:
                    break;
                case AgileSoftware.Developer.Station.enAgentChangeType.noChange:
                    break;
                default:
                    break;
            }
        }


        private static void XMLStation_StationCallChanged(object sender, AgileSoftware.Developer.Station.StationCallChangedEventArgs arg)
        {
            //Put your code here to handle the call event...
            int i;

            if (!Plugin.PluginConfiguration.PlayMsgButtons.Enabled) return;

            switch (arg.CallEvent.CallEventType)
            {
                case AgileSoftware.Developer.Device.enCallEventType.CSTACallCleared:
                    break;
                case AgileSoftware.Developer.Device.enCallEventType.CSTAConferenced:
                    if (arg.StationCall.CalledDN.Equals(Plugin.PluginConfiguration.PlayMsgButtons.ConferenceNumber))
                    {
                        //-ToolButtonPlayIVRMsg.IvrLineNumber = ((AgileSoftware.Developer.CSTA.CSTAConferencedEventArgs)arg.CallEvent.EventInfo).AddedParty.ID;
                        PrintToLog("-----------------> 1");
                        PrintEventToLog(arg);
                    }
                    break;
                case AgileSoftware.Developer.Device.enCallEventType.CSTAConnectionCleared:
                    try
                    {
                        if (Plugin.PluginConfiguration.PlayMsgButtons.Enabled)
                        {

                            if (arg.StationCall.CallState == AgileSoftware.Developer.Station.enCallState.Idle && Plugin.XMLStation.CurrentCalls.Count <= 1)
                            {

                                ToolStripButtonsPlayIVRMsg.Instance.CloseSession();  // Disable
                                ToolStripButtonsPlayIVRMsg.Instance.AllButtonsDiasable();


                                PrintToLog("-----------------> 3");
                                PrintEventToLog(arg);
                            }


                            string dropdeviceId = ((AgileSoftware.Developer.CSTA.CSTAConnectionClearedEventArgs)arg.CallEvent.EventInfo).DroppedConnection.DeviceID.ID;

                            if (dropdeviceId.Equals(ToolStripButtonsPlayIVRMsg.IVRLine))
                            {
                                //ToolPlayMsgButtons.RemoveElFromDic();
                                //ToolButtonPlayIVTMsg.thisInstance.RemoveElFromDic();
                                ToolStripButtonsPlayIVRMsg.Instance.CloseSession();  // enable

                                PrintToLog("-----------------> 2");
                                PrintEventToLog(arg);
                            }


                            if (arg.StationCall.CallDirection == AgileSoftware.Developer.Station.enCallDirection.Outgoing &&
                                dropdeviceId.Contains("T") && ToolStripButtonsPlayIVRMsg.IVRLine.Length > 0)
                            {
                                //-    // send dtmf 9 to IVR
                                //-    ///// - sg ToolPlayMsgButtons.instance.SendDTMF("" + Plugin.PluginConfiguration.PlayMsgButtons.StopPlayDTMF);
                                //-    ///// - sg ToolPlayMsgButtons.CallDisconnected();

                                //-    ToolButtonPlayIVRMsg.thisInstance.CloseForm();
                                //-    ToolButtonPlayIVRMsg.thisInstance.toolButton.Enabled = false;
                                ToolStripButtonsPlayIVRMsg.Instance.CloseSession(); // disable
                                ToolStripButtonsPlayIVRMsg.Instance.AllButtonsDiasable();
                                PrintToLog("-----------------> 4");
                                PrintEventToLog(arg);
                            }

                            if (arg.StationCall.CallDirection == AgileSoftware.Developer.Station.enCallDirection.Incoming &&
                                dropdeviceId.Equals(arg.StationCall.CallerDN) &&
                                ToolStripButtonsPlayIVRMsg.IVRLine.Length > 0)
                            {
                                // send dtmf 9 to IVR
                                ///// - sg ToolPlayMsgButtons.instance.SendDTMF("" + Plugin.PluginConfiguration.PlayMsgButtons.StopPlayDTMF);
                                ///// - sg ToolPlayMsgButtons.CallDisconnected();

                                //- ToolButtonPlayIVRMsg.thisInstance.CloseForm();
                                //- ToolButtonPlayIVRMsg.thisInstance.toolButton.Enabled = false;

                                ToolStripButtonsPlayIVRMsg.Instance.CloseSession();  // disable
                                ToolStripButtonsPlayIVRMsg.Instance.AllButtonsDiasable();

                                PrintToLog("-----------------> 5");
                                PrintEventToLog(arg);
                            }
                        }

                    }
                    catch (Exception exc)
                    {
                        Plugin.PimBroker.ErrorLogging.AddErrorToListInformation("TelephonyEventHandler, XMLStation_StationCallChanged(CSTAConnectionCleared), Message: " + exc.Message);
                        Plugin.PimBroker.ErrorLogging.AddErrorToListInformation("TelephonyEventHandler, XMLStation_StationCallChanged(CSTAConnectionCleared), StackTrace: " + exc.StackTrace);
                    }
                    break;
                case AgileSoftware.Developer.Device.enCallEventType.CSTAConsultationCallResponse:
                    break;
                case AgileSoftware.Developer.Device.enCallEventType.CSTADelivered:
                    break;
                case AgileSoftware.Developer.Device.enCallEventType.CSTADiverted:
                    break;
                case AgileSoftware.Developer.Device.enCallEventType.CSTAEstablished:
                    if (Plugin.PluginConfiguration.PlayMsgButtons.Enabled)
                    {
                        if (!((AgileSoftware.Developer.CSTA.CSTAEstablishedEventArgs)arg.CallEvent.EventInfo).CalledDevice.ID.Equals(Plugin.PluginConfiguration.PlayMsgButtons.ConferenceNumber))
                        {
                            ToolStripButtonsPlayIVRMsg.Instance.AllButtonsEnable();

                            PrintEventToLog(arg);
                        }
                        else
                            ToolStripButtonsPlayIVRMsg.IVRLine = ((AgileSoftware.Developer.CSTA.CSTAEstablishedEventArgs)arg.CallEvent.EventInfo).EstablishedConnection.DeviceID.ID;
                    }
                    break;
                case AgileSoftware.Developer.Device.enCallEventType.CSTAFailed:
                    break;
                case AgileSoftware.Developer.Device.enCallEventType.CSTAHeld:
                    break;
                case AgileSoftware.Developer.Device.enCallEventType.CSTANetworkReached:
                    break;
                case AgileSoftware.Developer.Device.enCallEventType.CSTAOriginated:
                    break;
                case AgileSoftware.Developer.Device.enCallEventType.CSTAQueued:
                    break;
                case AgileSoftware.Developer.Device.enCallEventType.CSTARetrieved:
                    break;
                case AgileSoftware.Developer.Device.enCallEventType.CSTAServiceInitiated:
                    break;
                case AgileSoftware.Developer.Device.enCallEventType.CSTASnapshotCallResponse:
                    break;
                case AgileSoftware.Developer.Device.enCallEventType.CSTATransfered:
                    break;
                default:
                    break;
            }
        }

        private static void PrintToLog(string txt)
        {
            Plugin.PimBroker.ErrorLogging.AddErrorToListInformation("ITNVPluginPlayMsg:TelephonyEventHandler ---> " + txt);
        }

        private static void PrintEventToLog(StationCallChangedEventArgs arg)
        {
            try
            {
                Plugin.PimBroker.ErrorLogging.AddErrorToListInformation("ITNVPluginPlayMsg:TelephonyEventHandler arg.CallEvent.CallEventType=" + arg.CallEvent.CallEventType + " begin ===>");
                Plugin.PimBroker.ErrorLogging.AddErrorToListInformation("ITNVPluginPlayMsg:TelephonyEventHandler arg.StationCall.CallAppearance=" + arg.StationCall.CallAppearance);
                Plugin.PimBroker.ErrorLogging.AddErrorToListInformation("ITNVPluginPlayMsg:TelephonyEventHandler arg.StationCall.CallConnection.CallID=" + arg.StationCall.CallConnection.CallID);
                Plugin.PimBroker.ErrorLogging.AddErrorToListInformation("ITNVPluginPlayMsg:TelephonyEventHandler arg.StationCall.CallConnection.DeviceID.ID=" + arg.StationCall.CallConnection.DeviceID.ID);
                Plugin.PimBroker.ErrorLogging.AddErrorToListInformation("ITNVPluginPlayMsg:TelephonyEventHandler arg.StationCall.CallConnection.DeviceID.Type=" + arg.StationCall.CallConnection.DeviceID.Type);
                Plugin.PimBroker.ErrorLogging.AddErrorToListInformation("ITNVPluginPlayMsg:TelephonyEventHandler arg.StationCall.CallDirection=" + arg.StationCall.CallDirection);
                Plugin.PimBroker.ErrorLogging.AddErrorToListInformation("ITNVPluginPlayMsg:TelephonyEventHandler arg.StationCall.CalledDN=" + arg.StationCall.CalledDN);
                Plugin.PimBroker.ErrorLogging.AddErrorToListInformation("ITNVPluginPlayMsg:TelephonyEventHandler arg.StationCall.CallerDN=" + arg.StationCall.CallerDN);
                Plugin.PimBroker.ErrorLogging.AddErrorToListInformation("ITNVPluginPlayMsg:TelephonyEventHandler arg.StationCall.CallFlag=" + arg.StationCall.CallFlag);
                Plugin.PimBroker.ErrorLogging.AddErrorToListInformation("ITNVPluginPlayMsg:TelephonyEventHandler arg.StationCall.CallMembers.Count=" + arg.StationCall.CallMembers.Count);
                for (int i = 0; i < arg.StationCall.CallMembers.Count; i++)
                {
                    Plugin.PimBroker.ErrorLogging.AddErrorToListInformation("ITNVPluginPlayMsg:TelephonyEventHandler arg.StationCall.CallMembers[" + i + "].MemberDN=" + arg.StationCall.CallMembers[i].MemberDN);
                    Plugin.PimBroker.ErrorLogging.AddErrorToListInformation("ITNVPluginPlayMsg:TelephonyEventHandler arg.StationCall.CallMembers[" + i + "].TypeOfNumber=" + arg.StationCall.CallMembers[i].TypeOfNumber);
                    Plugin.PimBroker.ErrorLogging.AddErrorToListInformation("ITNVPluginPlayMsg:TelephonyEventHandler arg.StationCall.CallMembers[" + i + "].IsListenHeld=" + arg.StationCall.CallMembers[i].IsListenHeld);
                }
                Plugin.PimBroker.ErrorLogging.AddErrorToListInformation("ITNVPluginPlayMsg:TelephonyEventHandler arg.StationCall.CallState=" + arg.StationCall.CallState);
                Plugin.PimBroker.ErrorLogging.AddErrorToListInformation("ITNVPluginPlayMsg:TelephonyEventHandler arg.StationCall.ConferencePendingState=" + arg.StationCall.ConferencePendingState);
                Plugin.PimBroker.ErrorLogging.AddErrorToListInformation("ITNVPluginPlayMsg:TelephonyEventHandler arg.StationCall.Key=" + arg.StationCall.Key);
                Plugin.PimBroker.ErrorLogging.AddErrorToListInformation("ITNVPluginPlayMsg:TelephonyEventHandler arg.StationCall.TransferPendingState=" + arg.StationCall.TransferPendingState);
                Plugin.PimBroker.ErrorLogging.AddErrorToListInformation("ITNVPluginPlayMsg:TelephonyEventHandler arg.StationCall.UCID=" + arg.StationCall.UCID);
                Plugin.PimBroker.ErrorLogging.AddErrorToListInformation("ITNVPluginPlayMsg:TelephonyEventHandler arg.StationCall.UUI=" + arg.StationCall.UUI);

            }
            catch { }
            switch (arg.CallEvent.CallEventType)
            {
                case AgileSoftware.Developer.Device.enCallEventType.CSTADelivered:
                    try
                    {
                        Plugin.PimBroker.ErrorLogging.AddErrorToListInformation("CSTADelivered------------>");
                        CSTADeliveredEventArgs argdel = arg.CallEvent.EventInfo as CSTADeliveredEventArgs;
                        try
                        {
                            Plugin.PimBroker.ErrorLogging.AddErrorToListInformation("AlertingDevice.ID: " + argdel.AlertingDevice.ID);
                            Plugin.PimBroker.ErrorLogging.AddErrorToListInformation("AlertingDevice.Type: " + argdel.AlertingDevice.Type);
                        }
                        catch { }
                        try
                        {
                            Plugin.PimBroker.ErrorLogging.AddErrorToListInformation("CalledDevice.ID: " + argdel.CalledDevice.ID);
                            Plugin.PimBroker.ErrorLogging.AddErrorToListInformation("CalledDevice.Type: " + argdel.CalledDevice.Type);
                        }
                        catch { }
                        try
                        {
                            Plugin.PimBroker.ErrorLogging.AddErrorToListInformation("CallingDevice.ID: " + argdel.CallingDevice.ID);
                            Plugin.PimBroker.ErrorLogging.AddErrorToListInformation("CallingDevice.Type: " + argdel.CallingDevice.Type);
                        }
                        catch { }
                        try
                        {
                            Plugin.PimBroker.ErrorLogging.AddErrorToListInformation("Connection.CallID: " + argdel.Connection.CallID);
                            Plugin.PimBroker.ErrorLogging.AddErrorToListInformation("Connection.DeviceID: " + argdel.Connection.DeviceID.ID);
                            Plugin.PimBroker.ErrorLogging.AddErrorToListInformation("Connection.DeviceID: " + argdel.Connection.DeviceID.Type);
                        }
                        catch { }

                        Plugin.PimBroker.ErrorLogging.AddErrorToListInformation("EventCause: " + argdel.EventCause);
                        Plugin.PimBroker.ErrorLogging.AddErrorToListInformation("GlobalCallLinkageID: " + argdel.GlobalCallLinkageID);
                        Plugin.PimBroker.ErrorLogging.AddErrorToListInformation("InvokeID: " + argdel.InvokeID);
                        try
                        {
                            Plugin.PimBroker.ErrorLogging.AddErrorToListInformation("LastRedirectionDevice.ID: " + argdel.LastRedirectionDevice.ID);
                            Plugin.PimBroker.ErrorLogging.AddErrorToListInformation("LastRedirectionDevice.Type: " + argdel.LastRedirectionDevice.Type);
                        }
                        catch { }
                        try
                        {
                            Plugin.PimBroker.ErrorLogging.AddErrorToListInformation("LocalConnectionState): " + argdel.LocalConnectionState);
                            Plugin.PimBroker.ErrorLogging.AddErrorToListInformation("MonitorCrossRefID: " + argdel.MonitorCrossRefID);
                        }
                        catch { }
                        try
                        {
                            Plugin.PimBroker.ErrorLogging.AddErrorToListInformation("NetworkCalledDevice.ID: " + argdel.NetworkCalledDevice.ID);
                            Plugin.PimBroker.ErrorLogging.AddErrorToListInformation("NetworkCalledDevice.Type: " + argdel.NetworkCalledDevice.Type);
                        }
                        catch { }
                        try
                        {
                            Plugin.PimBroker.ErrorLogging.AddErrorToListInformation("NetworkCallingDevice.ID: " + argdel.NetworkCallingDevice.ID);
                            Plugin.PimBroker.ErrorLogging.AddErrorToListInformation("NetworkCallingDevice.Type: " + argdel.NetworkCallingDevice.Type);
                        }
                        catch { }
                        try
                        {
                            Plugin.PimBroker.ErrorLogging.AddErrorToListInformation("OriginatingNIDConnection.CallID: " + argdel.OriginatingNIDConnection.CallID);
                            Plugin.PimBroker.ErrorLogging.AddErrorToListInformation("OriginatingNIDConnection.DeviceID.ID: " + argdel.OriginatingNIDConnection.DeviceID.ID);
                            Plugin.PimBroker.ErrorLogging.AddErrorToListInformation("OriginatingNIDConnection.DeviceID.Type: " + argdel.OriginatingNIDConnection.DeviceID.Type);
                        }
                        catch { }
                        try
                        {
                            Plugin.PimBroker.ErrorLogging.AddErrorToListInformation("PrivateData.DistributingVDN): " + ((PrivateDataDelivered)argdel.PrivateData).DistributingVDN);
                            Plugin.PimBroker.ErrorLogging.AddErrorToListInformation("PrivateData.SplitSkill): " + ((PrivateDataDelivered)argdel.PrivateData).SplitSkill);
                            Plugin.PimBroker.ErrorLogging.AddErrorToListInformation("PrivateData.UserEnteredCode.CollectVDN): " + ((PrivateDataDelivered)argdel.PrivateData).UserEnteredCode.CollectVDN);
                            Plugin.PimBroker.ErrorLogging.AddErrorToListInformation("PrivateData.UserEnteredCode.Data): " + ((PrivateDataDelivered)argdel.PrivateData).UserEnteredCode.Data);
                            Plugin.PimBroker.ErrorLogging.AddErrorToListInformation("PrivateData.UserEnteredCode.Indicator): " + ((PrivateDataDelivered)argdel.PrivateData).UserEnteredCode.Indicator);
                            Plugin.PimBroker.ErrorLogging.AddErrorToListInformation("PrivateData.UserEnteredCode.Type): " + ((PrivateDataDelivered)argdel.PrivateData).UserEnteredCode.Type);
                            Plugin.PimBroker.ErrorLogging.AddErrorToListInformation("PrivateData.Version): " + ((PrivateDataDelivered)argdel.PrivateData).Version);
                        }
                        catch { }
                        Plugin.PimBroker.ErrorLogging.AddErrorToListInformation("UserData: " + argdel.UserData);
                    }
                    catch { }
                    break;
                case AgileSoftware.Developer.Device.enCallEventType.CSTAEstablished:
                    try
                    {
                        CSTAEstablishedEventArgs argest = arg.CallEvent.EventInfo as CSTAEstablishedEventArgs;
                        Plugin.PimBroker.ErrorLogging.AddErrorToListInformation("CSTAEstablished------------>");
                        try
                        {
                            Plugin.PimBroker.ErrorLogging.AddErrorToListInformation("AnsweringDevice.ID: " + argest.AnsweringDevice.ID);
                            Plugin.PimBroker.ErrorLogging.AddErrorToListInformation("AnsweringDevice.Type: " + argest.AnsweringDevice.Type);
                        }
                        catch { }
                        try
                        {
                            Plugin.PimBroker.ErrorLogging.AddErrorToListInformation("CalledDevice.ID: " + argest.CalledDevice.ID);
                            Plugin.PimBroker.ErrorLogging.AddErrorToListInformation("CalledDevice.Type: " + argest.CalledDevice.Type);
                        }
                        catch { }
                        try
                        {
                            Plugin.PimBroker.ErrorLogging.AddErrorToListInformation("CallingDevice.ID: " + argest.CallingDevice.ID);
                            Plugin.PimBroker.ErrorLogging.AddErrorToListInformation("CallingDevice.Type: " + argest.CallingDevice.Type);
                        }
                        catch { }
                        try
                        {
                            Plugin.PimBroker.ErrorLogging.AddErrorToListInformation("EstablishedConnection.CallID: " + argest.EstablishedConnection.CallID);
                            Plugin.PimBroker.ErrorLogging.AddErrorToListInformation("EstablishedConnection.DeviceID: " + argest.EstablishedConnection.DeviceID.ID);
                            Plugin.PimBroker.ErrorLogging.AddErrorToListInformation("EstablishedConnection.DeviceID: " + argest.EstablishedConnection.DeviceID.Type);
                        }
                        catch { }
                        Plugin.PimBroker.ErrorLogging.AddErrorToListInformation("EventCause: " + argest.EventCause);
                        Plugin.PimBroker.ErrorLogging.AddErrorToListInformation("GlobalCallLinkageID: " + argest.GlobalCallLinkageID);
                        Plugin.PimBroker.ErrorLogging.AddErrorToListInformation("InvokeID: " + argest.InvokeID);
                        try
                        {
                            Plugin.PimBroker.ErrorLogging.AddErrorToListInformation("LastRedirectionDevice.ID: " + argest.LastRedirectionDevice.ID);
                            Plugin.PimBroker.ErrorLogging.AddErrorToListInformation("LastRedirectionDevice.Type: " + argest.LastRedirectionDevice.Type);
                        }
                        catch { }
                        Plugin.PimBroker.ErrorLogging.AddErrorToListInformation("LocalConnectionState): " + argest.LocalConnectionState);
                        Plugin.PimBroker.ErrorLogging.AddErrorToListInformation("MonitorCrossRefID: " + argest.MonitorCrossRefID);

                        Plugin.PimBroker.ErrorLogging.AddErrorToListInformation("UserData: " + argest.UserData);
                    }
                    catch { }
                    break;
                case AgileSoftware.Developer.Device.enCallEventType.CSTAConnectionCleared:

                    CSTAConnectionClearedEventArgs argdisc = arg.CallEvent.EventInfo as CSTAConnectionClearedEventArgs;
                    Plugin.PimBroker.ErrorLogging.AddErrorToListInformation("CSTAConnectionCleared------------>");
                    try
                    {
                        Plugin.PimBroker.ErrorLogging.AddErrorToListInformation("DroppedConnection.CallID: " + argdisc.DroppedConnection.CallID);
                    }
                    catch { }
                    try
                    {
                        Plugin.PimBroker.ErrorLogging.AddErrorToListInformation("ReleasingDevice.ID: " + argdisc.ReleasingDevice.ID);
                        Plugin.PimBroker.ErrorLogging.AddErrorToListInformation("ReleasingDevice.Type: " + argdisc.ReleasingDevice.Type);
                    }
                    catch { }
                    try
                    {
                        Plugin.PimBroker.ErrorLogging.AddErrorToListInformation("DroppedConnection.DeviceID.ID: " + argdisc.DroppedConnection.DeviceID.ID);
                        Plugin.PimBroker.ErrorLogging.AddErrorToListInformation("DroppedConnection.DeviceID.Type: " + argdisc.DroppedConnection.DeviceID.Type);
                    }
                    catch { }
                    try
                    {
                        Plugin.PimBroker.ErrorLogging.AddErrorToListInformation("EventCause: " + argdisc.EventCause);
                        Plugin.PimBroker.ErrorLogging.AddErrorToListInformation("GlobalCallLinkageID: " + argdisc.GlobalCallLinkageID);
                        Plugin.PimBroker.ErrorLogging.AddErrorToListInformation("InvokeID: " + argdisc.InvokeID);
                        Plugin.PimBroker.ErrorLogging.AddErrorToListInformation("LocalConnectionState: " + argdisc.LocalConnectionState);
                        Plugin.PimBroker.ErrorLogging.AddErrorToListInformation("MonitorCrossRefID: " + argdisc.MonitorCrossRefID);
                        Plugin.PimBroker.ErrorLogging.AddErrorToListInformation("UserData: " + argdisc.UserData);
                    }
                    catch { }

                    break;
            }
            Plugin.PimBroker.ErrorLogging.AddErrorToListInformation("ITNVPluginPlayMsg:TelephonyEventHandler arg.CallEvent.CallEventType=" + arg.CallEvent.CallEventType + " end  <===");
        }

    }
}
