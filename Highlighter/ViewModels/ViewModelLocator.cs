using Microsoft.Extensions.DependencyInjection;

namespace Highlighter.ViewModels {
    public class ViewModelLocator {
        public MainViewModel MainViewModel => App.ServiceProvider.GetRequiredService<MainViewModel>();
    }
}
