namespace NexGoal.Tools
{
    using System;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using System.Windows.Forms;
    using Fanex.Data;

    public partial class UCChangeCertificate : UserControl
    {
        private readonly ICertificateService certificateService;
        private readonly ICommonService commonService;

        public UCChangeCertificate()
        {
            InitializeComponent();

            certificateService = new CertificateService();
            commonService = new CommonService();
        }

        private void btnOpenOldFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Choose pfx file";
            openFileDialog.Multiselect = false;
            openFileDialog.Filter = "pfx file (*.pfx)|*.pfx";
            DialogResult dialogResult = openFileDialog.ShowDialog();

            if (dialogResult == DialogResult.OK)
            {
                txtOldCertificate.Text = openFileDialog.FileName;
            }
        }

        private void btnOpenNewFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Choose pfx file";
            openFileDialog.Multiselect = false;
            openFileDialog.Filter = "pfx file (*.pfx)|*.pfx";
            DialogResult dialogResult = openFileDialog.ShowDialog();

            if (dialogResult == DialogResult.OK)
            {
                txtNewCertificate.Text = openFileDialog.FileName;
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            EPAViewModel epaInfo = new EPAViewModel();
            X509KeyStorageFlags flags = X509KeyStorageFlags.MachineKeySet;

            var connectionString = new StringBuilder();
            connectionString.AppendFormat(Constants.ConnectionStringTemplate, txtServerName.Text, txtDatabaseName.Text, txtUserName.Text, txtPassword.Text);

            try
            {
                X509Certificate2 oldCert2 = new X509Certificate2(txtOldCertificate.Text, txtOldPassword.Text, flags);
                epaInfo = GetEPAInfoToChangeCertificate(connectionString.ToString());
                DecryptScore(epaInfo, oldCert2);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

            try
            {
                X509Certificate2 newCert2 = new X509Certificate2(txtNewCertificate.Text, txtNewPassword.Text, flags);
                EncryptScore(epaInfo, newCert2);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

            UpdateEPAScore(epaInfo, connectionString.ToString());
        }

        private EPAViewModel GetEPAInfoToChangeCertificate(string connectionString)
        {
            var criteria = new GetEPAInfoToChangeCertificateCriteria();

            IDbSetting setting = new DbSetting
            {
                ConnectionString = connectionString,
                SpName = criteria.GetSettingKey()
            };

            EPAViewModel epaInfo = commonService.GetEPAInfoToChangeCertificate(setting, criteria);

            return epaInfo;
        }

        private void DecryptScore(EPAViewModel epaInfo, X509Certificate2 cert2)
        {
            foreach (EPAModel epa in epaInfo.Epas)
            {
                epa.FinalScore = certificateService.Decrypt(cert2, epa.FinalScore);
                epa.Grade = certificateService.Decrypt(cert2, epa.Grade);
            }

            foreach (EPAProcessModel process in epaInfo.Processes)
            {
                process.TotalScore = certificateService.Decrypt(cert2, process.TotalScore);
                process.AdjustScore = certificateService.Decrypt(cert2, process.AdjustScore);
            }

            foreach (EPAGroupDetailModel gr in epaInfo.EpaGroups)
            {
                gr.Score = certificateService.Decrypt(cert2, gr.Score);
            }

            foreach (EPADetailModel detail in epaInfo.EpaDetails)
            {
                detail.Score = certificateService.Decrypt(cert2, detail.Score);
                detail.Rate = certificateService.Decrypt(cert2, detail.Rate);
            }
        }

        private void EncryptScore(EPAViewModel epaInfo, X509Certificate2 cert2)
        {
            foreach (EPAModel epa in epaInfo.Epas)
            {
                epa.FinalScore = certificateService.Encrypt(cert2, epa.FinalScore);
                epa.Grade = certificateService.Encrypt(cert2, epa.Grade);
            }

            foreach (EPAProcessModel process in epaInfo.Processes)
            {
                process.TotalScore = certificateService.Encrypt(cert2, process.TotalScore);
                process.AdjustScore = certificateService.Encrypt(cert2, process.AdjustScore);
            }

            foreach (EPAGroupDetailModel gr in epaInfo.EpaGroups)
            {
                gr.Score = certificateService.Encrypt(cert2, gr.Score);
            }

            foreach (EPADetailModel detail in epaInfo.EpaDetails)
            {
                detail.Score = certificateService.Encrypt(cert2, detail.Score);
                detail.Rate = certificateService.Encrypt(cert2, detail.Rate);
            }
        }

        private void UpdateEPAScore(EPAViewModel epaInfo, string connectionString)
        {
            var criteria = new UpdateEPAScoreCriteria
            {
                EpaInfo = epaInfo
            };

            IDbSetting setting = new DbSetting
            {
                ConnectionString = connectionString,
                SpName = criteria.GetSettingKey()
            };

            try
            {
                CommandResult commandResult = commonService.UpdateEPAScore(setting, criteria);

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
}