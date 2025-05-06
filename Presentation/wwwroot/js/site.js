document.addEventListener('DOMContentLoaded', () => {

    // === DROPDOWNS ===
    const dropdowns = document.querySelectorAll('[data-type="dropdown"]')

    document.addEventListener('click', function (event) {
        let clickedDropdown = null

        dropdowns.forEach(dropdown => {
            const targetId = dropdown.getAttribute('data-target')
            const targetElement = document.querySelector(targetId)

            if (dropdown.contains(event.target)) {
                clickedDropdown = targetElement

                document.querySelectorAll('.dropdown.dropdown-show').forEach(openDropDown => {
                    if (openDropDown !== targetElement) {
                        openDropDown.classList.remove('dropdown-show')
                    }
                })
                // Toggle the clicked dropdown visibility
                targetElement.classList.toggle('dropdown-show')
            }
        })

        if (!clickedDropdown && !event.target.closest('.dropdown')) {
            document.querySelectorAll('.dropdown.dropdown-show').forEach(openDropDown => {
                openDropDown.classList.remove('dropdown-show')
            })
        }
    })

    // === MODALS ===
    const modals = document.querySelectorAll('[data-type="modal"]')
    modals.forEach(modal => {
        modal.addEventListener('click', function () {
            const targetId = modal.getAttribute('data-target')
            const targetElement = document.querySelector(targetId)
            targetElement.classList.add('modal-show')
        })
    })

    const closeButtons = document.querySelectorAll('[data-type="close"]')
    closeButtons.forEach(button => {
        button.addEventListener('click', function () {
            const targetId = button.getAttribute('data-target')
            const targetElement = document.querySelector(targetId)
            targetElement.classList.remove('modal-show')
        })
    })

    // === CUSTOM SELECTS ===
    function initializeCustomSelect(select) { //suggested by chat GPT to resue data
        const trigger = select.querySelector('.custom-select-trigger')
        const triggerText = select.querySelector('.custom-select-text')
        const options = select.querySelectorAll('.custom-select-option')
        const hiddenInput = select.querySelector('input[type="hidden"]')
        const placeholder = select.getAttribute('data-placeholder') || "Choose Status"

        const setValue = (value = "", text = placeholder) => {
            triggerText.textContent = text
            hiddenInput.value = value
            select.classList.toggle('has-placeholder', !value)
        }

        // Chat GPT Check if there's already a value set (from the model)
        const currentValue = hiddenInput.value
        if (currentValue) {
            // Find the matching option to display its text
            const matchingOption = Array.from(options).find(option =>
                option.dataset.value === currentValue)

            if (matchingOption) {
                setValue(currentValue, matchingOption.textContent)
            } else {
                setValue() // Use default placeholder if no match
            }
        } else {
            setValue() // Use default placeholder
        }


        trigger.addEventListener('click', (e) => {
            e.stopPropagation()

            document.querySelectorAll('.custom-select.open')
                .forEach(el => el !== select && el.classList.remove('open'))

            select.classList.toggle('open')
        })

        options.forEach(option => {
            option.addEventListener('click', () => {
                setValue(option.dataset.value, option.textContent)
                select.classList.remove('open')
            })
        })

        document.addEventListener('click', e => {
            if (!select.contains(e.target)) {
                select.classList.remove('open')
            }
        })
    }
    document.querySelectorAll('.custom-select').forEach(initializeCustomSelect)


    // === FORM SUBMISSION HANDLING ===
    const forms = document.querySelectorAll('form:not(.no-ajax)') //suggested by chat GPT so that Signup and sign-in that have no-ajax would validate and submit correctly
    forms.forEach(form => {
        // Find all validatable fields
        const fields = form.querySelectorAll("input[data-val='true'], select[data-val='true'], textarea[data-val='true']");

        // Add input events for validation
        fields.forEach(field => {
            // For text inputs, validate on input
            field.addEventListener("input", function () {
                validateField(field);
            });

            // For all fields, validate on blur
            field.addEventListener("blur", function () {
                validateField(field);
            });

            // For checkboxes, validate on change
            if (field.type === "checkbox") {
                field.addEventListener("change", function () {
                    validateField(field);
                });
            }
        });

        form.addEventListener('submit', async (e) => {
            e.preventDefault()

            clearErrorMessages(form)


            const formData = new FormData(form)

            // Log all form data before submission
            for (let [key, value] of formData.entries()) {
                console.log(`${key}: ${value}`)
            }

            try {
                //const res = await fetch(form.action, {
                //    method: 'post',
                //    body: formData
                //})
                //suggested by Chat GPT
                const method = form.dataset.method || form.getAttribute('method') || 'post'

                const res = await fetch(form.action, {
                    method: method.toLowerCase(),
                    body: formData,
                    headers: {
                        'Accept': 'application/json'
                    }
                })


                if (res.ok) {
                    const modalElement = form.closest('.modal')
                    if (modalElement) {
                        modalElement.classList.remove('modal-show')
                    }
                    window.location.reload()
                } else if (res.status === 400) {
                    const data = await res.json()

                    if (data.errors) {
                        Object.keys(data.errors).forEach(key => {
                            const input = form.querySelector(`[name="${key}"]`)
                            const span = form.querySelector(`[data-valmsg-for="${key}"]`)

                            if (input) {
                                input.classList.add("input-validation-error")
                            }

                            if (span) {
                                span.classList.remove("field-validation-valid")
                                span.classList.add("field-validation-error")
                                span.innerText = data.errors[key].join('\n')
                            }
                        })
                    }
                } else {
                    // Try to read response body for more details
                    const errorText = await res.text()
                    console.error('Form submission failed:', res.status, errorText)
                }
            } catch{
                console.log('Error submitting form:') // Debug log from chat gpt
            }
        })

    })



    // === IMAGE PREVIEWER ===
    const previewSize = 150

    document.querySelectorAll('.image-previewer').forEach(previewer => {
        const fileInput = previewer.querySelector('input[type="file"]')
        const imagePreview = previewer.querySelector('.image-preview')

        if (!imagePreview) {
            return
        }

        previewer.addEventListener('click', () => fileInput.click())

        fileInput.addEventListener('change', ({ target: { files } }) => {
            const file = files[0]
            if (file) {
                processImage(file, imagePreview, previewer, previewSize)
            }
        })
    })

    async function loadImage(file) {
        return new Promise((resolve, reject) => {
            const reader = new FileReader()

            reader.onerror = () => reject(new Error("Failed to load file."))
            reader.onload = (e) => {
                const img = new Image()
                img.onerror = () => reject(new Error("Failed to load image."))
                img.onload = () => resolve(img)
                img.src = e.target.result
            }
            reader.readAsDataURL(file)
        })
    }

    async function processImage(file, imagePreview, previewer, previewSize = 150) {

        const img = await loadImage(file)
        const canvas = document.createElement('canvas')
        canvas.width = previewSize
        canvas.height = previewSize

        const ctx = canvas.getContext('2d')
        ctx.drawImage(img, 0, 0, previewSize, previewSize)

        imagePreview.src = canvas.toDataURL('image/jpeg')
        previewer.classList.add('selected')

    }
    function clearErrorMessages(form) {
        form.querySelectorAll('[data-val="true"]').forEach(input => {
            input.classList.remove('input-validation-error')
        })
        form.querySelectorAll('[data-valmsg-for]').forEach(span => {
            span.innerText = ''
            span.classList.remove('field-validation-error')
        })
    }

    // === RESET FORMS IN MODALS ON OPEN ===
    document.querySelectorAll('.modal').forEach(modal => {
        modal.querySelectorAll('form').forEach(form => {
            form.reset()

            const imagePreview = form.querySelector('.image-preview')
            if (imagePreview) {
                imagePreview.src = ''
            }

            const imagePreviewer = form.querySelector('.image-previewer')
            if (imagePreviewer) {
                imagePreviewer.classList.remove('selected')
            }
        })
    })
    // all prefill in forms with help of Chat GPT
    //=== EDIT MINIPROJECT FORM: Prefill Modal ===
    document.querySelectorAll('.edit-mini-project-button')
        .forEach(btn => {
            btn.addEventListener('click', () => {
                const id = btn.getAttribute('data-id')

                const title = btn.getAttribute('data-title')
                const description = btn.getAttribute('data-description')
                const clientName = btn.getAttribute('data-clientname')
                const startDate = btn.getAttribute('data-startdate')
                const endDate = btn.getAttribute('data-enddate')
                const budget = btn.getAttribute('data-budget')
                const statusId = btn.getAttribute('data-statusid')
                const projectImage = btn.getAttribute('data-image')

                // Set the hidden ID field
                const idInput = document.getElementById('edit-project-id')
                if (idInput) idInput.value = id


                // Set form field values - USE IDs from rendered HTML
                const titleInput = document.getElementById('edit-project-title') // Assuming id="Title"
                if (titleInput) titleInput.value = title || ''


                const clientNameInput = document.getElementById('edit-project-clientname') // Assuming id="ClientName"
                if (clientNameInput) clientNameInput.value = clientName || ''

                // Handle dates
                const startDateInput = document.getElementById('edit-project-startdate') // Assuming id="StartDate"
                if (startDateInput) startDateInput.value = startDate || ''

                const endDateInput = document.getElementById('edit-project-enddate') // Assuming id="EndDate"
                if (endDateInput) endDateInput.value = endDate || ''

                // Set budget
                const budgetInput = document.getElementById('edit-project-budget') // Assuming id="Budget"
                if (budgetInput) budgetInput.value = budget || ''


                // Set status dropdown (hidden input)
                const statusHiddenInput = document.getElementById('edit-project-status') // Assuming id="StatusId"
                if (statusHiddenInput) statusHiddenInput.value = statusId || ''

                // Set status dropdown (display text and selected option)
                const statusDisplayText = document.querySelector('#editMiniProjectModal .custom-select-text')
                const statusOptions = document.querySelectorAll('#editMiniProjectModal .custom-select-option')


                if (statusDisplayText && statusOptions) {
                    statusOptions.forEach(option => {
                        option.classList.remove('selected')
                        if (option.getAttribute('data-value') === statusId) {
                            option.classList.add('selected')
                            statusDisplayText.textContent = option.textContent

                        }
                    })
                }

                // Handle WYSIWYG editor (remains the same as you are using the ID)
                const textarea = document.getElementById('edit-project-description')
                if (textarea) textarea.value = description || ''
                const quillEditor = Quill.find(document.querySelector('#edit-project-description-wysiwyg-editor'))
                if (quillEditor) {
                    quillEditor.root.innerHTML = description || ''
                }

                // Handle image preview
                const img = document.querySelector('#editMiniProjectModal .image-preview')
                if (img) {
                    img.src = projectImage || ''
                    if (projectImage) {
                        document.querySelector('#editMiniProjectModal .image-previewer').classList.add('selected')
                    } else {
                        document.querySelector('#editMiniProjectModal .image-previewer').classList.remove('selected')
                        img.src = '' // Optionally clear the preview if no image
                    }
                }

                // Show the modal
                document.querySelector('#editMiniProjectModal').classList.add('modal-show')
            })
        })


    // === EDIT CLIENT  Prefill Modal ===
    document.querySelectorAll('.fa-ellipsis').forEach(icon => {
        icon.addEventListener('click', function () {

            const modal = document.querySelector('#editClientModal')
            if (!modal) return // Prevent error if modal is not on the page
            const form = modal.querySelector('form')
            if (!form) return // Optional: prevent error if form is missing


            // Reset form first
            form.reset()

            // Get client ID and store it in the form
            const clientId = this.dataset.id
            const idInput = form.querySelector('input[name="Id"]')
            if (idInput) {
                idInput.value = clientId

                // Pre-fill the status dropdown - Convert text status to numeric value
                let statusValue = this.dataset.status
                // Convert text status to value if needed
                if (statusValue === "Active") statusValue = "1"
                if (statusValue === "Inactive") statusValue = "0"

                const statusInput = form.querySelector('input[name="Status"]')
                if (statusInput) {
                    statusInput.value = statusValue
                }


                // Update the status dropdown text
                const select = statusInput?.closest('.custom-select')
                if (select) {
                    const option = select.querySelector(`.custom-select-option[data-value="${statusValue}"]`)
                    if (option) {
                        const triggerText = select.querySelector('.custom-select-text')
                        if (triggerText) {
                            triggerText.textContent = option.textContent
                        }
                    } else {
                        console.warn(`No matching option found for status value: ${statusValue}`)
                    }
                }
            }

            // Set other form fields, checking if they exist first
            const fieldsToSetEditClientModal = {
                'ClientName': this.dataset.name,
                'Email': this.dataset.email,
                'Phone': this.dataset.phone,
                'StreetName': this.dataset.streetname,
                'PostalCode': this.dataset.postalcode,
                'City': this.dataset.city,
                'Date': new Date(this.dataset.template).toISOString().slice(0, 10)
            }
            for (const [fieldName, value] of Object.entries(fieldsToSetEditClientModal)) {
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
            }

            // Show the modal
            modal.classList.add('modal-show')
        })
    })
    // === EDIT MEMBER FORM: Prefill Modal===

    const editButtons = document.querySelectorAll('[data-target="#editMemberModal"]')
    editButtons.forEach(button => {
        button.addEventListener('click', function () {
            const userId = this.getAttribute('data-user-id')


            if (userId) {
                fetch(`/Users/GetUserData/${userId}`)
                    .then(response => response.json())
                    .then(data => {
                        const form = document.querySelector('#editMemberModal form')


                        // Add hidden ID field
                        let idField = form.querySelector('input[name="Id"]')
                        if (!idField) {
                            idField = document.createElement('input')
                            idField.type = 'hidden'
                            idField.name = 'Id'
                            form.appendChild(idField)
                        }
                        idField.value = data.id

                        // Set values for input fields taken from current member
                        const editUserFields = {
                            FirstName: data.firstName,
                            LastName: data.lastName,
                            Email: data.email,
                            Phone: data.phoneNumber,
                            JobTitle: data.jobTitle,
                            StreetName: data.streetName,
                            PostalCode: data.postalCode,
                            City: data.city
                        }

                        for (const [name, value] of Object.entries(editUserFields)) {
                            const input = form.querySelector(`input[name="${name}"]`)
                            if (input) {
                                input.value = value || ''
                            }
                        }

                        // Set image preview
                        const imagePreview = form.querySelector('.image-preview')
                        if (imagePreview && data.userImage) {
                            imagePreview.src = data.userImage
                            form.querySelector('.image-previewer').classList.add('selected')
                        }

                        // Show the modal
                        document.querySelector('#editMemberModal').classList.add('modal-show')
                    })
                    .catch(error => console.error('Error loading user data:', error))
            }
        })
    })




})