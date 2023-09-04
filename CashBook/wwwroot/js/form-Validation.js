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

