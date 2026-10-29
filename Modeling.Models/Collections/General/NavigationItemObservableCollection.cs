using Modeling.Core.Abstractions.Collections;
using Modeling.Core.Collections;
using Modeling.Models.Abstractions.Collections.General;
using Modeling.Models.Navigation;

namespace Modeling.Models.Collections.General
{
    sealed class NavigationItemObservableCollection : BlockingCollection<NavigationItem, ObservableCollectionParametrizedWrapper<NavigationItem>>, INavigationItemsObservableCollection
    { }
}
