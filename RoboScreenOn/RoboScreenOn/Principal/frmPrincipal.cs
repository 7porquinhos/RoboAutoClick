using MetroFramework;
using MetroFramework.Forms;
using RoboScreenOn.BLL.Enum;
using RoboScreenOn.BLL.Utils.Auxiliar;
using RoboScreenOn.BLL.Utils.ExtensionMethods;
using RoboScreenOn.BLL.Utils.Singleton;
using RoboScreenOn.Formularios.Configuracoes;
using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using static RoboScreenOn.BLL.Utils.Singleton.MainInfo;

namespace RoboScreenOn
{
    public partial class frmPrincipal : MetroForm
    {
        #region Imports
        [DllImport("User32.dll")]
        private static extern short GetAsyncKeystate(Keys vKey);

        [DllImport("User32.dll")]
        internal static extern IntPtr SetForegroundWindow(IntPtr hWnd);
        #endregion

        #region Delegates
        private delegate void Processar();
        #endregion

        #region Variaveis
        public static ListBoxLog listBoxLog;
        TimeSpan dtInicio;
        TimeSpan dtFim;
        TimeSpan horaAtual;
        #endregion

        #region Construtores
        public frmPrincipal()
        {
            InitializeComponent();
        }
        #endregion

        #region Configurações FrmPrincipal
        private void frmPrincipal_Load(object sender, EventArgs e)
        {
            InicializaDelegates();
            InicializaConsoleLog();
            InicializaBtnParar();
            CarregarMainInfo();
            CarregaDataPausa();
            KeyLogger.Listen();
        }

        private void frmPrincipal_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                this.Hide();

