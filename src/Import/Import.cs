
#region Copyright notice
// ==========================================================
// This is an as-is implementation. you can certainly use
// this code with modification that you need.
// It will be appreciated if you keep the copyright text 
// while distribution.
// - Moim Hossain
//   2007
// ===========================================================
#endregion

#region Using Directive
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ImportLib;
using ImportLib.Mappings;
using ImportLib.Supports;
using ImportLib.Delimited;
#endregion

namespace Import
{
    /// <summary>
    ///     A sample form 
    /// </summary>
    public partial class Import : Form , ILogProvider
    {
        /// <summary>
        ///     Creates a new instance
        /// </summary>
        public Import()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Loading event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Import_Load(object sender, EventArgs e)
        {
            
        }

        /// <summary>
        ///     Opens a csv file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdOpen_Click(object sender, EventArgs e)
        {
            txtSourceFile.Text = string.Empty;
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.DefaultExt = "*.csv";
            if (ofd.ShowDialog(this) == DialogResult.OK)
            {
                txtSourceFile.Text = ofd.FileName;
            }
        }

        /// <summary>
        ///     Run the process
        /// </summary>
        private void Run()
        {
            ShowBusyIcon();
            try
            {
                Logger.LogProvider = this;
                WriteStatus("Started...please wait...");
                IDictionary<string, string> sourceProperties = new Dictionary<string, string>();
                sourceProperties.Add("fileName", txtSourceFile.Text);
                sourceProperties.Add("delimiter", txtDelimniter.Text);
                sourceProperties.Add("headerRow", chkHeaderRowPresent.Checked ? "Yes" : "No");
                sourceProperties.Add("TextQualifier", txtTextQualifier.Text);

                IDictionary<string, string> destinationProperties = new Dictionary<string, string>();
                destinationProperties.Add("sqlserver", txtSqlServername.Text);
                destinationProperties.Add("databaseName", txtDatabase.Text);
                destinationProperties.Add("tableName", txtTable.Text);
                destinationProperties.Add("dbUserName", txtUserName.Text);
                destinationProperties.Add("dbUserPassword", txtPassword.Text);
                destinationProperties.Add("integratedSecurity", txtIntegratedSecurity.Text);

                DelimitedDataSource src = new DelimitedDataSource();
                src.InitializeSource(sourceProperties);

                ColumnMappingController colMapController = new ColumnMappingController();
                List<Column> sources = new List<Column>();
                List<Column> destinations = new List<Column>();
                foreach (DataRow row in src.GetSchemaTable().Rows)
                {
                    string name = row["ColumnName"] as string;
                    sources.Add(new Column(name));
                    destinations.Add(new Column(name));

                    colMapController.Mappings.Add(new Map(new Column(name), new Column(name)));
                }
                colMapController.DestinationColumns = destinations.ToArray();

                ImportManager manager = new ImportManager();
                manager.LogProvider = this; // keep ssis log too
                ExecutionResult result = manager.Transfer(
                    StorageMedium.DelimitedFile,
                    StorageMedium.SqlServerDatabase,
                    sourceProperties,
                    destinationProperties, colMapController);
            }
            catch (Exception ex)
            {
                WriteStatus("Error : " + ex.Message); 
            }
            finally
            {
                ShowNormIcon();
            }
        }

        /// <summary>
        ///     Perform the import task
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdImport_Click(object sender, EventArgs e)
        {            
            System.Threading.Thread back = new System.Threading.Thread(new System.Threading.ThreadStart(Run));
            back.Start();            
        }

        /// <summary>
        ///     Show the busy icon
        /// </summary>
        private void ShowBusyIcon()
        {
            if (this.InvokeRequired)
            {
                AsyncInvocationDelegate aid = new AsyncInvocationDelegate(this.ShowBusyIcon);
                this.Invoke(aid);
            }
            else
            {
                this.Cursor = System.Windows.Forms.Cursors.IBeam;
            }
        }

        /// <summary>
        ///     Show normal icon
        /// </summary>
        private void ShowNormIcon()
        {
            if (this.InvokeRequired)
            {
                AsyncInvocationDelegate aid = new AsyncInvocationDelegate(this.ShowNormIcon);
                this.Invoke(aid);
            }
            else 
            {
                this.Cursor = System.Windows.Forms.Cursors.Arrow;
            }
        }

        delegate void AsyncInvocationDelegate();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        private void WriteStatus(string message)
        {
            if (listViewStatus.InvokeRequired)
            {
                WriteStatusDelegate dlg = new WriteStatusDelegate(WriteStatus);
                listViewStatus.Invoke(dlg, new object[] { message });
            }
            else
            {
                ListViewItem item = listViewStatus.Items.Add(message);
                listViewStatus.EnsureVisible(item.Index);

            }
        }

        delegate void WriteStatusDelegate(string message);

        #region ILogProvider Members

        /// <summary>
        ///     Write messages
        /// </summary>
        /// <param name="message"></param>
        public void WriteMessage(string message)
        {
            WriteStatus(message);
        }

        #endregion
    }
}