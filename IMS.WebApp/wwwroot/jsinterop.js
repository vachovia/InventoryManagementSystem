function preventFormSubmission(formId) {
    document.getElementById(`${formId}`).addEventListener('keydown', function (event) {
        console.log(event.key);
        if (event.key == "Enter") {
            event.preventDefault();
            return false;
        }
    });
}

// We have to register this file in App.razor as other js files in App.razor