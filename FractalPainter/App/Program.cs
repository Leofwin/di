using System;
using System.Windows.Forms;
using FractalPainting.App.Actions;
using FractalPainting.Infrastructure.UiActions;
using Ninject;

namespace FractalPainting.App
{
    internal static class Program
    {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
			var container = new StandardKernel();
	        container.Bind<IUiAction>().To<SaveImageAction>();
	        container.Bind<IUiAction>().To<DragonFractalAction>();
	        container.Bind<IUiAction>().To<KochFractalAction>();
	        container.Bind<IUiAction>().To<ImageSettingsAction>();
	        container.Bind<IUiAction>().To<PaletteSettingsAction>();
			try
			{
				var form = container.Get<MainForm>();
				Application.EnableVisualStyles();
				Application.Run(form);
				Application.SetCompatibleTextRenderingDefault(false);
	            
			}
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
    }
}