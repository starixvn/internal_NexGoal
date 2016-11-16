namespace NexGoal.Tools
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography.X509Certificates;
    using System.Windows.Forms;
    using NexGoal.Tools.Properties;

    public partial class UCChangeCertificate : UserControl
    {
        private const int NumberOfItemToOperate = 50;
        private readonly ICertificateService certificateService;
        private readonly ICommonService commonService;

        public UCChangeCertificate()
        {
            InitializeComponent();

            certificateService = new CertificateService();
            commonService = new CommonService();
        }

        private void OpenOldFile_Click(object sender, EventArgs e)
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

        private void OpenNewFile_Click(object sender, EventArgs e)
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

        private void Update_Click(object sender, EventArgs e)
        {
            if (CheckDataInput())
            {
                DialogResult result = MessageBox.Show(Resources.MsgConfirmUpdateScore, Resources.Confirmation, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    ChangeCertification();
                }
            }
        }

        private void ChangeCertification()
        {
            try
            {
                var oldCertificate = new X509Certificate2(txtOldCertificate.Text, txtOldPassword.Text, X509KeyStorageFlags.MachineKeySet);
                var newCertificate = new X509Certificate2(txtNewCertificate.Text, txtNewPassword.Text, X509KeyStorageFlags.MachineKeySet);
                var connectionString = string.Format(Constants.ConnectionStringTemplate, txtServerName.Text, txtDatabaseName.Text, txtUserName.Text, txtPassword.Text);

                ChangeCertificatedOfEPA(oldCertificate, newCertificate, connectionString);

                ChangeCertificatedOfPAFocus(oldCertificate, newCertificate, connectionString);

                MessageBox.Show(Resources.UpdateSuccessfully, Resources.Notification, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }

        private void ChangeCertificatedOfPAFocus(X509Certificate2 oldCertificate, X509Certificate2 newCertificate, string connectionString)
        {
            var isContinue = true;
            int idFrom = 1, idTo = NumberOfItemToOperate;
            while (isContinue)
            {
                var focuses = commonService.GetAllPAFocus(connectionString, idFrom, idTo);
                if (focuses.Any())
                {
                    DecryptPAFocus(focuses, oldCertificate);

                    EncryptPAFocus(focuses, newCertificate);

                    var result = commonService.UpdatePAFocus(connectionString, focuses);
                    if (!result.IsSuccessful)
                    {
                        throw new Exception("Update PA Focus unsuccessful");
                    }

                    idFrom = idTo;
                    idTo += NumberOfItemToOperate;
                }
                else
                {
                    isContinue = false;
                }
            }
        }

        private void ChangeCertificatedOfEPA(X509Certificate2 oldCertificate, X509Certificate2 newCertificate, string connectionString)
        {
            var isContinue = true;
            int idFrom = 1, idTo = NumberOfItemToOperate;
            while (isContinue)
            {
                var epas = commonService.GetEPAInfo(connectionString, idFrom, idTo);
                if (epas.Any())
                {
                    DecryptScore(epas, oldCertificate);

                    EncryptScore(epas, newCertificate);

                    var result = commonService.UpdateEPAScore(connectionString, epas);
                    if (!result.IsSuccessful)
                    {
                        throw new Exception("Update PA unsuccessful");
                    }

                    idFrom = idTo;
                    idTo += NumberOfItemToOperate;
                }
                else
                {
                    isContinue = false;
                }
            }
        }

        private void DecryptPAFocus(IEnumerable<PAFocusModel> focus, X509Certificate2 cert2)
        {
            foreach (var item in focus)
            {
                item.EncryptedBonusGiven = certificateService.Decrypt(cert2, item.EncryptedBonusGiven);
                item.EncryptedContribution = certificateService.Decrypt(cert2, item.EncryptedContribution);
                item.EncryptedIsPoorPerformance = certificateService.Decrypt(cert2, item.EncryptedIsPoorPerformance);
                item.EncryptedIsReadyPromotion = certificateService.Decrypt(cert2, item.EncryptedIsReadyPromotion);
            }
        }

        private void EncryptPAFocus(IEnumerable<PAFocusModel> focus, X509Certificate2 cert2)
        {
            foreach (var item in focus)
            {
                item.EncryptedBonusGiven = certificateService.Encrypt(cert2, item.EncryptedBonusGiven);
                item.EncryptedContribution = certificateService.Encrypt(cert2, item.EncryptedContribution);
                item.EncryptedIsPoorPerformance = certificateService.Encrypt(cert2, item.EncryptedIsPoorPerformance);
                item.EncryptedIsReadyPromotion = certificateService.Encrypt(cert2, item.EncryptedIsReadyPromotion);
            }
        }

        private void DecryptScore(IEnumerable<EPAModel> epaInfos, X509Certificate2 cert2)
        {
            foreach (EPAModel epaInfo in epaInfos)
            {
                epaInfo.FinalScore = certificateService.Decrypt(cert2, epaInfo.FinalScore);
                epaInfo.Grade = certificateService.Decrypt(cert2, epaInfo.Grade);

                foreach (EPAProcessModel process in epaInfo.EPAProcesses)
                {
                    process.TotalScore = certificateService.Decrypt(cert2, process.TotalScore);
                    process.AdjustScore = certificateService.Decrypt(cert2, process.AdjustScore);                    
                }
            }
        }

        private void EncryptScore(IEnumerable<EPAModel> epaInfos, X509Certificate2 cert2)
        {
            foreach (EPAModel epaInfo in epaInfos)
            {
                epaInfo.FinalScore = certificateService.Encrypt(cert2, epaInfo.FinalScore);
                epaInfo.Grade = certificateService.Encrypt(cert2, epaInfo.Grade);

                foreach (EPAProcessModel process in epaInfo.EPAProcesses)
                {
                    process.TotalScore = certificateService.Encrypt(cert2, process.TotalScore);
                    process.AdjustScore = certificateService.Encrypt(cert2, process.AdjustScore);                    
                }
            }
        }

        private bool CheckDataInput()
        {
            bool isValid = true;

            if (string.IsNullOrWhiteSpace(txtServerName.Text)
                || string.IsNullOrWhiteSpace(txtUserName.Text)
                || string.IsNullOrWhiteSpace(txtPassword.Text)
                || string.IsNullOrWhiteSpace(txtDatabaseName.Text)
                || string.IsNullOrEmpty(txtOldCertificate.Text)
                || string.IsNullOrEmpty(txtNewCertificate.Text))
            {
                isValid = false;
            }

            return isValid;
        }
    }
}