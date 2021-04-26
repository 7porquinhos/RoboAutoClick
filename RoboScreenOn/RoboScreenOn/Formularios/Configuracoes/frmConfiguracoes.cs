using MetroFramework.Forms;
using Microsoft.Win32;
using RoboScreenOn.BLL.Enum;
using RoboScreenOn.BLL.Utils.Singleton;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RoboScreenOn.Formularios.Configuracoes
{
    public partial class frmConfiguracoes : Form
    {

        #region Variaveis
        string path = "";
        List<string> ListaProximaExecucao = new List<string>();
        #endregion

        #region Construtor
        public frmConfiguracoes()
        {
            InitializeComponent();
        }
        #endregion

        #region Configuração frmConfigurações
        private void frmConfiguracoes_Load(object sender, EventArgs e)
        {
            InicializaDelegates();
            CarregarVariavelPath();
            CarregarEstadoCheckbox();
            CarregarEstadoCheckboxClick();
            CarregaComboBox();
        }

        private void FecharForm(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        #region Configuração Delegates
        private void InicializaDelegates()
        {
            btnFecharForm.Click += FecharForm;
            chbAtivado.CheckedChanged += IniciarAplicacao;
            chbclicarTeclado.CheckedChanged += TipoDeClick;
            cbProximaExecucao.SelectedIndexChanged += SelecionaItemComboBox;
        }
        #endregion

        #region Configuração Iniciar com Windows
        private void CarregarVariavelPath()
        {
            path = AppDomain.CurrentDomain.BaseDirectory.ToString();
        }


        private void CarregarEstadoCheckbox()
        {
            chbAtivado.Checked = Properties.Settings.Default.chbIniciarComWin;
            chbAtivado.Text = chbAtivado.Checked ? "Ativado" : "Desativado";
        }

        private void IniciarComWindows(bool iniciar = false)
        {
            try
            {
                if (iniciar)
                {
                    using (RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
                    {
                        key.SetValue(path, "\"" + Application.ExecutablePath + "\"");
                    }
                }
                else
                {
                    using (RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
                    {
                        key.DeleteValue(path, false);
                    }
                }
            }
            catch (Exception ex)
            {

                MainInfo.FCInfo().EscreverConsoleLog($"Erro: {ex.Message}", TipoConsoleLog.Erro, false, "IniciarComWindows", ex);
            }

        }

        private void IniciarAplicacao(object sender, EventArgs e)
        {
            if (chbAtivado.Checked)
            {
                Properties.Settings.Default.chbIniciarComWin = true;
                Properties.Settings.Default.Save();
                CarregarEstadoCheckbox();
                IniciarComWindows(true);
            }
            else
            {
                Properties.Settings.Default.chbIniciarComWin = false;
                Properties.Settings.Default.Save();
                CarregarEstadoCheckbox();
                IniciarComWindows(false);
            }
        }
        #endregion

        #region Configuração Click Mouse ou Teclado
        private void CarregarEstadoCheckboxClick()
        {
            chbclicarTeclado.Checked = Properties.Settings.Default.chbTipoClick;
            chbclicarTeclado.Text = chbclicarTeclado.Checked ? "Clicar com Teclado" : "Clicar com Mouse";
        }

        private void TipoDeClick(object sender, EventArgs e)
        {
            if (chbclicarTeclado.Checked)
            {
                Properties.Settings.Default.chbTipoClick = true;
                Properties.Settings.Default.Save();
                CarregarEstadoCheckboxClick();
            }
            else
            {
                Properties.Settings.Default.chbTipoClick = false;
                Properties.Settings.Default.Save();
                CarregarEstadoCheckboxClick();
            }
        }
        #endregion

        #region Configuração Label
        private void AlterarTextMinuto()
        {
            if (cbProximaExecucao.Text.Equals("01"))
                lblMinuto.Text = "Minuto";
            else
                lblMinuto.Text = "Minutos";
        }
        #endregion

        #region Configuração ComboBox
        private void DefinirProximaExecucao()
        {         
            if (cbProximaExecucao.Text.Equals("01"))
            {
                MainInfo.FCInfo().Timer = 60000;
                Properties.Settings.Default.cbExecutar = cbProximaExecucao.Text;
            }
            else if (cbProximaExecucao.Text.Equals("03"))
            {
                MainInfo.FCInfo().Timer = 180000;
                Properties.Settings.Default.cbExecutar = cbProximaExecucao.Text;
            }
            else
            {
                MainInfo.FCInfo().Timer = 300000;
                Properties.Settings.Default.cbExecutar = cbProximaExecucao.Text;
            }         
            Properties.Settings.Default.Save();
        }

        private void CarregaComboBox()
        {
            cbProximaExecucao.Text = string.IsNullOrEmpty(Properties.Settings.Default.cbExecutar) ? "01" : Properties.Settings.Default.cbExecutar;          
        }

        private void SelecionaItemComboBox(object sender, EventArgs e)
        {
            AlterarTextMinuto();
            DefinirProximaExecucao();
        }
        #endregion

    }
}
