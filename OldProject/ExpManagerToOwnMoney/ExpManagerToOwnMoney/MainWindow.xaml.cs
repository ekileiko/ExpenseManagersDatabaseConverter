using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Windows;
using ExpManagerToOwnMoney.Helpers;
using ExpManagerToOwnMoney.Models;
using FirebirdDataModel;
using LinqToDB;

namespace ExpManagerToOwnMoney
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// 
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Окно загружено
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshGrids();
        }
        /// <summary>
        /// Обновляет гриды
        /// </summary>
        private void RefreshGrids()
        {
            var path = ConfigurationManager.AppSettings["ExpManagerXMLPath"];
            if (File.Exists(path))
            {
                var xml = File.ReadAllText(path);
                try
                {
                    ExpManagerWorker.ExpManagerXmlModel = SimpleSerializer.DeserializeXmlToObject<ExpManagerXmlModel>(xml);
                }
                catch (Exception)
                {
                    ExpManagerWorker.ExpManagerXmlModel = new ExpManagerXmlModel();
                }
            }
            // ListView операций из ExpManager
            var lvEMOperations = new List<LvOperation>();
            foreach (var operation in ExpManagerWorker.ExpManagerXmlModel.Operations.OrderByDescending(c => c.Date))
            {
                var lvOperation = new LvOperation
                {
                    Id = operation.Id,
                    Date = operation.Date,
                    Type = operation.Type.ToString(),
                    Summa = operation.SumPerUnit.ToString(),
                    Comment = operation.Comment
                };
                switch (operation.Type)
                {
                    case ExpManagerXmlModel.OperationTypeEnum.Расход:
                        lvOperation.Account = ExpManagerWorker.ExpManagerXmlModel.Dictionaries.Accounts.First(c => c.Id == operation.SourceId).Name;
                        lvOperation.Category = ExpManagerWorker.ExpManagerXmlModel.Dictionaries.Categories.First(c => c.Id == operation.DestinationId).Name;
                        break;
                    case ExpManagerXmlModel.OperationTypeEnum.Доход:
                        lvOperation.Account = ExpManagerWorker.ExpManagerXmlModel.Dictionaries.Accounts.First(c => c.Id == operation.DestinationId).Name;
                        lvOperation.Category = ExpManagerWorker.ExpManagerXmlModel.Dictionaries.Categories.First(c => c.Id == operation.SourceId).Name;
                        break;
                    case ExpManagerXmlModel.OperationTypeEnum.Перевод:
                        lvOperation.Account = ExpManagerWorker.ExpManagerXmlModel.Dictionaries.Accounts.First(c => c.Id == operation.SourceId).Name
                            + " -> "
                            + ExpManagerWorker.ExpManagerXmlModel.Dictionaries.Accounts.First(c => c.Id == operation.DestinationId).Name;
                        if (Math.Abs(operation.CurrencySum - -1) > 0)
                            lvOperation.Summa = operation.SumPerUnit.ToString() + " -> " + operation.CurrencySum.ToString();
                        else
                            lvOperation.Summa = operation.SumPerUnit.ToString() + " -> " + operation.SumPerUnit.ToString();
                        break;
                }
                lvOperation.IsSelected = true;
                lvEMOperations.Add(lvOperation);
            }
            LvEMOperation.ItemsSource = lvEMOperations.OrderByDescending(c => c.Date).ThenBy(c => c.Id);

            // ListView операций из OwnMoney
            var lvOMOperations = new List<LvOperation>();
            using (var db = new BudgetDB())
            {
                var operationTypes = db.OPERATION_TYPE.ToList();
                var moneyAccounts = db.MONEY_ACCOUNT.ToList();
                var expenceItems = db.EXPENCE_ITEM.ToList();
                foreach (var operation in db.MONEY_OPERATION.Where(c => c.MONEY_TRANSFER_ID == null).ToList())
                {
                    var lvOperation = new LvOperation
                    {
                        Id = operation.ID,
                        Date = operation.OPERATION_DATE,
                        Summa = operation.MONEY_VALUE.HasValue ? operation.MONEY_VALUE.ToString() : "0",
                        Comment = operation.DESCRIPTION,
                    };
                    if (operation.OPERATION_TYPE_ID != null)
                        lvOperation.Type = operationTypes.First(c => c.ID == operation.OPERATION_TYPE_ID).NAME;
                    if (operation.MONEY_ACCOUNT_ID != null)
                        lvOperation.Account = moneyAccounts.First(c => c.ID == operation.MONEY_ACCOUNT_ID).NAME;
                    if (operation.EXPENCE_ITEM_ID != null)
                        lvOperation.Category = expenceItems.First(c => c.ID == operation.EXPENCE_ITEM_ID).NAME;

                    lvOMOperations.Add(lvOperation);
                }
                // Добавляем сгруппированные переводы
                foreach (var group in db.MONEY_OPERATION.Where(c => c.MONEY_TRANSFER_ID != null).ToList().GroupBy(c => c.MONEY_TRANSFER_ID))
                {
                    var operation1 = group.ElementAt(0);
                    var operation2 = group.ElementAt(1);
                    var lvOperation = new LvOperation
                    {
                        Id = operation1.ID,
                        Date = operation1.OPERATION_DATE,
                        Summa = (double)operation1.MONEY_VALUE + " -> " + (double)operation2.MONEY_VALUE,
                        Comment = operation1.DESCRIPTION,
                    };
                    if (operation1.OPERATION_TYPE_ID != null)
                        lvOperation.Type = operationTypes.First(c => c.ID == operation1.OPERATION_TYPE_ID).NAME;
                    if (operation1.MONEY_ACCOUNT_ID != null && operation2.MONEY_ACCOUNT_ID != null)
                    {
                        lvOperation.Account = moneyAccounts.First(c => c.ID == operation1.MONEY_ACCOUNT_ID).NAME;
                        lvOperation.Account += " -> ";
                        lvOperation.Account += moneyAccounts.First(c => c.ID == operation2.MONEY_ACCOUNT_ID).NAME;
                    }
                    if (operation1.EXPENCE_ITEM_ID != null)
                        lvOperation.Category = expenceItems.First(c => c.ID == operation1.EXPENCE_ITEM_ID).NAME;

                    lvOMOperations.Add(lvOperation);
                }
            }
            LvOMOperation.ItemsSource = lvOMOperations.OrderByDescending(c => c.Date);

            LvEMOperation.SelectedItems.Clear();
            int dayCount;
            Int32.TryParse(TbxDayCount.Text, out dayCount);
            foreach (var item in lvEMOperations.Where(item => item.Date.HasValue && item.Date.Value.AddDays(dayCount == 0 ? 10 : dayCount) > DateTime.Now))
            {
                if (!lvOMOperations.Any(c => 
                        c.Account == item.Account 
                    && c.Category == item.Category 
                    && c.Comment == item.Comment 
                    && c.Date == item.Date 
                    && c.Summa == item.Summa 
                    /*&& c.Type == item.Type*/
                    ))
                {
                    LvEMOperation.SelectedItems.Add(item);
                }
            }
        }
        /// <summary>
        /// Перенос записей из ExpManager в OwnMoney
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtMove_Click(object sender, RoutedEventArgs e)
        {
            var errorRecords = new List<int>();
            using (var db = new BudgetDB())
            {
                foreach (LvOperation selectedItem in LvEMOperation.SelectedItems)
                {
                    var operation = ExpManagerWorker.ExpManagerXmlModel.Operations.First(c => c.Id == selectedItem.Id);
                    // Переводы пишутся в другую таблицу
                    if (operation.Type != ExpManagerXmlModel.OperationTypeEnum.Перевод)
                    {
                        var moneyAccountId = 0;
                        var categoryId = 0;
                        var currencyId = 0;
                        var typeId = DBConformity.OperationTypeConformityEMtoOM[(int)operation.Type];
                        try
                        {
                            switch (operation.Type)
                            {
                                case ExpManagerXmlModel.OperationTypeEnum.Расход:
                                    {
                                        moneyAccountId = DBConformity.AccountConformityEMtoOM[operation.SourceId];
                                        categoryId = DBConformity.CategoryConformityEMtoOM[operation.DestinationId];
                                        currencyId = DBConformity.CurrencyConformityEMtoOM[ExpManagerWorker.ExpManagerXmlModel.Dictionaries.Accounts.First(c => c.Id == operation.SourceId).CurrencyId];
                                        break;
                                    }
                                case ExpManagerXmlModel.OperationTypeEnum.Доход:
                                    {
                                        moneyAccountId = DBConformity.AccountConformityEMtoOM[operation.DestinationId];
                                        categoryId = DBConformity.CategoryConformityEMtoOM[operation.SourceId];
                                        currencyId = DBConformity.CurrencyConformityEMtoOM[ExpManagerWorker.ExpManagerXmlModel.Dictionaries.Accounts.First(c => c.Id == operation.DestinationId).CurrencyId];
                                        break;
                                    }
                            }
                            
                            var moneyOperation = db.MONEY_OPERATION
                                .Value(c => c.DESCRIPTION, operation.Comment)
                                .Value(c => c.MONEY_ACCOUNT_ID, moneyAccountId)
                                .Value(c => c.OPERATION_DATE, operation.Date)
                                .Value(c => c.OPERATION_TYPE_ID, typeId)
                                .Value(c => c.EXPENCE_ITEM_ID, categoryId)
                                .Value(c => c.MONEY_VALUE, (decimal?)operation.SumPerUnit)
                                .Value(c => c.COMPLETE_OPERATION, 'T')
                                .Value(c => c.CURRENCY_ID, currencyId)
                                .Value(c => c.QUANTITY, (decimal?)operation.Quantity)
                                .Value(c => c.MEASURE_ID, 1);
                            moneyOperation.Insert();
                        }
                        catch (KeyNotFoundException ex)
                        {
                            // Нет значение в словаре, значит пропускаем
                            errorRecords.Add(operation.Id);
                        }
                    }
                    else
                    {
                        try
                        {
                            var moneyAccountFrom = DBConformity.AccountConformityEMtoOM[operation.SourceId];
                            var moneyAccountTo = DBConformity.AccountConformityEMtoOM[operation.DestinationId];
                            var currencyId = DBConformity.CurrencyConformityEMtoOM[ExpManagerWorker.ExpManagerXmlModel.Dictionaries.Accounts.First(c => c.Id == operation.SourceId).CurrencyId]; ;
                            var currencyExchangeId = DBConformity.CurrencyConformityEMtoOM[ExpManagerWorker.ExpManagerXmlModel.Dictionaries.Accounts.First(c => c.Id == operation.DestinationId).CurrencyId]; ;
                            // Переводы
                            var moneyTransfer = db.MONEY_TRANSFER
                                .Value(c => c.TRANSFER_TYPE_ID, 1)
                                .Value(c => c.TRANSFER_PERIOD_TYPE_ID, 2)
                                .Value(c => c.CALC_TYPE_ID, 1)
                                .Value(c => c.MONEY_ACCOUNT_FROM_ID, moneyAccountFrom)
                                .Value(c => c.MONEY_ACCOUNT_TO_ID, moneyAccountTo)
                                .Value(c => c.MONEY_TRANSFER_VALUE, (decimal?)operation.SumPerUnit)
                                .Value(c => c.DATE_FROM, operation.Date)
                                .Value(c => c.DATE_TO, operation.Date)
                                .Value(c => c.COMMISSION_VALUE, 0)
                                .Value(c => c.CURRENCY_ID, currencyId)
                                .Value(c => c.DESCRIPTION, operation.Comment)
                                .Value(c => c.TRANSFER_DATE, operation.Date)
                                .Value(c => c.MONEY_EXCHANGE_VALUE, (decimal?)(operation.CurrencySum == -1 ? operation.SumPerUnit : operation.CurrencySum))
                                .Value(c => c.CURRENCY_EXCHANGE_ID, currencyExchangeId);
                            var moneyTransferId = (int)moneyTransfer.InsertWithIdentity();
                            // На это дело есть триггеры
                            //var moneyOperation1 = db.MONEY_OPERATION
                            //    .Value(c => c.DESCRIPTION, operation.Comment)
                            //    .Value(c => c.MONEY_ACCOUNT_ID, moneyAccountFrom)
                            //    .Value(c => c.OPERATION_DATE, operation.Date)
                            //    .Value(c => c.OPERATION_TYPE_ID, 3) // Расход
                            //    .Value(c => c.MONEY_VALUE, (decimal?)operation.SumPerUnit)
                            //    .Value(c => c.MONEY_ACCOUNT_TRANSFER_ID, moneyAccountTo)
                            //    .Value(c => c.COMPLETE_OPERATION, 'T')
                            //    .Value(c => c.CURRENCY_ID, currencyId)
                            //    .Value(c => c.QUANTITY, 1)
                            //    .Value(c => c.MONEY_TRANSFER_ID, moneyTransferId);
                            //moneyOperation1.Insert();

                            //var moneyOperation2 = db.MONEY_OPERATION
                            //    .Value(c => c.DESCRIPTION, operation.Comment)
                            //    .Value(c => c.MONEY_ACCOUNT_ID, moneyAccountTo)
                            //    .Value(c => c.OPERATION_DATE, operation.Date)
                            //    .Value(c => c.OPERATION_TYPE_ID, 2) // Доход
                            //    .Value(c => c.MONEY_VALUE, (decimal?)operation.CurrencySum)
                            //    .Value(c => c.MONEY_ACCOUNT_TRANSFER_ID, moneyAccountFrom)
                            //    .Value(c => c.COMPLETE_OPERATION, 'T')
                            //    .Value(c => c.CURRENCY_ID, currencyExchangeId)
                            //    .Value(c => c.QUANTITY, 1)
                            //    .Value(c => c.MONEY_TRANSFER_ID, moneyTransferId);
                            //moneyOperation2.Insert();
                        }
                        catch (KeyNotFoundException ex)
                        {
                            // Нет значение в словаре, значит пропускаем
                            errorRecords.Add(operation.Id);
                        }
                    }
                }
            }
            if (errorRecords.Any())
            {
                MessageBox.Show("Не удалось перенести операции с Id: " + String.Join(", ", errorRecords), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                MessageBox.Show("Все операции успешно перенесены", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            RefreshGrids();
        }
        /// <summary>
        /// Обновить
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtRefresh_Click(object sender, RoutedEventArgs e)
        {
            RefreshGrids();
        }
    }
}
