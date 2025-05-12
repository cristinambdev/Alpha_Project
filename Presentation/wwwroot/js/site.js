document.addEventListener('DOMContentLoaded', () => {
    updateRelativeTimes()
    setInterval(updateRelativeTimes, 60000)

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
    function validateField(field) {
        const errorSpan = field.closest(".form-group")?.querySelector(".error-message");
        if (!errorSpan) return;

        if (!field.value.trim()) {
            errorSpan.textContent = `${field.name} is required.`;
            field.classList.add("input-error");
        } else {
            errorSpan.textContent = "";
            field.classList.remove("input-error");
        }
    }
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

            //by chat gpt so that the custom-select gets the selected value before the form is submitted
            document.querySelectorAll('.custom-select').forEach(select => {
                const selectedOption = select.querySelector('.custom-select-option.selected')
                const hiddenInput = select.querySelector('input[type="hidden"]')
                if (selectedOption && hiddenInput) {
                    hiddenInput.value = selectedOption.dataset.value
                }
            })

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
                const idInput = document.getElementById('edit-project-id');
                if (idInput) {
                    idInput.value = id; // 'id' should come from the clicked button's data-id attribute
                }


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
                const quillContainer = document.querySelector('#edit-project-description-wysiwyg-editor');
                if (quillContainer && quillContainer.__quill) {
                    quillContainer.__quill.root.innerHTML = description || '';
                }
                //const textarea = document.getElementById('edit-project-description')
                //if (textarea) textarea.value = description || ''
                //const quillEditor = Quill.find(document.querySelector('#edit-project-description-wysiwyg-editor'))
                //if (quillEditor) {
                //    quillEditor.root.innerHTML = description || ''
                //}

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


    // === EDIT CLIENT  Prefill Modal (triggered from dropdown) ===
    document.querySelectorAll('.dropdown-client .dropdown-action[data-target="#editClientModal"]').forEach(editButton => {
        editButton.addEventListener('click', function (event) {
            event.stopPropagation(); // Prevent click from closing the dropdown immediately

            const modal = document.querySelector('#editClientModal');
            if (!modal) return;

            const form = modal.querySelector('form');
            if (!form) return;

            form.reset();

            // Get data attributes from the <i> tag inside the clicked "Edit" button
            const clientId = this.dataset.id; 
            const name = this.querySelector('i').dataset.name;
            const email = this.querySelector('i').dataset.email;
            const phone = this.querySelector('i').dataset.phone;
            const streetname = this.querySelector('i').dataset.streetname;
            const postalcode = this.querySelector('i').dataset.postalcode;
            const city = this.querySelector('i').dataset.city;
            const status = this.querySelector('i').dataset.status;
            const template = this.querySelector('i').dataset.template;

            // Prepare the date to be formatted
            let formattedDate = '';
            if (template) {
                let dateObject;
                try {
                    dateObject = new Date(template);
                    formattedDate = dateObject.toISOString().slice(0, 10) || '';
                } catch (e) {
                }
            }
            // Pre-fill the form fields
            form.querySelector('input[name="Id"]').value = clientId || '';
            form.querySelector('input[name="ClientName"]').value = name || '';
            form.querySelector('input[name="Email"]').value = email || '';
            form.querySelector('input[name="Phone"]').value = phone || '';
            form.querySelector('input[name="StreetName"]').value = streetname || '';
            form.querySelector('input[name="PostalCode"]').value = postalcode || '';
            form.querySelector('input[name="City"]').value = city || '';
            form.querySelector('input[name="Status"]').value = status || ''; 
            form.querySelector('input[name="Date"]').value = formattedDate || '';


            // Update status dropdown text
            const select = form.querySelector('.custom-select');
            if (select) {
                const triggerText = select.querySelector('.custom-select-text');
                const option = select.querySelector(`.custom-select-option[data-value="${status}"]`);
                if (option && triggerText) {
                    triggerText.textContent = option.textContent;
                }
            }

            // Set the image preview
            const imagePreview = form.querySelector('.image-preview');
            if (imagePreview && this.querySelector('i').dataset.image) {
                imagePreview.src = this.querySelector('i').dataset.image;
                form.querySelector('.image-previewer').classList.add('selected');
            }

            // Show the modal
            modal.classList.add('modal-show');
        })
    })

    // === EDIT MEMBER FORM: Prefill Modal===

    const editButtons = document.querySelectorAll('[data-target="#editMemberModal"]')

    editButtons.forEach((button) => {
        button.addEventListener('click', () => {
            const userId = button.getAttribute('data-user-id')

            if (userId) {
                fetch(`/Users/GetUserData/${userId}`)
                    .then(response => response.json())
                    .then(data => {
                        const form = document.querySelector('#editMemberModal form')
                        if (!form) return;

                        let idField = form.querySelector('input[name="Id"]')
                        if (!idField) {
                            idField = document.createElement('input')
                            idField.type = 'hidden';
                            idField.name = 'Id';
                            form.appendChild(idField)
                        }
                        idField.value = data.id;

                        
                        // === Fill input fields ===
                        const editUserFields = {
                            FirstName: data.firstName,
                            LastName: data.lastName,
                            Email: data.email,
                            Phone: data.phoneNumber,
                            JobTitle: data.jobTitle,
                            //Role: data.role,
                            StreetName: data.streetName,
                            PostalCode: data.postalCode,
                            City: data.city
                        };

                        for (const [name, value] of Object.entries(editUserFields)) {
                            const input = form.querySelector(`input[name="${name}"]`)
                            if (input) {
                                input.value = value || '';
                            }
                        }
                        // === Handle custom-select for Role ===
                        const roleValue = data.role;
                        const roleSelect = form.querySelector('.custom-select')

                        if (roleSelect) {
                            const hiddenInput = roleSelect.querySelector('input[name="Role"]')
                            const triggerText = roleSelect.querySelector('.custom-select-text')
                            const options = roleSelect.querySelectorAll('.custom-select-option')
                            const placeholder = roleSelect.getAttribute('data-placeholder') || "Choose role"

                            // Set value first
                            if (hiddenInput) {
                                hiddenInput.value = roleValue || '';
                            }

                            // Reset custom select
                            options.forEach(option => {
                                option.classList.remove('same-as-selected');
                            });

                            // Reset custom select active state
                            options.forEach(option => option.classList.remove('same-as-selected'));

                            if (roleValue) {
                                const matchingOption = Array.from(options).find(option => option.dataset.value === roleValue);
                                if (matchingOption) {
                                    triggerText.textContent = matchingOption.textContent;
                                    matchingOption.classList.add('same-as-selected');
                                    roleSelect.classList.remove('has-placeholder');
                                } else {
                                    triggerText.textContent = placeholder;
                                    roleSelect.classList.add('has-placeholder');
                                }
                            } else {
                                triggerText.textContent = placeholder;
                                roleSelect.classList.add('has-placeholder');
                            }
                        }
                        // === Set image preview ===
                        const imagePreview = form.querySelector('.image-preview');
                        const previewWrapper = form.querySelector('.image-previewer')

                        if (imagePreview) {
                            imagePreview.src = data.userImage || '';
                            if (data.userImage) {
                                previewWrapper?.classList.add('selected')
                            } else {
                                previewWrapper?.classList.remove('selected')
                            }
                        }

                        // === Show modal ===
                        document.querySelector('#editMemberModal')?.classList.add('modal-show')
                    })
                    .catch(error => console.error('Error loading user data:', error))
            }
        })
    })




})

function updateRelativeTimes() {
    const elements = document.querySelectorAll('.notification-item .time')
    const now = new Date();

    elements.forEach(el => {
        const created = new Date(el.getAttribute('data-created'))
        const diff = now - created;
        const diffSeconds = Math.floor(diff / 100)
        const diffMinutes = Math.floor(diffSeconds / 60)
        const diffHours = Math.floor(diffMinutes / 60)
        const diffDays = Math.floor(diffHours / 24)
        const diffWeeks = Math.floor(diffDays / 7)

        let relativeTime = '';

        if (diffMinutes < 1) {
            relativeTime = '0 min ago'
        } else if (diffMinutes < 60) {
            realtiveTime = diffMinutes + ' min ago'
        } else if (diffHours < 2) {
            relativeTime = diffHours + ' hour ago'
        } else if (diffHours < 24) {
            reltiveTime = diffHours + ' hours ago'
        } else if (diffDays < 2) {
            relativeTime = diffDays + ' day ago'
        } else if (diffDays < 7) {
            relativeTime = diffDays + 'days ago'
        } else {
            relativeTime = diffWeeks + ' weeks ago'
        }
        el.textContent = relativeTime;
            
        
    })
}