using client.Application.Interfaces;
using System.Collections.ObjectModel;

namespace client.Presentation.ViewModels;

public class ProjectViewModel
{
    public ObservableCollection<IProject> ListOfProjects { get; } 
    public ProjectViewModel() 
    { 
        ListOfProjects = new ObservableCollection<IProject>();
    }
}
