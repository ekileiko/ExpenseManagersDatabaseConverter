using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using FirebirdDataModel;

namespace ExpManagerToOwnMoney.Helpers
{
    public static class DBConformity
    {
        /// <summary>
        /// Соответсвие счетов. 1 - ExpManager, 2 - OwnMoney
        /// </summary>
        public static Dictionary<int, int> AccountConformityEMtoOM 
        { 
            get
            {
                var result = new Dictionary<int, int>();
                if (MemoryCache.Default["AccountConformityEMtoOM"] == null)
                {
                    using (var db = new BudgetDB())
                    {
                        var dbAccounts = db.MONEY_ACCOUNT.ToList();
                        foreach (var account in ExpManagerWorker.ExpManagerXmlModel.Dictionaries.Accounts)
                        {
                            try
                            {
                                // Сопоставление по названию
                                var item = dbAccounts.FirstOrDefault(c => c.NAME == account.Name);
                                if(item != null)
                                    result.Add(account.Id, item.ID);
                            }
                            catch (Exception)
                            {
                                // Ненайденные пропускаем
                            }
                        }
                    }
                    MemoryCache.Default["AccountConformityEMtoOM"] = result;
                }
                else
                {
                    result = MemoryCache.Default["AccountConformityEMtoOM"] as Dictionary<int, int>;
                }
                return result;
            } 
        }

        /// <summary>
        /// Соответсвие категорий. 1 - ExpManager, 2 - OwnMoney
        /// </summary>
        public static Dictionary<int, int> CategoryConformityEMtoOM 
        {
            get
            {
                var result = new Dictionary<int, int>();
                if (MemoryCache.Default["CategoryConformityEMtoOM"] == null)
                {
                    using (var db = new BudgetDB())
                    {
                        var dbCategories = db.EXPENCE_ITEM.ToList();
                        foreach (var category in ExpManagerWorker.ExpManagerXmlModel.Dictionaries.Categories)
                        {
                            try
                            {
                                // Сопоставление по названию
                                var item = dbCategories.FirstOrDefault(c => c.NAME == category.Name);
                                if (item != null)
                                    result.Add(category.Id, item.ID);
                            }
                            catch (Exception)
                            {
                                // Ненайденные пропускаем
                            }
                        }
                    }
                    MemoryCache.Default["CategoryConformityEMtoOM"] = result;
                }
                else
                {
                    result = MemoryCache.Default["CategoryConformityEMtoOM"] as Dictionary<int, int>;
                }
                return result;
            }
        }
        /// <summary>
        /// Соответсвие типов операций. 1 - ExpManager, 2 - OwnMoney
        /// </summary>
        public static Dictionary<int, int> OperationTypeConformityEMtoOM = new Dictionary<int, int>()
        {
            { 0, 3 }, // Расход
            { 1, 2 }, // Доход
        };
        /// <summary>
        /// Соответсвие валют операций. 1 - ExpManager, 2 - OwnMoney
        /// </summary>
        public static Dictionary<int, int> CurrencyConformityEMtoOM = new Dictionary<int, int>()
        {
            { 1, 1 },    // Доллар
            { 2, 2 },    // Евро
            { 8, 168 },  // Польский злотый
            { 62, 151 }, // Белорусский рубль
            { 0, 3 }, // Российский рубль
        };
    }
}
