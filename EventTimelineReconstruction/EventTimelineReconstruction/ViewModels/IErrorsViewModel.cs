using System.Collections.ObjectModel;
using System.ComponentModel;

namespace EventTimelineReconstruction.ViewModels;

public interface IErrorsViewModel : INotifyDataErrorInfo
{
    ObservableCollection<string> Errors { get; }

    void AddError(string propertyName, string errorMessage);
    void ClearErrors(string propertyName);
    void UpdateErrorsLanguage(string oldLanguage);
}