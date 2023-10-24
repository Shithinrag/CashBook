SELECT 
    COALESCE(I.id,E.id,c1.id,c2.id,NULL_Variable.id) as Id,
    COALESCE(I.name, E.name,c1.name,c2.name,NULL_Variable.name) as name,
	COALESCE(I.openingBalance, E.openingBalance,c1.openingBalance,c2.openingBalance,NULL_Variable.openingBalance) as openingBalance,
    COALESCE(I.Income_Total, 0) as Income_Total,
	COALESCE(C2.TO_TOTAL, 0) as To_Total,
    COALESCE(E.Expense_Total, 0) as Expense_Total,
	COALESCE(C1.From_Total, 0) as FROM_Total,
    COALESCE(I.Income_Total, 0)+ COALESCE(I.openingBalance, E.openingBalance,c1.openingBalance,c2.openingBalance,NULL_Variable.openingBalance)+ COALESCE(C2.TO_TOTAL, 0)
	- COALESCE(E.Expense_Total, 0)-COALESCE(C1.From_Total, 0) as Net_Amount
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

		FULL JOIN

		(SELECT 
            P.Id,
            P.name,
			P.openingBalance,
			SUM(T.amount) as FROM_Total
        FROM 
            Payment as P 
        LEFT JOIN 
            [Transaction] as T ON  P.id = T.paymentFromId 		
		WHERE T.type = 'Contra'
		GROUP BY P.id,P.name,P.openingBalance) as C1 ON C1.id = E.id

		FULL JOIN

		(SELECT 
            P.Id,
            P.name,
			P.openingBalance,
			SUM(T.amount) as To_Total
        FROM 
            Payment as P 
        LEFT JOIN 
            [Transaction] as T ON  P.id = T.paymentToId
			WHERE T.type = 'Contra'
		GROUP BY P.id,P.name,P.openingBalance) as C2 ON C2.id = C1.id 

		FULL JOIN
		(SELECT 
            P.Id,
            P.name,
			P.openingBalance           
			
        FROM 
            Payment as P 
         LEFT JOIN 
            [Transaction] as T ON  P.id = T.paymentFromId or P.id = T.paymentToId
       WHERE 
	   T.paymentFromId IS NULL and T.paymentToId IS NULL 
	   GROUP BY P.id, P.name, P.openingBalance) as NULL_variable ON NULL_variable.id = E.id

		ORDER BY id