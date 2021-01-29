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
        /// <summary>
        /// Загружаемый плагин
        /// </summary>
        public IDataSource plugin { get; set;}
        LoaderPlugins loaderPlugins;
        /// <summary>
        /// Инизиализирует компоненты формы <c>FormLoadPlugin</c> и отображает список  плагинов.
        /// </summary>
        /// <param name="plgn">Объект типа <c>IDataSource</c>.</param>
        public FormLoadPlugin(IDataSource plgn)
        {
            this.plugin = plgn;
            InitializeComponent();
            loaderPlugins = new LoaderPlugins();
            loaderPlugins.OnError += new EventHandler<EventArgsString>(catchError);
            radioListBoxPlugins.DataSource = loaderPlugins.GetInstances<IDataSource>();
            radioListBoxPlugins.DisplayMember = "NamePlugin";
            SetSelectedItem();
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
            plugin = loaderPlugins.LoadPlugin(((IDataSource)radioListBoxPlugins.SelectedItem).NamePlugin,  (IDataSource)radioListBoxPlugins.SelectedItem);
            plugin.OnError += new EventHandler<EventArgsString>(catchError);
            WriteToConfig();
            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void WriteToConfig()
        {
            System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings["namePlugin"].Value = plugin.NamePlugin;
            config.Save();
            ConfigurationManager.RefreshSection("appSettings");
        }
            
        private void UpdateInformation()
        {
            if (radioListBoxPlugins.SelectedItems.Count == 1)
            {
                btnLoad.Enabled = true;
                labelDescription.Text = ((IDataSource)radioListBoxPlugins.Items[radioListBoxPlugins.SelectedIndex]).DescriptionPlugin;
                if (CheckLoadedPlugins())
                {
                    labelStatus.Text = "(Загружен)";
                    btnLoad.Enabled = false;
                }
                else
                    labelStatus.Text = "(Не загружен)";
            }
        }

        private bool CheckLoadedPlugins()
        {
            if (plugin != null)
            {
                if (((IDataSource)radioListBoxPlugins.SelectedItem).NamePlugin == ConfigurationManager.AppSettings["namePlugin"])
                    return true;           
            }
            return false;
        }

        private void SetSelectedItem()
        {
            if (plugin != null)
            {
                for (int i = 0; i < radioListBoxPlugins.Items.Count; i++)
                {
                    if (((IDataSource)radioListBoxPlugins.Items[i]).NamePlugin == ConfigurationManager.AppSettings["namePlugin"])
                    {
                        radioListBoxPlugins.SetSelected(i, true);
                        radioListBoxPlugins.SelectedItem = radioListBoxPlugins.Items[i];
                        UpdateInformation();
                        break;
                    }
                }
            }
            else
            {
                ClearInformation();
                btnLoad.Enabled = false;
                radioListBoxPlugins.SetSelected(0, false);
            }
        }

        private void ClearInformation()
        {
            labelDescription.Text = string.Empty;
            labelStatus.Text = string.Empty;
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

    }   
}
