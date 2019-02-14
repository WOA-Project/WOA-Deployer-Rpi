using System;
using System.Collections.Generic;
using System.Windows;

namespace Deployer.Gui.Common.Services
{
    public class ViewService : IViewService
    {
        private readonly IDictionary<string, Type> viewDic = new Dictionary<string, Type>();

        public void Register(string token, Type viewType)
        {
            viewDic.Add(token, viewType);
        }

        public void Show(string key, object viewModel)
        {
            if (viewDic.TryGetValue(key, out var viewType))
            {
                var view =  (Window)Activator.CreateInstance(viewType);
                view.DataContext = viewModel;
                view.Owner = Application.Current.MainWindow;
                view.Show();
            }
        }
    }
}