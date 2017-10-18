namespace eHesabim.Services {
    /// <summary>The permission type enum.</summary>
    public enum PermissionTypeEnum {
        /// <summary>The admin.</summary>
        Admin = 1,

        /// <summary>The user.</summary>
        User = 2,

        /// <summary>The guest.</summary>
        Guest = 3
    }

    public enum PermissionFormEnum {
        User = 1,
        Customer = 2,
        BankAccount = 3,
        BankCredit = 4,
        BankCreditCard = 5,
        Expense = 6
    }

    public enum FormEnum {
        User = 1,
        Permission = 2,
        Customer = 3,
        CustomerTransaction = 4,
        BankAccount = 5,
        BankAccountTransaction = 6,
        BankCredit = 7,
        BankCreditSub = 8,
        BankCreditCard = 9,
        BankCreditCardPeriod = 10,
        BankCreditCardPayment = 11,
        Expense = 12,
        ExpenseGroup = 13,
        ExpenseStore = 14
    }

    public enum CustomerTransactionTypeEnum {
        Parent = 100100,
        Debit = 100101,
        Claim = 100102
    }

    public enum BankAccountTransactionTypeEnum {
        Parent = 100100,
        Debit = 100101,
        Claim = 100102
    }

    public enum ExpenseTypeEnum {
        Parent = 100200,
        Cash = 100201,
        CreditCard = 100202,
        CreditCardMulti = 100203
    }

    public enum AccountTypeEnum {
        // parent
        Parent = 100300,

        // nakit
        Cash = 100301,

        // vadesiz hesap
        DemandDeposit = 100302,

        // vadeli hesap
        TimeDeposit = 100303,

        // kredili mevduat hesabı
        OverdraftDeposit = 100304
    }
}
