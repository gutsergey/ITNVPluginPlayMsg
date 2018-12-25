using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Linq;

namespace ITNVPluginPlayMsg
{
    /// <summary>
    /// It contains the logic of creating buttons in the CCE Desktop Tool Strip.
    /// </summary>
    internal class ToolStripButtonsPlayIVRMsg
    {
        public static ToolStripButtonsPlayIVRMsg Instance;

        private ToolStripComboBox cmbMsgNumber;
        private ToolStripComboBox cmbLanguage;

        private ToolStripButton btnPlay;
        private ToolStripButton btnClose;
        private ToolStripButton btnFromBeginning;
        private ToolStripButton btnNext;
        private ToolStripLabel lblIVRlinesstatus;

        private ToolStripLabel lblMsg1;
        private ToolStripLabel lblMsg2;

        private List<string> curmsg = null;
        private int cursor;
        private bool doconference = true;
        public static string PhoneNumber = "";
        public static string IVRLine = "";

        private static ITNVPlayMsgManager.ITNVPlayMsgManager Playmsgmngr;
        private static string ConferenceNumber = "";

        private string textMsg1lbl = "";
        private string textMsg2lbl = "";
        private string textMsglast = "";

        private void CreateButtons()
        {
            ConferenceNumber = Plugin.PluginConfiguration.PlayMsgButtons.ConferenceNumber;
            Playmsgmngr = Plugin.PluginConfiguration.PlayMsgButtons.Playmsgmng;

            ToolStripSeparator toolStripSeparator1;
            toolStripSeparator1 = new ToolStripSeparator();
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new System.Drawing.Size(16, 35);

            //PictureBox pictureBox = new PictureBox();
            //pictureBox.Image = global::ITNVPluginPlayMsg.Properties.Resources.Logo;
            //pictureBox.InitialImage = global::ITNVPluginPlayMsg.Properties.Resources.Logo;
            //pictureBox.Location = new System.Drawing.Point(99, 61);
            //pictureBox.Name = "pictureBox";
            //pictureBox.Size = new System.Drawing.Size(67, 24);
            //pictureBox.TabIndex = 12;
            //pictureBox.TabStop = false;


            lblIVRlinesstatus = new ToolStripLabel();
            lblIVRlinesstatus.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            lblIVRlinesstatus.Name = "lblIVRlinesstatus";
            lblIVRlinesstatus.Size = new System.Drawing.Size(100, 35);
            lblIVRlinesstatus.AutoSize = false;
            lblIVRlinesstatus.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            lblIVRlinesstatus.Text = "";
            lblIVRlinesstatus.Click += new EventHandler(lblIVRlinesstatus_Click);

            cmbMsgNumber = new ToolStripComboBox();
            cmbMsgNumber.Name = "cmbMsgNumber";
            cmbMsgNumber.Size = new System.Drawing.Size(200, 35);

            cmbLanguage = new ToolStripComboBox();
            cmbLanguage.Name = "cmbLanguage";
            cmbLanguage.Size = new System.Drawing.Size(105, 35);

            lblMsg1 = new ToolStripLabel();
            lblMsg1.AutoSize = false;
            lblMsg1.Size = new System.Drawing.Size(200, 35);

            lblMsg2 = new ToolStripLabel();
            lblMsg2.AutoSize = false;
            lblMsg2.Size = new System.Drawing.Size(200, 35);

            btnPlay = new ToolStripButton();
            btnPlay.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            btnPlay.Name = "btnPlay";
            btnPlay.Size = new System.Drawing.Size(37, 35);
            btnPlay.Click += new EventHandler(btnPlay_Click);
            btnPlay.Image = global::ITNVPluginPlayMsg.Properties.Resources.Play;

            btnClose = new ToolStripButton();
            btnClose.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            btnClose.Name = "btnClose";
            btnClose.Size = new System.Drawing.Size(37, 35);
            btnClose.Click += new EventHandler(btnClose_Click);
            btnClose.Image = global::ITNVPluginPlayMsg.Properties.Resources.Close;

            btnFromBeginning = new ToolStripButton();
            btnFromBeginning.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            btnFromBeginning.Name = "btnFromBeginning";
            btnFromBeginning.Size = new System.Drawing.Size(37, 35);
            btnFromBeginning.Click += new EventHandler(btnFromBeginning_Click);
            btnFromBeginning.Image = global::ITNVPluginPlayMsg.Properties.Resources.Beginning;

            btnNext = new ToolStripButton();
            btnNext.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            btnNext.Name = "btnNext";
            btnNext.Size = new System.Drawing.Size(37, 35);
            btnNext.Click += new EventHandler(btnNext_Click);
            btnNext.Image = global::ITNVPluginPlayMsg.Properties.Resources.NextNew;

            //There are four tool stip bars in the CCE Desktop host window. 
            //Assumes to add the tool strip items to the first top tool strip.
            ToolStrip hostWindowToolStrip = Plugin.GuiHost.GetMainFormToolStrip(AgileSoftware.GUIHost.enToolStripPosition.TopSecond);

            //Check if the last item is a seperator
            if (hostWindowToolStrip.Items.Count > 0 &&
                hostWindowToolStrip.Items[hostWindowToolStrip.Items.Count - 1].GetType() != typeof(System.Windows.Forms.ToolStripSeparator))
            {
                hostWindowToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                                            lblIVRlinesstatus,
                                            cmbMsgNumber,
                                            toolStripSeparator1,
                                            btnPlay,
                                            lblMsg1,
                                            btnNext,
                                            new ToolStripLabel() {Size = new System.Drawing.Size(10, 35), AutoSize = false },
                                            btnFromBeginning,
                                            new ToolStripLabel() {Size = new System.Drawing.Size(10, 35), AutoSize = false },
                                            cmbLanguage,
                                            lblMsg2,
                                            btnClose }
                                    );
            }
            else
            {
                hostWindowToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                                            lblIVRlinesstatus,
                                            cmbMsgNumber,
                                            btnPlay,
                                            lblMsg1,
                                            btnNext,
                                            new ToolStripLabel() {Size = new System.Drawing.Size(10, 35), AutoSize = false },
                                            btnFromBeginning,
                                            new ToolStripLabel() {Size = new System.Drawing.Size(10, 35), AutoSize = false },
                                            cmbLanguage,
                                            lblMsg2,
                                            btnClose }
                                    );

            }

            TextsAndTooltips();
            LoadData();
            AllButtonsDiasable();

        }

        private void lblIVRlinesstatus_Click(object sender, EventArgs e)
        {
            if (!Plugin.PluginConfiguration.PlayMsgButtons.ShowIVRLines)
            {
                lblIVRlinesstatus.Text = "";
                return;
            }

            if (!btnClose.Enabled) return;
            ITNVPlayMsgManager.HuntInfo huntinfo = Playmsgmngr.GetHuntInfo(ConferenceNumber);
            if (huntinfo.total > 0)
                lblIVRlinesstatus.Text = "" + huntinfo.total + "-" + huntinfo.free;
        }

        public  void AllButtonsDiasable()
        {
            btnClose.Enabled = false;
            btnNext.Enabled = false;
            btnPlay.Enabled = false;
            btnFromBeginning.Enabled = false;
            cmbLanguage.Enabled = false;
            cmbMsgNumber.Enabled = false;
            lblMsg1.Text = "";
            lblMsg2.Text = "";
            doconference = true;
            lblIVRlinesstatus.Text = "";
        }

        public void AllButtonsEnable()
        {
            btnClose.Enabled = true;
            btnNext.Enabled = true;
            btnPlay.Enabled = true;
            btnFromBeginning.Enabled = true;
            cmbLanguage.Enabled = true;
            cmbMsgNumber.Enabled = true;
            lblMsg1.Text = "";
            lblMsg2.Text = "";

            if (!Plugin.PluginConfiguration.PlayMsgButtons.ShowIVRLines)
            {
                lblIVRlinesstatus.Text = "";
                return;
            }

            ITNVPlayMsgManager.HuntInfo huntinfo =   Playmsgmngr.GetHuntInfo(ConferenceNumber);
            if (huntinfo.total > 0)
                lblIVRlinesstatus.Text = "" + huntinfo.total + "-" + huntinfo.free;
        }

        private void TextsAndTooltips()
        {
            if (!Plugin.PluginConfiguration.General.Language.Equals("en-US"))
            {
                
                //lblLanguage.Text = "שפה";
                //lblMsgNumber.Text = "מסי שאלה";
                //lblMsgNumber.Text = "מסי שאלון";

                btnNext.ToolTipText = "הקראת הבאה";
                btnPlay.ToolTipText =  "הקראת שאלה";
                btnFromBeginning.ToolTipText = "הקראה מההתחלה";
                btnClose.ToolTipText = "נתק";

                textMsg1lbl = "הקו תפוס";
                textMsg2lbl = "הקו תפוס או לא נבחרה הודעה";

                textMsglast = "אחרונה";

            }
            else
            {
                //lblLanguage.Text = "Language";
                //lblMsgNumber.Text = "msg num";
                //lblQuestionnaire.Text = "list num";

                btnNext.ToolTipText = "Play next message";
                btnPlay.ToolTipText = "Play message";
                btnFromBeginning.ToolTipText = "Play from the beginning";
                btnClose.ToolTipText = "Stop";

                textMsg1lbl = "line is busy";
                textMsg2lbl = "line is busy or no message selected";

                textMsglast = "last";
            }
        }

        private void LoadData()
        {
            cmbLanguage.Enabled = true;
            cmbLanguage.ComboBox.DataSource = Plugin.PluginConfiguration.PlayMsgButtons.Language;
            //cmbLanguage.Text = cmbLanguage.Items[0].ToString();

            cursor = 1;
            curmsg = new List<string>();
            curmsg.Clear();
            curmsg = Plugin.PluginConfiguration.PlayMsgMessages.Messages.Select(x => x.msg).ToList<string>();
            curmsg.Insert(0,"");
            ////curmsg = curmsg.OrderBy(x => x).ToList();
            List<string> msg1 = curmsg.Take(cursor).ToList();
            cmbMsgNumber.ComboBox.DataSource = msg1;
        }


        public void CloseSession()
        {
            bool rc = Playmsgmngr.CloseSession(PhoneNumber);
            doconference = true;

            cursor = 1;

            List<string> msg1 = curmsg.Take(cursor).ToList();
            cmbMsgNumber.ComboBox.DataSource = msg1;
            lblMsg1.Text = "";

            int irc = Plugin.XMLStation.CallPartyDrop(IVRLine);
            IVRLine = "";

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            CloseSession();
        }

        private void btnFromBeginning_Click(object sender, EventArgs e)
        {
            cursor = 1;

            List<string> msg1 = curmsg.Take(cursor).ToList();
            cmbMsgNumber.ComboBox.DataSource = msg1;
            lblMsg1.Text = "";
        }
        private void btnPlay_Click(object sender, EventArgs e)
        {
            string msg = Playmsgmngr.GetMessage(PhoneNumber);
            if (msg.Equals("0") && cmbMsgNumber.Text.Trim().Length>0)
            {
                lblMsg2.Text = "";
                bool rc = Playmsgmngr.PlayMessage(PhoneNumber, cmbMsgNumber.Text, cmbLanguage.Text);
            }
            else
            {
                lblMsg2.ForeColor = System.Drawing.Color.Red;
                lblMsg2.Text = textMsg2lbl;
            }
        }
        private void btnNext_Click(object sender, EventArgs e)
        {
            string txt = "";
            bool rc = false;
            if (doconference)
            {
                PlayMessage();
                return;
            }

            string msg = Playmsgmngr.GetMessage(PhoneNumber).Split(';')[0];
            if (msg.Equals("9"))
            {
                rc = Playmsgmngr.CloseSession(PhoneNumber);
                cursor = 1;
                doconference = true;
                msg = "0";
            }
            if (msg.Equals("0"))
            {
                PlayMessage();
            }
            else
            {
                lblMsg2.ForeColor = System.Drawing.Color.Red;
                lblMsg2.Text = textMsg1lbl;
            }

        }

        private void PlayMessage()
        {
            string txt = "";
            bool rc = false;

            cursor++;
            lblMsg2.Text = "";
            List<string> msg1 = curmsg.Take(cursor).ToList();
            cmbMsgNumber.ComboBox.DataSource = msg1;

            lblMsg1.ForeColor = System.Drawing.Color.Black;
            if (cursor >= curmsg.Count)
            {
                txt = textMsglast;
                lblMsg1.ForeColor = System.Drawing.Color.Red;
            }

            if (cursor <= curmsg.Count)
            {
                if (doconference)
                {
                    rc = Playmsgmngr.OpenSession(PhoneNumber, ConferenceNumber, msg1.Last(), cmbLanguage.Text);
                    doconference = false;
                }
                else
                {
                    rc = Playmsgmngr.PlayMessage(PhoneNumber, msg1.Last(), cmbLanguage.Text);
                }
            }
            lblMsg1.Text = msg1.Last() + " " + txt;
        }

        internal static void StartWorking()
        {
            //Create Tool Strip buttons
            Instance = new ToolStripButtonsPlayIVRMsg();
            Instance.CreateButtons();
        }
    }
}
