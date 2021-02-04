using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DataSource;
using LibraryCommon;
using LibraryCore;

namespace Library
{
    /// <summary>
    /// Класс <c>FormLoadPlugin</c> окна загрузки плагинов.
    /// </summary>
    public partial class FormLoadPlugin : Form
    {
        private IDataSource _plugin;
        /// <summary>
        /// Загружаемый плагин
        /// </summary>
        public IDataSource Plugin
        {
            get
            {
                return _plugin;
            }
            private set
            {
                _plugin = value;
            }
        }
        LoaderPlugins _loaderPlugins;
        EventHandler<EventArgsString> _onError;
        /// <summary>
        /// Инизиализирует компоненты формы <c>FormLoadPlugin</c> и отображает список  плагинов.
        /// </summary>
        /// <param name="plgn">Объект типа <c>IDataSource</c>.</param>
        public FormLoadPlugin(IDataSource plgn)
        {
            _plugin = plgn;
            InitializeComponent();
            _loaderPlugins = new LoaderPlugins();
            _loaderPlugins.OnError += new EventHandler<EventArgsString>(catchError);
            radioListBoxPlugins.DataSource = _loaderPlugins.GetInstances<IDataSource>();
            radioListBoxPlugins.DisplayMember = "NamePlugin";
            SetSelectedPlugin();
            if (radioListBoxPlugins.Items.Count == 0)
            {
                btnLoad.Enabled = false;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void radioListBoxPlugins_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateInformation();
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            LoadSelectedPlugin();
        }

        private void LoadSelectedPlugin()
        {
            _plugin = _loaderPlugins.LoadPlugin(((IDataSource)radioListBoxPlugins.SelectedItem).NamePlugin,  (IDataSource)radioListBoxPlugins.SelectedItem);
            _plugin.OnError += new EventHandler<EventArgsString>(catchError);
            WriteToConfig();
            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void WriteToConfig()
        {
            try
            {
                System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                config.AppSettings.Settings["pluginSelected"].Value = _plugin.NamePlugin;
                config.Save();
                ConfigurationManager.RefreshSection("appSettings");
            }
            catch (Exception ex)
            {
                _onError?.Invoke(this, new EventArgsString("Невозможно сохранить информацию о плагине в конфигурационный файл - прерывание по исключению:" + "\n" + ex.Message));
            }
        }
            
        private void UpdateInformation()
        {
            if (radioListBoxPlugins.SelectedItems.Count == 1)
            {
                btnLoad.Enabled = true;
                labelDescription.Text = ((IDataSource)radioListBoxPlugins.Items[radioListBoxPlugins.SelectedIndex]).DescriptionPlugin;
                if (CheckLoadedPlugins())
                {
                    btnLoad.Enabled = false;
                }
            }
        }

        private bool CheckLoadedPlugins()
        {
            if (_plugin != null)
            {
                if (((IDataSource)radioListBoxPlugins.SelectedItem).NamePlugin == ConfigurationManager.AppSettings["pluginSelected"])
                    return true;           
            }
            return false;
        }

        private void SetSelectedPlugin()
        {
            if (_plugin != null)
            {
                SetRadioButtonCheck();
            }
            else if(ConfigurationManager.AppSettings["pluginSelected"] != " ")
            {
                SetRadioButtonCheck();
                LoadSelectedPlugin();
            }
            else
            {
                ClearInformation();
                btnLoad.Enabled = false;
                radioListBoxPlugins.ClearSelected();
            }
        }

        private void SetRadioButtonCheck()
        {
            for (int i = 0; i < radioListBoxPlugins.Items.Count; i++)
            {
                if (((IDataSource)radioListBoxPlugins.Items[i]).NamePlugin == ConfigurationManager.AppSettings["pluginSelected"])
                {
                    radioListBoxPlugins.SetSelected(i, true);
                    radioListBoxPlugins.SelectedItem = radioListBoxPlugins.Items[i];
                    UpdateInformation();
                    break;
                }
            }
        }

        private void ClearInformation()
        {
            labelDescription.Text = string.Empty;
        }
        /// <summary>
        /// Выводит на экран сообщение с информацией об исключении.
        /// </summary>
        /// <param name="sender">Объект типа <c>object</c>.</param>
        /// <param name="e">Объект типа <c>EventArgsString</c>.</param>
        private void catchError(object sender, EventArgsString e)
        {
            MessageBox.Show(e.Message);
        }
        /// <summary>
        /// Событие - ошибка с передачей строки.
        /// </summary>
        public event EventHandler<EventArgsString> OnError
        {
            add { _onError += value; }
            remove { _onError -= value; }
        }
    }
}   

