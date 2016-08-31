namespace NexGoal.Tools
{
    using System;
    using System.Text;
    using System.Windows.Forms;
    using Fanex.Data;
    using NexGoal.Tools.Properties;

    public partial class UCResetScore : UserControl
    {
        private readonly ICommonService commonService;

        public UCResetScore()
        {
            InitializeComponent();
            commonService = new CommonService();
        }

        private void ResetScore_Click(object sender, EventArgs e)
        {
            if (CheckDataInput())
            {
                DialogResult result = MessageBox.Show(Resources.MsgConfirmResetScore, Resources.Confirmation, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    var criteria = new ResetScoreCriteria();
                    var connectionString = new StringBuilder();

                    connectionString.AppendFormat(Constants.ConnectionStringTemplate, txtServerName.Text, txtDatabaseName.Text, txtUserName.Text, txtPassword.Text);

                    IDbSetting setting = new DbSetting
                    {
                        ConnectionString = connectionString.ToString(),
                        SpName = criteria.GetSettingKey()
                    };

                    try
                    {
                        CommandResult commandResult = commonService.ResetScore(setting, criteria);

                        if (commandResult.IsSuccessful)
                        {
                            MessageBox.Show(Resources.UpdateSuccessfully, Resources.Notification, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show(commandResult.Message, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private bool CheckDataInput()
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