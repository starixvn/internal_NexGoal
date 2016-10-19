namespace NexGoal.Tools
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using System.Windows.Forms;
    using Fanex.Data;
    using NexGoal.Tools.Properties;

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
                    IList<EPAModel> epaInfos = new List<EPAModel>();
                    IEnumerable<PAFocusModel> pafocus = new List<PAFocusModel>();
                    X509KeyStorageFlags flags = X509KeyStorageFlags.MachineKeySet;

                    var connectionString = new StringBuilder();
                    connectionString.AppendFormat(Constants.ConnectionStringTemplate, txtServerName.Text, txtDatabaseName.Text, txtUserName.Text, txtPassword.Text);

                    try
                    {
                        X509Certificate2 oldCert2 = new X509Certificate2(txtOldCertificate.Text, txtOldPassword.Text, flags);
                        epaInfos = GetEPAInfoToChangeCertificate(connectionString.ToString());
                        DecryptScore(epaInfos, oldCert2);

                        pafocus = commonService.GetAllPAFocus(connectionString.ToString());
                        DecryptPAFocus(pafocus, oldCert2);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        return;
                    }

                    try
                    {
                        X509Certificate2 newCert2 = new X509Certificate2(txtNewCertificate.Text, txtNewPassword.Text, flags);
                        EncryptScore(epaInfos, newCert2);

                        EncryptPAFocus(pafocus, newCert2);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        return;
                    }

                    commonService.UpdatePAFocus(connectionString.ToString(), pafocus);
                    UpdateEPAScore(epaInfos, connectionString.ToString());
                }
            }
        }

        private IList<EPAModel> GetEPAInfoToChangeCertificate(string connectionString)
        {
            var criteria = new GetEPAInfoToChangeCertificateCriteria();

            IDbSetting setting = new DbSetting
            {
                ConnectionString = connectionString,
                SpName = criteria.GetSettingKey()
            };

            IList<EPAModel> epaInfos = commonService.GetEPAInfoToChangeCertificate(setting, criteria);

            return epaInfos;
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

        private void DecryptScore(IList<EPAModel> epaInfos, X509Certificate2 cert2)
        {
            foreach (EPAModel epaInfo in epaInfos)
            {
                epaInfo.FinalScore = certificateService.Decrypt(cert2, epaInfo.FinalScore);
                epaInfo.Grade = certificateService.Decrypt(cert2, epaInfo.Grade);

                foreach (EPAProcessModel process in epaInfo.EPAProcesses)
                {
                    process.TotalScore = certificateService.Decrypt(cert2, process.TotalScore);
                    process.AdjustScore = certificateService.Decrypt(cert2, process.AdjustScore);

                    foreach (EPAGroupDetailModel gr in process.EPAGroupDetails)
                    {
                        gr.Score = certificateService.Decrypt(cert2, gr.Score);

                        foreach (EPADetailModel detail in gr.EPADetails)
                        {
                            detail.Score = certificateService.Decrypt(cert2, detail.Score);
                            detail.Rate = certificateService.Decrypt(cert2, detail.Rate);
                        }
                    }
                }
            }
        }

        private void EncryptScore(IList<EPAModel> epaInfos, X509Certificate2 cert2)
        {
            foreach (EPAModel epaInfo in epaInfos)
            {
                epaInfo.FinalScore = certificateService.Encrypt(cert2, epaInfo.FinalScore);
                epaInfo.Grade = certificateService.Encrypt(cert2, epaInfo.Grade);

                foreach (EPAProcessModel process in epaInfo.EPAProcesses)
                {
                    process.TotalScore = certificateService.Encrypt(cert2, process.TotalScore);
                    process.AdjustScore = certificateService.Encrypt(cert2, process.AdjustScore);

                    foreach (EPAGroupDetailModel gr in process.EPAGroupDetails)
                    {
                        gr.Score = certificateService.Encrypt(cert2, gr.Score);

                        foreach (EPADetailModel detail in gr.EPADetails)
                        {
                            detail.Score = certificateService.Encrypt(cert2, detail.Score);
                            detail.Rate = certificateService.Encrypt(cert2, detail.Rate);
                        }
                    }
                }
            }
        }

        private void UpdateEPAScore(IList<EPAModel> epaInfos, string connectionString)
        {
            int skip = 0,
                numberOfRecords = 50;
            List<EPAModel> thisBatch;
            Queue<List<EPAModel>> allBatches = new Queue<List<EPAModel>>();

            do
            {
                thisBatch = epaInfos.Skip(skip).Take(numberOfRecords).ToList();

                if (thisBatch.Any())
                {
                    allBatches.Enqueue(thisBatch);
                    skip += numberOfRecords;
                }
            }
            while (thisBatch.Any());

            try
            {
                while (allBatches.Any())
                {
                    List<EPAModel> currentBatch = allBatches.Dequeue();

                    var criteria = new UpdateEPAScoreCriteria
                    {
                        EpaInfos = currentBatch
                    };
                    criteria.ConvertToXml();

                    IDbSetting setting = new DbSetting
                    {
                        ConnectionString = connectionString,
                        SpName = criteria.GetSettingKey()
                    };

                    CommandResult commandResult = commonService.UpdateEPAScore(setting, criteria);
                }

                MessageBox.Show(Resources.UpdateSuccessfully, Resources.Notification, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
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