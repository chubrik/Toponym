using System.Collections.Generic;
using System.Linq;
using Toponym.Core.Models;
using Toponym.Site.Models;

namespace Toponym.Site.Services {
    public class ItemService {

        private readonly DataService _dataService;
        private IReadOnlyList<Item> _items;

        public ItemService(DataService dataService) {
            _dataService = dataService;
        }

        public IReadOnlyList<Item> GetItems() => _items ?? (_items = LoadItems());

        private IReadOnlyList<Item> LoadItems() {
            var data = _dataService.ReadData<List<ItemStorageData>>(CoreConstants.DataFile);
            return data.Select(i => new Item(i)).ToList();
        }
    }
}
