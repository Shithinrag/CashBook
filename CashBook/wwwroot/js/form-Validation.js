//Validation for Name
function validateName(nameInput, nameError) {
    const nameValue = nameInput.value.trim();
    if (nameValue === '') {
        nameError.textContent = 'Name is required';
        return false; // Return false to prevent form submission
    } else {
        nameError.textContent = '';
        return true; // Return true to allow form submission
    }
}
//Validation for Opening Balance
function validateAmount(openingBalanceInput) {
    const amountPattern = /^\d+(\.\d{1,2})?$/;
    const openingBalanceValue = openingBalanceInput.value.trim();
    const openingBalanceError = document.getElementById('openingBalance-error');
    if (openingBalanceValue === '' || !amountPattern.test(openingBalanceValue)) {
        openingBalanceError.textContent = 'Invalid Amount. Use up to two decimal places.';
        return false; // Return false to prevent form submission
    } else {
        openingBalanceError.textContent = '';
        return true; // Return true to allow form submission
    }
}
//Validation for Contra entry for Add/Update Transaction
function ValidateContra() {
    if (document.getElementById("type-transaction").value === "Contra") {
        if (document.getElementById("paymentfromid-2-transaction").value === document.getElementById("paymenttoid-1-transaction").value) {
            showToastDanger("Payment From and To should not be same");
            return false;
        }
        return true;
    }
    return true; // Return false if the value is not "Contra"
}   

