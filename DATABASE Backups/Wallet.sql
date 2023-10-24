SELECT 
    COALESCE(I.id,E.id) as Id,
    COALESCE(I.name, E.name) as name,
	COALESCE(I.openingBalance, E.openingBalance) as openingBalance,
    COALESCE(I.Income_Total, 0) as Income_Total,
    COALESCE(E.Expense_Total, 0) as Expense_Total,
    COALESCE(I.Income_Total, 0) - COALESCE(E.Expense_Total, 0) as Net_Amount
FROM
			(SELECT 
            P.Id,
            P.name,
			P.openingBalance,
            SUM(T.amount) as Income_Total
        FROM 
            Payment as P 
        LEFT JOIN 
            [Transaction] as T ON  P.id = T.paymentFromId 
        WHERE 
            T.type = 'Income' 
        GROUP BY 
            P.id, P.name, P.openingBalance) as I

		FULL JOIN

			(SELECT 
            P.Id,
            P.name,
			P.openingBalance,
            SUM(T.amount) as Expense_Total
        FROM 
            Payment as P 
        LEFT JOIN 
            [Transaction] as T ON  P.id = T.paymentFromId 
        WHERE 
            T.type = 'Expense' 
        GROUP BY 
            P.id, P.name, P.openingBalance)as E ON I.id = E.id

		