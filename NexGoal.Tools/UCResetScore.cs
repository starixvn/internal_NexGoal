using System;
using System.Text;
using System.Windows.Forms;
using Fanex.Data;

namespace NexGoal.Tools
{
    public partial class UCResetScore : UserControl
    {
        private readonly ICommonService commonService;

        private const string connectionStringTemplate = @"packet size=4096;Data Source={0};TrustServerCertificate=true;Connect Timeout=60;
            Asynchronous Processing=true;Initial Catalog={1};integrated security=false; user id={2}; password={3};";

        public UCResetScore()
        {
            InitializeComponent();
            commonService = new CommonService();
        }

        private void btnResetScore_Click(object sender, EventArgs e)
        {
            if (CheckInputData())
            {
                DialogResult result = MessageBox.Show("Are you sure you want to reset score?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    var criteria = new ResetScoreCriteria();
                    var connectionString = new StringBuilder();

                    connectionString.AppendFormat(
                        connectionStringTemplate,
                        txtServerName.Text,
                        txtDatabaseName.Text,
                        txtUserName.Text,
                        txtPassword.Text);

                    IDbSetting setting = new DbSetting
                    {
                        ConnectionString = connectionString.ToString(),
                        SpName = criteria.GetSettingKey()
                    };

                    try
                    {
                        CommandResult commandResult = commonService.ResetScore(setting, criteria);

                        if (!commandResult.IsSuccessful)
                        {
                            MessageBox.Show(commandResult.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            MessageBox.Show("Reset score successfully!", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please input data", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool CheckInputData()
        {
            bool isValid = true;

            if (string.IsNullOrWhiteSpace(txtServerName.Text)
                || string.IsNullOrWhiteSpace(txtUserName.Text)
                || string.IsNullOrWhiteSpace(txtPassword.Text)
                || string.IsNullOrWhiteSpace(txtDatabaseName.Text))
            {
                isValid = false;
            }

            return isValid;
        }
    }
}