using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using DataSource;
using LibraryCommon;
using LibraryCore;
using System.Windows.Forms;

namespace Library
{
    /// <summary>
    /// Загрузчик плагинов.
    /// </summary>
    public class LoaderPlugins
    {
        EventHandler<EventArgsString> _onError;
        /// <summary>
        /// Возвращает список плагинов.
        /// </summary>
        /// <typeparam name="T">Обобщенный тип.</typeparam>
        /// <returns>Список элементов обобщенного типа.</returns>
        public List<T> GetInstances<T>()
        {
            string dir = System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
            DirectoryInfo info = new DirectoryInfo(dir);
            if (!info.Exists)
                return null;
            var list = new List<T>();
            try
            {
                foreach (FileInfo file in info.GetFiles("Plugin" + "*.dll"))
                {
                    Assembly currentAssembly = null;
                    try
                    {
                        var name = AssemblyName.GetAssemblyName(file.FullName);
                        currentAssembly = Assembly.Load(name);
                    }
                    catch {continue;}
                    currentAssembly.GetTypes()
                        .Where(t => t != typeof(T) && typeof(T).IsAssignableFrom(t))
                        .ToList()
                        .ForEach(x => list.Add((T)Activator.CreateInstance(x)));
                }
            }
            catch (Exception ex)
            {
                _onError?.Invoke(this, new EventArgsString("При загрузке плагинов с интерфейсом: " + typeof(T).Name + " вызвано исключение:" + "\n" + ex.Message));
            }
            return list;
        }

        /// <summary>
        /// Загрузка плагина.
        /// </summary>
        /// <param name="namePlugin">Строка с названием плагина.</param>
        /// <param name="plugin">Объект типа <c>IDataSource</c>.</param>
        /// <returns></returns>
        public IDataSource LoadPlugin(string namePlugin, IDataSource plugin)
        {
            string path = System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
            foreach (string file in System.IO.Directory.GetFiles(path, "*.dll"))
            {
                if (!file.Contains($"{namePlugin}"))
                    continue;
                System.Reflection.Assembly a = System.Reflection.Assembly.LoadFile(file);
                try
                {
                    foreach (Type t in a.GetTypes())
                    {
                        foreach (Type i in t.GetInterfaces())
                        {
                            if (i.FullName == "IDataSource")
                            {
                                IDataSource p = (IDataSource)Activator.CreateInstance(t);

                                if (p.NamePlugin == namePlugin)
                                {
                                    plugin = p;
                                }
                            }

                        }
                    }
                }
                catch (Exception ex)
                {
                    _onError?.Invoke(this, new EventArgsString("При загрузке плагина вызвано исключение:" + "\n" + ex.Message));
                    return plugin;
                }
            }
            return plugin;
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
