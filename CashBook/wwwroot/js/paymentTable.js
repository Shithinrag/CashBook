function ChangeStatus(id) {
    // Get the label element by ID
    const statusLabel = document.getElementById(`statusLabel-${id}`);

    // Update the class and innerHTML based on the new status
    if (statusLabel.innerHTML === "Active") {
        statusLabel.className = "badge badge-danger";
        statusLabel.innerHTML = "Inactive";
    } else {
        statusLabel.className = "badge badge-success";
        statusLabel.innerHTML = "Active";
    }
    fetch(`/Payment/ToggleStatus/${id}`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json; charset=utf-8"
        }
    });
}

function EditPayment(id) {
    if (!$("#right-sidebar").hasClass("open")) {
        // If it doesn't have the class "open", toggle it to add the class
        $("#right-sidebar").toggleClass("open");
    }
    $("#chats-tab").click();
    UpdatePaymentButtonClick(id);    
}

