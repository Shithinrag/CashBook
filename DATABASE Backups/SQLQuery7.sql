SELECT 
    COALESCE(income.paymentFromId, expenses.paymentFromId) as paymentFromId,
    COALESCE(income.name, expenses.name) as name,
    COALESCE(income.Income_Total, 0) as Income_Total,
    COALESCE(expenses.Expense_Total, 0) as Expense_Total,
    COALESCE(income.Income_Total, 0) - COALESCE(expenses.Expense_Total, 0) as Net_Amount
FROM
    (
        SELECT 
            T.paymentFromId,
            P.name, 
            SUM(T.amount) as Income_Total 
        FROM 
            [Transaction] as T 
        LEFT JOIN 
            [Payment] as P ON T.paymentFromId = P.id 
        WHERE 
            T.type = 'Income' 
        GROUP BY 
            T.paymentFromId, P.name
    ) as income
FULL OUTER JOIN
    (
        SELECT 
            T.paymentFromId,
            P.name, 
            SUM(T.amount) as Expense_Total 
        FROM 
            [Transaction] as T 
        LEFT JOIN 
            [Payment] as P ON T.paymentFromId = P.id 
        WHERE 
            T.type = 'Expense' 
        GROUP BY 
            T.paymentFromId, P.name
    ) as expenses ON income.paymentFromId = expenses.paymentFromId
                 AND income.name = expenses.name;
