// === EDIT CLIENT  Prefill Modal ===
document.querySelectorAll('.miniProject-dropdown').forEach(icon => {
    icon.addEventListener('click', function () {

        const modal = document.querySelector('#editMiniProjectModal',)
        if (!modal) return; // Prevent error if modal is not on the page
        const form = modal.querySelector('form')
        if (!form) return; // Optional: prevent error if form is missing


        // Reset form first
        form.reset();

        // Get client ID and store it in the form
        const miniProjectId = this.dataset.id
        const idInput = form.querySelector('input[name="Id"]')
        if (idInput) {
            idInput.value = miniProjectId
            console.log(`Setting Mini Project ID: ${miniProjectId}`) // Debug log
        }
      
        const statusInput = form.querySelector('input[name="Status"]')
        if (statusInput) {
            statusInput.value = statusValue
            console.log(`Setting status value: ${statusValue}`) // Debug log
        }


        // Update the status dropdown text
        const select = statusInput?.closest('.custom-select');
        if (select) {
            const option = select.querySelector(`.custom-select-option[data-value="${statusValue}"]`);
            if (option) {
                const triggerText = select.querySelector('.custom-select-text');
                if (triggerText) {
                    triggerText.textContent = option.textContent;
                    console.log(`Setting dropdown text: ${option.textContent}`) // Debug log
                }
            } else {
                console.warn(`No matching option found for status value: ${statusValue}`)
            }
        }

        // Set other form fields, checking if they exist first
        const fieldsToSelectMiniProjectModal = {
            'Title': this.dataset.title,
            'Description': this.dataset.description,
            'ClientName': this.dataset.clientname,
            'StartDate': new Date(this.dataset.template).toISOString().slice(0, 10),
            'EndDate': new Date(this.dataset.template).toISOString().slice(0, 10),
            'Budget': this.dataset.budget,
        }
        for (const [fieldName, value] of Object.entries(fieldsToSelectMiniProjectModal)) {
            const field = form.querySelector(`[name="${fieldName}"]`)
            if (field) {
                field.value = value
            } else {
                console.warn(`Field not found: ${fieldName}`)
            }
        }

        // Set the image preview
        const imagePreview = form.querySelector('.image-preview')
        if (imagePreview && this.dataset.image) {
            imagePreview.src = this.dataset.image
            form.querySelector('.image-previewer').classList.add('selected')
            console.log(`Setting image: ${this.dataset.image}`) // Debug log
        }

    })
})