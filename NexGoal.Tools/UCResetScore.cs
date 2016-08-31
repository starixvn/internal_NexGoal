namespace NexGoal.Tools
{
    using System;
    using System.Text;
    using System.Windows.Forms;
    using Fanex.Data;

    public partial class UCResetScore : UserControl
    {
        private readonly ICommonService commonService;

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
                            MessageBox.Show("Update successfully!", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show(commandResult.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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