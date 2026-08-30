using Jay;
using System;
using System.Collections.Generic;
using System.Xml;


    //[Facade Pattern] 데이터 표들이 들어가는 단일 창구
    // 데이터 종류별 테이블(Enemy, Wave, Unit)을 '보유'하고
    // 로딩 순서를 조율하는 역할
    // 실제 로드/ 해석/ 보관/ 조회는 각각의 테이블이 책임집니다.

    // 새 데이터 종류를 추가하고 싶으면 
    // 1. Table클래스를 만들고
    // 2. DataManager에 추가하면 되는 구조입니다.
    public class DataManager 
    {
        
        private static DataManager instance { get; } = new DataManager();

        private static readonly SemaphoreSlim SendLock 
            = new SemaphoreSlim(1, 1);
        public static DataManager Instance
        {
            get
            {
                SendLock.Wait();
                try
                {
                    return instance;
                }
                finally
                {
                    SendLock.Release();
                }
                

            }
        }

        public GenreTable Genres { get; } = new GenreTable();
        public KeyWordTable Keywords { get; } = new KeyWordTable();
        public ItemCategoryTable ItemCategories { get; } = new ItemCategoryTable();
        public ItemTable Items { get; } = new ItemTable();

        public bool IsReady { get; private set; } = false;
        public event Action OnReady;
        
        public void Load()
        {
            Genres.Load();
            Keywords.Load();
            ItemCategories.Load();
            Items.Load();
            IsReady = true;
            OnReady?.Invoke();
            Console.WriteLine($"[DataManager] : Loaded {Genres.Count} Genres, {Keywords.Count} Keywords, {ItemCategories.Count} ItemCategories, {Items.Count} Items");
        }

        public GenreDef GetGenreDef(int id) => Genres.Get(id);
        public KeyWordDef GetKeyWordDef(int id) => Keywords.Get(id);
        public ItemCategoryDef GetItemCategoryDef(int id) => ItemCategories.Get(id);
        public ItemDef GetItemDef(int id) => Items.Get(id);

        public List<ItemDef> GetAllItemDefs() => Items.GetListAll();
        public List<ItemDef> GetItemDefsByCategory(CategoryType categoryType)
        {
            List<ItemDef> itemsInCategory = new List<ItemDef>();
            foreach (var item in Items.GetListAll())
            {
                if (item.CategoryType == categoryType)
                {
                    itemsInCategory.Add(item);
                }
            }
            return itemsInCategory;
        }

        public List<KeyWordDef> GetKeyWordDefsByGenre(string genre)
        {
            List<KeyWordDef> keyWordDefs = new List<KeyWordDef>();
            foreach (var keyWordDef in Keywords.GetAllList())
            {
                if (keyWordDef.GenreName == genre)
                {
                    keyWordDefs.Add(keyWordDef);
                }
            }
            return keyWordDefs;
        }
    }
