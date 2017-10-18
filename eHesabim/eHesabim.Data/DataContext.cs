using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

using eHesabim.Data.Domain;
using eHesabim.Data.Mapping;

namespace eHesabim.Data {
    /// <summary>The data context.</summary>
    public class DataContext : DbContext, IDataContext {
        /// <summary>Initializes a new instance of the <see cref="DataContext"/> class.</summary>
        public DataContext()
            : base("DataContext") {
            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;
            Database.Log = s => System.Diagnostics.Debug.WriteLine(s);
        }

        /// <summary>Gets or sets the bank.</summary>
        public IDbSet<Bank> Bank { get; set; }

        /// <summary>Gets or sets the bank account.</summary>
        public IDbSet<BankAccount> BankAccount { get; set; }

        /// <summary>Gets or sets the bank account transaction.</summary>
        public IDbSet<BankAccountTransaction> BankAccountTransaction { get; set; }

        /// <summary>Gets or sets the bank credit.</summary>
        public IDbSet<BankCredit> BankCredit { get; set; }

        /// <summary>Gets or sets the bank credit sub.</summary>
        public IDbSet<BankCreditSub> BankCreditSub { get; set; }

        /// <summary>Gets or sets the bank credit card.</summary>
        public IDbSet<BankCreditCard> BankCreditCard { get; set; }

        /// <summary>Gets or sets the bank credit card payment.</summary>
        public IDbSet<BankCreditCardPayment> BankCreditCardPayment { get; set; }

        /// <summary>Gets or sets the bank credit card period.</summary>
        public IDbSet<BankCreditCardPeriod> BankCreditCardPeriod { get; set; }

        /// <summary>Gets or sets the city.</summary>
        public IDbSet<City> City { get; set; }

        /// <summary>Gets or sets the county.</summary>
        public IDbSet<County> County { get; set; }

        /// <summary>Gets or sets the customer.</summary>
        public IDbSet<Customer> Customer { get; set; }

        /// <summary>Gets or sets the customer transaction.</summary>
        public IDbSet<CustomerTransaction> CustomerTransaction { get; set; }

        /// <summary>Gets or sets the email account.</summary>
        public IDbSet<EmailAccount> EmailAccount { get; set; }

        /// <summary>Gets or sets the email queue.</summary>
        public IDbSet<EmailQueue> EmailQueue { get; set; }

        /// <summary>Gets or sets the email template.</summary>
        public IDbSet<EmailTemplate> EmailTemplate { get; set; }

        /// <summary>Gets or sets the expense.</summary>
        public IDbSet<Expense> Expense { get; set; }

        /// <summary>Gets or sets the expense group.</summary>
        public IDbSet<ExpenseGroup> ExpenseGroup { get; set; }

        /// <summary>Gets or sets the expense store.</summary>
        public IDbSet<ExpenseStore> ExpenseStore { get; set; }

        /// <summary>Gets or sets the type.</summary>
        public IDbSet<Type> Type { get; set; }

        /// <summary>Gets or sets the user.</summary>
        public IDbSet<User> User { get; set; }

        /// <summary>Gets or sets the view expense group report.</summary>
        public IDbSet<ViewExpenseGroupReport> ViewExpenseGroupReport { get; set; }

        /// <summary>The set.</summary>
        /// <typeparam name="TEntity">The t entity.</typeparam>
        /// <returns>The dbset.</returns>
        public new IDbSet<TEntity> Set<TEntity>() where TEntity : class {
            return base.Set<TEntity>();
        }

        /// <summary>The modify context.</summary>
        /// <param name="itemToModify">The item to modify.</param>
        /// <typeparam name="T">The t.</typeparam>
        public void ModifyContext<T>(T itemToModify) where T : class {
            Entry(itemToModify).State = EntityState.Modified;
        }

        /// <summary>The set auto detect changes.</summary>
        /// <param name="enabled">The enabled.</param>
        public void SetAutoDetectChanges(bool enabled) {
            Configuration.AutoDetectChangesEnabled = enabled;
            Configuration.ValidateOnSaveEnabled = enabled;
        }

        /// <summary>The delete object.</summary>
        /// <param name="itemToDelete">The item to delete.</param>
        /// <typeparam name="T">The t.</typeparam>
        public void DeleteObject<T>(T itemToDelete) where T : class {
            Entry(itemToDelete).State = EntityState.Deleted;
        }

        /// <summary>The execute stored procedure.</summary>
        /// <param name="procedureName">The procedure name.</param>
        /// <param name="sqlParameters">The sql parameters.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public string ExecuteStoredProcedure(string procedureName, params object[] sqlParameters) {
            var queryResult = Database.SqlQuery<string>(procedureName, sqlParameters);

            var result = queryResult.FirstOrDefault();
            return result;
        }

        /// <summary>The execute stored procedure.</summary>
        /// <param name="procedureName">The procedure name.</param>
        /// <param name="sqlParameters">The sql parameters.</param>
        /// <typeparam name="T">The t.</typeparam>
        /// <returns>The i enumerable.</returns>
        public IEnumerable<T> ExecuteStoredProcedure<T>(string procedureName, params object[] sqlParameters) {
            var queryResult = Database.SqlQuery<T>(procedureName, sqlParameters);

            return queryResult;
        }

        /// <summary>The on model creating.</summary>
        /// <param name="modelBuilder">The model builder.</param>
        protected override void OnModelCreating(DbModelBuilder modelBuilder) {
            modelBuilder.Configurations.Add(new BankMap());
            modelBuilder.Configurations.Add(new BankAccountMap());
            modelBuilder.Configurations.Add(new BankAccountTransactionMap());
            modelBuilder.Configurations.Add(new BankCreditMap());
            modelBuilder.Configurations.Add(new BankCreditSubMap());
            modelBuilder.Configurations.Add(new BankCreditCardMap());
            modelBuilder.Configurations.Add(new BankCreditCardPaymentMap());
            modelBuilder.Configurations.Add(new BankCreditCardPeriodMap());
            modelBuilder.Configurations.Add(new CityMap());
            modelBuilder.Configurations.Add(new CountyMap());
            modelBuilder.Configurations.Add(new CustomerMap());
            modelBuilder.Configurations.Add(new CustomerTransactionMap());
            modelBuilder.Configurations.Add(new EmailAccountMap());
            modelBuilder.Configurations.Add(new EmailQueueMap());
            modelBuilder.Configurations.Add(new EmailTemplateMap());
            modelBuilder.Configurations.Add(new ExpenseMap());
            modelBuilder.Configurations.Add(new ExpenseGroupMap());
            modelBuilder.Configurations.Add(new ExpenseStoreMap());
            modelBuilder.Configurations.Add(new TypeMap());
            modelBuilder.Configurations.Add(new UserMap());
            modelBuilder.Configurations.Add(new ViewExpenseGroupReportMap());

            base.OnModelCreating(modelBuilder);
        }
    }
}