                notifyIcon1.Visible = true;
            }
        }
        #endregion

        #region Start & Stop
        private void StartRobo(object sender, EventArgs e)
        {
            var task = Task.Factory.StartNew(() =>
            {
                Invoke(new Processar(() =>
                {
                    if (TimeSpan.Parse(dateTimePicker1.Text) > TimeSpan.Parse(dateTimePicker2.Text))
                    {
                        MetroMessageBox.Show(this, "Inicio de stop não pode ser maior que o Fim!");
                    }
                    else
                    {
                        timer1.Interval = 1000;
                        timer1.Enabled = true;
                        btnParar.Enabled = true;
                        btnIniciar.Enabled = false;
                        MainInfo.FCInfo().EscreverConsoleLog("Iniciar...", TipoConsoleLog.Info);
                    }

                }));

            });
        }

        private void StopRobo(object sender, EventArgs e)
        {
            var task = Task.Factory.StartNew(() =>
            {
                Invoke(new Processar(() =>
                {
                    timer1.Enabled = false;
                    btnParar.Enabled = false;
                    btnIniciar.Enabled = true;
                    MainInfo.FCInfo().EscreverConsoleLog("Parar.", TipoConsoleLog.Info);
                }));

            });

        }
        #endregion

        #region Configuração TimerTick
        private void IniciarTimer(object sender, EventArgs e)
        {

            try
            {
                string hora = DateTime.Now.ToString("HH:mm:ss");
                horaAtual = TimeSpan.Parse(hora);
                if (horaAtual < dtInicio || horaAtual > dtFim)
                    LoopTimer();
                else
                    MainInfo.FCInfo().EscreverConsoleLog($"Sistema pausado, retorno as {dtFim}", TipoConsoleLog.Info);
            }
            catch (Exception ex)
            {
                EscreverConsoleLog(ex.Message, TipoConsoleLog.Erro, false, MethodBase.GetCurrentMethod().Name, ex);
            }
        }

        private void LoopTimer()
        {

            try
            {
                if (MainInfo.FCInfo().MousePosition == Cursor.Position && MainInfo.FCInfo().KeyPressPass == MainInfo.FCInfo().KeyPress)
                {
                    if (Properties.Settings.Default.chbTipoClick)
                        SendKeys.SendWait("^");
                    else
                        AutMouseClick.MouseClick(Cursor.Position.X, Cursor.Position.Y);

                    timer1.Interval = MainInfo.FCInfo().Timer == 0 ? 60000 : MainInfo.FCInfo().Timer;
                    lblTimeProximaExecucao.Text = $"{DateTime.Now.CalcularProximaExecucao(string.IsNullOrEmpty(Properties.Settings.Default.cbExecutar) ? 01 : Convert.ToInt32(Properties.Settings.Default.cbExecutar))}";
                    MainInfo.FCInfo().EscreverConsoleLog("Executado com Sucesso!", TipoConsoleLog.Info);
                }
                else
                {
                    timer1.Interval = MainInfo.FCInfo().Timer == 0 ? 60000 : MainInfo.FCInfo().Timer;
                    lblTimeProximaExecucao.Text = $"{DateTime.Now.CalcularProximaExecucao(string.IsNullOrEmpty(Properties.Settings.Default.cbExecutar) ? 01 : Convert.ToInt32(Properties.Settings.Default.cbExecutar))}";
                    MainInfo.FCInfo().EscreverConsoleLog("Execução não necessaria!", TipoConsoleLog.Info);
                }
                MainInfo.FCInfo().MousePosition = Cursor.Position;
                MainInfo.FCInfo().KeyPressPass = MainInfo.FCInfo().KeyPress;
            }
            catch (Exception ex)
            {

                EscreverConsoleLog(ex.Message, TipoConsoleLog.Erro, false, MethodBase.GetCurrentMethod().Name, ex);
            }

        }
        #endregion

        #region Configuração Delegates
        private void InicializaDelegates()
        {
            btnIniciar.Click += StartRobo;
            btnParar.Click += StopRobo;
            timer1.Tick += IniciarTimer;
            notifyIcon1.MouseDoubleClick += ExibirNotifyIcon;
            btnConfigurarSistema.Click += ConfigurarSistema;
            dateTimePicker1.ValueChanged += ValidaTempoInicio;
            dateTimePicker2.ValueChanged += ValidaTempoFim;
        }
        #endregion

        #region Configurações do Sistema
        private void ConfigurarSistema(object sender, EventArgs e)
        {
            frmConfiguracoes frmConfiguracoes = new frmConfiguracoes();
            frmConfiguracoes.ShowDialog();
        }
        #endregion

        #region Configuração Console Log
        private void InicializaConsoleLog()
        {
            listBoxLog = new ListBoxLog(lbLog);
            ParametrizarDelegates();
            FCInfo().EscreverConsoleLog("Carregando...", TipoConsoleLog.Info);
            FCInfo().EscreverConsoleLog("CONSOLE LOG INICIADO", TipoConsoleLog.Info);
        }

        private void ParametrizarDelegates()
        {
            FCInfo().EscreverConsoleLog = new MainInfo.DelegateEscreverConsoleLog(this.EscreverConsoleLog);
        }

        private void EscreverConsoleLog(string mensagem, TipoConsoleLog tipoConsoleLog, bool gravarLog = false, string nomeProcesso = "", Exception exception = null)
        {
            listBoxLog.Log(tipoConsoleLog, mensagem);
        }
        #endregion

        #region Configuração Variavel MainInfo
        private void CarregarMainInfo()
        {
            if (Properties.Settings.Default.cbExecutar.Equals("01"))
                MainInfo.FCInfo().Timer = 60000;
            else if (Properties.Settings.Default.cbExecutar.Equals("03"))
                MainInfo.FCInfo().Timer = 180000;
            else if (Properties.Settings.Default.cbExecutar.Equals("05"))
                MainInfo.FCInfo().Timer = 300000;
        }
        #endregion

        #region Configuração Botão
        private void InicializaBtnParar()
        {
            btnParar.Enabled = false;
        }
        #endregion

        #region Configuração NotifyIcon
        private void ExibirNotifyIcon(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;

            this.notifyIcon1.Visible = false;
        }


        #endregion

        #region Configuração Valida TempoStop
        private void ConfiguraDateTimePicker()
        {
            dateTimePicker1.MinDate = DateTime.Parse(DateTime.Now.ToString("dd/MM/yyyy 00:00:00"));
            dateTimePicker2.MinDate = DateTime.Parse(DateTime.Now.ToString("dd/MM/yyyy 00:00:00"));
        }

        private void CarregaDataPausa()
        {
            ConfiguraDateTimePicker();
            string horaInicio = Properties.Settings.Default.dateInicio;
            string horaFim = Properties.Settings.Default.dateFim;
            dateTimePicker1.Text = string.IsNullOrEmpty(horaInicio) ? "00:00:00" : horaInicio;
            dateTimePicker2.Text = string.IsNullOrEmpty(horaFim) ? "00:00:00" : horaFim;
        }

        private void ValidaTempoInicio(object sender, EventArgs e)
        {
            dtInicio = TimeSpan.Parse(dateTimePicker1.Text);
            Properties.Settings.Default.dateInicio = dateTimePicker1.Text;
            Properties.Settings.Default.Save();
        }
        private void ValidaTempoFim(object sender, EventArgs e)
        {
            dtFim = TimeSpan.Parse(dateTimePicker2.Text);
            Properties.Settings.Default.dateFim = dateTimePicker2.Text;
            Properties.Settings.Default.Save();
        }
        #endregion
    }
}
